using NetworkCheckers.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace NetworkCheckers.Network.Services
{
    /// <summary>
    /// Informações sobre serviço
    /// </summary>
    public sealed class ServiceInfo
    {
        // Variáveis
        #region Variables

        /// <summary>
        /// Nome do serviço disponibilizado
        /// </summary>
        private readonly string _Name;

        /// <summary>
        /// Parâmetros extras
        /// </summary>
        private IDictionary<string, string> _Parameters;

        /// <summary>
        /// Porta do serviço IPv4
        /// </summary>
        private readonly ushort _Port;

        /// <summary>
        /// Servidor pai
        /// </summary>
        private ServerInfo _Server;

        /// <summary>
        /// Elemento associado, não salvo nem passado via XML
        /// </summary>
        private object _Tag;

        /// <summary>
        /// Carregamento via xml?
        /// </summary>
        private bool _Xml;

        #endregion

        // Construtores
        #region Constructors

        /// <summary>
        /// Nova informação de serviço
        /// </summary>
        /// <param name="name">Nome do serviço</param>
        /// <param name="port">Porta de conexão IPv4 e IPv6</param>
        public ServiceInfo(string name, ushort port)
        {
            // Valida entradas
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            // Guarda parâmetros
            this._Name = name;
            this._Port = port;

            // Inicializa parâmetros
            this._Parameters = new Dictionary<string, string>();
        }

        #endregion

        // Propriedades
        #region Properties

        /// <summary>
        /// Endereços de conexão ao serviço
        /// </summary>
        public IEnumerable<IPEndPoint> Address
        {
            // Recupera
            get
            {
                // Há endereços?
                List<IPAddress> addresses = this._Server?.AddressInternal;
                if (addresses != null)
                {
                    // Para cada endereço
                    for (int i = 0, c = addresses.Count; i < c; i++)
                    {
                        // Recupera endereço
                        IPAddress address = addresses[i];
                        if (address != null)
                        {
                            // IPv4?
                            if ((address.AddressFamily == AddressFamily.InterNetwork) && (this._Port != 0))
                            {
                                // Concatena com porta local
                                yield return new IPEndPoint(address, this._Port);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Nome do serviço disponível
        /// </summary>
        public string Name
        {
            // Recupera
            get { return this._Name; }
        }

        /// <summary>
        /// Parâmetros extras
        /// </summary>
        public IDictionary<string, string> Parameters
        {
            // Recupera
            get { return this._Parameters; }
        }

        /// <summary>
        /// Acesso ao servidor deste serviço
        /// </summary>
        public ServerInfo Server
        {
            // Recupera
            get { return this._Server; }
        }

        /// <summary>
        /// Elemento associado, não recuperado nem passado via rede
        /// </summary>
        public object Tag
        {
            // Recupera
            get { return this._Tag; }

            // Atualiza
            set { this._Tag = value; }
        }

        #endregion

        // Eventos
        #region Events

        /// <summary>
        /// Informações atualizadas do servidor
        /// </summary>
        public event EventHandler Updated
        {
            add
            {
                // Possui servidor?
                ServerInfo server = this._Server;
                if (server != null)
                {
                    // Adiciona evento ao servidor
                    server.Updated += value;
                }
            }

            remove
            {
                // Possui servidor?
                ServerInfo server = this._Server;
                if (server != null)
                {
                    // Remove evento ao servidor
                    server.Updated -= value;
                }
            }
        }

        #endregion

        // Métodos internos
        #region Internal

        /// <summary>
        /// Lê informações em XML
        /// </summary>
        /// <param name="reader">Leitor</param>
        /// <param name="parent">Servidor</param>
        internal ServiceInfo(XmlReader reader, ServerInfo parent)
        {
            // Indica carregamento por xml
            this._Xml = true;

            // Guarda referência do pai
            this._Server = parent;

            // Recupera nome
            this._Name = reader.GetAttribute("name");
            if (this._Name == null) throw new ArgumentNullException(nameof(this._Name));

            // Tenta recuperar porta comum
            if (!ushort.TryParse(reader.GetAttribute("port"), out this._Port))
            {
                throw new ArgumentNullException(nameof(this._Port));
            }

            // Inicializa parâmetros
            Dictionary<string, string> parameters = new Dictionary<string, string>(0);
            this._Parameters = new ReadOnlyDictionary<string, string>(parameters);

            // Possui parâmetros extras?
            if (!reader.IsEmptyElement)
            {
                // Chave de parâmetro
                string key = null;

                // Enquanto ler elementos
                while (reader.Read())
                {
                    // Verifica tipo de dado
                    switch (reader.NodeType)
                    {
                        // Elemento
                        case XmlNodeType.Element:
                            key = reader.Name;
                            break;

                        // Texto
                        case XmlNodeType.Text:

                            // Mas não há chave definida?
                            if (key == null)
                                throw new ArgumentNullException(nameof(key));

                            // Adiciona parâmetro
                            parameters.Add(key, reader.Value);
                            break;

                        // Fechamento de elemento
                        case XmlNodeType.EndElement:

                            // Possui chave?
                            if (key != null)
                            {
                                // Libera chave
                                key = null;
                            }
                            else
                            {
                                // Não há chave, chegou ao fim de informação do serviço
                                return;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Grava informações em XML
        /// </summary>
        /// <param name="writer">Gravador</param>
        internal void Write(XmlWriter writer)
        {
            // Grava valores
            writer.WriteAttribute("name", this._Name);

            // Grava porta
            writer.WriteAttribute("port", this._Port);

            // Possui parâmetros extras?
            if (this._Parameters != null)
            {
                // Para cada parâmetro extra
                foreach (KeyValuePair<string, string> pair in this._Parameters)
                {
                    // Grava valores
                    writer.WriteElement(pair.Key, pair.Value);
                }
            }
        }

        #endregion
    }
}
