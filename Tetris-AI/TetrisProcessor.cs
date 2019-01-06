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
        public Tetromino LookAheadPiece { get; private set; }
        /* Piece hovering in the top of the grid. */
        public Tetromino CurrentPiece { get; private set; }
        /* Top left corner of the preview piece in the grid. */
        public int CurrentPieceColumn { get; private set; }
        public int CurrentPieceRow { get; private set; }

        /* Determines whether there is a bot move to draw. */
        public bool BotMove { get; set; }
        /* Tetromino the bot is currently working with. */
        public Tetromino BotPiece { get; set; }
        /* Coordinates of the bot tetromino. */
        public int BotMoveColumn { get; set; }
        public int BotMoveRow { get; set; }
        /* Look ahead tetromino the bot is currently working with. */
        public Tetromino BotLookAheadPiece { get; set; }
        /* Coordinates of the bot look ahead tetromino. */
        public int BotLookAheadMoveColumn { get; set; }
        public int BotLookAheadMoveRow { get; set; }

        /* Tetromino generator used for obtaining the next piece. */
        private RandomGenerator RGN;

        /* Create a new eventhandler via a delegate method. */
        public delegate void EventHandler();
        /* Event raised when the grid is updated. */
        public event EventHandler GridUpdateEvent;
        /* Event raised when a new piece is selected. */
        public event EventHandler NewPieceEvent;

        /* The amount of lines cleared in the current grid. */
        public int LinesCleared { get; private set; }

        public TetrisProcessor()
        {
            /* Initialize grid. */
            Grid = new Grid(GRID_WIDTH, GRID_HEIGHT);

            /* Initialize generator and get first piece. */
            RGN = new RandomGenerator();
            LookAheadPiece = RGN.Next();

            /* Update grid. */
            RaiseUpdateEvent();
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
            LinesCleared += Grid.ClearFullRows();

            /* Update grid as rows might have been cleared. */
            RaiseUpdateEvent();
        }

        /* Rotates the preview piece. 
         * Direction 1 will rotate clockwise,
         * Other directions (-1) will rotate counter-clockwise. */
        public void RotatePreviewPiece(int direction)
        {
            /* Remove the current preview piece. */
            Grid.DeleteGrid(CurrentPiece, CurrentPieceRow, CurrentPieceColumn);

            /* Rotate preview piece. */
            if (direction == 1)
                CurrentPiece.Rotate90Deg();
            else
                CurrentPiece.Rotate270Deg();

            /* Preview rotated piece. */
            PreviewPiece(CurrentPiece, CalculatePreviewRowOffset(CurrentPiece), CurrentPieceColumn);
        }

        /* Moves the preview piece along the grid.
         * A positive amount will move it right, 
         * and a negative amount will move it left. */
        public void MovePreviewPiece(int amount)
        {
            /* Remove the current preview piece from the grid. */
            Grid.DeleteGrid(CurrentPiece, CurrentPieceRow, CurrentPieceColumn);
            /* Update preview piece location. */
            CurrentPieceColumn += amount;
            /* Insert the updated preview piece. */
            PreviewPiece(CurrentPiece, CurrentPieceRow, CurrentPieceColumn);
        }

        /* Sets the next tetromino as the current preview piece,
         * and selects a new next tetromino. */
        public void PreviewNextPiece()
        {
            /* Preview next piece. */
            PreviewPiece(LookAheadPiece, CalculatePreviewRowOffset(LookAheadPiece), CalculatePreviewColumnOffset(LookAheadPiece));
            /* Get next piece. */
            LookAheadPiece = RGN.Next();

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
            CurrentPiece = piece;
            CurrentPieceRow = row;
            CurrentPieceColumn = col;

            /* Grid has been updated by adding a preview piece. */
            RaiseUpdateEvent();
        }

        /* Places the preview piece down from it's current position. */
        public void PlacePreviewPiece()
        {
            /* Remove the current preview piece from the grid. */
            Grid.DeleteGrid(CurrentPiece, CurrentPieceRow, CurrentPieceColumn);
            /* Insert the piece in the correct row. */
            AddPiece(CurrentPiece, CurrentPieceColumn);
            /* Preview the next piece. */
            PreviewNextPiece();
        }

        /* Calculates the smallest distance from the tetromino to the playing field. */
        private int MinDistanceToField(Tetromino piece, int col)
        {
            /* Get the first distance from piece to field. */
            int minDistance = Grid.DistanceToFirstBlock(piece.LowestBlockRow(0), col);

            /* Compare remaining distances. */
            for (int i = 1; i < piece.Width; i++)
            {
                /* Get distance. */
                int distance = Grid.DistanceToFirstBlock(piece.LowestBlockRow(i), col + i);

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
            for (int i = 0; i < piece.Width && row == -1; i++)
            {
                for (int j = 0; j < piece.Width; j++)
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
            return (GRID_WIDTH - piece.Width) / 2;
        }
        
        /* Raised when there is an update in the grid. */
        public void RaiseUpdateEvent()
        {
            if (GridUpdateEvent != null)
                GridUpdateEvent.Invoke();
        }

        /* Raised when a new piece is selected. */
        private void RaiseNewPieceEvent()
        {
            if (NewPieceEvent != null)
                NewPieceEvent.Invoke();
        }
    }
}
