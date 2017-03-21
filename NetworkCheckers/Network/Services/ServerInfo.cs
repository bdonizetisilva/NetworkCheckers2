using NetworkCheckers.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml;

namespace NetworkCheckers.Network.Services
{
    /// <summary>
    /// Informações sobre um servidor
    /// </summary>
    public sealed class ServerInfo : IComparable<ServerInfo>, IEquatable<ServerInfo>, IComparable<ulong>, IEnumerable<ServiceInfo>
    {
        // Variáveis
        #region Variables
        /// <summary>
        /// Endereços conhecidos do mesmo servidor
        /// </summary>
        private List<IPAddress> _Address;

        /// <summary>
        /// Descrição breve sobre o servidor
        /// </summary>
        private string _Description;

        /// <summary>
        /// Identificador do servidor
        /// </summary>
        private ulong _Id;

        /// <summary>
        /// Hash do identificador
        /// </summary>
        private int _IdHash;

        /// <summary>
        /// Nome do servidor
        /// </summary>
        private string _Name;

        /// <summary>
        /// Parâmetros extras
        /// </summary>
        private IDictionary<string, string> _Parameters;

        /// <summary>
        /// Serviços disponíveis
        /// </summary>
        private IDictionary<string, ServiceInfo> _Services;

        /// <summary>
        /// Último momento em que houve recebimento do servidor
        /// </summary>
        private int _Touch;
        #endregion

        // Construtores
        #region Constructors
        /// <summary>
        /// Novo conjunto de informações de servidor
        /// </summary>
        public ServerInfo()
        {
            // Sorteia identificador único da seção
            this.Id = Randomizer.NextUInt64();
            this._Parameters = new Dictionary<string, string>();

            // Nova lista de serviços
            this._Services = new Dictionary<string, ServiceInfo>();
        }
        #endregion

        // Propriedades
        #region Properties
        /// <summary>
        /// Endereços conhecidos do servidor
        /// </summary>
        public IEnumerable<IPAddress> Address
        {
            // Recupera
            get { return this._Address; }
        }

        /// <summary>
        /// Último momento em que servidor esteve ativo na rede
        /// </summary>
        public DateTime Alive
        {
            // Recupera
            get
            {
                // Sem informação? Retorna menor valor possível
                if (this._Touch == 0)
                    return DateTime.MinValue;

                // Quanto tempo o servidor está sem responder (em ms)
                int dead = this._Touch - Environment.TickCount;

                // Desconta o tempo do atual
                return DateTime.FromFileTimeUtc(DateTime.UtcNow.Ticks - (dead * 10000));
            }
        }

        /// <summary>
        /// Descrição do servidor
        /// </summary>
        public string Description
        {
            // Recupera
            get { return this._Description; }

            // Atualiza
            set
            {
                // Está do lado do cliente? Impede alteração
                if (this.IsClientSide)
                    throw new InvalidOperationException();

                // Atualiza descrição
                this._Description = value;
            }
        }

        /// <summary>
        /// Identificador da seção do servidor
        /// </summary>
        /// <remarks>Identificador gerado aleatoriamente a cada execusão de disponibilidade de serviço</remarks>
        public ulong Id
        {
            // Recupera
            get { return this._Id; }

            // Atualiza
            set
            {
                // Está do lado do cliente? Impede alteração
                if (this.IsClientSide)
                    throw new InvalidOperationException();

                // Atualiza valor
                this._Id = value;

                // Atualiza hashing do ID
                this._IdHash = unchecked((int)CRC32.Compute(this._Id));
            }
        }

        /// <summary>
        /// Instância do lado do cliente
        /// </summary>
        public bool IsClientSide
        {
            // Se já possuir endereços associados
            get { return (this._Address != null); }
        }

        /// <summary>
        /// Instância do lado do servidor
        /// </summary>
        public bool IsServerSide
        {
            // Não possuir listagem de endereços associados
            get { return (this._Address == null); }
        }

        /// <summary>
        /// Nome do servidor
        /// </summary>
        public string Name
        {
            // Recupera
            get { return this._Name; }

            // Atualiza
            set
            {
                // Está do lado do cliente? Impede alteração
                if (this.IsClientSide)
                    throw new InvalidOperationException();

                // Atualiza nome
                this._Name = value;
            }
        }

        /// <summary>
        /// Parâmetros extras do servidor
        /// </summary>
        public IDictionary<string, string> Parameters
        {
            // Recupera
            get { return this._Parameters; }
        }

        /// <summary>
        /// Serviços disponíveis
        /// </summary>
        public IDictionary<string, ServiceInfo> Services
        {
            // Recupera
            get { return this._Services; }
        }

        /// <summary>
        /// Endereços conhecidos do servidor
        /// </summary>
        internal List<IPAddress> AddressInternal
        {
            // Recupera
            get { return this._Address; }
        }

        /// <summary>
        /// Momento de última atualização
        /// </summary>
        internal int LastTouch
        {
            // Recupera
            get { return this._Touch; }
        }
        #endregion

        // Eventos
        #region Events
        /// <summary>
        /// Informações atualizadas do servidor
        /// </summary>
        public event EventHandler Updated;
        #endregion

        // Métodos internos
        #region Internal
        /// <summary>
        /// Carrega informações do XML
        /// </summary>
        /// <param name="reader">Leitor XML</param>
        /// <param name="touch">Momento de criação</param>
        /// <param name="source">Fonte de dados</param>
        internal ServerInfo(XmlReader reader, int touch, IPAddress source)
        {
            // Valida entrada
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            // Recupera identificador
            if (!ulong.TryParse(reader.GetAttribute("id"), out this._Id))
                throw new ArgumentNullException(nameof(this._Id));

            // Calcula hash da id
            this._IdHash = unchecked((int)CRC32.Compute(this._Id));

            // Recupera nome e descrição
            this._Name = reader.GetAttribute("name");
            this._Description = reader.GetAttribute("description");

            // Inicializa lista de serviços
            Dictionary<string, ServiceInfo> services = new Dictionary<string, ServiceInfo>();
            this._Services = new ReadOnlyDictionary<string, ServiceInfo>(services);

            // Cria lista de parâmetros extras
            Dictionary<string, string> parameters = new Dictionary<string, string>(0);
            this._Parameters = new ReadOnlyDictionary<string, string>(parameters);

            // Possui elementos?
            if (!reader.IsEmptyElement)
            {
                // Enquanto ler elementos
                while (reader.Read())
                {
                    // Verifica tipo de dado
                    switch (reader.NodeType)
                    {
                        // Elemento
                        case XmlNodeType.Element:

                            // Verifica tipo de dado
                            switch (reader.Name)
                            {
                                // Serviço
                                case "Service":

                                    // Recupera informações do serviço
                                    ServiceInfo service = new ServiceInfo(reader, this);

                                    // Adiciona ao dicionário
                                    services.Add(service.Name, service);
                                    break;

                                // Outros
                                default:

                                    // Inicializa chave
                                    string key = reader.Name;

                                    // Quantidade de fechamentos esperados
                                    int findcloses = 1;

                                    // Enquanto ler elementos
                                    while (reader.Read())
                                    {
                                        // Verifica tipo de dado
                                        switch (reader.NodeType)
                                        {
                                            // Elemento
                                            case XmlNodeType.Element:
                                                key = reader.Name;
                                                findcloses++;
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
                                                findcloses--;
                                                break;
                                        }

                                        // Fechou todos os elements esperados? Sai do laço
                                        if (findcloses == 0)
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }

            // Inicia touch
            this._Touch = touch;

            // Inicia listagem de endereços
            this._Address = new List<IPAddress>(2);
            if (source != null)
                this._Address.Add(source);
        }

        /// <summary>
        /// Atualizando informações
        /// </summary>
        /// <param name="source">Fonte de informações, deve possuir mesma ID</param>
        /// <param name="from">Endereço de origem das informações</param>
        /// <param name="touch">Momento da chegada de informações</param>
        internal void Update(ServerInfo source, IPAddress from, int touch)
        {
            // Está do lado do servidor? Não faz sentido adicionar endereço, este é um papel do Broadcast
            if (this.IsServerSide)
                throw new InvalidOperationException();

            // Atualiza momento da alteração
            this._Touch = touch;

            // Se houve alterações no servidor
            bool changed = false;

            // Endereço válido?
            if (from != null)
            {
                // Não possui o endereço ainda?
                if (!this._Address.Contains(from))
                {
                    // Adiciona na lista de endereços
                    this._Address.Add(from);

                    // Força que houve atualizações
                    changed = true;
                }
            }

            // Há alterações no nome ou descrição do servidor? Indica alterações
            if ((!string.Equals(this._Name, source._Name)) || (!string.Equals(this._Description, source._Description)))
            {
                changed = true;

                // Atualiza valores
                this._Name = source._Name;
                this._Description = source._Description;
            }


            // Houve atualização? Dispara
            if (changed)
                this.Updated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gravador XML
        /// </summary>
        /// <param name="writer">Gravador</param>
        internal void Write(XmlWriter writer)
        {
            // Valida entrada
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            // Grava identificador
            writer.WriteAttribute("id", this._Id);

            // Grava nome e descrição
            if (this._Name != null)
                writer.WriteAttribute("name", this._Name);
            if (this._Description != null)
                writer.WriteAttribute("description", this._Description);

            // Possui serviços?
            if (this._Services != null)
            {
                // Para cada serviço disponibilizado
                foreach (ServiceInfo service in this._Services.Values)
                {
                    // Grava dados de serviço
                    writer.WriteStartElement("Service");
                    service.Write(writer);
                    writer.WriteEndElement();
                }
            }

            // Para cada parâmetro extra
            foreach (KeyValuePair<string, string> pair in this._Parameters)
            {
                // Imprime valor
                writer.WriteElement(pair.Key, pair.Value);
            }
        }
        #endregion

        // Métodos públicos
        #region Public
        /// <summary>
        /// Tenta recuperar serviço
        /// </summary>
        /// <param name="name">Nome do serviço</param>
        /// <returns>Informações do serviço ou <i>null</i> caso não encontrar</returns>
        public ServiceInfo this[string name]
        {
            get
            {
                // Recupera serviços
                IDictionary<string, ServiceInfo> services = this._Services;
                if (services != null)
                {
                    // Tenta recuperar serviço
                    if (services.TryGetValue(name, out ServiceInfo service))
                        return service;
                }

                // Não possui
                return null;
            }
        }

        /// <summary>
        /// Adiciona informações de serviço na lista
        /// </summary>
        /// <param name="info">Informações a adicionar</param>
        public void Add(ServiceInfo info)
        {
            // Está do lado do cliente? Impede alteração
            if (this.IsClientSide)
                throw new InvalidOperationException();

            // Valida entrada
            if (info != null)
            {
                // Adiciona na coleção
                this._Services.Add(info.Name, info);
            }
        }

        /// <summary>
        /// Compara elemento com identificador
        /// </summary>
        /// <param name="other">Elemento a comparar</param>
        /// <returns>Comparador</returns>
        public int CompareTo(ulong other)
        {
            // Compara identificador
            return this._Id.CompareTo(other);
        }

        /// <summary>
        /// Compara informações de serviço
        /// </summary>
        /// <param name="other">Elemento a comparar</param>
        /// <returns>Comparador</returns>
        public int CompareTo(ServerInfo other)
        {
            // Entrada nula? Este é maior
            if (other == null)
                return 1;

            // Compara valores
            return this._Id.CompareTo(other._Id);
        }

        /// <summary>
        /// Verifica igualdade de instâncias
        /// </summary>
        /// <param name="other">Elemento a verificar</param>
        /// <returns>Iguais</returns>
        public bool Equals(ServerInfo other)
        {
            // Entrada não nula?
            if (other != null)
            {
                // Identificadores iguais?
                return this._Id == other._Id;
            }

            // Não iguais
            return false;
        }

        /// <summary>
        /// Verifica igualdade de instâncias
        /// </summary>
        /// <param name="obj">Elemento a verificar</param>
        /// <returns>Iguais</returns>
        public override bool Equals(object obj)
        {
            // Compara com entrada esperada
            return this.Equals(obj as ServerInfo);
        }

        /// <summary>
        /// Gera enumerador dos serviços disponíveis
        /// </summary>
        /// <returns>Enumerador</returns>
        public IEnumerator<ServiceInfo> GetEnumerator()
        {
            // Recupera enumerador dos serviços
            return this._Services.Values.GetEnumerator();
        }

        /// <summary>
        /// Gera enumerador dos serviços disponíveis
        /// </summary>
        /// <returns>Enumerador</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Recupera enumerador dos serviços
            return this._Services.Values.GetEnumerator();
        }

        /// <summary>
        /// Hashing do servidor
        /// </summary>
        /// <returns>Hash</returns>
        public override int GetHashCode()
        {
            // Retorna identificador
            return this._IdHash.GetHashCode();
        }

        /// <summary>
        /// Representatividade do servidor
        /// </summary>
        /// <returns>Texto representativo</returns>
        public override string ToString()
        {
            // Possui nome?
            if (!string.IsNullOrWhiteSpace(this._Name))
            {
                // Retorna nome
                return this._Name + " [" + this._Id.GetHashCode() + "]";
            }

            // Somente identificador
            return this._Id.GetHashCode().ToString();
        }
        #endregion
    }
}
