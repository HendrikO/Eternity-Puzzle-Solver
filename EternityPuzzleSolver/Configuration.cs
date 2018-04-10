using System;
using System.Collections.Generic;

namespace EternityPuzzleSolver
{
    public class Configuration
    {
        public Configuration()
        {
            this.Pieces = new List<Tuple<int, int, int>>();
            this.Fitness = 0;
        }

        public List<Tuple<int, int, int>> Pieces { get; set; }
        public int Fitness { get; set; }
        public void CalculateFitness()
        {
            // We need to go row by row and evaluate the fitness

            int piecesListIndex = 0;
            // Get number of rows
            double numRows = Math.Sqrt(this.Pieces.Count);
            for (int row = 0; row < numRows; ++row)
            {
                // Figure out how many triangles in this row
                int numTriangles = row * 2 + 1;
                for (int triangle = 0; triangle < numTriangles; ++triangle)
                {
                    // Automatically assume the triangle fails on all edges
                    // Remove penalties as we go along analyzing the edges
                    this.Fitness = this.Fitness + 3;

                    // Up right triangle
                    if (triangle % 2 == 0)
                    {
                        //************Border Dectection***********
                        // Check if left most triangle
                        if (triangle == 0)
                        {
                            // Check if left edge is black
                            if (this.Pieces[piecesListIndex].Item2 == 0)
                            {
                                // Remove penalty
                                this.Fitness--;
                            }

                        }

                        // Check if right most triangle
                        if (triangle == numTriangles - 1)
                        {
                            // Check if right edge is black
                            if (this.Pieces[piecesListIndex].Item3 == 0)
                            {
                                // Remove penalty
                                this.Fitness--;
                            }
                        }

                        // Check if last row
                        if (row == (int)numRows - 1)
                        {
                            // Remove penalties if bottom edge is black
                            if (this.Pieces[piecesListIndex].Item1 == 0)
                            {
                                // Remove penalty
                                this.Fitness--;
                            }
                        }
                        //************Border Dectection***********
                        //*************Edge Dectection************
                        else
                        {
                            // need to compare to row below
                            if (this.Pieces[piecesListIndex].Item1 == this.Pieces[piecesListIndex + numTriangles + 1].Item1
                                && this.Pieces[piecesListIndex].Item1 != 0)
                            {
                                this.Fitness--;
                                this.Fitness--;
                            }

                        }
                        // Check triangle to the right
                        if (triangle != numTriangles - 1)
                        {
                            // Remove penalty if both are the same
                            if (this.Pieces[piecesListIndex].Item3 == this.Pieces[piecesListIndex + 1].Item3
                                && this.Pieces[piecesListIndex].Item3 != 0)
                            {
                                this.Fitness--;
                            }
                        }

                        // Check triangle to the left
                        if (triangle != 0)
                        {
                            if (this.Pieces[piecesListIndex].Item2 == this.Pieces[piecesListIndex - 1].Item2
                               && this.Pieces[piecesListIndex].Item2 != 0)
                            {
                                this.Fitness--;
                            }
                        }
                        //*************Edge Dectection************
                    }
                    // Upside down triangle
                    else
                    {
                        //*************Edge Dectection************
                        // Check triangle to the right
                        if (this.Pieces[piecesListIndex].Item2 == this.Pieces[piecesListIndex + 1].Item2
                            && this.Pieces[piecesListIndex].Item2 != 0)
                        {
                            this.Fitness--;
                        }

                        // Check triangle to the left
                        if (this.Pieces[piecesListIndex].Item3 == this.Pieces[piecesListIndex - 1].Item3
                            && this.Pieces[piecesListIndex].Item3 != 0)
                        {
                            this.Fitness--;
                        }
                        //*************Edge Dectection************
                    }

                    piecesListIndex++;
                }
            }
        }
    }
}
