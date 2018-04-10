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
        public PuzzlePiece(int firstColor, int secondColor, int thirdColor)
        {
            this.Arrangements = new List<Tuple<int, int, int>>
            {

                // Populate list of non inversed arrangements 
                new Tuple<int, int, int>(firstColor, secondColor, thirdColor),
                new Tuple<int, int, int>(thirdColor, firstColor, secondColor),
                new Tuple<int, int, int>(secondColor, thirdColor, firstColor)
            };
        }

        public List<Tuple<int, int, int>> Arrangements;
    }
}
