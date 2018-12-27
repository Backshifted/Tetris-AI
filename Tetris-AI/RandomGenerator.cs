using System;
using System.Collections.Generic;

namespace Tetris_AI
{
    public class RandomGenerator
    {
        /* A queue filled with 7 tetrominoes in random order. */
        private Queue<Tetromino> PieceQueue = new Queue<Tetromino>();

        public RandomGenerator()
        {
            /* Fill the queue with tetrominoes. */
            GeneratePieces();
        }

        /* Get the next tetromino from the queue. */
        public Tetromino Next()
        {
            /* Refill the queue when it's empty. */
            if (PieceQueue.Count == 0)
                GeneratePieces();

            /* Get the next tetromino from the queue. */
            return PieceQueue.Dequeue();
        }

        /* Fills the queue with 7 new pieces. */
        private void GeneratePieces()
        {
            Random r = new Random();
            /* Get a list of all pieces. */
            List<Tetromino> Pieces = new List<Tetromino>(Tetromino.All());

            /* The first item in the queue has to be an I, J, L or T piece.
             * These are the first four in the array, thus we can generate a random
             * number up to four to get one of those pieces. */

            int i = r.Next(4);
            PieceQueue.Enqueue(Pieces[i]);
            Pieces.RemoveAt(i);

            /* Generate the remaining pieces. */
            while (Pieces.Count > 0)
            {
                i = r.Next(Pieces.Count);
                PieceQueue.Enqueue(Pieces[i]);
                Pieces.RemoveAt(i);
            }
        }
    }
}
