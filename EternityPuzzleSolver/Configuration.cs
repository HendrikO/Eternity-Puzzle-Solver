using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EternityPuzzleSolver
{
    public class Configuration
    {
        public Configuration()
        {
            this.Pieces = new List<PuzzlePiece>();
            this.Fitness = 0;
        }

        public List<PuzzlePiece> Pieces { get; set; }
        public int Fitness { get; set; }
        public void CalculateFitness()
        {
            this.Fitness = 0;
            int index = 0;
            // Go row by row
            int numRows = (int)Math.Sqrt(this.Pieces.Count);
            for (int i = 0; i < numRows; ++i)
            {
                // Go piece by piece in the row
                int numPieces = i * 2 + 1;
                for (int j = 0; j < numPieces; ++j)
                {
                    // Start off by assuming the worst
                    int penalty = 0;

                    // Determine if upright triangle or upside down
                    if (j % 2 == 0)
                    {
                        // Upright
                        // Border detection gets done first
                        if (j == 0)
                        {
                            // Left most piece
                            if (this.Pieces[index].CurrentArrangement.Item2 == 0)
                            {
                                penalty += 3;
                            }
                        }

                        if (j == numPieces - 1)
                        {
                            // Right most piece
                            if (this.Pieces[index].CurrentArrangement.Item3 == 0)
                            {
                                penalty += 3;
                            }
                        }

                        if (i == numRows - 1)
                        {
                            // Bottom piece
                            if (this.Pieces[index].CurrentArrangement.Item1 == 0)
                            {
                                penalty += 3;
                            }
                        }

                        // Border detection is complete

                        // Edge piece detection gets done next

                        // Check to the left
                        if (j != 0)
                        {
                            if (this.Pieces[index - 1].CurrentArrangement.Item2 == this.Pieces[index].CurrentArrangement.Item2)
                            {
                                penalty++;
                            }
                        }

                        // Check to the right
                        if (j != numPieces - 1)
                        {
                            if (this.Pieces[index].CurrentArrangement.Item3 == this.Pieces[index + 1].CurrentArrangement.Item3)
                            {
                                penalty++;
                            }
                        }

                        // Check on the bottom
                        if (i != numRows - 1)
                        {                            
                            if (this.Pieces[index].CurrentArrangement.Item1 == this.Pieces[index + numPieces + 1].CurrentArrangement.Item1)
                            {
                                penalty++;
                            }
                        }
                    }
                    else
                    {
                        // Upside down
                        // Always going to be a piece on the left, right and above

                        // Check to the left
                        if (this.Pieces[index].CurrentArrangement.Item3 == this.Pieces[index - 1].CurrentArrangement.Item3)
                        {
                            penalty++;
                        }

                        // Check to the right
                        if (this.Pieces[index].CurrentArrangement.Item2 == this.Pieces[index + 1].CurrentArrangement.Item2)
                        {
                            penalty++;
                        }

                        // Check on top
                        if (this.Pieces[index].CurrentArrangement.Item1 == this.Pieces[index - (numPieces - 1)].CurrentArrangement.Item1)
                        {
                            penalty++;
                        }
                    }

                    // Add penalty
                    //Debug.Assert(penalty > -1 && penalty < 4);
                    this.Fitness += penalty;
                    index++;
                }
            }
        }
    }
}
