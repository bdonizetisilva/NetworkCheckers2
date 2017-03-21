using System;
using System.Xml;

namespace NetworkCheckers.Utils
{
    /// <summary>
    /// Auxiliares da <see cref="XmlWriter"/>
    /// </summary>
    public static class XmlWriterHelper
    {
        // Constantes
        #region Consts
        /// <summary>
        /// Formatação de DataHora aceita em XML e MySQL
        /// </summary>
        private const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        #endregion

        #region WriteAttribute
        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, byte value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, sbyte value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, bool value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, DateTime value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value.ToString(DATETIME_FORMAT));
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, DateTimeOffset value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, decimal value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, double value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, float value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, short value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, ushort value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, int value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, uint value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue((long)value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, long value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, ulong value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, object value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(value);
            source.WriteEndAttribute();
        }

        /// <summary>
        /// Grava atributo diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do atributo</param>
        /// <param name="value">Valor do atributo</param>
        public static void WriteAttribute(this XmlWriter source, string name, string value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartAttribute(name);
            source.WriteValue(FilterText(value));
            source.WriteEndAttribute();
        }
        #endregion

        #region WriteElement
        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, byte value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, sbyte value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, bool value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, DateTime value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value.ToString(DATETIME_FORMAT));
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, DateTimeOffset value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, decimal value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, double value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, float value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, short value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, ushort value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, int value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, uint value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, long value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, ulong value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(XmlConvert.ToString(value));
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, object value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(value);
            source.WriteEndElement();
        }

        /// <summary>
        /// Grava elemento diretamente
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="name">Nome do elemento</param>
        /// <param name="value">Valor do elemento</param>
        public static void WriteElement(this XmlWriter source, string name, string value)
        {
            // Inicializa, grava valor, finaliza
            source.WriteStartElement(name);
            source.WriteValue(FilterText(value));
            source.WriteEndElement();
        }
        #endregion

        #region WriteValue
        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, byte? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue((int)value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, sbyte? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue((int)value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, DateTime? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(value.Value.ToString(DATETIME_FORMAT));
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, DateTimeOffset? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, decimal? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, double? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, float? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, short? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue((int)value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, ushort? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue((int)value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, int? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, uint? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue((long)value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, long? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(value.Value);
            else
                source.WriteValue(false);
        }

        /// <summary>
        /// Grava valor nullable
        /// </summary>
        /// <param name="source">Gravador</param>
        /// <param name="value">Valor</param>
        public static void WriteNullableValue(this XmlWriter source, ulong? value)
        {
            // Verifica se há valor e grava
            if (value.HasValue)
                source.WriteValue(XmlConvert.ToString(value.Value));
            else
                source.WriteValue(false);
        }
        #endregion

        /// <summary>
        /// Filtra texto removendo chars inválidos de XML
        /// </summary>
        /// <param name="input">Entra de texto</param>
        /// <returns>Texto filtrado</returns>
        private static string FilterText(string input)
        {
            // Sem texto? Retorna!
            if (input == null) return null;

            // Tamanho do texto de entrada
            int length = input.Length;

            // Tamanho do texto de saída
            int outlength = 0;

            // Nova linha?
            bool newline = true;

            // Chars de saída
            char[] output = new char[length];

            // Para cada char da entrada
            for (int i = 0; i < length; i++)
            {
                // Lê char
                char c = input[i];

                // Verifica entrada
                switch (c)
                {
                    // Quebras de linha
                    case '\r':
                    case '\n':

                        // Fora de nova linha?
                        if (!newline)
                        {
                            // Copia char
                            output[outlength++] = c;

                            // Está em nova linha
                            newline = true;
                        }
                        break;

                    // Espaços vazios
                    case ' ':
                    case '\t':

                        // Fora de nova linha?
                        if (!newline)
                        {
                            // Copia char
                            output[outlength++] = c;
                        }
                        break;

                    // Outros chars
                    default:

                        // Copia chars válidos somente
                        output[outlength++] = (c >= 0x20) ? c : '?';

                        // Não está mais em nova linha
                        newline = false;
                        break;
                }
            }

            // Retorna texto filtrado
            return new string(output, 0, outlength);
        }
    }
}
