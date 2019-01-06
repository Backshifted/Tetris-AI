using System.Threading;

namespace Tetris_AI
{
    public class TetrisBot
    {
        /* Reference to the tetris processor the bot uses. */
        private readonly TetrisProcessor Processor;

        /* Factors for calculating grid scores. */
        private double HoleFactor = -0.4;
        private double HoleWeightFactor = -0.005;
        private double SmoothnessFactor = -0.2;
        private double AggregateHeightFactor = -0.5;

        /* Delay when updating the grid with the new best move in ms. */
        public int BestMoveDelay { get; set; }
        /* Delay between movement on the tetris processor grid in ms. */
        public int GridMoveDelay { get; set; }
        /* Determines whether the bot uses to look-ahead piece in its calculations. */
        public bool UseLookAhead { get; set; }
        /* Status of the bot. */
        public bool Enabled { get; private set; }

        public TetrisBot(TetrisProcessor processor)
        {
            /* Assign the tetris processor. */
            Processor = processor;

            /* Default values. */
            BestMoveDelay = 100;
            GridMoveDelay = 20;

            UseLookAhead = true;
        }

        /* Hooks the bot to the tetris processor. */
        public void Start()
        {
            if (Processor != null && !Enabled)
            {
                Enabled = true;
                Processor.NewPieceEvent += PerformMove;
                PerformMove();
            }
        }

        /* Unhooks the bot from the tetris processor. */
        public void Stop()
        {
            if (Processor != null && Enabled)
            {
                Enabled = false;
                Processor.NewPieceEvent -= PerformMove;
            }
        }

        /* Assigns new score calculation factors. */
        public void SetScoreFactors(double hFactor, double hwFactor, double sFactor, double thFactor)
        {
            HoleFactor = hFactor;
            HoleWeightFactor = hwFactor;
            SmoothnessFactor = sFactor;
            AggregateHeightFactor = thFactor;
        }

        /* Starts a new thread to calculate the best move. */
        public void PerformMove()
        {
            /* Create a new thread. */
            Thread t = new Thread(() => CalculateBestMoveAsync());
            /* Start the thread. */
            t.Start();
        }

        /* Calculates the best move for the current grid and performs that move. */
        private void CalculateBestMoveAsync()
        {
            /* Copy of the processor grid, used for score calculations. */
            Grid currentGrid = Processor.Grid.Clone();
            /* The current workable tetromino. */
            Tetromino currentPiece = Processor.CurrentPiece.Clone();
            /* The look ahead tetromino, used for next level calculations. */
            Tetromino lookAheadPiece = Processor.LookAheadPiece.Clone();

            /* Remove the current piece from the top of the grid. */
            currentGrid.DeleteGrid(currentPiece, Processor.CurrentPieceRow, Processor.CurrentPieceColumn);

            if (UseLookAhead)
                PerformTetrisMove(CalculateBestMove(currentGrid, currentPiece, lookAheadPiece));
            else
                PerformTetrisMove(CalculateBestMove(currentGrid, currentPiece));
        }

        /* Calculates the move with the highest grid score,
         * taking only the current piece into consideration. */
        private TetrisMove CalculateBestMove(Grid grid, Tetromino piece)
        {
            /* Create a variable to hold the best move. */
            TetrisMove bestMove = new TetrisMove();
            /* Grid used for calculations. */
            Grid tempGrid = null;

            /* Reset the bot move in the tetris processor. */
            Processor.BotPiece = null;
            Processor.BotLookAheadPiece = null;
            Processor.BotMove = true;

            /* Calculate best permutation*/
            for (int rotation = 0; rotation < piece.UniqueRotations; rotation++)
            {
                /* Calculate boundaries. */
                int leftMostColumn = piece.LeftMostBlockColumn();
                int rightMostColumn = grid.Width - piece.RightMostBlockColumn() - 1;

                for (int column = -leftMostColumn; column <= rightMostColumn; column++)
                {
                    /* Create a grid with current pieces. */
                    tempGrid = PlaceTetromino(grid.Clone(), piece, column);

                    double score = CalculateGridScore(tempGrid);

                    if (score > bestMove.Score)
                    {
                        /* Update best move. */
                        bestMove.Column = column;
                        bestMove.Rotation = rotation;
                        bestMove.Score = score;

                        /* Update bot move in tetris processor. */
                        Processor.BotPiece = piece;
                        Processor.BotMoveRow = MinDistanceToField(grid, piece, column);
                        Processor.BotMoveColumn = column;

                        /* Update view. */
                        Processor.RaiseUpdateEvent();

                        Thread.Sleep(BestMoveDelay);
                    }
                }

                /* Rotate piece for the next permutation. */
                piece.Rotate90Deg();
            }

            /* Remove the preview of the bots move. */
            Processor.BotMove = false;

            return bestMove;
        }

        /* Calculates the move with the highest grid score,
         * taking the current and look ahead pieces into consideration. */
        private TetrisMove CalculateBestMove(Grid grid, Tetromino piece, Tetromino lookAheadPiece)
        {
            /* Create a variable to hold the best move. */
            TetrisMove bestMove = new TetrisMove();
            /* Grids used for calculations. */
            Grid tempGrid = null;
            Grid lookAheadGrid = null;

            /* Reset the bot move in the tetris processor. */
            Processor.BotPiece = null;
            Processor.BotLookAheadPiece = null;
            Processor.BotMove = true;

            /* Calculate best permutation*/
            for (int rotation = 0; rotation < piece.UniqueRotations; rotation++)
            {
                /* Calculate boundaries. */
                int leftMostColumn = piece.LeftMostBlockColumn();
                int rightMostColumn = grid.Width - piece.RightMostBlockColumn() - 1;

                for (int column = -leftMostColumn; column <= rightMostColumn; column++)
                {
                    /* Calculate best permutation for look ahead piece. */
                    for (int lookAheadRotation = 0; lookAheadRotation < lookAheadPiece.UniqueRotations; lookAheadRotation++)
                    {
                        /* Calculate look ahead boundaries. */
                        int lookAheadLeftMostColumn = lookAheadPiece.LeftMostBlockColumn();
                        int lookAheadRightMostColumn = grid.Width - lookAheadPiece.RightMostBlockColumn() - 1;

                        for (int lookAheadColumn = -lookAheadLeftMostColumn; lookAheadColumn <= lookAheadRightMostColumn; lookAheadColumn++)
                        {
                            /* Create a grid with current pieces. */
                            tempGrid = PlaceTetromino(grid.Clone(), piece, column);
                            lookAheadGrid = PlaceTetromino(tempGrid.Clone(), lookAheadPiece, lookAheadColumn);

                            double score = CalculateGridScore(lookAheadGrid);

                            if (score > bestMove.Score)
                            {
                                /* Update best move. */
                                bestMove.Column = column;
                                bestMove.Rotation = rotation;
                                bestMove.Score = score;

                                /* Update bot move in tetris processor. */
                                Processor.BotPiece = piece;
                                Processor.BotMoveRow = MinDistanceToField(grid, piece, column);
                                Processor.BotMoveColumn = column;

                                Processor.BotLookAheadPiece = lookAheadPiece;
                                Processor.BotLookAheadMoveRow = MinDistanceToField(tempGrid, lookAheadPiece, lookAheadColumn);
                                Processor.BotLookAheadMoveColumn = lookAheadColumn;

                                /* Update view. */
                                Processor.RaiseUpdateEvent();

                                Thread.Sleep(BestMoveDelay);
                            }
                        }

                        /* Rotate look ahead piece for the next permutation. */
                        lookAheadPiece.Rotate90Deg();
                    }
                }

                /* Rotate piece for the next permutation. */
                piece.Rotate90Deg();
            }

            /* Remove the preview of the bots move. */
            Processor.BotMove = false;

            return bestMove;
        }

        /* Moves a tetromino from the default position to the desired position. */
        private void PerformTetrisMove(TetrisMove move)
        {
            /* Rotate the tetromino. */
            switch (move.Rotation)
            {
                case 1:
                    Processor.RotatePreviewPiece(1);
                    break;
                case 2:
                    Processor.RotatePreviewPiece(1);
                    goto case 1;
                case 3:
                    Processor.RotatePreviewPiece(-1);
                    break;
            }

            /* Calculate how for the tetromino must be moved. */
            int relativeDistance = move.Column - Processor.CurrentPieceColumn;

            if (relativeDistance != 0)
            {
                /* Calculate movment direction. */
                int direction = relativeDistance / AbsoluteValue(relativeDistance);

                while (relativeDistance != 0)
                {
                    /* Move the piece across the grid. */
                    Processor.MovePreviewPiece(direction);
                    /* Decrease movement amount. */
                    relativeDistance -= direction;
                    Thread.Sleep(GridMoveDelay);
                }
            }

            Processor.PlacePreviewPiece();
        }

        /* Calculates the score for the current grid after clearing full rows. */
        private double CalculateGridScore(Grid grid)
        {
            /* Do calculations after clearing full rows in the grid. 
             * This gives row clearing moves a better score. */
            grid.ClearFullRows();

            return CalculateHoleScore(grid) + CalculateSmoothnessScore(grid) + CalculateAggregateHeightScore(grid);
        }

        /* Calculates the score for the sum of heights of each column. */
        private double CalculateAggregateHeightScore(Grid grid)
        {
            int height = 0;

            /* Loop through all columns. */
            for (int i = 0; i < grid.Width; i++)
                height += grid.Height - grid.HighestBlockRow(i);

            return height * AggregateHeightFactor;
        }

        /* Calculates the score for the amount of holes and the amount of blocks on top of holes. */
        private double CalculateHoleScore(Grid grid)
        {
            int holeCount = 0;
            int holeWeightCount = 0;
            /* Determines whether the loop is currently at an empty space. */
            bool emptySpace = false;
            /* Determines whether there is a empty space in the current column. */
            bool emptySpaceInColumn = false;

            /* Loop through each column from the bottom up. */
            for (int col = 0; col < grid.Width; col++)
            {
                for (int row = grid.Height - 1; row >= 0; row--)
                {
                    /* Check for empty space/hole */
                    if (!grid.GetBlock(row, col))
                    {
                        emptySpace = true;
                        emptySpaceInColumn = true;
                    }
                    /* Check for block. */
                    else
                    {
                        /* If there is an empty space and a block is found, 
                         * the empty space is a hole. */
                        if (emptySpace)
                        {
                            emptySpace = false;
                            holeCount++;
                        }

                        /* If there is an empty space below this block, 
                         * increment the hole weight count*/
                        if (emptySpaceInColumn)
                        {
                            holeWeightCount++;
                        }
                    }
                }

                /* Reset empty spaces for the next column. */
                emptySpaceInColumn = false;
                emptySpace = false;
            }

            /* Return the scores for the amount of holes and weights on top of holes. */
            return holeCount * HoleFactor + holeWeightCount * HoleWeightFactor;
        }

        /* Calculates sum of height differences for each adjecent column. */
        private double CalculateSmoothnessScore(Grid grid)
        {
            /* Calculate the difference between the first and second column. */
            int previousColumnHeight = grid.ColumnHeight(0);
            int currentColumnHeight = grid.ColumnHeight(1);
            int smoothness = AbsoluteValue(previousColumnHeight - currentColumnHeight);

            /* Calculate height difference for the remaining columns. */
            for (int col = 2; col < grid.Width; col++)
            {
                /* Advance to the next column. */
                previousColumnHeight = currentColumnHeight;
                currentColumnHeight = grid.ColumnHeight(col);
                /* Increment the smoothness score with the height difference. */
                smoothness += AbsoluteValue(previousColumnHeight - currentColumnHeight);
            }

            return smoothness * SmoothnessFactor;
        }

        /* Adds a tetromino to the given tetris grid. */
        public Grid PlaceTetromino(Grid grid, Tetromino piece, int col)
        {
            /* The offset determines how far from the top the piece must be placed. */
            int offset = MinDistanceToField(grid, piece, col);

            /* Insert the piece grid into the playing grid. */
            grid.InsertGrid(piece, offset, col);

            return grid;
        }

        /* Calculates the smallest distance from the tetromino to the playing field. */
        private int MinDistanceToField(Grid grid, Tetromino piece, int col)
        {
            /* Get the first distance from piece to field. */
            int minDistance = grid.DistanceToFirstBlock(piece.LowestBlockRow(0), col);

            /* Compare remaining distances. */
            for (int i = 1; i < piece.Width; i++)
            {
                /* Get distance. */
                int distance = grid.DistanceToFirstBlock(piece.LowestBlockRow(i), col + i);

                /* Select smallest distance.*/
                if (minDistance > distance)
                    minDistance = distance;
            }

            return minDistance;
        }

        /* Calculates the abolute value of an integer. */
        private int AbsoluteValue(int number)
        {
            if (number < 0)
                return -number;

            return number;
        }
    }

    /* Wrapper class for tetris moves. */
    public class TetrisMove
    {
        public int Column;
        public int Rotation;
        public double Score;

        public TetrisMove()
        {
            Column = 0;
            Rotation = 0;
            /* Scores can have negative values, thus the smallest value is initialized. */
            Score = double.MinValue;
        }
    }
}
