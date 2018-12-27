namespace Tetris_AI
{
    public class Tetromino : Grid
    {
        /* Since tetromino grids are always a square
         * only 1 length is required. */
        public int Size { get { return Width; } }

        public Tetromino(Grid grid) : base(grid.Array, grid.Width) { }

        /* Return all rotations of the current piece in clock-wise order. */
        public Tetromino[] GetRotations()
        {
            return new Tetromino[4] {
                this,
                new Tetromino(base.Rotate90Deg()),
                new Tetromino(base.Rotate180Deg()),
                new Tetromino(base.Rotate270Deg())
            };
        }

        /* Rotate the piece by 90 degrees. */
        public new void Rotate90Deg()
        {
            Array = base.Rotate90Deg().Array;
        }

        /* Rotate the piece by 180 degrees. */
        public new void Rotate180Deg()
        {
            Array = base.Rotate180Deg().Array;
        }

        /* Rotate the piece by 270 degrees. */
        public new void Rotate270Deg()
        {
            Array = base.Rotate270Deg().Array;
        }

        /* Returns all available tetrominoes. */
        public static Tetromino[] All()
        {
            return new Tetromino[] { new IPiece(), new JPiece(), new LPiece(), new TPiece(), new SPiece(), new ZPiece(), new OPiece() };
        }
    }

    public class IPiece : Tetromino
    {
        public IPiece() :
            base(new Grid(new bool[] {
                false, false, false, false,
                true, true, true, true,
                false, false, false, false,
                false, false, false, false
            }, 4))
        { }
    }

    public class JPiece : Tetromino
    {
        public JPiece() :
            base(new Grid(new bool[] {
                true, false, false,
                true, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class LPiece : Tetromino
    {
        public LPiece() :
            base(new Grid(new bool[] {
                false, false, true,
                true, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class TPiece : Tetromino
    {
        public TPiece() :
            base(new Grid(new bool[] {
                false, true, false,
                true, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class SPiece : Tetromino
    {
        public SPiece() :
            base(new Grid(new bool[] {
                false, true, true,
                true, true, false,
                false, false, false,
            }, 3))
        { }
    }

    public class ZPiece : Tetromino
    {
        public ZPiece() :
            base(new Grid(new bool[] {
                true, true, false,
                false, true, true,
                false, false, false,
            }, 3))
        { }
    }

    public class OPiece : Tetromino
    {
        public OPiece() :
            base(new Grid(new bool[] {
                true, true,
                true, true,
            }, 2))
        { }
    }
}
