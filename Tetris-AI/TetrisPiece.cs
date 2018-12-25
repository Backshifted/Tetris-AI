using System;

namespace Tetris_AI
{
    public class TetrisPiece
    {
        /* Grid used to store block locations in row major order.
         * 0 = no block, 1 = block in that position*/
        public bool[] grid;
        /* Integer to hold the grid dimensions for rotation.
         * It is always a square thus only 1 length is needed. */
        public int gridSize;

        public TetrisPiece(bool[] grid, int gridSize)
        {
            this.grid = grid;
            this.gridSize = gridSize;
        }

        public TetrisPiece(bool[] grid) : this(grid, (int)Math.Sqrt(grid.Length)) {
        }

        /* Return all rotations of the current piece in clock-wise order. */
        public TetrisPiece[] getRotations()
        {
            return new TetrisPiece[4] {
                this,
                new TetrisPiece(rotateGrid90(grid, gridSize), gridSize),
                new TetrisPiece(rotateGrid180(grid, gridSize), gridSize),
                new TetrisPiece(rotateGrid90R(grid, gridSize), gridSize)
            };
        }

        /* Rotates a matrix by 90 degrees.
         * This is achieved by transposing the array and accessing the target
         * array's columns in reverse order. */
        private static bool[] rotateGrid90(bool[] grid, int gridSize)
        {
            /* Create a target grid. */
            bool[] rotatedGrid = new bool[gridSize * gridSize];

            /* Two nested loops to calculate row and column positions. */
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    /* Transpose array and access the columns in rotatedGrid in reverse order. */
                    rotatedGrid[j * gridSize + (gridSize - i - 1)] = grid[i * gridSize + j];
                }
            }

            return rotatedGrid;
        }

        /* Rotates a matrix by -90 degrees.
         * This is achieved by transposing the array and accessing the source
         * array's columns in reverse order. */
        private static bool[] rotateGrid90R(bool[] grid, int gridSize)
        {
            bool[] rotatedGrid = new bool[gridSize * gridSize];

            /* Two nested loops to calculate row and column positions. */
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    /* Transpose array and access the columns in grid in reverse order. */
                    rotatedGrid[j * gridSize + i] = grid[i * gridSize + (gridSize - j - 1)];
                }
            }

            return rotatedGrid;
        }

        /* Rotates a matrix by 180 degrees.
         * This is achieved by transposing the array and accessing the source
         * array's columns in reverse order. */
        private static bool[] rotateGrid180(bool[] grid, int gridSize)
        {
            bool[] rotatedGrid = new bool[gridSize * gridSize];

            /* Two nested loops to calculate row and column positions. */
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    /* Transpose array by accessing rotatedGrid's rows and columns in reverse order. */
                    rotatedGrid[(gridSize - j - 1) * gridSize + (gridSize - i - 1)] = grid[i * gridSize + j];
                }
            }

            return rotatedGrid;
        }
    }

    public class LongPiece : TetrisPiece
    {
        public LongPiece() :
            base(new bool[] {
                false, false, false, false,
                true, true, true, true,
                false, false, false, false,
                false, false, false, false
            }, gridSize: 4)
        { }
    }

    public class JPiece : TetrisPiece
    {
        public JPiece() :
            base(new bool[] {
                true, false, false,
                true, true, true,
                false, false, false,
            }, gridSize: 3)
        { }
    }

    public class LPiece : TetrisPiece
    {
        public LPiece() :
            base(new bool[] {
                false, false, true,
                true, true, true,
                false, false, false,
            }, gridSize: 3)
        { }
    }

    public class TPiece : TetrisPiece
    {
        public TPiece() :
            base(new bool[] {
                false, true, false,
                true, true, true,
                false, false, false,
            }, gridSize: 3)
        { }
    }

    public class SPiece : TetrisPiece
    {
        public SPiece() :
            base(new bool[] {
                false, true, true,
                true, true, false,
                false, false, false,
            }, gridSize: 3)
        { }
    }

    public class ZPiece : TetrisPiece
    {
        public ZPiece() :
            base(new bool[] {
                true, true, false,
                false, true, true,
                false, false, false,
            }, gridSize: 3)
        { }
    }

    public class SquarePiece : TetrisPiece
    {
        public SquarePiece() :
            base(new bool[] {
                true, true,
                true, true,
            }, gridSize: 2)
        { }
    }
}
