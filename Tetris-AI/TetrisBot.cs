using System.Threading;

namespace Tetris_AI
{
    public class TetrisBot
    {
        /* Reference to the tetris processor the bot uses. */
        private readonly TetrisProcessor Processor;

        /* Factors for calculating grid scores. */
        private double AggregateHeightPenalty = -0.5;
        private double HeightDifferencePenalty = -0.005;
        private double HolePenalty = -0.4;
        private double HoleWeightPenalty = -0.005;
        private double UnevennessPenalty = -0.2;
        private double WellPenalty = -0.002;

        /* Delay when updating the grid with the new best move in ms. */
        public int BestMoveDelay { get; set; }
        /* Delay between movement on the tetris processor grid in ms. */
        public int GridMoveDelay { get; set; }
        /* Determines whether the bot should prioritize making tetrisses (4 row clears at once). */
        public bool PrioritizeTetrisses { get; set; }
        /* Determines when the bot should ignore tetris prioritization,
         * depending on how high the playing field is. */
        public int ForceNormalPlayFieldHeight { get; set; }
        /* Status of the bot. */
        public bool Enabled { get; private set; }

        public TetrisBot(TetrisProcessor processor)
        {
            /* Assign the tetris processor. */
            Processor = processor;

            /* Default values. */
            BestMoveDelay = 100;
            GridMoveDelay = 20;
            
            PrioritizeTetrisses = true;
            ForceNormalPlayFieldHeight = 16;
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
        public void SetScoreFactors(double agPenalty, double hPenalty, double hwPenalty, double uePenalty)
        {
            AggregateHeightPenalty = agPenalty;
            HolePenalty = hPenalty;
            HoleWeightPenalty = hwPenalty;
            UnevennessPenalty = uePenalty;
        }

        /* Assigns new score calculation factors for tetris priority mode. */
        public void SetScoreFactors(double agPenalty, double hdPenalty, double hPenalty, double hwPenalty, double uePenalty, double wPenalty)
        {
            AggregateHeightPenalty = agPenalty;
            HeightDifferencePenalty = hdPenalty;
            HolePenalty = hPenalty;
            HoleWeightPenalty = hwPenalty;
            UnevennessPenalty = uePenalty;
            WellPenalty = wPenalty;
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
            
            PerformTetrisMove(CalculateBestMove(currentGrid, currentPiece, lookAheadPiece));
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

            bool prioritizeTetrisses = false;
            int rightBoundaryOffset = 1;
            /* Highest point in the current field. */
            int fieldHeight = 0;

            if (PrioritizeTetrisses)
            {
                int tetrisReady = BotHelperFunctions.TetrisReady(grid);
                if (tetrisReady == 0 && IsIPiece(piece))
                {
                    /* Rotate the I piece upright. */
                    piece.Rotate90Deg();
                    /* Move it over to the right of the grid to make a tetris. */
                    return new TetrisMove(grid.Width - piece.LeftMostBlockColumn() - 1, 1);
                }

                fieldHeight = BotHelperFunctions.FieldHeight(grid);
                
                if (fieldHeight < ForceNormalPlayFieldHeight && tetrisReady != 2)
                {
                    /* If it is still safe to do so, play without the last column. */
                    rightBoundaryOffset = 2;
                    prioritizeTetrisses = true;
                }
            }

            /* Calculate best permutation*/
            for (int rotation = 0; rotation < piece.UniqueRotations; rotation++)
            {
                /* Calculate boundaries. */
                int leftMostColumn = piece.LeftMostBlockColumn();
                int rightMostColumn = grid.Width - piece.RightMostBlockColumn() - rightBoundaryOffset;

                for (int column = -leftMostColumn; column <= rightMostColumn; column++)
                {
                    /* Calculate best permutation for look ahead piece. */
                    for (int lookAheadRotation = 0; lookAheadRotation < lookAheadPiece.UniqueRotations; lookAheadRotation++)
                    {
                        /* Calculate look ahead boundaries. */
                        int lookAheadLeftMostColumn = lookAheadPiece.LeftMostBlockColumn();
                        int lookAheadRightMostColumn = grid.Width - lookAheadPiece.RightMostBlockColumn() - rightBoundaryOffset;

                        for (int lookAheadColumn = -lookAheadLeftMostColumn; lookAheadColumn <= lookAheadRightMostColumn; lookAheadColumn++)
                        {
                            /* Create a grid with current pieces. */
                            tempGrid = PlaceTetromino(grid.Clone(), piece, column);
                            lookAheadGrid = PlaceTetromino(tempGrid.Clone(), lookAheadPiece, lookAheadColumn);

                            double score = CalculateGridScore(lookAheadGrid, prioritizeTetrisses);

                            if (score > bestMove.Score)
                            {
                                /* Update best move. */
                                bestMove.Column = column;
                                bestMove.Rotation = rotation;
                                bestMove.Score = score;

                                /* Update bot move in tetris processor. */
                                Processor.BotPiece = piece;
                                Processor.BotMoveRow = BotHelperFunctions.DistanceToField(grid, piece, column);
                                Processor.BotMoveColumn = column;

                                Processor.BotLookAheadPiece = lookAheadPiece;
                                Processor.BotLookAheadMoveRow = BotHelperFunctions.DistanceToField(tempGrid, lookAheadPiece, lookAheadColumn);
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
                int direction = relativeDistance / BotHelperFunctions.AbsoluteValue(relativeDistance);

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

        /* Adds a tetromino to the given tetris grid. */
        public Grid PlaceTetromino(Grid grid, Tetromino piece, int col)
        {
            /* The offset determines how far from the top the piece must be placed. */
            int offset = BotHelperFunctions.DistanceToField(grid, piece, col);

            /* Insert the piece grid into the playing grid. */
            grid.InsertGrid(piece, offset, col);

            return grid;
        }

        /* Calculates the score for the current grid after clearing full rows. */
        private double CalculateGridScore(Grid grid, bool prioritizeTetrisses, int fieldHeight = 0)
        {
            /* Do calculations after clearing full rows in the grid. 
             * This gives row clearing moves a better score. */
            grid.ClearFullRows();

            if (prioritizeTetrisses)
                return BotHelperFunctions.AggregateHeightScore(grid, AggregateHeightPenalty) +
                    BotHelperFunctions.HeightDifferenceScore(grid, HeightDifferencePenalty) +
                    BotHelperFunctions.HoleScore(grid, HolePenalty, HoleWeightPenalty) +
                    BotHelperFunctions.UnevennessScore(grid, UnevennessPenalty) +
                    BotHelperFunctions.WellScore(grid, WellPenalty);
            else
                return BotHelperFunctions.AggregateHeightScore(grid, AggregateHeightPenalty) +
                    BotHelperFunctions.HoleScore(grid, HolePenalty, HoleWeightPenalty) +
                    BotHelperFunctions.UnevennessScore(grid, UnevennessPenalty);
        }

        /* Determines whether a tetromino is an I piece. */
        private bool IsIPiece(Tetromino piece)
        {
            /* Only tetrominoes with width 4 can be I pieces. */
            return piece.Width == 4;
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

        public TetrisMove(int column, int rotation)
        {
            Column = column;
            Rotation = rotation;
        }
    }

    public static class BotHelperFunctions
    {
        /* Calculates the score for the sum of heights of each column. */
        public static double AggregateHeightScore(Grid grid, double aggregateHeightPenalty)
        {
            int height = 0;

            /* Loop through all columns. */
            for (int i = 0; i < grid.Width; i++)
                height += grid.Height - grid.HighestBlockRow(i);

            return height * aggregateHeightPenalty;
        }

        /* Calculates the score for the amount of holes and the amount of blocks on top of holes. */
        public static  double HoleScore(Grid grid, double holePenalty, double holeWeightPenalty)
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
            return holeCount * holePenalty + holeWeightCount * holeWeightPenalty;
        }

        /* Calculates sum of height differences for each adjecent column. */
        public static double UnevennessScore(Grid grid, double unevennessPenalty)
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

            return smoothness * unevennessPenalty;
        }

        /* Calculates the score for the total depth of all wells deeper than two. */
        public static double WellScore(Grid grid, double wellPenalty)
        {
            /* The well on the left of the screen is defined by the difference between
             * the first and second columns. */
            int currentColumnHeight = grid.ColumnHeight(0);
            int rightColumnHeight = grid.ColumnHeight(1);
            int leftColumnHeight = rightColumnHeight;

            int aggregateWellDepth = WellDepth(leftColumnHeight, currentColumnHeight, rightColumnHeight);

            /* Calculate well depth for columns 1 through 7. */
            for (int col = 1; col < grid.Width - 2; col++)
            {
                /* Advance to the next column. */
                leftColumnHeight = currentColumnHeight;
                currentColumnHeight = rightColumnHeight;
                rightColumnHeight = grid.ColumnHeight(col + 1);

                /* Increment the aggregate well depth. */
                aggregateWellDepth += WellDepth(leftColumnHeight, currentColumnHeight, rightColumnHeight);
            }

            /* Wells are only calculated in tetris priority mode, thus the last column is grid.Width - 2 instead of - 1. */
            leftColumnHeight = currentColumnHeight;
            currentColumnHeight = rightColumnHeight;
            rightColumnHeight = leftColumnHeight;

            /* Increment the aggregate well depth. */
            aggregateWellDepth += WellDepth(leftColumnHeight, currentColumnHeight, rightColumnHeight);

            return aggregateWellDepth * wellPenalty;
        }

        /* Calculates the score for the height difference between the highest and lowest column. */
        public static double HeightDifferenceScore(Grid grid , double heightDifferencePenalty)
        {
            return (FieldHeight(grid) - LowestColumnHeight(grid)) * heightDifferencePenalty;
        }

        /* Calculates the depth of a well deeper than two blocks. */
        public static int WellDepth(int leftColumnHeight, int middleColumnHeight, int rightColumnHeight)
        {
            int heightDifferenceLeft = leftColumnHeight - middleColumnHeight;
            int heightDifferenceRight = rightColumnHeight - middleColumnHeight;

            if (heightDifferenceLeft < heightDifferenceRight)
                return heightDifferenceLeft > 2 ? heightDifferenceLeft : 0;
            else
                return heightDifferenceRight > 2 ? heightDifferenceRight : 0;
        }

        /* Calculates the smallest distance from the tetromino to the playing field. */
        public static int DistanceToField(Grid grid, Tetromino piece, int col)
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

        /* Determines whether four rows are filled with blocks except for the last column. 
         * Starting from the row above the highest block in the last column.
         * And determines if there are holes in or above the tetris ready field. 
         * Return values: 0 for tetrisready, 1 for not tetrisready and 2 for holes. */
        public static int TetrisReady(Grid grid)
        {
            if (grid.HighestBlockRow(grid.Width - 1) != grid.Height)
                return 2;

            /* Find the highest block in the last column. */
            int startingRow = grid.Height - 1;
            /* Stays true until an empty space is found*/
            bool tetrisReady = true;
            /* Determines whether a row is empty and is also the exit condition for the loop,
             * as there will be no blocks above an empty row. */
            bool rowEmpty = false;
            /* Holds the current block data. */
            bool currentBlock = false;
            /* Holds all the values of the starting row. */
            bool[] previousRow = new bool[9];

            /* Loop up until an empty row is found or until the grid is tetrisready. */
            for (int row = startingRow; !(row <= startingRow - 4 && tetrisReady) && !rowEmpty; row--)
            {
                rowEmpty = true;
                /* Loop through all columns except the last. */
                for (int col = 0; col < grid.Width - 1; col++)
                {
                    /* Get current block in grid. */
                    currentBlock = grid.GetBlock(row, col);

                    /* Insert values of first row. */
                    if (row != startingRow && !previousRow[col] && currentBlock)
                        return 2;

                    previousRow[col] = currentBlock;

                    /* Grid is not tetris ready when an empty space is found. */
                    tetrisReady &= currentBlock;
                    /* Row is not empty when a block is found. */
                    rowEmpty &= !currentBlock;
                }
            }

            return tetrisReady ? 0 : 1;
        }

        /* Finds the first row with a block and translates it into field height. */
        public static int FieldHeight(Grid grid)
        {
            /* Loop through the grid until a block is found. */
            for (int row = 0; row < grid.Height; row++)
            {
                for (int col = 0; col < grid.Width; col++)
                {
                    if (grid.GetBlock(row, col))
                        return grid.Height - row;
                }
            }

            /* If no block was found, the height is 0. */
            return 0;
        }

        /* Finds the height of the lowest column in the grid. */
        public static int LowestColumnHeight(Grid grid)
        {
            int lowestRowInColumn = 0;

            /* Loop through the grid until a block is found. */
            for (int col = 0; col < grid.Width; col++)
            {
                for (int row = 0; row < grid.Height; row++)
                {
                    /* If the highest block in the column is found. */
                    if (grid.GetBlock(row, col))
                    {
                        /* Compare column height with lowest column height. */
                        if (row > lowestRowInColumn)
                            lowestRowInColumn = row;

                        /* Continue to the next column. */
                        break;
                    }
                }
            }

            /* Translate row into column height. */
            return grid.Height - lowestRowInColumn;
        }

        /* Calculates the abolute value of an integer. */
        public static int AbsoluteValue(int number)
        {
            if (number < 0)
                return -number;

            return number;
        }
    }
}
