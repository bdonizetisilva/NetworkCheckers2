using NetworkCheckers.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

namespace NetworkCheckers.Network.Services
{
    /// <summary>
    /// Busca de servidores na rede
    /// </summary>
    public sealed class ServerFinder : IEnumerable<ServerInfo>, IDisposable
    {
        // Constantes
        #region Consts
        /// <summary>
        /// Threshold padrão de servidor morto, em ms
        /// </summary>
        private const int DEFAULT_DEAD_THRESHOLD = 10000;
        #endregion

        // Variáveis
        #region Variables
        /// <summary>
        /// Sincronismo de threads
        /// </summary>
        private readonly object _SyncRoot = new object();

        /// <summary>
        /// Thread backgroudn para tentar inicializar o bind do socket
        /// </summary>
        private Thread _BindThread;

        /// <summary>
        /// Tempo de vida, em ms, para um servidor ser considerado morto
        /// </summary>
        private int _DeadThreshold = DEFAULT_DEAD_THRESHOLD;

        /// <summary>
        /// Informação de servidores
        /// </summary>
        private readonly List<ServerInfo> _Servers = new List<ServerInfo>(2);

        /// <summary>
        /// Socket de conexão em broadcast
        /// </summary>
        private BroadcastSocket _Socket;
        #endregion

        // Construtores
        #region Constructors
        /// <summary>
        /// Nova busca de servidores
        /// </summary>
        /// <param name="port">Porta de escuta</param>
        public ServerFinder(ushort port)
        {
            // Valida porta
            if (port == 0)
                throw new ArgumentOutOfRangeException(nameof(port));

            // Cria socket para escuta
            this._Socket = new BroadcastSocket(port);

            // Tenta inicializar escuta
            if (this._Socket.TryBind())
            {
                // Linka recebimento de pacotes
                this._Socket.Received += this.Socket_Received;
            }
            else
            {
                // Cria thread para executar binding
                this._BindThread = new Thread(this.BindThread)
                {
                    Priority = ThreadPriority.BelowNormal
                };

                this._BindThread.Start();
            }
        }
        #endregion

        // Propriedades
        #region Properties
        /// <summary>
        /// Tempo de vida em milisegundos antes de um servidor ser considerado 'morto' e não ser mais listado
        /// </summary>
        public int DeadThreshold
        {
            // Recupera
            get { return this._DeadThreshold; }

            // Atualiza
            set
            {
                // Valida entrada
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(DeadThreshold));

                // Atualiza valor
                this._DeadThreshold = value;
            }
        }
        #endregion

        // Eventos
        #region Events
        /// <summary>
        /// Novo servidor encontrado
        /// </summary>
        public event EventHandler<ServerEventArgs> Found;
        #endregion

        // Métodos públicos
        #region Public
        /// <summary>
        /// Destrutor
        /// </summary>
        ~ServerFinder()
        {
            // Libera recursos
            this.Dispose();
        }

        /// <summary>
        /// Libera recursos alocados
        /// </summary>
        public void Dispose()
        {
            // Finaliza thread de bind, caso houver
            this._BindThread = this._BindThread.SafeAbort();

            // Possui socket?
            BroadcastSocket socket = this._Socket;
            this._Socket = null;
            if (socket != null)
            {
                // Libera eventos e socket
                socket.Received -= this.Socket_Received;
                socket.Dispose();
            }
        }

        /// <summary>
        /// Enumera todos os servidores encontrados
        /// </summary>
        /// <returns>Servidores</returns>
        public IEnumerator<ServerInfo> GetEnumerator()
        {
            // Sincroniza
            lock (this._SyncRoot)
            {
                // Momento atual
                int now = Environment.TickCount;

                // Para cada servidor
                for (int i = 0, c = this._Servers.Count; i < c; i++)
                {
                    // Servidor válido?
                    ServerInfo server = this._Servers[i];
                    if ((server != null) && ((now - server.LastTouch) < this._DeadThreshold))
                    {
                        // Incrementa servidor ao retorno
                        yield return server;
                    }
                }
            }
        }

        /// <summary>
        /// Enumera todos os servidores encontrados
        /// </summary>
        /// <returns>Servidores</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Repassa
            return this.GetEnumerator();
        }
        #endregion

        // Métodos privados
        #region Private
        /// <summary>
        /// Thread para binding
        /// </summary>
        private void BindThread()
        {
            try
            {
                // Loop
                while (true)
                {
                    // Possui socket e conseguiu executar o bind?
                    BroadcastSocket socket = this._Socket;
                    if ((socket != null) && (socket.TryBind()))
                    {
                        // Linka recebimento de pacotes
                        socket.Received += this.Socket_Received;

                        // Sai do laço
                        break;
                    }

                    // Aguarda até próxima tentativa
                    Thread.Sleep(2000);
                }
            }

            // Ignora thread abortada
            catch (ThreadAbortException ex) { ex.Ignore(); }

            // Reporta exceções
            catch (Exception ex) { ex.Log(); }

            // Libera referência da thread
            this._BindThread = null;
        }

        /// <summary>
        /// Pacote de dados recebido
        /// </summary>
        private void Socket_Received(object sender, BroadcastReceivedEventArgs e)
        {
            try
            {
                // Dados válidos?
                if (e.Data.Length > 4)
                {
                    // Calcula e valida CRC
                    uint inputcrc = BitConverter.ToUInt32(e.Data, 0);
                    uint computedcrc = CRC32.Compute(e.Data, 4, e.Data.Length - 4);
                    if (inputcrc == computedcrc)
                    {
                        // Cria stream com os dados recebidos
                        using (MemoryStream stream = new MemoryStream(e.Data, 4, e.Data.Length - 4, false))
                        {
                            // Inicia leitor XML
                            using (XmlReader xml = XmlReader.Create(stream))
                            {
                                // Momento atual
                                int now = Environment.TickCount;

                                // Executa primeira leitura
                                while (xml.Read())
                                {
                                    // Está numa estrutura de informação de servidor?
                                    if ((xml.NodeType == XmlNodeType.Element) && (xml.Name == "Server"))
                                    {
                                        // Servidor adicionado na lista?
                                        bool added = false;

                                        // Recupera informações do servidor
                                        ServerInfo info = new ServerInfo(xml, now, e.Source);

                                        // Sincroniza
                                        lock (this._SyncRoot)
                                        {
                                            // Tenta recuperar elemento na lista
                                            ServerInfo server = this._Servers.FirstOrDefault(x => x.Name == info.Name);
                                            if (server != null)
                                            {
                                                // Servidor já está na lista, mas já está considerado como morto?
                                                // Então simula que foi adicionado
                                                added |= (now - server.LastTouch) > this._DeadThreshold;

                                                // Adiciona endereço na lista
                                                server.Update(info, e.Source, now);
                                            }
                                            else
                                            {
                                                // Adiciona servidor na lista
                                                this._Servers.Add(info);
                                                added = true;
                                            }
                                        }

                                        // Se adicionado (agora já fora do lock)
                                        if (added)
                                        {
                                            // Dispara evento de adição
                                            EventHandler<ServerEventArgs> foundhandler = this.Found;
                                            foundhandler?.Invoke(this, new ServerEventArgs(info));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Reporta exceções
            catch (Exception ex) { ex.Log(); }
        }
        #endregion
    }
}
