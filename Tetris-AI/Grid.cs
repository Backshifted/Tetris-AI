namespace Tetris_AI
{
    /* A bit grid class width getters and setters. */
    public class Grid
    {
        /* An array to store the grid in a row major order. */
        public bool[] Array { get; set; }

        public int Width { get; }
        public int Height { get; }

        /* Creates a grid with the given dimensions and fills it with the input grid. */
        public Grid(bool[] gridArray, int width, int height)
        {
            Array = gridArray;
            Width = width;
            Height = height;
        }

        /* Creates a grid with the given dimensions. */
        public Grid(int width, int height) : this(new bool[width * height], width, height) { }

        /* Creates a grid with the same width and height and fills it with the input grid. */
        public Grid(bool[] gridArray, int size) : this(gridArray, size, size) { }

        /* Creates a grid with the same width and height. */
        public Grid(int size) : this(size, size) { }

        /* ORs the input value with the value in the grid. */
        public void ORBlock(int row, int col, bool val)
        {
            SetBlock(row, col, val || GetBlock(row, col));
        }

        /* Sets a value in the grid. */
        public void SetBlock(int row, int col, bool val)
        {
            /* Return when out of bounds. */
            if (row < 0 || row >= Height || col < 0 || col >= Width)
                return;

            Array[row * Width + col] = val;
        }

        /* Gets a value from the grid. */
        public bool GetBlock(int row, int col)
        {
            /* Return false when out of bounds. */
            if (row < 0 || row >= Height || col < 0 || col >= Width)
                return false;

            return Array[row * Width + col];
        }

        /* Gets the position of the highest block in the column. */
        public int HighestBlockPosition(int col)
        {
            for (int i = 0; i < Height; i++)
            {
                if (GetBlock(i, col))
                    return i;
            }

            return Height;
        }

        /* Gets the position of the lowest block in the column. */
        public int LowestBlockPosition(int col)
        {
            for (int i = Height - 1; i >= 0; i--)
            {
                if (GetBlock(i, col))
                    return i;
            }

            return -100;
        }

        /* Calculates the distance from the given position first block from the bottom up. */
        public int DistanceToFirstBlock(int row, int col)
        {
            return HighestBlockPosition(col) - row - 1;
        }

        /* Returns true when the row is full of blocks, false otherwise. */
        public bool RowFull(int row)
        {
            for (int i = 0; i < Width; i++)
            {
                if (!GetBlock(row, i))
                    return false;
            }

            return true;
        }

        /* Returns true when the row doesn't contain any blocks, false otherwise. */
        public bool RowEmpty(int row)
        {
            for (int i = 0; i < Width; i++)
            {
                if (GetBlock(row, i))
                    return false;
            }

            return true;
        }

        /* Clears a row and moves the rows above it down by 1.
         * The empty rows parameter specifies until which row number
         * rows should be moved down. */
        public void ClearRow(int row, int emptyRows)
        {
            /* Move rows above down by 1. */
            for (int i = row; i >= emptyRows; i--)
            {
                for (int j = 0; j < Width; j++)
                {
                    SetBlock(i, j, GetBlock(i - 1, j));
                }
            }
        }

        /* Clears all full rows in the field. */
        public void ClearFullRows()
        {
            int emptyRows = 0;
            int rowsLeft = 4;
            bool rowCleared = false;

            /* Scan until a non-empty row is found. 
             * Or until 4 rows have been scanned since clearing a row,
             * as no more than 4 consecutive rows can be cleared at once. */
            for (int i = 0; i < Height && rowsLeft > 0; i++)
            {
                if(RowEmpty(i))
                    emptyRows++;

                if (RowFull(i))
                {
                    ClearRow(i, emptyRows++);
                    /* Start counting down the rows left to scan. */
                    rowCleared = true;
                }

                /* Count down the rows left to scan. */
                if (rowCleared)
                    rowsLeft--;
            }
        }

        public void InsertGrid(Grid grid, int row, int col)
        {
            for (int i = 0; i < grid.Height && row + i < Height; i++)
            {
                for (int j = 0; j < grid.Width && col + j < Width; j++)
                {
                    ORBlock(row + i, col + j, grid.GetBlock(i, j));
                }
            }
        }

        public void DeleteGrid(Grid grid, int row, int col)
        {
            for (int i = 0; i < grid.Height && row + i < Height; i++)
            {
                for (int j = 0; j < grid.Width && col + j < Width; j++)
                {
                    if (grid.GetBlock(i, j))
                        SetBlock(row + i, col + j, false);
                }
            }
        }

        /* Rotates a matrix by 90 degrees.
         * This is achieved by transposing the array and reversing the rows. */
        public Grid Rotate90Deg()
        {
            /* Create a target grid. */
            Grid rotatedGrid = new Grid(Height, Width);

            /* Two nested loops to calculate row and column positions. */
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    /* Transpose array and access the rows in rotatedGrid in reverse order. */
                    rotatedGrid.Array[j * rotatedGrid.Width + (rotatedGrid.Width - i - 1)] = Array[i * Width + j];
                }
            }

            return rotatedGrid;
        }

        /* Rotates a matrix by 180 degrees.
         * This is achieved by reversing both rows and columns */
        public Grid Rotate180Deg()
        {
            /* Create a target grid. */
            Grid rotatedGrid = new Grid(Width, Height);

            /* Two nested loops to calculate row and column positions. */
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    /* Reverse both columns and arrays. */
                    rotatedGrid.Array[(rotatedGrid.Height - i - 1) * rotatedGrid.Width + (rotatedGrid.Width - j - 1)] = Array[i * Width + j];
                }
            }

            return rotatedGrid;
        }

        /* Rotates a matrix by 270 or -90 degrees.
         * This is achieved by transposing the array and reversing the columns */
        public Grid Rotate270Deg()
        {
            /* Create a target grid. */
            Grid rotatedGrid = new Grid(Height, Width);

            /* Two nested loops to calculate row and column positions. */
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    /* Transpose array and access the columns in rotatedGrid in reverse order. */
                    rotatedGrid.Array[(rotatedGrid.Height - j - 1) * rotatedGrid.Width + i] = Array[i * Width + j];
                }
            }

            return rotatedGrid;
        }
    }
}
