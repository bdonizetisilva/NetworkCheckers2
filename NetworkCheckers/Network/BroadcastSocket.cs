using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkCheckers.Utils;

namespace NetworkCheckers.Network
{
    /// <summary>
    /// Socket Broadcast UDP em IPv4 e IPv6 (multicast)
    /// </summary>
    public sealed class BroadcastSocket : IDisposable
    {
        // Constantes
        #region Consts
        /// <summary>
        /// Tamanho do buffer de recepção de dados
        /// </summary>
        private const int BUFFER_SIZE = 1024 * 8;
        #endregion

        // Variáveis
        #region Variables
        /// <summary>
        /// Sincronia entre threads
        /// </summary>
        private readonly object SyncRoot = new object();

        /// <summary>
        /// Buffer de dados do IPv4
        /// </summary>
        private byte[] _Buffer;

        /// <summary>
        /// Destino em IPv4
        /// </summary>
        private IPEndPoint _Destiny;

        /// <summary>
        /// Callback de recebimento IPv4
        /// </summary>
        private AsyncCallback _ReceiveCallbak;

        /// <summary>
        /// Eventos de recebimento
        /// </summary>
        private EventHandler<BroadcastReceivedEventArgs> _Received;

        /// <summary>
        /// Último endereço de recebimento em IPv4
        /// </summary>
        private IPEndPoint _ReceivedSource;

        /// <summary>
        /// Callback de envio
        /// </summary>
        private AsyncCallback _SendCallback;

        /// <summary>
        /// Socket em IPv4
        /// </summary>
        private Socket _Socket;

        /// <summary>
        /// Origem de escuta em IPv4
        /// </summary>
        private EndPoint _Source;
        #endregion

        // Estáticos
        #region Statics

        /// <summary>
        /// Endereço Multicast IPv4 quando em ClassC
        /// </summary>
        private readonly static IPAddress Broadcast = new IPAddress(new byte[] { 192, 168, 88, 255 });

        #endregion

        // Construtores
        #region Constructors

        /// <summary>
        /// Novo socket em Broadcast
        /// </summary>
        /// <param name="port">Porta de broadcast</param>
        public BroadcastSocket(ushort port)
        {
            // Valida porta
            if (port == 0)
                throw new ArgumentOutOfRangeException(nameof(port));

            // Inicializa endereços de destino
            this._Destiny = new IPEndPoint(BroadcastSocket.Broadcast, port);

            // Inicializa endereços de origem
            this._Source = new IPEndPoint(IPAddress.Any, port);
            this._ReceivedSource = new IPEndPoint(IPAddress.Any, port);

            // Inicializa sockets
            this._Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this._Socket.EnableBroadcast = true;

            // Callback de envio
            this._SendCallback = new AsyncCallback(this.SendCallBack);
        }

        #endregion

        // Eventos
        #region Events

        /// <summary>
        /// Recebimento de dados em broadcast, modo assíncrono
        /// </summary>
        public event EventHandler<BroadcastReceivedEventArgs> Received
        {
            add
            {
                // Sincroniza
                lock (this.SyncRoot)
                {
                    // Inicializa bind
                    this.Bind(true);

                    // Combina evento
                    this._Received = (EventHandler<BroadcastReceivedEventArgs>)Delegate.Combine(this._Received, value);
                }
            }

            remove
            {
                // Sincroniza
                lock (this.SyncRoot)
                {
                    // Remove evento
                    this._Received = (EventHandler<BroadcastReceivedEventArgs>)Delegate.Remove(this._Received, value);
                }
            }
        }

        #endregion

        // Métodos públicos
        #region Public

        /// <summary>
        /// Libera recursos alocados
        /// </summary>
        public void Dispose()
        {
            // Libera socket IPv4
            Socket sck4 = this._Socket;
            this._Socket = null;
            sck4?.Dispose();
        }

        /// <summary>
        /// Envia dados de forma assíncrona
        /// </summary>
        /// <param name="data">Dados a enviar</param>
        public void SendAsync(byte[] data)
        {
            // Valida dados a enviar
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Possui dado a enviar?
            if (data.Length > 0)
            {
                // Envia IPv4
                BroadcastSocket.SendTo(data, this._Socket, this._Destiny, this._SendCallback);
            }
        }

        /// <summary>
        /// Envia dados de forma síncrona
        /// </summary>
        /// <param name="data">Dados a enviar</param>
        public void SendSync(byte[] data)
        {
            // Valida dados a enviar
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Possui dado a enviar?
            if (data.Length > 0)
            {
                // Envia IPv4
                BroadcastSocket.SendTo(data, this._Socket, this._Destiny);
            }
        }

        /// <summary>
        /// Tenta inicializar escuta de dados
        /// </summary>
        /// <returns>Escuta inicializada</returns>
        public bool TryBind()
        {
            // Executa binding
            return this.Bind(false);
        }

        #endregion

        // Métodos privados
        #region Private

        /// <summary>
        /// Envia dados via socket, sem dispara exceções
        /// </summary>
        /// <param name="data">Dados a enviar</param>
        /// <param name="socket">Socet de envio</param>
        /// <param name="destiny">Destino dos dados</param>
        private static void SendTo(byte[] data, Socket socket, IPEndPoint destiny)
        {
            try
            {
                // Envia dados
                socket.SendTo(data, 0, data.Length, SocketFlags.None, destiny);
            }

            // Redispara thread abortada
            catch (ThreadAbortException) { throw; }

            // Ignora outras exceções
            catch {; }
        }

        /// <summary>
        /// Envia dados via socket, sem dispara exceções
        /// </summary>
        /// <param name="data">Dados a enviar</param>
        /// <param name="socket">Socet de envio</param>
        /// <param name="destiny">Destino dos dados</param>
        /// <param name="callback">Chamada ao finalizar envio</param>
        private static void SendTo(byte[] data, Socket socket, IPEndPoint destiny, AsyncCallback callback)
        {
            try
            {
                // Envia dados
                socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, destiny, callback, socket);
            }

            // Redispara thread abortada
            catch (ThreadAbortException) { throw; }

            // Ignora outras exceções
            catch {; }
        }

        /// <summary>
        /// Inicializa escuta da porta
        /// </summary>
        /// <param name="throwexceptions">Disparar exceções</param>
        /// <returns>Escuta inicializada</returns>
        private bool Bind(bool throwexceptions)
        {
            try
            {
                // Sincroniza
                lock (this.SyncRoot)
                {
                    // Não há buffers criados ainda?
                    if (this._Buffer == null)
                    {
                        // Cria métodos de recebimento
                        this._ReceiveCallbak = new AsyncCallback(this.ReceiveCallBack);

                        // Inicia escutas
                        this._Socket.Bind(this._Source);

                        // Cria buffers de recepção
                        this._Buffer = new byte[Math.Max(this._Socket.ReceiveBufferSize, BUFFER_SIZE)];

                        // Inicializa recebimento de pacotes
                        this._Socket.BeginReceiveFrom(this._Buffer, 0, this._Buffer.Length, SocketFlags.None, ref this._Source, this._ReceiveCallbak, null);
                    }
                }
            }

            // Redispara thread abortada
            catch (ThreadAbortException) { throw; }

            catch
            {
                // Libera buffer
                this._Buffer = null;

                // Reporta exceções?
                if (throwexceptions)
                    throw;

                // Não inicializado
                return false;
            }

            // Inicializado
            return true;
        }

        /// <summary>
        /// Recebimento de dados em IPv4
        /// </summary>
        private void ReceiveCallBack(IAsyncResult a)
        {
            try
            {
                // Recupera socket associado
                Socket socket = this._Socket;
                if (socket != null)
                {
                    // Referência a origem de dados
                    EndPoint source = (EndPoint)this._ReceivedSource;

                    // Recupera dados recebidos
                    int read = socket.EndReceiveFrom(a, ref source);

                    // Dados recebidos
                    byte[] data = null;

                    // Houve dados lidos?
                    if (read > 0)
                    {
                        // Recupera dados
                        data = new byte[read];
                        Buffer.BlockCopy(this._Buffer, 0, data, 0, read);
                    }

                    // Linka evento para mais recebimento
                    socket.BeginReceiveFrom(this._Buffer, 0, this._Buffer.Length, SocketFlags.None, ref this._Source, this._ReceiveCallbak, a.AsyncState);

                    // Houve dados lidos?
                    if (data != null)
                    {
                        // Há evento associado?
                        EventHandler<BroadcastReceivedEventArgs> received = this._Received;
                        if (received != null)
                        {
                            // Cria argumentos de pacote
                            BroadcastReceivedEventArgs args = new BroadcastReceivedEventArgs(data, source);

                            // Dispara evento
                            received.Invoke(this, args);
                        }
                    }
                }
            }

            // Reporta exceções
            catch (Exception ex) { ex.Log(); }
        }

        /// <summary>
        /// Retorno do envio de dados
        /// </summary>
        private void SendCallBack(IAsyncResult a)
        {
            try
            {
                // Recupera socket associado
                Socket socket = a.AsyncState as Socket;
                if (socket != null)
                {
                    // Finaliza envio
                    socket.EndSendTo(a);
                }
            }

            // Reporta exceções
            catch (Exception ex) { Console.WriteLine(nameof(BroadcastSocket) + " > " + ex.Message); }
        }
        #endregion
    }
}
