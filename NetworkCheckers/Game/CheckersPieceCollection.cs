using System.Collections;

namespace NetworkCheckers.Game
{
    /// <summary>
    /// Coleção de peças de dama
    /// </summary>
    public class CheckersPieceCollection : CollectionBase
    {
        /// <summary>
        /// Adiciona uma peça a coleção
        /// </summary>
        /// <param name="item">Peça a ser adicionada</param>
        /// <returns>Posição me que foi inserida</returns>
        public int Add(CheckersPiece item)
        {
            return InnerList.Add(item);
        }

        /// <summary>
        /// Adiciona uma lista de peças a coleção
        /// </summary>
        /// <param name="items">Lista de peças a serem adicionadas</param>
        public void AddRange(CheckersPiece[] items)
        {
            // Para cada peça na lista
            foreach (CheckersPiece item in items)
                // Adiciona
                Add(item);
        }

        /// <summary>
        /// Insere uma peça em uma determinada posição
        /// </summary>
        /// <param name="index">Posiçao a inserir</param>
        /// <param name="item">Peça a ser inserida</param>
        public void Insert(int index, CheckersPiece item)
        {
            // Insere peça
            InnerList.Insert(index, item);
        }

        /// <summary>
        /// Remove a peça passada
        /// </summary>
        /// <param name="item">Peça a ser removida</param>
        public void Remove(CheckersPiece item)
        {
            // Remove a peça
            InnerList.Remove(item);
        }

        /// <summary>
        /// Retorna se uma peça existe na coleção
        /// </summary>
        /// <param name="item">Peça a ser procurada</param>
        /// <returns>Se a peça existe</returns>
        public bool Contains(CheckersPiece item)
        {
            // Retorna se a peça existe
            return InnerList.Contains(item);
        }

        /// <summary>
        /// Retorna a posiçao que uma peça se encontra na coleçao
        /// </summary>
        /// <param name="item">Peça a ser procurada</param>
        /// <returns>Posição da peça na coleção</returns>
        public int IndexOf(CheckersPiece item)
        {
            // Index da peça
            return InnerList.IndexOf(item);
        }

        /// <summary>
        /// Copia a lista passada começando da posição informada
        /// </summary>
        /// <param name="array">Lista a ser copiada</param>
        /// <param name="index">Posição para começo</param>
        public void CopyTo(CheckersPiece[] array, int index)
        {
            // Copia lista a partir do index passado
            InnerList.CopyTo(array, index);
        }

        /// <summary>
        /// Cria uma cópia da coleção atual
        /// </summary>
        /// <returns></returns>
        public CheckersPieceCollection Clone()
        {
            // Cria uma nova coleçao
            CheckersPieceCollection clone = new CheckersPieceCollection();

            // Adiciona todos os itens desta na nova
            clone.AddRange(ToArray());

            // Retorna a nova
            return clone;
        }

        /// <summary>
        /// Retorna ou atualiza a peça na posição passada
        /// </summary>
        /// <param name="index">Posição desejada</param>
        /// <returns>Peça na posiçao</returns>
        public CheckersPiece this[int index]
        {
            get
            {
                return (CheckersPiece)InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }

        /// <summary>
        /// Converte a coleçao atual para um array
        /// </summary>
        /// <returns>Array com os elementos da coleçao</returns>
        public CheckersPiece[] ToArray()
        {
            return (CheckersPiece[])InnerList.ToArray(typeof(CheckersPiece));
        }
    }
}
