using System.Net;

namespace NetworkCheckers.Network
{
    /// <summary>
    /// Dados recebidos em broadcast
    /// </summary>
    public sealed class BroadcastReceivedEventArgs
    {
        // Propriedades
        #region Properties

        /// <summary>
        /// Dados recebidos
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Origem dos dados, podendo ser <i>null</i> caso não identificado
        /// </summary>
        public IPAddress Source { get; private set; }

        #endregion

        // Construtores
        #region Constructors

        /// <summary>
        /// Novo pacote de eventos
        /// </summary>
        /// <param name="data">Dados recebidos</param>
        internal BroadcastReceivedEventArgs(byte[] data)
        {
            // Guarda dados
            this.Data = data;
        }

        /// <summary>
        /// Novo pacote de eventos
        /// </summary>
        /// <param name="data">Dados recebidos</param>
        /// <param name="source">Origem dos dados, pode ser <i>null</i></param>
        internal BroadcastReceivedEventArgs(byte[] data, EndPoint source)
        {
            // Guarda dados
            this.Data = data;

            // Origem informada?
            IPEndPoint ipsource = source as IPEndPoint;
            if (ipsource != null)
            {
                // Guarda endereço
                this.Source = new IPAddress(ipsource.Address.GetAddressBytes());
            }
        }

        #endregion
    }
}
