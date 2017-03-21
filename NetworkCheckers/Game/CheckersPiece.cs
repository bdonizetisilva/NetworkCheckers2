using System;

namespace NetworkCheckers.Game
{

    // Enumeradores referentes a peça
    #region Enums

    /// <summary>
    /// Tipo de peça
    /// </summary>
    public enum CheckersRank
    {
        /// <summary>
        /// Peão
        /// </summary>
        Pawn = 0,

        /// <summary>
        /// Rei
        /// </summary>
        King = 1,
    }

    /// <summary>
    /// Direção de movimentação da peça
    /// </summary>
    public enum CheckersDirection
    {
        /// <summary>
        /// Para cima
        /// </summary>
        Up = 0,

        /// <summary>
        /// Para baixo
        /// </summary>
        Down = 1,
    }

    #endregion

    /// <summary>
    /// Representa uma peça do jogo de damas
    /// </summary>
    public class CheckersPiece
    {
        // Variáveis
        #region Variaveis

        /// <summary>
        /// Jogo que é dono da peça
        /// </summary>
        private readonly CheckersGame _Owner;

        /// <summary>
        /// Jogados a quem a peça pertence
        /// </summary>
        private readonly int _Player;

        /// <summary>
        /// Qual o tipo de peça, peao ou rei
        /// </summary>
        private CheckersRank _Rank;

        /// <summary>
        /// Localização da peça no tabuleiro
        /// </summary>
        private System.Drawing.Point _Location;

        /// <summary>
        /// A peça está em jogo ou já foi capturada?
        /// </summary>
        private bool _InPlay;

        #endregion

        // Construtores
        #region Construtores

        /// <summary>Cria uma nova peça genérica que náo está em jogo.</summary>
        /// <param name="player">Qual jogador é dono da peça.</param>
        /// <param name="rank">Qual o tipo de peça.</param>
        public CheckersPiece(int player, CheckersRank rank)
            : this(null, player, rank, System.Drawing.Point.Empty, false)
        {
        }

        /// <summary>Cria uma nova peça que está em jogo.</summary>
        /// <param name="player">Qual jogador é dono da peça.</param>
        /// <param name="rank">Qual o tipo de peça.</param>
        /// <param name="location">Localização da peça no tabuleiro.</param>
        public CheckersPiece(int player, CheckersRank rank, System.Drawing.Point location)
            : this(null, player, rank, location, true)
        {
        }

        /// <summary>
        /// Construtor interno para um nova peça que pode ou não estar em jogo
        /// </summary>
        /// <param name="owner">Jogo ao qual a peça pertence</param>
        /// <param name="player">Qual jogador é dono da peça.</param>
        /// <param name="rank">Qual o tipo de peça.</param>
        /// <param name="location">Localização da peça no tabuleiro.</param>
        /// <param name="inPlay">A peça está em jogo?</param>
        internal CheckersPiece(CheckersGame owner, int player, CheckersRank rank, System.Drawing.Point location, bool inPlay)
        {
            // Se o index do jogador é diferente de 1 ou 2
            if ((player < 1) || (player > 2))
                // Lança exceção
                throw new ArgumentOutOfRangeException(nameof(player), player,
                    @"A variável 'player' deve ser um número com valor 1 ou 2.");

            // Se a localização não está dentro dos limites do tabuleiro
            if ((location.X < 0) || (location.X >= CheckersGame.BoardSize.Width) || (location.Y < 0) ||
                (location.Y >= CheckersGame.BoardSize.Height))
                // Lança exceção
                throw new ArgumentOutOfRangeException(nameof(location), location,
                    @"A variável 'location' deve ser um valor válido no tabuleiro.");


            this._Owner = owner;
            this._Player = player;
            this._Rank = rank;
            this._Location = location;
            this._InPlay = inPlay;
        }

        #endregion

        // Propriedades
        #region Propriedades

        /// <summary>
        /// Jogo ao qual a peça pertence
        /// </summary>
        public CheckersGame Owner
        {
            get
            {
                return this._Owner;
            }
        }

        /// <summary>
        /// A peça está em jogo?
        /// </summary>
        public bool InPlay
        {
            get
            {
                return this._InPlay;
            }
        }

        /// <summary>
        /// Jogador ao qual a peça pertence
        /// </summary>
        public int Player
        {
            get
            {
                return this._Player;
            }
        }

        /// <summary>
        /// A direção da peça é relativo ao ponto de vista usando pelo tabuleiro interno.
        /// Se for player 1 retornará UP e se for 2 retornará DOWN
        /// </summary>
        public CheckersDirection Direction
        {
            get
            {
                return ((this._Player != 1) ? (CheckersDirection.Down) : (CheckersDirection.Up));
            }
        }

        /// <summary>
        /// Tipo de peça, peão ou rei
        /// </summary>
        public CheckersRank Rank
        {
            get
            {
                return this._Rank;
            }
        }

        /// <summary>
        /// Localização da peça no tabuleiro
        /// </summary>
        public System.Drawing.Point Location
        {
            get
            {
                return this._Location;
            }
        }

        #endregion

        // Métodos internos
        #region Internos
        
        /// <summary>
        /// Peça movida, atualiza localização
        /// </summary>
        /// <param name="location">Nova localização da peça</param>
        internal void Moved(System.Drawing.Point location)
        {
            this._Location = location;
        }

        /// <summary>
        /// Peça promovida a rei
        /// </summary>
        internal void Promoted()
        {
            this._Rank = CheckersRank.King;
        }

        /// <summary>
        /// Peça foi capturada e será removida do jogo
        /// </summary>
        internal void RemovedFromPlay()
        {
            // Atualiza localização
            this._Location = System.Drawing.Point.Empty;

            // Indica que está fora do jogo
            this._InPlay = false;
        }

        #endregion

        // Métodos públicos
        #region Publicos

        /// <summary>Cria uma peça igual para um outro jogo (possívelmene clonado).</summary>
        /// <param name="game">Jogo para o qual será criada a peça</param>
        /// <returns>A nova peça criada.</returns>
        public CheckersPiece Clone(CheckersGame game)
        {
            // Recupera a peça a clonada
            CheckersPiece clonedPiece = game.PieceAt(Location);
            
            // Certifica que a peça existe e é igual
            if (clonedPiece?.Player != Player || (clonedPiece.InPlay != InPlay) || (clonedPiece.Rank != Rank))
                return null;

            // Retorna peça clonada
            return clonedPiece;
        }

        #endregion
    }
}
