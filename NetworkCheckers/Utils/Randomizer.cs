using System;

namespace NetworkCheckers.Utils
{
    /// <summary>
    /// Auxiliar de randomizador
    /// </summary>
    public static class Randomizer
    {
        // Estáticos
        #region Statics
        /// <summary>
        /// Classe randomizadora
        /// </summary>
        private static readonly Random _Random = new Random();
        #endregion

        // Métodos públicos
        #region Public
        /// <summary>
        /// Gera bytes randômicos
        /// </summary>
        /// <param name="length">Quantidade de bytes</param>
        /// <returns>Bytes</returns>
        public static byte[] NextBytes(int length)
        {
            // Buffer auxiliar
            byte[] buff = new byte[length];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return buff;
        }

        /// <summary>
        /// Gera randômico 64bits em ponto flutuante
        /// </summary>
        /// <returns>Número</returns>
        public static double NextDouble()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(double)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToDouble(buff, 0);
        }

        /// <summary>
        /// Gera randômico 16bits
        /// </summary>
        /// <returns>Número</returns>
        public static short NextInt16()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(short)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToInt16(buff, 0);
        }

        /// <summary>
        /// Gera randômico 32bits
        /// </summary>
        /// <returns>Número</returns>
        public static int NextInt32()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(int)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToInt32(buff, 0);
        }

        /// <summary>
        /// Gera randômico 64bits
        /// </summary>
        /// <returns>Número</returns>
        public static long NextInt64()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(long)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToInt64(buff, 0);
        }

        /// <summary>
        /// Gera randômico 32bits em ponto flutuante
        /// </summary>
        /// <returns>Número</returns>
        public static float NextSingle()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(float)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToSingle(buff, 0);
        }

        /// <summary>
        /// Gera randômico 16bits
        /// </summary>
        /// <returns>Número</returns>
        public static ushort NextUInt16()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(ushort)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToUInt16(buff, 0);
        }

        /// <summary>
        /// Gera randômico 32bits
        /// </summary>
        /// <returns>Número</returns>
        public static uint NextUInt32()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(uint)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToUInt32(buff, 0);
        }

        /// <summary>
        /// Gera randômico 64bits
        /// </summary>
        /// <returns>Número</returns>
        public static ulong NextUInt64()
        {
            // Buffer auxliar
            byte[] buff = new byte[sizeof(ulong)];

            // Randomiza bytes
            Randomizer._Random.NextBytes(buff);

            // Retorna
            return BitConverter.ToUInt64(buff, 0);
        }
        #endregion
    }
}
