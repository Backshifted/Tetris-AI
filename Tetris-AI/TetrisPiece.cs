namespace Tetris_AI
{
    public class TetrisPiece : Grid
    {
        public int Size { get; }

        public TetrisPiece(Grid grid) : base(grid.Array, grid.Width)
        {
            Size = grid.Width;
        }

        /* Return all rotations of the current piece in clock-wise order. */
        public TetrisPiece[] getRotations()
        {
            return new TetrisPiece[4] {
                this,
                new TetrisPiece(Rotate90Deg()),
                new TetrisPiece(Rotate180Deg()),
                new TetrisPiece(Rotate270Deg())
            };
        }
    }

    public class LongPiece : TetrisPiece
    {
        public LongPiece() :
            base(new Grid(new bool[] {
                false, false, false, false,
                true, true, true, true,
                false, false, false, false,
                false, false, false, false
            }, 4))
        { }
    }

    public class JPiece : TetrisPiece
    {
        public JPiece() :
            base(new Grid(new bool[] {
                true, false, false,
                true, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class LPiece : TetrisPiece
    {
        public LPiece() :
            base(new Grid(new bool[] {
                false, false, true,
                true, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class TPiece : TetrisPiece
    {
        public TPiece() :
            base(new Grid(new bool[] {
                false, true, false,
                true, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class SPiece : TetrisPiece
    {
        public SPiece() :
            base(new Grid(new bool[] {
                false, true, true,
                true, true, false,
                false, false, false,
            }, 3))
        { }
    }

    public class ZPiece : TetrisPiece
    {
        public ZPiece() :
            base(new Grid(new bool[] {
                true, true, false,
                false, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class SquarePiece : TetrisPiece
    {
        public SquarePiece() :
            base(new Grid(new bool[] {
                true, true,
                true, true,
            }, 2))
        { }
    }
}
