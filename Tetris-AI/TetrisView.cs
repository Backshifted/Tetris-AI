using System.Drawing;
using System.Windows.Forms;

namespace Tetris_AI
{
    public partial class TetrisView : UserControl
    {
        public readonly int DefaultBlockSize = 30;
        public readonly int DefaultBorderWidth = 2;
        public new readonly Color DefaultBackColor = Color.Gainsboro;
        public readonly Color DefaultBlockColor = Color.Aqua;
        public readonly Color DefaultBorderColor = Color.DarkGray;

        /* The block size determines the size of the block and the component itself. */
        private int blockSize = 30;
        public int BlockSize
        {
            get { return blockSize; }
            set
            {
                blockSize = value;
                Width = TetrisProcessor.GRID_WIDTH * value;
                Height = TetrisProcessor.GRID_HEIGHT * value;
            }
        }

        /* Color the blocks will be drawn with. */
        public Color BlockColor
        {
            get { return ForeColor; }
            set
            {
                ForeColor = value;
                BlockBrush = new SolidBrush(value);
            }
        }

        /* Border color of the blocks and the component itself. */
        private Color borderColor;
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                BorderPen = new Pen(value, BorderWidth);
            }
        }

        /* Determines the width of block and grid borders. */
        private int borderWidth;
        public int BorderWidth
        {
            get { return borderWidth; }
            set
            {
                borderWidth = value;
                BorderPen = new Pen(BorderColor, value);
            }
        }

        /* Make width publicly unsetable. */
        public new int Width
        {
            get { return base.Width; }
            private set { base.Width = value; }
        }

        /* Make height publicly unsetable. */
        public new int Height
        {
            get { return base.Height; }
            private set { base.Height = value; }
        }

        /* Make the size constrained to the block size. */
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = new Size(TetrisProcessor.GRID_WIDTH * BlockSize, TetrisProcessor.GRID_HEIGHT * BlockSize); }
        }

        /* Brushes & pens used to draw the tetris field with. */
        private SolidBrush BlockBrush;
        private Pen BorderPen;

        public TetrisProcessor Processor { get; set; }

        public TetrisView()
        {
            /* Set user control's dimensions. */
            BlockSize = DefaultBlockSize;
            Width = TetrisProcessor.GRID_WIDTH * BlockSize;
            Height = TetrisProcessor.GRID_HEIGHT * BlockSize;

            InitializeComponent();

            /* Make the user control selectable to allow for user input. */
            SetStyle(ControlStyles.Selectable, true);
        }

        /* Sets the processor and the size of the tetris view. */
        public void InitView()
        {
            /* If no external processor has been set, create a new one. */
            if (Processor == null)
                Processor = new TetrisProcessor();

            /* Prevent flickering by double buffering the user control. */
            DoubleBuffered = true;

            /* Set all initial values. */

            BorderPen = new Pen(DefaultBorderColor, DefaultBorderWidth);

            BackColor = DefaultBackColor;
            BlockColor = DefaultBlockColor;
            BorderColor = DefaultBorderColor;
            BorderWidth = DefaultBorderWidth;
            
            /* Add a listener to the grid update event. */
            Processor.GridUpdateEvent += Processor_UpdateEvent;
            /* Preview the first piece. */
            Processor.PreviewNextPiece();
        }

        /* Redraw the user control when the grid has been updated. */
        private void Processor_UpdateEvent()
        {
            Invalidate();
        }

        /* Custom paint method. */
        protected override void OnPaint(PaintEventArgs e)
        {
            /* If there is no processor or the control is in design mode,
             * paint it with default values. */
            if (DesignMode || Processor == null)
            {
                base.OnPaint(e);
                PaintShading(e.Graphics);
                PaintDefaultBorder(e.Graphics);
            }
            else
            {
                PaintBlocks(e.Graphics);
                PaintShading(e.Graphics);
                PaintBorder(e.Graphics);
            }
        }

        /* Paints tetrominoes in the current grid. */
        private void PaintBlocks(Graphics graphics)
        {
            /* Paint all the blocks in the grid with their borders. */
            for (int row = 0; row < TetrisProcessor.GRID_HEIGHT; row++)
            {
                for (int col = 0; col < TetrisProcessor.GRID_WIDTH; col++)
                {
                    /* Only paint blocks if there is a tetromino there. */
                    if (Processor.Grid.GetBlock(row, col))
                    {
                        Rectangle r = new Rectangle(col * BlockSize, row * BlockSize, BlockSize, BlockSize);
                        graphics.FillRectangle(BlockBrush, r);
                        /* Draw block border. */
                        graphics.DrawRectangle(BorderPen, r);
                    }
                }
            }
        }

        /* Paints a transparent square on the top two rows. */
        private void PaintShading(Graphics graphics)
        {
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 0, 0, Width, BlockSize * 2);
        }

        /* Paints the border of the user control. */
        private void PaintBorder(Graphics graphics)
        {
            graphics.DrawRectangle(BorderPen, 1, 1, Width - 2, Height - 2);
        }

        /* Paints the border of the user control with default values. */
        private void PaintDefaultBorder(Graphics graphics)
        {
            graphics.DrawRectangle(new Pen(Color.DarkGray, 2), 1, 1, Width - 2, Height - 2);
        }

        /* Handles key input to the user control. */
        private void TetrisView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Left)
            {
                Processor.MovePreviewPiece(-1);
            }
            else if (e.KeyData == Keys.Right)
            {
                Processor.MovePreviewPiece(1);
            }
            else if (e.KeyData == Keys.Z)
            {
                Processor.RotatePreviewPiece(-1);   
            }
            else if (e.KeyData == Keys.X)
            {
                Processor.RotatePreviewPiece(1);
            }
            else if (e.KeyData == Keys.Space)
            {
                Processor.PlacePreviewPiece();
            }
        }
    }
}
