using System;
using System.Collections.Generic;

namespace EternityPuzzleSolver
{
    public class Board
    {
        public Board()
        {
            this.NumberPieces = 0;
            this.NumberColors = 0;
            this.PuzzlePieces = new List<PuzzlePiece>();
            this.Configurations = new List<Configuration>();
        }

        public int NumberPieces { get; set; }

        public int NumberColors { get; set; }

        public List<PuzzlePiece> PuzzlePieces { get; set; }

        public List<Configuration> Configurations { get; set; }

        public void BuildBoard(int repetitions)
        {
            for (int i = 0; i < repetitions; ++i)
            {
                this.BuildBoard();
            }
        }

        public void BuildBoard()
        {
            Random rndPiece = new Random();
            Random rndOrientation = new Random();
            List<PuzzlePiece> temp = new List<PuzzlePiece>(this.PuzzlePieces);
            Configuration configuration = new Configuration();
                
            for (int i = 0; i < this.PuzzlePieces.Count; ++i)
            {
                // pick a piece at random
                int index = rndPiece.Next(temp.Count);
                var piece = temp[index];
                temp.RemoveAt(index);

                // supply a configuration at random
                int indexOrienation = rndOrientation.Next(3);
                configuration.Pieces.Add(piece.Arrangements[indexOrienation]);
            }

            configuration.CalculateFitness();
            this.Configurations.Add((configuration));
        }

        public void ClearBoard()
        {
            this.Configurations.Clear();
        }
    }
}
