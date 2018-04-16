/*
 *              /\
 *             /  \
 *            /    \ 
 *         item2  item3
 *          /        \
 *         /          \
 *        /___item1____\
 *
 *
 *
 *        _____item1_____
 *        \             /
 *         \           /
 *        item3     item2 
 *           \       / 
 *            \     /
 *             \   /
 *              \ / 
 *
 *
 *
*/


using System;
using System.Collections.Generic;

namespace EternityPuzzleSolver
{
    public class PuzzlePiece
    {
        public PuzzlePiece(int firstColor, int secondColor, int thirdColor, int id)
        {
            this.Arrangements = new List<Tuple<int, int, int>>
            {

                // Populate list of non inversed arrangements 
                new Tuple<int, int, int>(firstColor, secondColor, thirdColor),
                new Tuple<int, int, int>(thirdColor, firstColor, secondColor),
                new Tuple<int, int, int>(secondColor, thirdColor, firstColor)
            };

            Random random = new Random();
            this.CurrentArrangement = this.Arrangements[random.Next(this.Arrangements.Count)];

            this.ID = id;
        }

        public int ID { get; set; }

        public List<Tuple<int, int, int>> Arrangements { get; set; }

        public Tuple<int, int, int> CurrentArrangement { get; set; }

        public void SwapArrangement()
        {
            Random random = new Random();
            this.CurrentArrangement = this.Arrangements[random.Next(this.Arrangements.Count)];
        }
    }
}
