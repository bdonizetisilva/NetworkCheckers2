using NetworkCheckers.Utils;
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace NetworkCheckers.Network.Services
{
    /// <summary>
    /// Publicador de servidor
    /// </summary>
    public sealed class ServerPublisher : IDisposable
    {
        // Variáveis
        #region Variables
        /// <summary>
        /// Notificação ativa
        /// </summary>
        private bool _Active;

        /// <summary>
        /// Dados a serem enviados
        /// </summary>
        private byte[] _Data;

        /// <summary>
        /// Thread responsável pelo envio dos dados
        /// </summary>
        private Thread _SendThread;

        /// <summary>
        /// Informações sobre o servidor
        /// </summary>
        private readonly ServerInfo _Server;
        #endregion

        // Construtores
        #region Constructors
        /// <summary>
        /// Nova descoberta de serviço
        /// </summary>
        public ServerPublisher(ServerInfo server, ushort port)
        {
            // Valida entrada
            if (server == null)
                throw new ArgumentNullException(nameof(server));
            if (port == 0)
                throw new ArgumentOutOfRangeException(nameof(port));

            // Cria novo socket de envio
            BroadcastSocket socket = new BroadcastSocket(port);

            // Guarda referência ao servidor
            this._Server = server;

            // Atualiza dados
            this.Update();

            // Executa o primeiro envio para teste
            socket.SendSync(this._Data);

            // Cria e inicializa thread de envio
            this._SendThread = new Thread(this.SendThread)
            {
                Priority = ThreadPriority.Lowest
            };

            this._Active = true;
            this._SendThread.Start(new object[] { socket });
        }
        #endregion

        // Métodos públicos
        #region Public
        /// <summary>
        /// Finalização
        /// </summary>
        ~ServerPublisher()
        {
            // Libera recursos
            this.Dispose();
        }

        /// <summary>
        /// Libera recursos alocados
        /// </summary>
        public void Dispose()
        {
            // Indica que finalizou
            this._Active = false;

            // Aborta thread
            this._SendThread = this._SendThread.SafeAbort();
        }

        /// <summary>
        /// Atualiza impressão de dados referente ao servidor
        /// </summary>
        public void Update()
        {
            // Binário para dados de descoberta
            using (MemoryStream memory = new MemoryStream())
            {
                // Salta 4bytes referentes ao CRC
                memory.Write(new byte[4], 0, 4);

                // Parâmetros do XML
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    CheckCharacters = true,
                    CloseOutput = false,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    Indent = false,
                    OmitXmlDeclaration = true
                };

                // Gravação de XML
                using (XmlWriter xml = XmlWriter.Create(memory, settings))
                {
                    // Grava informações sobre o serviço
                    xml.WriteStartElement("Server");
                    this._Server.Write(xml);
                    xml.WriteEndElement();
                }

                // Volta a posição referente aos dados, calcula e grava CRC
                memory.Position = 4;
                uint crc = CRC32.Compute(memory);
                memory.Position = 0;
                memory.Write(BitConverter.GetBytes(crc), 0, 4);

                // Recupera dados em binário
                this._Data = memory.ToArray();
            }
        }
        #endregion

        // Métodos privados
        #region Private
        /// <summary>
        /// Thread de notificação em rede
        /// </summary>
        private void SendThread(object args)
        {
            try
            {
                // Recupera argumentos de entrada
                object[] input = (object[])args;
                BroadcastSocket socket = (BroadcastSocket)input[0];

                // Aguarda até próximo envio
                Thread.Sleep(1000);

                // Enquanto estiver com serviço ativo
                while (this._Active)
                {
                    try
                    {
                        // Envia dados
                        socket.SendSync(this._Data);

                        // Aguarda até próximo envio
                        Thread.Sleep(2000);
                    }

                    // Thread abortada, redispara
                    catch (ThreadAbortException) { throw; }

                    // Reporta exceções
                    catch (Exception ex) { ex.Log(); }
                }
            }

            // Ignora thread abortada
            catch (ThreadAbortException ex) { ex.Ignore(); }

            // Reporta outras exceções
            catch (Exception ex) { ex.Log(); }
        }
        #endregion
    }
}
