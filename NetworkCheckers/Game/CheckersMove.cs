using System;
using System.Drawing;
using System.Collections;

namespace NetworkCheckers.Game
{
    using Enumerable = System.Linq.Enumerable;

    public class CheckersMove
    {
        // Variáveis
        #region Variaveis
        
        /// <summary>
        /// Jogo ao qual o movimento pertence
        /// </summary>
        private readonly CheckersGame _Game;

        /// <summary>
        /// Jogo antes da realização do movimento
        /// </summary>
        private CheckersGame _InitialGame;

        /// <summary>
        /// Peça movimentada
        /// </summary>
        private readonly CheckersPiece _Piece;

        /// <summary>
        /// Peça antes do movimento
        /// </summary>
        private CheckersPiece _InitialPiece;

        /// <summary>
        /// Peça no tabuleiro
        /// </summary>
        private CheckersPiece[,] _Board;

        /// <summary>
        /// Localização atual
        /// </summary>
        private Point _CurrentLocation;

        /// <summary>
        /// Lista de saltos realizados
        /// </summary>
        private ArrayList _Jumped;

        /// <summary>
        /// Caminho percorrido
        /// </summary>
        private ArrayList _Path;

        /// <summary>
        /// A peça se tornou rei
        /// </summary>
        private bool _Kinged;

        /// <summary>
        /// A peça não pode ser movida
        /// </summary>
        private bool _CannotMove;

        #endregion

        // Propriedades
        #region Propriedades

        /// <summary>
        /// Jogo ao qual o movimento pertence
        /// </summary>
        public CheckersGame Game
        {
            get
            {
                return this._Game;
            }
        }

        /// <summary>
        /// Jogo antes do movimento acontecer
        /// </summary>
        public CheckersGame InitialGame
        {
            get
            {
                return this._InitialGame;
            }
        }

        /// <summary>
        /// Peça que está sendo movida
        /// </summary>
        public CheckersPiece Piece
        {
            get
            {
                return this._Piece;
            }
        }

        /// <summary>
        /// Peça que foi movida
        /// </summary>
        public CheckersPiece InitialPiece
        {
            get
            {
                return this._InitialPiece;
            }
        }

        /// <summary>
        /// Peças que serão puladas se o caminho for tomado
        /// </summary>
        public CheckersPiece[] Jumped
        {
            get
            {
                return (CheckersPiece[])this._Jumped.ToArray(typeof(CheckersPiece));
            }
        }

        /// <summary>
        /// A peça se tornou um rei com esse movimento?
        /// </summary>
        public bool Kinged
        {
            get
            {
                return this._Kinged;
            }
        }

        /// <summary>
        /// Localização do movimento
        /// </summary>
        public Point CurrentLocation
        {
            get
            {
                return this._CurrentLocation;
            }
        }

        /// <summary>
        /// Caminho realizado pelo movimento
        /// </summary>
        public Point[] Path
        {
            get
            {
                return (Point[])this._Path.ToArray(typeof(Point));
            }
        }

        /// <summary>
        /// Retorna se a peça foi movida
        /// </summary>
        public bool Moved
        {
            get
            {
                return (this._Path.Count != 0);
            }
        }

        /// <summary>
        /// Retorna se existe um movimento possível a partir desse ponto
        /// </summary>
        public bool CanMove
        {
            get
            {
                return (!this._CannotMove) && (EnumMoves().Length != 0);
            }
        }

        /// <summary>
        /// Retorna se algum movimento é requerido a partir deste ponto
        /// </summary>
        public bool MustMove
        {
            get
            {
                return ((!Moved) || ((CanMove) && (!this._Game.OptionalJumping)));
            }
        }

        #endregion

        // Construtores
        #region Construtores

        /// <summary>
        /// Será criada um movimento indiretamente por um jogo através do método BeginMove
        /// </summary>
        /// <param name="game">Jogo qual o movimento pertence</param>
        /// <param name="piece">Peça movida</param>
        /// <param name="makeReadOnlyCopy">É somente leitura?</param>
        internal CheckersMove(CheckersGame game, CheckersPiece piece, bool makeReadOnlyCopy)
        {
            this._Game = game;
            this._Piece = piece;
            this._InitialGame = (((makeReadOnlyCopy) && (!game.IsReadOnly)) ? (CheckersGame.MakeReadOnly(game)) : (game));
            this._InitialPiece = this._InitialGame.Pieces[Array.IndexOf(game.Pieces, piece)];
            this._Board = (CheckersPiece[,])game.Board.Clone();
            this._CurrentLocation = piece.Location;
            this._Jumped = new ArrayList();
            this._Path = new ArrayList();
            this._CannotMove = false;
            this._Kinged = false;
        }

        #endregion

        // Métodos internos
        #region Internos

        /// <summary>Cria um movimento a partir do caminho passado</summary>
        /// <param name="game">Jogo para o qual será criado o movimento</param>
        /// <param name="piece">A peça que será movida</param>
        /// <param name="path">O caminho pelo qual a peça será movida</param>
        /// <returns>O movimento resultante</returns>
        internal static CheckersMove FromPath(CheckersGame game, CheckersPiece piece, Point[] path)
        {
            // Cria um novo movimento
            CheckersMove move = new CheckersMove(game, piece, true);

            // Para cada casa no caminho passado
            foreach (Point p in path)
                // Se não conseguiu mover a peça
                if (move.Move(p) == false)
                    // Retorna null
                    return null;

            // Se moveu a peça por todas localizações, retorna o movimento
            return move;
        }

        #endregion

        // Métodos públicos
        #region Publicos

        /// <summary>
        /// Cria um movimento duplicado para que diferentes caminhos possam ser testados
        /// </summary>
        /// <returns>Novo movimento gerado</returns>
        public CheckersMove Clone()
        {
            // Clona o movimento
            CheckersMove move = new CheckersMove(this._Game, this._Piece, false)
            {
                _InitialGame = this._InitialGame,
                _InitialPiece = this._InitialPiece,
                _Board = (CheckersPiece[,]) this._Board.Clone(),
                _CurrentLocation = this._CurrentLocation,
                _Jumped = (ArrayList) this._Jumped.Clone(),
                _Kinged = this._Kinged,
                _Path = (ArrayList) this._Path.Clone(),
                _CannotMove = this._CannotMove
            };

            // Retorna o movimento clonado
            return move;
        }

        /// <summary>
        /// Cria um movimento duplicado para o jogo (provavelmente clonado) passado
        /// </summary>
        /// <returns>Novo movimento gerado</returns>
        public CheckersMove Clone(CheckersGame game)
        {
            return FromPath(game, this._Piece.Clone(game), Path);
        }

        /// <summary>
        /// Retorna se o movimento para a dada localização é valido
        /// </summary>
        /// <param name="location">Localização para a qual mover</param>
        /// <returns>True se for valido ou False para invalido</returns>
        public bool IsValidMove(Point location)
        {
            // Para cada movimento na lista de movimentos possíveis, verifica se existe algum ponto igual a localização passada
            return Enumerable.Any(this.EnumMoves(), p => p == location);
        }

        /// <summary>
        /// Retorna se o movimento para a dada localização é valido
        /// </summary>
        /// <param name="location">Localização para a qual mover</param>
        /// <param name="optionalJumping">Sobrescreve parametro de pulo opcional</param>
        /// <returns>True se for valido ou False para invalido</returns>
        public bool IsValidMove(Point location, bool optionalJumping)
        {
            return Enumerable.Any(this.EnumMoves(optionalJumping), p => p == location);
        }

        /// <summary>
        /// Move uma peça para a nova localização
        /// </summary>
        /// <param name="location">Nova localização</param>
        /// <returns>Retorna se o movimento foi válido</returns>
        public bool Move(Point location)
        {
            // Se a peça não pode ser movida
            if (this._CannotMove)
                // Retorna falso
                return false;

            CheckersPiece[] jumpedArray;

            // Recupera pontos de movimentos possiveis
            Point[] points = EnumMovesCore(this._CurrentLocation, out jumpedArray);

            // Para ponto
            for (int i = 0; i < points.Length; i++)
            {
                // Se o ponto é diferente da localizaçao
                if (points[i] != location) continue;

                // Move a peça
                if (jumpedArray[i] != null)
                {
                    this._Jumped.Add(jumpedArray[i]);
                    this._Board[jumpedArray[i].Location.X, jumpedArray[i].Location.Y] = null;
                }

                if ((Math.Abs(this._CurrentLocation.X - location.X) == 1) && (Math.Abs(this._CurrentLocation.Y - location.Y) == 1))
                    this._CannotMove = true;

                // Atuliza posição da peça
                this._Board[this._CurrentLocation.X, this._CurrentLocation.Y] = null;
                this._Board[location.X, location.Y] = this._Piece;
                this._CurrentLocation = location;

                // Adiciona movimento no caminho percorrido pela peça
                this._Path.Add(location);

                // Se a peça chegou no final do tabuleiro e é um peão, torna ele um rei
                if ((!this.MustMove) && (this._Piece.Rank == CheckersRank.Pawn))
                    this._Kinged = (((this._Piece.Direction == CheckersDirection.Up) && (this._CurrentLocation.Y == 0)) || ((this._Piece.Direction == CheckersDirection.Down) && (this._CurrentLocation.Y == CheckersGame.BoardSize.Height - 1)));

                // Retorna que o movimento foi válido
                return true;
            }
            
            // Retorna que o movimento é invalido
            return false;
        }

        /// <summary>
        /// Retorna se a peça está nos limites do tabuleiro
        /// </summary>
        /// <param name="location">Localização da peça</param>
        /// <returns>Se está nos limites</returns>
        public bool InBounds(Point location)
        {
            return this._Game.InBounds(location);
        }

        /// <summary>
        /// Retorna se a peça está nos limites do tabuleiro
        /// </summary>
        /// <param name="x">Posiçao X para teste</param>
        /// <param name="y">Posicao Y para teste</param>
        /// <returns>Se está nos limites</returns>
        public bool InBounds(int x, int y)
        {
            return this._Game.InBounds(x, y);
        }

        #endregion

        // Métodos privados
        #region Privados
        
        /// <summary>
        /// Retorna a lista de movimentos e seus respectivos saltos
        /// </summary>
        /// <param name="fromLocation">Localizaçao da peça</param>
        /// <param name="jumped">Saltos retornados</param>
        /// <returns>Lista de movimentos</returns>
        private Point[] EnumMovesCore(Point fromLocation, out CheckersPiece[] jumped)
        {
            return EnumMovesCore(fromLocation, out jumped, this._Game.OptionalJumping);
        }

        /// <summary>
        /// Retorna a lista de movimentos e seus respectivos saltos
        /// </summary>
        /// <param name="fromLocation">Localizaçao da peça</param>
        /// <param name="jumped">Saltos retornados</param>
        /// <param name="optionalJumping">Se o salto é opcional no jogo</param>
        /// <returns>Lista de movimentos</returns>
        private Point[] EnumMovesCore(Point fromLocation, out CheckersPiece[] jumped, bool optionalJumping)
        {
            ArrayList jumpedList;

            // Recupera saltos
            ArrayList jumpMoves = EnumJumpMovesCore(fromLocation, out jumpedList);

            // Recupera movimentos unicos
            ArrayList singleMoves = EnumSingleMovesCore(fromLocation, optionalJumping, jumpMoves);
            
            // Adiciona null para movimentos unicos
            for (int i = 0; i < singleMoves.Count; i++)
                jumpedList.Add(null);
            
            // Adiciona retornos
            jumped = (CheckersPiece[])jumpedList.ToArray(typeof(CheckersPiece));
            singleMoves.AddRange(jumpMoves);
            return (Point[])singleMoves.ToArray(typeof(Point));
        }

        private ArrayList EnumSingleMovesCore(Point fromLocation, bool optionalJumping)
        {
            if (optionalJumping)
            {
                ArrayList jumpedList;
                ArrayList jumpMoves = EnumJumpMovesCore(fromLocation, out jumpedList);
                return EnumSingleMovesCore(fromLocation, true, jumpMoves);
            }
            return EnumSingleMovesCore(fromLocation, false, null);
        }

        private ArrayList EnumSingleMovesCore(Point fromLocation, bool optionalJumping, ArrayList jumpMoves)
        {
            // Create resizable list of jumpable moves and their respective jumps
            ArrayList moves = new ArrayList();
            // Check for single moves, if not jumping already took place
            if (this._Path.Count > 0)
                return moves;

            bool canSingleMove = true;
            if (!optionalJumping)
            {
                // Determine whether or not a single jump can take place
                canSingleMove = (jumpMoves.Count == 0) || (this._Game.OptionalJumping);
                if (canSingleMove)
                {
                    // Further testing for single moves; test all other pieces on the board
                    foreach (CheckersPiece testPiece in this._Game.EnumPlayerPieces(this._Game.Turn))
                    {
                        ArrayList dummy;
                        if (EnumJumpMovesCore(testPiece, testPiece.Location, out dummy).Count == 0)
                            continue;
                        canSingleMove = false;
                        break;
                    }
                }
            }

            // Check whether or not a single move can take place
            if (!canSingleMove)
                return moves;

            // Append single-move moves in the enumeration (if able to)
            if (this._Piece.Location == fromLocation)
            {
                if ((this._Piece.Direction == CheckersDirection.Up) || (this._Piece.Rank == CheckersRank.King))
                {
                    if (InBounds(fromLocation.X - 1, fromLocation.Y - 1) && (this._Board[fromLocation.X - 1, fromLocation.Y - 1] == null))
                    {
                        moves.Add(new Point(fromLocation.X - 1, fromLocation.Y - 1));
                    }
                    if (InBounds(fromLocation.X + 1, fromLocation.Y - 1) && (this._Board[fromLocation.X + 1, fromLocation.Y - 1] == null))
                    {
                        moves.Add(new Point(fromLocation.X + 1, fromLocation.Y - 1));
                    }
                }
                if ((this._Piece.Direction == CheckersDirection.Down) || (this._Piece.Rank == CheckersRank.King))
                {
                    if (InBounds(fromLocation.X - 1, fromLocation.Y + 1) && (this._Board[fromLocation.X - 1, fromLocation.Y + 1] == null))
                    {
                        moves.Add(new Point(fromLocation.X - 1, fromLocation.Y + 1));
                    }
                    if (InBounds(fromLocation.X + 1, fromLocation.Y + 1) && (this._Board[fromLocation.X + 1, fromLocation.Y + 1] == null))
                    {
                        moves.Add(new Point(fromLocation.X + 1, fromLocation.Y + 1));
                    }
                }
            }
            return moves;
        }

        private ArrayList EnumJumpMovesCore(Point fromLocation, out ArrayList jumped)
        {
            return EnumJumpMovesCore(this._Piece, fromLocation, out jumped);
        }

        private ArrayList EnumJumpMovesCore(CheckersPiece piece, Point fromLocation, out ArrayList jumped)
        {
            ArrayList moves = new ArrayList();
            jumped = new ArrayList();
            // Append jumps (not of same team)
            if ((piece.Direction == CheckersDirection.Up) || (piece.Rank == CheckersRank.King))
            {
                if (this._Game.InBounds(fromLocation.X - 1, fromLocation.Y - 1) && (this._Board[fromLocation.X - 1, fromLocation.Y - 1] != null) && (this._Board[fromLocation.X - 1, fromLocation.Y - 1].Player != piece.Player))
                    if (InBounds(fromLocation.X - 2, fromLocation.Y - 2) && (this._Board[fromLocation.X - 2, fromLocation.Y - 2] == null))
                    {
                        moves.Add(new Point(fromLocation.X - 2, fromLocation.Y - 2));
                        jumped.Add(this._Board[fromLocation.X - 1, fromLocation.Y - 1]);
                    }
                if (InBounds(fromLocation.X + 1, fromLocation.Y - 1) && (this._Board[fromLocation.X + 1, fromLocation.Y - 1] != null) && (this._Board[fromLocation.X + 1, fromLocation.Y - 1].Player != piece.Player))
                    if (InBounds(fromLocation.X + 2, fromLocation.Y - 2) && (this._Board[fromLocation.X + 2, fromLocation.Y - 2] == null))
                    {
                        moves.Add(new Point(fromLocation.X + 2, fromLocation.Y - 2));
                        jumped.Add(this._Board[fromLocation.X + 1, fromLocation.Y - 1]);
                    }
            }
            if ((piece.Direction == CheckersDirection.Down) || (piece.Rank == CheckersRank.King))
            {
                if (InBounds(fromLocation.X - 1, fromLocation.Y + 1) && (this._Board[fromLocation.X - 1, fromLocation.Y + 1] != null) && (this._Board[fromLocation.X - 1, fromLocation.Y + 1].Player != piece.Player))
                    if (InBounds(fromLocation.X - 2, fromLocation.Y + 2) && (this._Board[fromLocation.X - 2, fromLocation.Y + 2] == null))
                    {
                        moves.Add(new Point(fromLocation.X - 2, fromLocation.Y + 2));
                        jumped.Add(this._Board[fromLocation.X - 1, fromLocation.Y + 1]);
                    }
                if (InBounds(fromLocation.X + 1, fromLocation.Y + 1) && (this._Board[fromLocation.X + 1, fromLocation.Y + 1] != null) && (this._Board[fromLocation.X + 1, fromLocation.Y + 1].Player != piece.Player))
                    if (InBounds(fromLocation.X + 2, fromLocation.Y + 2) && (this._Board[fromLocation.X + 2, fromLocation.Y + 2] == null))
                    {
                        moves.Add(new Point(fromLocation.X + 2, fromLocation.Y + 2));
                        jumped.Add(this._Board[fromLocation.X + 1, fromLocation.Y + 1]);
                    }
            }
            return moves;
        }

        #endregion

        // Enumeradores
        #region Enumeradores

        /// <summary>Enumerates all possible moves from this point.</summary>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumMoves()
        {
            if (this._CannotMove)
                return new Point[0];
            CheckersPiece[] jumpArray;
            return EnumMovesCore(this._CurrentLocation, out jumpArray);
        }
        /// <summary>Enumerates all possible moves from this point.</summary>
        /// <param name="optionalJumping">Overrides the game's OptionalJumping parameter for the enumeration.</param>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumMoves(bool optionalJumping)
        {
            if (this._CannotMove)
            {
                return new Point[0];
            }
            CheckersPiece[] jumpArray;
            return EnumMovesCore(this._CurrentLocation, out jumpArray, optionalJumping);
        }
        /// <summary>Enumerates all possible moves from this point.</summary>
        /// <param name="jumped">Returns the array of Checkers piecse that are jumped, with respect to the moves.</param>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumMoves(out CheckersPiece[] jumped)
        {
            if (this._CannotMove)
            {
                jumped = new CheckersPiece[0];
                return new Point[0];
            }
            return EnumMovesCore(this._CurrentLocation, out jumped);
        }
        /// <summary>Enumerates all possible moves from this point.</summary>
        /// <param name="optionalJumping">Overrides the game's OptionalJumping parameter for the enumeration.</param>
        /// <param name="jumped">Returns the array of Checkers piecse that are jumped, with respect to the moves.</param>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumMoves(bool optionalJumping, out CheckersPiece[] jumped)
        {
            if (this._CannotMove)
            {
                jumped = new CheckersPiece[0];
                return new Point[0];
            }
            return EnumMovesCore(this._CurrentLocation, out jumped, optionalJumping);
        }
        /// <summary>Enumerates all possible single moves from this point.</summary>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumSingleMoves()
        {
            if (this._CannotMove)
                return new Point[0];
            return (Point[])EnumSingleMovesCore(this._CurrentLocation, this._Game.OptionalJumping).ToArray(typeof(Point));
        }
        /// <summary>Enumerates all possible single moves from this point.</summary>
        /// <param name="optionalJumping">Overrides the game's OptionalJumping parameter for the enumeration.</param>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumSingleMoves(bool optionalJumping)
        {
            if (this._CannotMove)
                return new Point[0];
            return (Point[])EnumSingleMovesCore(this._CurrentLocation, optionalJumping).ToArray(typeof(Point));
        }
        /// <summary>Enumerates all possible jump moves from this point.</summary>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumJumpMoves()
        {
            if (this._CannotMove)
                return new Point[0];
            ArrayList jumpArray;
            return (Point[])EnumJumpMovesCore(this._CurrentLocation, out jumpArray).ToArray(typeof(Point));
        }
        /// <summary>Enumerates all possible jump moves from this point.</summary>
        /// <param name="jumped">Returns the array of Checkers piecse that are jumped, with respect to the moves.</param>
        /// <returns>An array of points which represent valid moves.</returns>
        public Point[] EnumJumpMoves(out CheckersPiece[] jumped)
        {
            if (this._CannotMove)
            {
                jumped = new CheckersPiece[0];
                return new Point[0];
            }
            ArrayList jumpedList;
            ArrayList moves = EnumJumpMovesCore(this._CurrentLocation, out jumpedList);
            jumped = (CheckersPiece[])jumpedList.ToArray(typeof(CheckersPiece));
            return (Point[])moves.ToArray(typeof(Point));
        }

        #endregion
    }
}
