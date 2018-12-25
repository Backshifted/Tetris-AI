using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_AI
{
    public class TetrisProcessor
    {
        /* A 10x22 Grid that holds the state of each piece in row major order.
         * 0 = no block, 1 = a block in that position. */
        public bool[] grid = new bool[10 * 22];
        /* Integer used to keep track of the current field height. */
        public int currentHeight;

        public TetrisProcessor()
        {
            grid = new bool[10 * 22];
            currentHeight = 0;
        }
    }
}
