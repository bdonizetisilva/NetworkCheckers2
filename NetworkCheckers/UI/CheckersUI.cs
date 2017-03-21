using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Windows.Forms;
using NetworkCheckers.Game;

internal class ResFinder
{
}

namespace NetworkCheckers.UI
{
    // ReSharper disable once InconsistentNaming
    public enum CheckersUIState
    {
        Idle = 0,
        IdleMoving = 3,
        DragMoving = 1,
        ClickMoving = 2,
        ShowMove = 4,
    }

    public class MoveEventArgs : EventArgs
    {
        readonly bool _MovedByPlayer;
        readonly bool _IsWinningMove;
        readonly CheckersMove _Move;
        public MoveEventArgs(bool movedByPlayer, bool isWinningMove, CheckersMove move)
        {
            this._MovedByPlayer = movedByPlayer;
            this._IsWinningMove = isWinningMove;
            this._Move = move;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool MovedByPlayer
        {
            get
            {
                return this._MovedByPlayer;
            }
        }
        public bool IsWinningMove
        {
            get
            {
                return this._IsWinningMove;
            }
        }
        public CheckersMove Move
        {
            get
            {
                return this._Move;
            }
        }
    }
    public delegate void MoveEventHandler(object sender, MoveEventArgs e);

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ResFinder), "Checkers.UI.ToolboxBitmap.png")]
    [Designer(typeof(CheckersDesigner))]
    // ReSharper disable once InconsistentNaming
    public class CheckersUI : UserControl
    {
        public static readonly Size SquareSize = new Size(32, 32);
        public static readonly Size BoardPixelSize = new Size(SquareSize.Width * CheckersGame.BoardSize.Width, SquareSize.Height * CheckersGame.BoardSize.Height);

        public event EventHandler GameStarted;
        public event EventHandler GameStopped;
        public event EventHandler PiecePickedUp;
        public event MoveEventHandler PieceMoved;
        public event MoveEventHandler PieceMovedPartial;
        public event MoveEventHandler PieceBadMove;
        public event EventHandler PieceDeselected;
        public event EventHandler TurnChanged;
        public event EventHandler WinnerDeclared;

        private CheckersGame _Game;
        private CheckersUIState _State = CheckersUIState.Idle;
        private CheckersMove _MovePiece;
        private CheckersMove _DestPiece;
        private int _BlinkCount;
        private bool _MoveAfterShow;
        private bool _DelayAfterShow;
        private Point _InitialDrag = Point.Empty;
        private int _InitialDragGrace;
        private Bitmap _BoardImage;
        private Point[] _SelectedSquares = new Point[0];
        private Point[] _DarkenedSquares = new Point[0];
        private Point _FocussedSquare = Point.Empty;

        private bool _TextBorder = true;
        private Color _TextBorderColor = Color.Black;
        private ContentAlignment _TextAlign = ContentAlignment.MiddleCenter;
        private BorderStyle _BorderStyle = BorderStyle.Fixed3D;
        private int _BoardMargin = 4;
        private Color _BoardBackColor = Color.DarkSeaGreen;
        private Color _BoardForeColor = Color.OldLace;
        private Color _BoardGridColor = Color.Gray;
        private Image[] _CustomPieceImages = new Image[4];
        private bool _Player1Active = true;
        private bool _Player2Active = true;
        private bool _HighlightPossibleMoves = true;
        private bool _HighlightSelection = true;
        private Image[] _PieceImages = new Image[4];
        private Cursor[] _PieceCursors = new Cursor[4];
        private bool _ShowJumpMessage = true;

        #region Class Controls

        private System.Windows.Forms.Timer _TmrShowMove;
        private System.Windows.Forms.Timer _TmrBlink;
        private IContainer components;

        #endregion

        #region Class Construction

        public CheckersUI()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.FixedWidth, true);
            SetStyle(ControlStyles.FixedHeight, true);
            // Initialize the image
            CreateBoard();
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._TmrShowMove = new System.Windows.Forms.Timer(this.components);
            this._TmrBlink = new System.Windows.Forms.Timer(this.components);
            // 
            // tmrShowMove
            // 
            this._TmrShowMove.Tick += new System.EventHandler(this.tmrShowMove_Tick);
            // 
            // tmrBlink
            // 
            this._TmrBlink.Tick += new System.EventHandler(this.tmrBlink_Tick);
            // 
            // CheckersUI
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "CheckersUI";
            this.Size = new System.Drawing.Size(360, 276);
            this.Resize += new System.EventHandler(this.CheckersUI_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CheckersUI_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CheckersUI_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CheckersUI_MouseMove);
            this.MouseLeave += new System.EventHandler(this.CheckersUI_MouseLeave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CheckersUI_MouseDown);

        }

        #endregion

        #endregion

        #region Class Properties

        [DefaultValue(BorderStyle.Fixed3D)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return this._BorderStyle;
            }
            set
            {
                this._BorderStyle = value;
                SetBoundsCore(Left, Top, 0, 0, BoundsSpecified.Size);
                Refresh();
            }
        }

        [DefaultValue(""), Browsable(true), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Refresh();
            }
        }

        [DefaultValue(true)]
        public bool TextBorder
        {
            get
            {
                return this._TextBorder;
            }
            set
            {
                this._TextBorder = value;
                Refresh();
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color TextBorderColor
        {
            get
            {
                return this._TextBorderColor;
            }
            set
            {
                this._TextBorderColor = value;
                Refresh();
            }
        }

        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get
            {
                return this._TextAlign;
            }
            set
            {
                this._TextAlign = value;
                Refresh();
            }
        }

        [DefaultValue(typeof(Color), "White")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                Refresh();
            }
        }

        [DefaultValue(4)]
        public int BoardMargin
        {
            get
            {
                return this._BoardMargin;
            }
            set
            {
                this._BoardMargin = value;
                SetBoundsCore(Left, Top, 0, 0, BoundsSpecified.Size);
                Refresh();
            }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [DefaultValue(typeof(Color), "DarkSeaGreen")]
        public Color BoardBackColor
        {
            get
            {
                return this._BoardBackColor;
            }
            set
            {
                this._BoardBackColor = value;
                Refresh();
            }
        }

        [DefaultValue(typeof(Color), "OldLace")]
        public Color BoardForeColor
        {
            get
            {
                return this._BoardForeColor;
            }
            set
            {
                this._BoardForeColor = value;
                Refresh();
            }
        }

        [DefaultValue(typeof(Color), "Gray")]
        public Color BoardGridColor
        {
            get
            {
                return this._BoardGridColor;
            }
            set
            {
                this._BoardGridColor = value;
                Refresh();
            }
        }

        [DefaultValue(null)]
        public Image CustomPawn1
        {
            get
            {
                return this._CustomPieceImages[0];
            }
            set
            {
                this._CustomPieceImages[0] = value;
            }
        }
        [DefaultValue(null)]
        public Image CustomKing1
        {
            get
            {
                return this._CustomPieceImages[1];
            }
            set
            {
                this._CustomPieceImages[1] = value;
            }
        }
        [DefaultValue(null)]
        public Image CustomPawn2
        {
            get
            {
                return this._CustomPieceImages[2];
            }
            set
            {
                this._CustomPieceImages[2] = value;
            }
        }
        [DefaultValue(null)]
        public Image CustomKing2
        {
            get
            {
                return this._CustomPieceImages[3];
            }
            set
            {
                this._CustomPieceImages[3] = value;
            }
        }

        [DefaultValue(true)]
        public bool Player1Active
        {
            get
            {
                return this._Player1Active;
            }
            set
            {
                this._Player1Active = value;
                CheckMoving();
            }
        }
        [DefaultValue(true)]
        public bool Player2Active
        {
            get
            {
                return this._Player2Active;
            }
            set
            {
                this._Player2Active = value;
                CheckMoving();
            }
        }

        [DefaultValue(true)]
        public bool HighlightSelection
        {
            get
            {
                return this._HighlightSelection;
            }
            set
            {
                this._HighlightSelection = value;
                Refresh();
            }
        }

        [DefaultValue(true)]
        public bool HighlightPossibleMoves
        {
            get
            {
                return this._HighlightPossibleMoves;
            }
            set
            {
                this._HighlightPossibleMoves = value;
                Refresh();
            }
        }

        [DefaultValue(true)]
        public bool ShowJumpMessage
        {
            get
            {
                return this._ShowJumpMessage;
            }
            set
            {
                this._ShowJumpMessage = value;
            }
        }


        [Browsable(false)]
        public CheckersGame Game
        {
            get
            {
                return this._Game;
            }
        }

        [Browsable(false)]
        public CheckersPiece Holding
        {
            get
            {
                return ((IsHolding) ? (this._MovePiece.Piece) : (null));
            }
        }

        [Browsable(false)]
        public bool IsHolding
        {
            get
            {
                return (IsPlaying) && ((this._State == CheckersUIState.DragMoving) || (this._State == CheckersUIState.ClickMoving));
            }
        }

        [Browsable(false)]
        public CheckersMove CurrentMove
        {
            get
            {
                return ((IsMoving) ? (this._MovePiece.Clone()) : (null));
            }
        }

        [Browsable(false)]
        public bool IsMoving
        {
            get
            {
                return (IsMovingByPlayer) || (IsMovingByControl);
            }
        }

        [Browsable(false)]
        public bool IsMovingByPlayer
        {
            get
            {
                return (IsPlaying) && ((this._State == CheckersUIState.DragMoving) || (this._State == CheckersUIState.ClickMoving) || (this._State == CheckersUIState.IdleMoving));
            }
        }

        [Browsable(false)]
        public bool IsMovingByControl
        {
            get
            {
                return (IsPlaying) && (this._State == CheckersUIState.ShowMove);
            }
        }

        [Browsable(false)]
        public bool IsPlaying
        {
            get
            {
                return ((this._Game != null) && (this._Game.IsPlaying));
            }
        }

        [Browsable(false)]
        public int Winner
        {
            get
            {
                return ((this._Game != null) ? (this._Game.Winner) : (0));
            }
        }

        #endregion


        #region Control Framework

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            int borderSize = ((this._BorderStyle == BorderStyle.Fixed3D) ? (2) : (((this._BorderStyle == BorderStyle.FixedSingle) ? (1) : (0))));
            base.SetBoundsCore(x, y, BoardPixelSize.Width + 2 + (borderSize * 2) + (this._BoardMargin * 2), BoardPixelSize.Height + 2 + (borderSize * 2) + (this._BoardMargin * 2), specified);
        }

        private void CheckersUI_Resize(object sender, EventArgs e)
        {
            CreateBoard();
        }

        private void CheckersUI_Paint(object sender, PaintEventArgs e)
        {
            // Draw control background and border
            e.Graphics.Clear(BackColor);
            int borderSize = 0;
            if (this._BorderStyle == BorderStyle.Fixed3D)
            {
                Pen penLight = new Pen(Color.FromKnownColor(KnownColor.ControlLight));
                Pen penLightLight = new Pen(Color.FromKnownColor(KnownColor.ControlLightLight));
                Pen penDark = new Pen(Color.FromKnownColor(KnownColor.ControlDark));
                Pen penDarkDark = new Pen(Color.FromKnownColor(KnownColor.ControlDarkDark));
                //
                e.Graphics.DrawLine(penDark, 0, 0, Width - 2, 0);
                e.Graphics.DrawLine(penDarkDark, 0, 1, Width - 3, 1);
                e.Graphics.DrawLine(penDark, 0, 1, 0, Height - 2);
                e.Graphics.DrawLine(penDarkDark, 1, 2, 1, Height - 3);
                e.Graphics.DrawLine(penLightLight, 0, Height - 1, Width - 1, Height - 1);
                e.Graphics.DrawLine(penLight, 1, Height - 2, Width - 1, Height - 2);
                e.Graphics.DrawLine(penLightLight, Width - 1, 0, Width - 1, Height - 1);
                e.Graphics.DrawLine(penLight, Width - 2, 1, Width - 2, Height - 2);
                //
                penLight.Dispose();
                penLightLight.Dispose();
                penDark.Dispose();
                penDarkDark.Dispose();
                borderSize = 2;
            }
            else if (this._BorderStyle == BorderStyle.FixedSingle)
            {
                Pen penFrame = new Pen(Color.FromKnownColor(KnownColor.WindowFrame));
                //
                e.Graphics.DrawLine(penFrame, 0, 0, Width, 0);
                e.Graphics.DrawLine(penFrame, 0, 0, 0, Height);
                e.Graphics.DrawLine(penFrame, 0, Height - 1, Width, Height - 1);
                e.Graphics.DrawLine(penFrame, Width - 1, 0, Width - 1, Height);
                //
                penFrame.Dispose();
                borderSize = 1;
            }
            e.Graphics.DrawImage(this._BoardImage, this._BoardMargin + borderSize, this._BoardMargin + borderSize, this._BoardImage.Width, this._BoardImage.Height);
            // Draw the text
            int nextLineOffset = 0;
            SizeF textSize = e.Graphics.MeasureString(base.Text, Font);
            foreach (string s in (base.Text + '\n').Split('\n', '\r'))
            {
                if (s == string.Empty)
                    continue;
                SizeF stringSize = e.Graphics.MeasureString(s, Font);
                int x, y;
                switch (this._TextAlign)
                {
                    case ContentAlignment.TopLeft:
                        x = 0;
                        y = 0;
                        break;
                    case ContentAlignment.TopCenter:
                        x = (int)((this.Size.Width - stringSize.Width) / 2);
                        y = 0;
                        break;
                    case ContentAlignment.TopRight:
                        x = (int)(this.Size.Width - stringSize.Width);
                        y = 0;
                        break;
                    case ContentAlignment.MiddleLeft:
                        x = 0;
                        y = (int)((this.Size.Height - textSize.Height) / 2);
                        break;
                    case ContentAlignment.MiddleCenter:
                        x = (int)((this.Size.Width - stringSize.Width) / 2);
                        y = (int)((this.Size.Height - textSize.Height) / 2);
                        break;
                    case ContentAlignment.MiddleRight:
                        x = (int)(this.Size.Width - stringSize.Width);
                        y = (int)((this.Size.Height - textSize.Height) / 2);
                        break;
                    case ContentAlignment.BottomLeft:
                        x = 0;
                        y = (int)(this.Size.Height - textSize.Height);
                        break;
                    case ContentAlignment.BottomCenter:
                        x = (int)((this.Size.Width - stringSize.Width) / 2);
                        y = (int)(this.Size.Height - textSize.Height);
                        break;
                    case ContentAlignment.BottomRight:
                        x = (int)(this.Size.Width - stringSize.Width);
                        y = (int)(this.Size.Height - textSize.Height);
                        break;
                    default:
                        x = y = 0;
                        break;
                }
                y += nextLineOffset;
                nextLineOffset += (int)(stringSize.Height);
                if (this._TextBorder)
                {
                    Brush borderBrush = new SolidBrush(this._TextBorderColor);
                    e.Graphics.DrawString(s, Font, borderBrush, x - 1, y - 1);
                    e.Graphics.DrawString(s, Font, borderBrush, x + 1, y - 1);
                    e.Graphics.DrawString(s, Font, borderBrush, x - 1, y + 1);
                    e.Graphics.DrawString(s, Font, borderBrush, x + 1, y + 1);
                }
                e.Graphics.DrawString(s, Font, new SolidBrush(ForeColor), x, y);
            }
        }

        #endregion

        #region Image Functions

        private void CreateBoard()
        {
            this._BoardImage = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppArgb);
            Refresh();
        }

        private void CreateImages()
        {
            // Get piece images
            this._PieceImages[0] = ((this._CustomPieceImages[0] != null) ? (AdjustPieceImage(this._CustomPieceImages[0])) : (CreatePieceImage(Color.LightGray)));
            this._PieceImages[1] = ((this._CustomPieceImages[1] != null) ? (AdjustPieceImage(this._CustomPieceImages[1])) : (CreatePieceImage(Color.LightGray)));
            this._PieceImages[2] = ((this._CustomPieceImages[2] != null) ? (AdjustPieceImage(this._CustomPieceImages[2])) : (CreatePieceImage(Color.Firebrick)));
            this._PieceImages[3] = ((this._CustomPieceImages[3] != null) ? (AdjustPieceImage(this._CustomPieceImages[3])) : (CreatePieceImage(Color.Firebrick)));
            // Get piece cursors
            this._PieceCursors[0] = new Cursor((new Bitmap(this._PieceImages[0])).GetHicon());
            this._PieceCursors[1] = new Cursor((new Bitmap(this._PieceImages[1])).GetHicon());
            this._PieceCursors[2] = new Cursor((new Bitmap(this._PieceImages[2])).GetHicon());
            this._PieceCursors[3] = new Cursor((new Bitmap(this._PieceImages[3])).GetHicon());
        }
        private Image AdjustPieceImage(Image pieceImage)
        {
            if ((pieceImage.Size.Width == 32) && (pieceImage.Size.Height == 32))
                return pieceImage;
            return new Bitmap(pieceImage, 32, 32);
        }
        private Bitmap CreatePieceImage(Color color)
        {
            Bitmap pieceImage = new Bitmap(32, 32);
            pieceImage.MakeTransparent();
            Graphics g = Graphics.FromImage(pieceImage);
            Brush fillBrush = new SolidBrush(color);
            Pen ringColor = new Pen(LightenColor(color, 0x28));
            g.FillEllipse(fillBrush, 2, 2, 32 - 5, 32 - 5);
            g.DrawEllipse(ringColor, 3, 3, 32 - 7, 32 - 7);
            g.DrawEllipse(Pens.Black, 2, 2, 32 - 5, 32 - 5);
            ringColor.Dispose();
            fillBrush.Dispose();
            g.Dispose();
            return pieceImage;
        }

        private Color LightenColor(Color color, int intensity)
        {
            if (intensity < 0)
                return DarkenColor(color, -intensity);
            return Color.FromArgb(((color.R + intensity > 0xFF) ? (0xFF) : (color.R + intensity)), ((color.G + intensity > 0xFF) ? (0xFF) : (color.G + intensity)), ((color.B + intensity > 0xFF) ? (0xFF) : (color.B + intensity)));
        }
        private Color DarkenColor(Color color, int intensity)
        {
            if (intensity < 0)
                return LightenColor(color, -intensity);
            return Color.FromArgb(((color.R - intensity < 0) ? (0) : (color.R - intensity)), ((color.G - intensity < 0) ? (0) : (color.G - intensity)), ((color.B - intensity < 0) ? (0) : (color.B - intensity)));
        }
        private Color BlendColor(Color color1, Color color2, int blendPercent)
        {
            if ((blendPercent < 0) || (blendPercent > 100))
                throw new ArgumentOutOfRangeException("blendPercent", blendPercent, @"The parameter 'blendPercent' must be a valid percent range [0 - 100].");
            if (blendPercent == 0)
                return color1;
            if (blendPercent == 100)
                return color2;
            int r = (int)((color1.R * ((100 - blendPercent) / 100.0)) + (color2.R * (blendPercent / 100.0)));
            int g = (int)((color1.G * ((100 - blendPercent) / 100.0)) + (color2.G * (blendPercent / 100.0)));
            int b = (int)((color1.B * ((100 - blendPercent) / 100.0)) + (color2.B * (blendPercent / 100.0)));
            return Color.FromArgb(r, g, b);
        }

        #endregion

        #region Game Functions

        private void CheckersGame_GameStarted(object sender, EventArgs e)
        {
            OnGameStarted();
        }
        private void CheckersGame_GameStopped(object sender, EventArgs e)
        {
            OnGameStopped();
        }
        private void CheckersGame_TurnChanged(object sender, EventArgs e)
        {
            OnTurnChanged();
        }
        private void CheckersGame_WinnerDeclared(object sender, EventArgs e)
        {
            OnWinnerDeclared();
        }

        private void SetGame(CheckersGame g)
        {
            if (this._Game != null)
            {
                if (this._Game.IsPlaying)
                    throw new InvalidOperationException("Game has already started.");
                this._Game.GameStarted -= CheckersGame_GameStarted;
                this._Game.GameStopped -= this.CheckersGame_GameStopped;
                this._Game.TurnChanged -= this.CheckersGame_TurnChanged;
                this._Game.WinnerDeclared -= this.CheckersGame_WinnerDeclared;
            }
            this._Game = g;
            this._Game.GameStarted += this.CheckersGame_GameStarted;
            this._Game.GameStopped += this.CheckersGame_GameStopped;
            this._Game.TurnChanged += this.CheckersGame_TurnChanged;
            this._Game.WinnerDeclared += this.CheckersGame_WinnerDeclared;
        }

        private void OnGameStarted()
        {
            CreateImages();
            Refresh();
            if (GameStarted != null)
                GameStarted(this, EventArgs.Empty);
        }

        private void OnGameStopped()
        {
            this._MovePiece = null;
            this._State = CheckersUIState.Idle;
            Cursor = Cursors.Default;
            this._FocussedSquare = Point.Empty;
            this._SelectedSquares = new Point[0];
            this._DarkenedSquares = new Point[0];
            Refresh();
            this.GameStopped?.Invoke(this, EventArgs.Empty);
        }

        private void OnTurnChanged()
        {
            this.TurnChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnWinnerDeclared()
        {
            this.WinnerDeclared?.Invoke(this, EventArgs.Empty);
        }

        private void CheckMoving()
        {
            if ((this._State != CheckersUIState.DragMoving) && (this._State != CheckersUIState.ClickMoving) && (this._State != CheckersUIState.IdleMoving))
                return;
            if (this._MovePiece == null)
                return;
            // Be sure moving is possible
            if (((this._MovePiece.Piece.Player == 1) && (!this._Player1Active)) || ((this._MovePiece.Piece.Player == 2) && (!this._Player2Active)))
                StopMove();
        }
        private void StopMove()
        {
            // Must drop piece
            this._State = CheckersUIState.Idle;
            this._FocussedSquare = Point.Empty;
            this._SelectedSquares = new Point[0];
            this._DarkenedSquares = new Point[0];
            Refresh();
        }

        // Moves the piece with optional refreshing
        private bool MovePieceCore(CheckersMove move, bool showMove, bool noDelay)
        {
            if (!IsPlaying)
                return false;
            // !!!!! Smooth movements
            // Move the piece
            if (showMove)
            {
                if (this._State != CheckersUIState.Idle)
                    StopMove();
                if (!this._Game.IsValidMove(move))
                    return false;
                ShowMoveCore(move, noDelay, false, true, false);
                return true;
            }
            if (!this._Game.MovePiece(move))
                return false;
            Refresh();
            if ((this._Game.Winner != 0) && (WinnerDeclared != null))
            {
                WinnerDeclared(this, EventArgs.Empty);
                this._FocussedSquare = Point.Empty;
            }
            return true;
        }

        private void DoFocusSquare(Point location, bool refresh)
        {
            if (!IsPlaying)
                return;
            if (this._State == CheckersUIState.ShowMove)
                return;
            // Get piece location (hit-test)
            CheckersPiece piece = this._Game.PieceAt(location);
            if (IsMovingByPlayer)
            {
                if (((location.X % 2) != (location.Y % 2)) && (((IsMovingByPlayer) && (IsHolding)) || (location == this._MovePiece.CurrentLocation)))
                {
                    this._FocussedSquare = location;
                    if (refresh)
                        Refresh();
                }
                else if (!this._FocussedSquare.IsEmpty)
                {
                    this._FocussedSquare = Point.Empty;
                    if (refresh)
                        Refresh();
                }
                return;
            }
            bool doHighlight = true;
            if (((this._Game.Turn == 1) && (!this._Player1Active)) || ((this._Game.Turn == 2) && (!this._Player2Active)))
                doHighlight = false;
            if ((piece == null) || (!this._Game.CanMovePiece(piece)))
                doHighlight = false;
            if (!doHighlight)
            {
                if (!this._FocussedSquare.IsEmpty)
                {
                    this._FocussedSquare = Point.Empty;
                    if (refresh)
                        Refresh();
                }
                return;
            }
            // Highlight board
            if ((!this._FocussedSquare.IsEmpty) && (this._FocussedSquare == piece.Location))
                return;
            this._FocussedSquare = piece.Location;
            if (refresh)
                Refresh();
        }

        #endregion

        #region UI Control

        private void CheckersUI_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IsPlaying)
                return;
            if (e.Button != MouseButtons.Left)
                return;
            CheckersPiece piece;

            switch (this._State)
            {
                case CheckersUIState.Idle:
                    // Be sure moving is possible on -any- piece this turn
                    if (((this._Game.Turn == 1) && (!this._Player1Active)) || ((this._Game.Turn == 2) && (!this._Player2Active)))
                        break;
                    // Get the piece to move, and be sure movement is possible
                    piece = this._Game.PieceAt(PointToGame(new Point(e.X, e.Y)));
                    if (piece == null)
                        break;
                    if (piece.Player != this._Game.Turn)
                        break;
                    // Begin moving the piece
                    this._MovePiece = this._Game.BeginMove(piece);
                    goto case CheckersUIState.IdleMoving;
                case CheckersUIState.IdleMoving:
                    // Be sure moving is possible on -any- piece this turn
                    if (((this._Game.Turn == 1) && (!this._Player1Active)) || ((this._Game.Turn == 2) && (!this._Player2Active)))
                        break;
                    if (this._MovePiece == null)
                        break;
                    // Get the piece to move, and be sure movement is possible
                    piece = this._MovePiece.Piece;
                    if (piece == null)
                        break;
                    if (piece.Player != this._Game.Turn)
                        break;
                    if (this._MovePiece.CurrentLocation != PointToGame(new Point(e.X, e.Y)))
                        break;
                    // Set new state and the point of the initial drag
                    this._State = CheckersUIState.DragMoving;
                    this._InitialDrag = new Point(e.X, e.Y);
                    this._InitialDragGrace = 2;
                    // Set cursor and highlight the squares
                    Cursor = this._PieceCursors[(piece.Player - 1) * 2 + ((piece.Rank == CheckersRank.Pawn) ? (0) : (1))];
                    this._SelectedSquares = this._MovePiece.EnumMoves();
                    this._DarkenedSquares = new Point[0];
                    Refresh();
                    if (PiecePickedUp != null)
                        PiecePickedUp(this, EventArgs.Empty);
                    break;
                case CheckersUIState.DragMoving:
                    // Should never reach here (unless UI/focus glitch occurred)
                    goto case CheckersUIState.ClickMoving;
                case CheckersUIState.ClickMoving:
                    // Drop the piece
                    this._State = CheckersUIState.DragMoving;
                    CheckersUI_MouseUp(sender, e);
                    break;
            }
        }

        private void CheckersUI_MouseUp(object sender, MouseEventArgs e)
        {
            if (!IsPlaying)
                return;
            if (e.Button != MouseButtons.Left)
                return;
            this._BlinkCount = 0;

            switch (this._State)
            {
                case CheckersUIState.Idle:
                    // Do nothing
                    break;
                case CheckersUIState.IdleMoving:
                    // Do nothing
                    break;
                case CheckersUIState.DragMoving:
                    // Check for drag proximity
                    if ((!this._InitialDrag.IsEmpty) && (this._InitialDragGrace > 0))
                        if ((e.X >= this._InitialDrag.X - 1) && (e.X <= this._InitialDrag.X + 1) && (e.Y >= this._InitialDrag.Y - 1) && (e.Y <= this._InitialDrag.Y + 1))
                        {
                            this._State = CheckersUIState.ClickMoving;
                            break;
                        }
                    // Drop the piece
                    Cursor = Cursors.Default;
                    Point location = PointToGame(new Point(e.X, e.Y));
                    // Move the piece
                    bool dropSuccess = false, partialSuccess = false, invalidJump = false;
                    CheckersPiece piece = this._MovePiece.Piece;
                    CheckersMove move = this._MovePiece;
                    if (this._MovePiece.Move(location))
                    {
                        if (this._MovePiece.MustMove)
                        {
                            this._State = CheckersUIState.IdleMoving;
                            partialSuccess = true;
                            this._DarkenedSquares = new[] { this._MovePiece.CurrentLocation };
                        }
                        else
                        {
                            // Move the piece on the gameboard
                            dropSuccess = MovePieceCore(this._MovePiece, false, false);
                            this._MovePiece = null;
                            this._State = CheckersUIState.Idle;
                        }
                    }
                    else
                    {
                        invalidJump = ((!this._Game.OptionalJumping) && (this._MovePiece.IsValidMove(location, true)));
                        this._MovePiece = null;
                        this._State = CheckersUIState.Idle;
                    }
                    this._SelectedSquares = new Point[0];
                    DoFocusSquare(location, false);
                    Refresh();
                    bool isWinningMove = !IsPlaying;
                    if (dropSuccess)
                    {
                        if (PieceMoved != null)
                            PieceMoved(this, new MoveEventArgs(true, isWinningMove, move.Clone()));
                    }
                    else if (partialSuccess)
                    {
                        if (PieceMovedPartial != null)
                            PieceMovedPartial(this, new MoveEventArgs(true, false, move.Clone()));
                    }
                    else
                    {
                        if (location == piece.Location)
                            if (PieceDeselected != null)
                                PieceDeselected(this, EventArgs.Empty);
                        if (location != piece.Location)
                            if (PieceBadMove != null)
                                PieceBadMove(this, new MoveEventArgs(true, false, move.Clone()));
                    }
                    if (invalidJump)
                    {
                        if (this._ShowJumpMessage)
                            MessageBox.Show(this, @"You must jump your opponent's piece.", @"Checkers", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        BlinkPieces(this._Game.EnumMovablePieces());
                    }
                    break;
                case CheckersUIState.ClickMoving:
                    // Do nothing
                    break;
            }
        }

        private void CheckersUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsPlaying)
                return;
            if (this._InitialDragGrace > 0)
                this._InitialDragGrace--;
            DoFocusSquare(PointToGame(new Point(e.X, e.Y)), true);
        }

        private void CheckersUI_MouseLeave(object sender, EventArgs e)
        {
            if (!IsPlaying)
                return;
            if (this._FocussedSquare.IsEmpty)
                return;
            this._FocussedSquare = Point.Empty;
            Refresh();
        }

        private void tmrBlink_Tick(object sender, EventArgs e)
        {
            if (this._State != CheckersUIState.Idle)
            {
                this._TmrBlink.Stop();
                return;
            }
            if (this._BlinkCount == 0)
            {
                this._DarkenedSquares = new Point[0];
                this._TmrBlink.Stop();
            }
            if (this._BlinkCount > 0)
                this._BlinkCount--;
            Refresh();
        }

        private void BlinkPieces(CheckersPiece[] pieces)
        {
            Point[] squares = new Point[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
                squares[i] = pieces[i].Location;
            BlinkSquares(squares);
        }
        private void BlinkSquares(Point[] squares)
        {
            if (this._State != CheckersUIState.Idle)
                return;
            this._DarkenedSquares = squares;
            this._BlinkCount = 2;
            this._TmrBlink.Interval = 200;
            this._TmrBlink.Start();
            Refresh();
        }

        private void tmrShowMove_Tick(object sender, EventArgs e)
        {
            if (this._State != CheckersUIState.ShowMove)
                return;
            if (this._BlinkCount > 0)
            {
                this._FocussedSquare = (((this._BlinkCount % 2) == 0) ? (this._MovePiece.CurrentLocation) : (Point.Empty));
                this._BlinkCount--;
                Refresh();
                this._TmrShowMove.Interval = ((this._BlinkCount == 0) ? (600) : (100));
                return;
            }

            bool failed = false;
            if ((this._MovePiece.Path.Length < this._DestPiece.Path.Length))
            {
                this._TmrShowMove.Interval = 500;
                failed = !this._MovePiece.Move(this._DestPiece.Path[this._MovePiece.Path.Length]);
                if (((!failed) && (this._MovePiece.Path.Length < this._DestPiece.Path.Length)) || (this._DelayAfterShow))
                {
                    Refresh();
                    if (PieceMovedPartial != null)
                        PieceMovedPartial(this, new MoveEventArgs(false, false, this._MovePiece.Clone()));
                    return;
                }
            }
            // Completed the path
            CheckersMove move = this._DestPiece;
            this._DestPiece = null;
            this._MovePiece = null;
            this._State = CheckersUIState.Idle;
            this._TmrShowMove.Stop();
            bool winner = (Winner != 0);
            if ((this._MoveAfterShow) && (!failed))
                MovePieceCore(move, false, false);
            else
                Refresh();
            if ((PieceMoved != null) && (!this._DelayAfterShow))
                PieceMoved(this, new MoveEventArgs(false, !((IsPlaying) || ((Winner != 0) && (winner))), move.Clone()));
            // If move was invalid, show msgbox now that movement has stopped
            if (failed)
                MessageBox.Show(this, @"Could not show piece movement .. path seems to be invalid!\n\nAborting move.", @"Checkers", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowMoveCore(CheckersMove move, bool noDelay, bool blink, bool moveWhenDone, bool delayWhenDone)
        {
            if (this._State != CheckersUIState.Idle)
                return;
            // Remember movement and begin a new movement to follow the same path
            this._DestPiece = move;
            this._MovePiece = this._DestPiece.InitialGame.BeginMove(this._DestPiece.InitialPiece);
            this._State = CheckersUIState.ShowMove;
            this._MoveAfterShow = moveWhenDone;
            this._DelayAfterShow = delayWhenDone;
            if (blink)
            {
                this._FocussedSquare = this._MovePiece.CurrentLocation;
                this._BlinkCount = 3;
                Refresh();
                this._TmrShowMove.Interval = 100;
            }
            else
            {
                this._FocussedSquare = Point.Empty;
                Refresh();
                this._BlinkCount = 0;
                this._TmrShowMove.Interval = (noDelay) ? (10) : (600);
            }
            this._TmrShowMove.Start();
        }

        #endregion

        /// <summary>Begins the checkers game with a new CheckersGame object if none is selected.</summary>
        public void Play()
        {
            if (IsPlaying)
                throw new InvalidOperationException("Game has already started.");
            if (this._Game == null)
                SetGame(new CheckersGame());
            this._Game.Play();
        }
        /// <summary>Begins the checkers game.</summary>
        /// <param name="game">The game to play.</param>
        public void Play(CheckersGame game)
        {
            if (IsPlaying)
                throw new InvalidOperationException("Game has already started.");
            SetGame(game);
            Play();
        }

        /// <summary>Stops a decided game or forces a game-in-progress to stop prematurely with no winner.</summary>
        public void Stop()
        {
            if (this._Game == null)
                return;
            this._Game.Stop();
        }

        /// <summary> Computes the location of the specified point into Checkers game board coordinates. </summary>
        /// <param name="p">The screen coordinate to convert.</param>
        /// <returns>The location in Checkers game board coorinates.</returns>
        public Point PointToGame(Point p)
        {
            int borderSize;
            if (this._BorderStyle == BorderStyle.Fixed3D)
                borderSize = 2;
            else if (this._BorderStyle == BorderStyle.FixedSingle)
                borderSize = 1;
            else
                borderSize = 0;
            int x = (p.X - this._BoardMargin - borderSize - 1) / SquareSize.Width;
            int y = (p.Y - this._BoardMargin - borderSize - 1) / SquareSize.Height;
            return new Point(x, y);
        }

        /// <summary>Moves a Checkers piece on the board.</summary>
        /// <returns>True if the piece was moved successfully.</returns>
        public bool MovePiece(CheckersPiece piece, Point[] path)
        {
            return MovePiece(piece, path, true);
        }
        /// <summary>Moves a Checkers piece on the board.</summary>
        /// <returns>True if the piece was moved successfully.</returns>
        public bool MovePiece(CheckersPiece piece, Point[] path, bool showMove)
        {
            return MovePiece(piece, path, showMove, false);
        }
        /// <summary>Moves a Checkers piece on the board.</summary>
        /// <returns>True if the piece was moved successfully.</returns>
        public bool MovePiece(CheckersPiece piece, Point[] path, bool showMove, bool noDelay)
        {
            if (!IsPlaying)
                return false;
            CheckersMove move = this._Game.BeginMove(piece);
            foreach (Point point in path)
                if (!move.Move(point))
                    return false;
            return MovePiece(move, showMove, noDelay);
        }
        /// <summary>Moves a Checkers piece on the board.</summary>
        /// <param name="move">The movement object to which the piece will move to.</param>
        /// <returns>True if the piece was moved successfully.</returns>
        public bool MovePiece(CheckersMove move)
        {
            return MovePiece(move, true);
        }
        /// <summary>Moves a Checkers piece on the board.</summary>
        /// <param name="move">The movement object to which the piece will move to.</param>
        /// <param name="showMove">Decides whether or not the move should be shown.</param>
        /// <returns>True if the piece was moved successfully.</returns>
        public bool MovePiece(CheckersMove move, bool showMove)
        {
            return MovePiece(move, showMove, false);
        }
        /// <summary>Moves a Checkers piece on the board.</summary>
        /// <param name="move">The movement object to which the piece will move to.</param>
        /// <param name="showMove">Decides whether or not the move should be shown.</param>
        /// <param name="noDelay">Delay animation on moviment</param>
        /// <returns>True if the piece was moved successfully.</returns>
        public bool MovePiece(CheckersMove move, bool showMove, bool noDelay)
        {
            if (!IsPlaying)
                return false;
            return MovePieceCore(move, showMove, noDelay);
        }
        

        /// <summary>Shows the last move that was made.</summary>
        public void ShowLastMove()
        {
            ShowLastMove(true);
        }
        /// <summary>Shows the last move that was made.</summary>
        /// <param name="blink">Decides whether or not to make the piece to be moved blink before moving.</param>
        public void ShowLastMove(bool blink)
        {
            ShowLastMove(blink, false);
        }
        /// <summary>Shows the last move that was made.</summary>
        /// <param name="blink">Decides whether or not to make the piece to be moved blink before moving.</param>
        /// <param name="noDelay">True for no delay in movement.</param>
        public void ShowLastMove(bool blink, bool noDelay)
        {
            if ((!IsPlaying) && (Winner == 0))
                return;
            if (this._Game.LastMove == null)
                return;
            ShowMoveCore(this._Game.LastMove, noDelay, blink, false, false);
        }

        /// <summary> Refreshes the Checkers baord and the control.</summary>
        public override void Refresh()
        {
            if (this._BoardImage == null)
                return;
            Graphics g = Graphics.FromImage(this._BoardImage);
            Pen penGridColor = new Pen(this._BoardGridColor);
            Brush brushBackColor = new SolidBrush(this._BoardBackColor);
            Brush brushForeColor = new SolidBrush(this._BoardForeColor);

            // Draw the grid and the board background
            g.DrawRectangle(penGridColor, 0, 0, BoardPixelSize.Width + 1, BoardPixelSize.Height + 1);
            g.FillRectangle(brushBackColor, 1, 1, BoardPixelSize.Width, BoardPixelSize.Height);

            // Draw the squares and pieces
            for (int y = 0; y < CheckersGame.BoardSize.Height; y++)
            {
                for (int x = 0; x < CheckersGame.BoardSize.Width; x++)
                {
                    if ((x % 2) == (y % 2))
                        continue;
                    // Get whether or not square is a 'valid move' square
                    bool isDarkenedSquare = false, isValidMoveSquare = false;
                    bool isFocussedSquare = (((this._HighlightSelection) || (this._State == CheckersUIState.ShowMove)) && (this._FocussedSquare.X == x) && (this._FocussedSquare.Y == y));
                    if (this._HighlightPossibleMoves)
                        foreach (Point p in this._SelectedSquares)
                            if ((p.X == x) && (p.Y == y))
                            {
                                isValidMoveSquare = true;
                                break;
                            }
                    if ((this._BlinkCount % 2) == 0)
                        foreach (Point p in this._DarkenedSquares)
                            if ((p.X == x) && (p.Y == y))
                            {
                                isDarkenedSquare = true;
                                break;
                            }
                    // Draw the square
                    if (isFocussedSquare)
                    {
                        g.FillRectangle(new SolidBrush(BlendColor(this._BoardForeColor, Color.FromArgb(49, 106, 197), 50)), x * SquareSize.Width + 1, y * SquareSize.Height + 1, SquareSize.Width, SquareSize.Height);
                        g.FillRectangle(new SolidBrush(BlendColor(this._BoardForeColor, Color.FromArgb(193, 210, 238), 50)), x * SquareSize.Width + 2, y * SquareSize.Height + 2, SquareSize.Width - 2, SquareSize.Height - 2);
                    }
                    else if (isDarkenedSquare)
                    {
                        g.FillRectangle(new SolidBrush(BlendColor(this._BoardForeColor, Color.FromArgb(197, 106, 49), 50)), x * SquareSize.Width + 1, y * SquareSize.Height + 1, SquareSize.Width, SquareSize.Height);
                        g.FillRectangle(new SolidBrush(BlendColor(this._BoardForeColor, Color.FromArgb(238, 210, 193), 50)), x * SquareSize.Width + 2, y * SquareSize.Height + 2, SquareSize.Width - 2, SquareSize.Height - 2);
                    }
                    else if (isValidMoveSquare)
                    {
                        g.FillRectangle(new SolidBrush(BlendColor(this._BoardForeColor, Color.FromArgb(152, 180, 226), 50)), x * SquareSize.Width + 1, y * SquareSize.Height + 1, SquareSize.Width, SquareSize.Height);
                        g.FillRectangle(new SolidBrush(BlendColor(this._BoardForeColor, Color.FromArgb(224, 232, 246), 50)), x * SquareSize.Width + 2, y * SquareSize.Height + 2, SquareSize.Width - 2, SquareSize.Height - 2);
                    }
                    else
                    {
                        g.FillRectangle(brushForeColor, x * SquareSize.Width + 1, y * SquareSize.Height + 1, SquareSize.Width, SquareSize.Height);
                    }

                    // Do no further drawing if not playing
                    if ((!IsPlaying) && (Winner == 0))
                        continue;

                    // Draw game pieces
                    CheckersPiece piece = this._Game.Board[x, y];
                    switch (this._State)
                    {
                        case CheckersUIState.Idle:
                            break;
                        case CheckersUIState.IdleMoving:
                            if (piece == this._MovePiece.Piece)
                                piece = null;               // Hide piece being moved
                            if (this._MovePiece.CurrentLocation == new Point(x, y))
                                piece = this._MovePiece.Piece;    // Show piece in new location
                            foreach (CheckersPiece jumped in this._MovePiece.Jumped)
                                if (piece == jumped)
                                    piece = null;  // Hide jumped (in-progress)
                            break;
                        case CheckersUIState.DragMoving:
                            if (piece == this._MovePiece.Piece)
                                piece = null;               // Hide piece being moved
                            foreach (CheckersPiece jumped in this._MovePiece.Jumped)
                                if (piece == jumped)
                                    piece = null;  // Hide jumped (in-progress)
                            break;
                        case CheckersUIState.ClickMoving:
                            goto case CheckersUIState.DragMoving;
                        case CheckersUIState.ShowMove:
                            piece = this._MovePiece.Game.Board[x, y];
                            if (piece == this._MovePiece.Piece)
                                piece = null;               // Hide piece being moved
                            if (this._MovePiece.CurrentLocation == new Point(x, y))
                                piece = this._MovePiece.Piece;    // Show piece in new location
                            foreach (CheckersPiece jumped in this._MovePiece.Jumped)
                                if (piece == jumped)
                                    piece = null;  // Hide jumped (in-progress)
                            break;
                    }
                    // Draw the piece
                    if (piece != null)
                    {
                        Image image = null;
                        if (piece.Player == 1)
                            image = this._PieceImages[((piece.Rank == CheckersRank.Pawn) ? (0) : (1))];
                        else if (piece.Player == 2)
                            image = this._PieceImages[((piece.Rank == CheckersRank.Pawn) ? (2) : (3))];
                        if (image == null)
                            continue;
                        g.DrawImage(image, x * SquareSize.Width + 1, y * SquareSize.Height + 1, 32, 32);
                    }
                }
            }

            g.Dispose();
            brushForeColor.Dispose();
            brushBackColor.Dispose();
            penGridColor.Dispose();
            base.Refresh();
        }
    }
}
