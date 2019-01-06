namespace Tetris_AI
{
    public class Tetromino : Grid
    {
        /* The amount of asymmetrical rotations of a tetromino. */
        public int UniqueRotations { get; private set; }

        public Tetromino(Grid grid, int uniqueRotations) : base(grid.Array, grid.Width)
        {
            UniqueRotations = uniqueRotations;
        }
        
        public new Tetromino Clone()
        {
            return new Tetromino(base.Clone(), UniqueRotations);
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
            }, 4), 2)
        { }
    }

    public class JPiece : Tetromino
    {
        public JPiece() : base(new Grid(new bool[] {
            true, false, false,
            true, true, true,
            false, false, false
        }, 3), 4) { }
    }

    public class LPiece : Tetromino
    {
        public LPiece() : base(new Grid(new bool[] {
            false, false, true,
            true, true, true,
            false, false, false,
        }, 3), 4) { }
    }

    public class TPiece : Tetromino
    {
        public TPiece() : base(new Grid(new bool[] {
            false, true, false,
            true, true, true,
            false, false, false,
        }, 3), 4) { }
    }

    public class SPiece : Tetromino
    {
        public SPiece() : base(new Grid(new bool[] {
            false, true, true,
            true, true, false,
            false, false, false,
        }, 3), 2) { }
    }

    public class ZPiece : Tetromino
    {
        public ZPiece() : base(new Grid(new bool[] {
            true, true, false,
            false, true, true,
            false, false, false,
        }, 3), 2) { }
    }

    public class OPiece : Tetromino
    {
        public OPiece() : base(new Grid(new bool[] {
            true, true,
            true, true,
        }, 2), 1) { }
    }
}
