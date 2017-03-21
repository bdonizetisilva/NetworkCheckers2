using System;
using System.IO;

namespace NetworkCheckers.Utils
{
    /// <summary>
    /// Cyclic Redundancy Check de 32bits
    /// </summary>
    public unsafe class CRC32
    {
        // Constantes
        #region Consts
        /// <summary>
        /// Tabela polinominal
        /// </summary>
        public readonly static uint[] Table = new uint[] {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419,
            0x706AF48F, 0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4,
            0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07,
            0x90BF1D91, 0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
            0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
            0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
            0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4,
            0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
            0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3,
            0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A,
            0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599,
            0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
            0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190,
            0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
            0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E,
            0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
            0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED,
            0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
            0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3,
            0xFBD44C65, 0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
            0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A,
            0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5,
            0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
            0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
            0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17,
            0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6,
            0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615,
            0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
            0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344,
            0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
            0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A,
            0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1,
            0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C,
            0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF,
            0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
            0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE,
            0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31,
            0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C,
            0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
            0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
            0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
            0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1,
            0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
            0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278,
            0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7,
            0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66,
            0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
            0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605,
            0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
            0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B,
            0x2D02EF8D
        };

        /// <summary>
        /// Tamanho do buffer de leitura em bytes
        /// </summary>
        private const int BUFFER_SIZE = 0x10000; // 64Kb

        /// <summary>
        /// Valor inicial padrão
        /// </summary>
        private const uint DEFAULT_SEED = 0xFFFFFFFF;
        #endregion

        // Variáveis
        #region Variables
        /// <summary>
        /// Valor atual
        /// </summary>
        private uint _Value;
        #endregion

        // Construtores
        #region Constructors
        /// <summary>
        /// Novo cálculo de CRC
        /// </summary>
        public CRC32()
        {
            // Inicializa
            this.Reset();
        }
        #endregion

        // Propriedades
        #region Properties
        /// <summary>
        /// Valor calculado
        /// </summary>
        public uint Value
        {
            // Recupera valor
            get { return this._Value ^ CRC32.DEFAULT_SEED; }
        }
        #endregion

        // Métodos públicos
        #region Public
        /// <summary>
        /// Reinicia cálculo do CRC
        /// </summary>
        public void Reset()
        {
            // Reinicia valor
            this._Value = 0 ^ CRC32.DEFAULT_SEED;
        }

        /// <summary>
        /// Atualiza CRC
        /// </summary>
        /// <param name="value">Valor de entrada</param>
        public void Update(byte value)
        {
            // Calcula hashing
            this._Value = CRC32.Table[(this._Value ^ value) & 0xFF] ^ (this._Value >> 8);
        }

        /// <summary>
        /// Atualiza CRC
        /// </summary>
        /// <param name="input">Buffer de dados</param>
        /// <param name="offset">Posição inicial no buffer</param>
        /// <param name="count">Quantidade de bytes a processar</param>
        public void Update(byte[] input, int offset, int count)
        {
            // Valida entradas
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset + count > input.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            // Calcula limite
            int limit = offset + count;

            // Fixa entrada
            fixed (byte* pi = input)
            {
                // Fixa tabela
                fixed (uint* table = CRC32.Table)
                {
                    // Para cada byte
                    for (int i = offset; i < limit; i++)
                    {
                        // Atualiza
                        this._Value = table[(this._Value ^ pi[i]) & 0xFF] ^ (this._Value >> 8);
                    }
                }
            }
        }
        #endregion

        // Métodos estáticos auxiliares
        #region Statics

        // Stream
        #region Stream
        /// <summary>
        /// Calcula valor de hash CRC32 de um stream
        /// </summary>
        /// <param name="input">Leitura de dados</param>
        /// <returns>CRC32</returns>
        public static uint Compute(Stream input)
        {
            // Calcula hashing
            return Compute(input, long.MaxValue);
        }

        /// <summary>
        /// Calcula valor de hash CRC32 de um stream
        /// </summary>
        /// <param name="input">Leitura de dados</param>
        /// <param name="count">Quantidade máxima de bytes a serem lidos</param>
        /// <returns>CRC32</returns>
        public static uint Compute(Stream input, long count)
        {
            // Valida entradas
            if (input == null) throw new ArgumentNullException(nameof(input));

            // Buffer de leitura
            byte[] buffer = new byte[BUFFER_SIZE];

            // Valida posição máxima de leitura
            count = Math.Min(count, input.Length - input.Position);

            // Inicializa valor
            uint crc = 0;

            unchecked
            {
                // Fixa buffer
                fixed (byte* pb = buffer)
                {
                    // Fixa tabela
                    fixed (uint* table = CRC32.Table)
                    {
                        // Loop de leitura
                        while (count > 0)
                        {
                            // Quantidade de dados a serem lidos
                            int toread = (count > BUFFER_SIZE) ? BUFFER_SIZE : (int)count;

                            // Executa leitura do stream
                            int read = input.Read(buffer, 0, toread);
                            if (read > 0)
                            {
                                // Iniciando valor
                                crc ^= CRC32.DEFAULT_SEED;

                                // Para cada valor lido
                                for (int i = 0; i < read; i++)
                                {
                                    // Calcula hashing
                                    crc = table[(crc ^ pb[i]) & 0xFF] ^ (crc >> 8);
                                }

                                // Finaliza valor
                                crc ^= CRC32.DEFAULT_SEED;

                                // Decrementa quantidade de dados para serem lidos
                                count -= read;
                            }
                            else
                            {
                                // Não há mais dados para serem lidos, sai do laço
                                break;
                            }
                        }
                    }
                }
            }

            // Retorna valor calculado
            return crc;
        }

        /// <summary>
        /// Calcula hashing CRC32 de um stream
        /// </summary>
        /// <param name="input">Leitura de dados</param>
        /// <returns>CRC32</returns>
        public static byte[] ComputeBytes(Stream input)
        {
            // Calcula valor
            uint crc = Compute(input, long.MaxValue);

            // Transforma em vetor de bytes
            return new byte[] { (byte)(crc >> 24), (byte)(crc >> 16), (byte)(crc >> 8), (byte)(crc) };
        }

        /// <summary>
        /// Calcula hashing CRC32 de um stream
        /// </summary>
        /// <param name="input">Leitura de dados</param>
        /// <param name="count">Quantidade máxima de bytes a serem lidos</param>
        /// <returns>CRC32</returns>
        public static byte[] ComputeBytes(Stream input, long count)
        {
            // Calcula valor
            uint crc = Compute(input, count);

            // Transforma em vetor de bytes
            return new byte[] { (byte)(crc >> 24), (byte)(crc >> 16), (byte)(crc >> 8), (byte)(crc) };
        }

        #endregion

        // Bytes
        #region Bytes
        /// <summary>
        /// Calcula valor de hash CRC32 de um array de bytes
        /// </summary>
        /// <param name="data">Dados a serem calculados</param>
        /// <returns>CRC32</returns>
        public static uint Compute(params byte[] data)
        {
            // Valida entrada
            if (data == null) throw new ArgumentNullException(nameof(data));

            // Calcula hashing
            return Compute(data, 0, data.Length);
        }

        /// <summary>
        /// Calcula valor de hash CRC32 de um array de bytes
        /// </summary>
        /// <param name="input">Dados a serem calculados</param>
        /// <param name="offset">Posição inicial de leitura</param>
        /// <param name="count">Quantidade de elementos a serem lidos</param>
        /// <returns>CRC32</returns>
        public static uint Compute(byte[] input, int offset, int count)
        {
            // Valida entrada
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if ((offset + count) > input.Length) throw new ArgumentOutOfRangeException(nameof(offset));

            // Inicia valor
            uint crc = 0;

            unchecked
            {
                // Fixa entrada de dados
                fixed (byte* pi = input)
                {
                    // Fixa tabela
                    fixed (uint* table = CRC32.Table)
                    {
                        // Última posição de leitura
                        int last = offset + count;

                        // Inicia valor
                        crc ^= CRC32.DEFAULT_SEED;

                        // Para cada valor
                        for (int i = offset; i < last; i++)
                        {
                            // Calcula hashing
                            crc = table[(crc ^ pi[i]) & 0xFF] ^ (crc >> 8);
                        }

                        // Finaliza valor
                        return crc ^ CRC32.DEFAULT_SEED;
                    }
                }
            }
        }

        /// <summary>
        /// Calcula valor de hash CRC32 de dados de memória
        /// </summary>
        /// <param name="data">Dados a serem calculados</param>
        /// <param name="count">Quantidade de bytes a serem lidos</param>
        /// <returns>CRC32</returns>
        public unsafe static uint Compute(byte* data, int count)
        {
            // Valida entrada
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            // Inicia valor
            uint crc = 0;

            unchecked
            {
                // Última posição de leitura
                byte* last = data + count;

                // Inicia valor
                crc ^= CRC32.DEFAULT_SEED;

                // Fixa tabela
                fixed (uint* table = CRC32.Table)
                {
                    // Para cada valor
                    for (byte* value = data; value < last; value++)
                    {
                        // Calcula hashing
                        crc = table[(crc ^ *value) & 0xFF] ^ (crc >> 8);
                    }
                }

                // Finaliza valor
                return crc ^ CRC32.DEFAULT_SEED;
            }
        }
        #endregion

        // Números
        #region Numbers
        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="SByte"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params sbyte[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (sbyte* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(sbyte));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="Int16"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params short[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (short* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(short));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="UInt16"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params ushort[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (ushort* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(ushort));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="Int32"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params int[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (int* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(int));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="UInt32"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params uint[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (uint* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(uint));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="Int64"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params long[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (long* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(long));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="UInt64"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params ulong[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (ulong* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(ulong));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="Single"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params float[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (float* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(float));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="Double"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params double[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (double* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(double));
                }
            }

            // Não encontrado
            return 0;
        }

        /// <summary>
        /// Calcula valor de hashing de um array de <see cref="Decimal"/>
        /// </summary>
        /// <param name="input">Entrada de dados</param>
        /// <returns>CRC</returns>
        public unsafe static uint Compute(params decimal[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                // Fixa entrada
                fixed (decimal* data = input)
                {
                    // Calcula dados diretamente da memória
                    return Compute((byte*)data, input.Length * sizeof(decimal));
                }
            }

            // Não encontrado
            return 0;
        }
        #endregion

        // Genéricos
        #region Generic
        /// <summary>
        /// Calcula valor de hash em CRC32 de um array genérico
        /// </summary>
        /// <param name="input">Dados a serem calculados</param>
        /// <returns>CRC32</returns>
        public unsafe static uint Compute(params object[] input)
        {
            // Havendo elementos
            if ((input != null) && (input.Length > 0))
            {
                unchecked
                {
                    // Inicia valor
                    uint crc = 0 ^ CRC32.DEFAULT_SEED;

                    // Fixa tabela
                    fixed (uint* table = CRC32.Table)
                    {
                        // Para cada valor
                        foreach (object inobject in input)
                        {
                            // Calcula hashing do objeto
                            int hash = inobject != null ? inobject.GetHashCode() : 0;

                            // Fixa bytes
                            byte* data = (byte*)&hash;

                            // Calcula
                            crc = table[(crc ^ *data++) & 0xFF] ^ (crc >> 8);
                            crc = table[(crc ^ *data++) & 0xFF] ^ (crc >> 8);
                            crc = table[(crc ^ *data++) & 0xFF] ^ (crc >> 8);
                            crc = table[(crc ^ *data) & 0xFF] ^ (crc >> 8);
                        }
                    }

                    // Retorna valor
                    return crc ^ CRC32.DEFAULT_SEED;
                }
            }

            // Não encontrado
            return 0;
        }
        #endregion

        // Outros métodos
        #region Public
        /// <summary>
        /// Transforma bytes CRC32 em valor CRC32
        /// </summary>
        /// <param name="crc">Bytes CRC32 (4 bytes)</param>
        /// <returns>Valor CRC32</returns>
        public static uint GetValue(byte[] crc)
        {
            // Valida entrada
            if (crc == null) throw new ArgumentNullException(nameof(crc));
            if (crc.Length != 4) throw new ArgumentOutOfRangeException(nameof(crc));

            // Recupera valor
            return (uint)((crc[0] << 24) | (crc[1] << 16) | (crc[2] << 8) | crc[3]);
        }
        #endregion
        #endregion
    }
}
