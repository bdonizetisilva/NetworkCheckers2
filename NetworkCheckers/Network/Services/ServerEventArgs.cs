using System;

namespace NetworkCheckers.Network.Services
{
    /// <summary>
    /// Servidor enc
    /// </summary>
    public sealed class ServerEventArgs : EventArgs
    {
        // Propriedades
        #region Properties
        /// <summary>
        /// Informações do servidor
        /// </summary>
        public ServerInfo Server { get; private set; }
        #endregion

        // Construtores
        #region Constructors
        /// <summary>
        /// Novo conjunto de informações
        /// </summary>
        /// <param name="server">Servidor</param>
        internal ServerEventArgs(ServerInfo server)
        {
            // Guarda parâmetros
            this.Server = server;
        }
        #endregion
    }
}
