namespace Tetris_AI
{
    public class TetrisProcessor
    {
        public const int GRID_WIDTH = 10;
        public const int GRID_HEIGHT = 22;
        /* A 10x22 Grid that holds the state of each piece in row major order.
         * 0 = no block, 1 = a block in that position. */
        public Grid Grid;

        public TetrisProcessor()
        {
            Grid = new Grid(GRID_WIDTH, GRID_HEIGHT);
        }

        /* Adds a piece to the tetris grid. */
        public void AddPiece(TetrisPiece newPiece, int col)
        {
            /* The offset determines how far from the top the piece must be placed. */
            int offset = MinDistanceToField(newPiece, col);

            /* Insert the piece grid into the playing grid. */
            Grid.InsertGrid(newPiece.Grid, offset, col);

            /* Update the grid. */
            Grid.ClearFullRows();
        }

        /* Calculates the smallest distance from the tetris piece to the playing field. */
        private int MinDistanceToField(TetrisPiece tetrisPiece, int col)
        {
            /* Get the first distance from piece to field. */
            int minDistance = Grid.DistanceToFirstBlock(tetrisPiece.Grid.LowestBlockPosition(0), col);

            /* Compare remaining distances. */
            for (int i = 1; i < tetrisPiece.Size; i++)
            {
                /* Get distance. */
                int distance = Grid.DistanceToFirstBlock(tetrisPiece.Grid.LowestBlockPosition(i), col + i);

                /* Select smallest distance.*/
                if (minDistance > distance)
                    minDistance = distance;
            }

            return minDistance;
        }
    }
}
