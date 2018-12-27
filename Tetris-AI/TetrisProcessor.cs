namespace Tetris_AI
{
    public class TetrisProcessor
    {
        /* Tetris grid dimensions. */
        public const int GRID_WIDTH = 10;
        public const int GRID_HEIGHT = 22;
        /* A 10x22 Grid that holds the state of each piece in row major order.
         * 0 = no block, 1 = a block in that position. */
        public Grid Grid;

        /* The look-ahead piece that will be selected after the current piece has been placed. */
        private Tetromino NextPiece;
        /* Piece hovering in the top of the grid. */
        private Tetromino previewPiece;
        /* Top left corner of the preview piece in the grid. */
        private int previewPieceColumn;
        private int previewPieceRow;

        /* Tetromino generator used for obtaining the next piece. */
        private RandomGenerator RGN;

        /* Create a new eventhandler via a delegate method. */
        public delegate void EventHandler();
        /* Event raised when the grid is updated. */
        public event EventHandler GridUpdateEvent;
        /* Event raised when a new piece is selected. */
        public event EventHandler NewPieceEvent;

        public TetrisProcessor()
        {
            /* Initialize grid. */
            Grid = new Grid(GRID_WIDTH, GRID_HEIGHT);

            /* Initialize generator and get first piece. */
            RGN = new RandomGenerator();
            NextPiece = RGN.Next();

            /* Update grid. */
            RaiseUpdateEvent();
        }

        private void RaiseUpdateEvent()
        {
            if (GridUpdateEvent != null)
                GridUpdateEvent.Invoke();
        }

        private void RaiseNewPieceEvent()
        {
            if (NewPieceEvent != null)
                NewPieceEvent.Invoke();
        }

        /* Adds a piece to the tetris grid. */
        public void AddPiece(Tetromino piece, int col)
        {
            /* The offset determines how far from the top the piece must be placed. */
            int offset = MinDistanceToField(piece, col);

            /* Insert the piece grid into the playing grid. */
            Grid.InsertGrid(piece, offset, col);

            /* Update grid as a new piece had been placed. */
            RaiseUpdateEvent();

            /* Update the grid. */
            Grid.ClearFullRows();

            /* Update grid as rows might have been cleared. */
            RaiseUpdateEvent();
        }

        /* Rotates the preview piece. 
         * Direction 1 will rotate clockwise,
         * Other directions (-1) will rotate counter-clockwise. */
        public void RotatePreviewPiece(int direction)
        {
            /* Remove the current preview piece. */
            Grid.DeleteGrid(previewPiece, previewPieceRow, previewPieceColumn);

            /* Rotate preview piece. */
            if (direction == 1)
                previewPiece.Rotate90Deg();
            else
                previewPiece.Rotate270Deg();

            /* Preview rotated piece. */
            PreviewPiece(previewPiece, CalculatePreviewRowOffset(previewPiece), previewPieceColumn);
        }

        /* Moves the preview piece along the grid.
         * A positive amount will move it right, 
         * and a negative amount will move it left. */
        public void MovePreviewPiece(int amount)
        {
            /* Remove the current preview piece from the grid. */
            Grid.DeleteGrid(previewPiece, previewPieceRow, previewPieceColumn);
            /* Update preview piece location. */
            previewPieceColumn += amount;
            /* Insert the updated preview piece. */
            PreviewPiece(previewPiece, previewPieceRow, previewPieceColumn);
        }

        /* Sets the next tetromino as the current preview piece,
         * and selects a new next tetromino. */
        public void PreviewNextPiece()
        {
            /* Preview next piece. */
            PreviewPiece(NextPiece, CalculatePreviewRowOffset(NextPiece), CalculatePreviewColumnOffset(NextPiece));
            /* Get next piece. */
            NextPiece = RGN.Next();

            /* New piece has been previewed. */
            RaiseNewPieceEvent();
        }

        /* Adds a piece to the top of the grid, 
         * where it can be moved and rotated. */
        public void PreviewPiece(Tetromino piece, int row, int col)
        {
            /* Add preview piece to the grid. */
            Grid.InsertGrid(piece, row, col);

            /* Update preview piece variables. */
            previewPiece = piece;
            previewPieceRow = row;
            previewPieceColumn = col;

            /* Grid has been updated by adding a preview piece. */
            RaiseUpdateEvent();
        }

        /* Places the preview piece down from it's current position. */
        public void PlacePreviewPiece()
        {
            /* Remove the current preview piece from the grid. */
            Grid.DeleteGrid(previewPiece, previewPieceRow, previewPieceColumn);
            /* Insert the piece in the correct row. */
            AddPiece(previewPiece, previewPieceColumn);
            /* Preview the next piece. */
            PreviewNextPiece();
        }

        /* Calculates the smallest distance from the tetromino to the playing field. */
        private int MinDistanceToField(Tetromino piece, int col)
        {
            /* Get the first distance from piece to field. */
            int minDistance = Grid.DistanceToFirstBlock(piece.LowestBlockPosition(0), col);

            /* Compare remaining distances. */
            for (int i = 1; i < piece.Size; i++)
            {
                /* Get distance. */
                int distance = Grid.DistanceToFirstBlock(piece.LowestBlockPosition(i), col + i);

                /* Select smallest distance.*/
                if (minDistance > distance)
                    minDistance = distance;
            }

            return minDistance;
        }

        /* Calculates the amount of rows the tetromino must be moved up
         * to touch the top of the grid. */
        private int CalculatePreviewRowOffset(Tetromino piece)
        {
            /* Startvalue of -1 as 0 is a valid row. */
            int row = -1;

            /* Loop through the entire grid or until a block has been found. */
            for (int i = 0; i < piece.Size && row == -1; i++)
            {
                for (int j = 0; j < piece.Size; j++)
                {
                    if (piece.GetBlock(i, j))
                    {
                        row = i;
                        break;
                    }
                }
            }

            return -row;
        }

        /* Calculates the amount of columns the tetromino must be moved
         * to the right to be in the center of the grid. */
        private int CalculatePreviewColumnOffset(Tetromino piece)
        {
            return (GRID_WIDTH - piece.Size) / 2;
        }
    }
}
