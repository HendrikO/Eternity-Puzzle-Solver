using System;
using System.Collections.Generic;
using System.Linq;

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

        public void StartEvolution()
        {
            // Select Parents
            this.SelectParents(out List<Configuration> parents);

            // Generate Children (crossover and mutation)
            this.GenerateChildren(parents, out List<Configuration> children);

            // Select Survivors
            // TODO: Implement Diversity Function, for now elitism

            // Order configurations by fitness
            List<Configuration> tempConfigs = new List<Configuration>(this.Configurations);
            tempConfigs.AddRange(parents);
            tempConfigs.AddRange(children);
            tempConfigs = tempConfigs.OrderBy(x => x.Fitness).ToList();

            // Only want to add the two most fit and randomize the rest
            this.Configurations.Clear();

            this.Configurations.Add(tempConfigs[0]);
            this.Configurations.Add(tempConfigs[1]);

            while (this.Configurations.Count != Constants.PopulationSize)
            {
                this.BuildBoard();
            }
        }

        private void SelectParents(out List<Configuration> parents)
        {
            parents = new List<Configuration>();

            // First step is to shuffle the configurations
            var shuffledConfigurations = this.Configurations.OrderBy(a => Guid.NewGuid()).ToList();

            // Create a stack out of it
            // Will pop from the stack to get competitors
            Stack<Configuration> configurations = new Stack<Configuration>(shuffledConfigurations);

            // Tournament Selection
            while (parents.Count < Constants.NumberParents)
            {
                // Pop the competitors out of the stack
                List<Configuration> competitors = new List<Configuration>
                {
                    configurations.Pop(),
                    configurations.Pop(),
                    configurations.Pop()
                }.OrderBy(x => x.Fitness).ToList();

                // Add the most fit of the competitors
                parents.Add(competitors[0]);
            }
        }

        private void GenerateChildren(List<Configuration> parents, out List<Configuration> children)
        {
            children = new List<Configuration>();
            Stack<Configuration> parentStack = new Stack<Configuration>(parents);

            //*Recombination N-point Crossover*
            // This is a modified version of n-point crossover that depends on the number of pieces

            while (parentStack.Count != 0)
            {
                // Select two parents at random
                var father = parentStack.Pop();
                var mother = parentStack.Pop();
                var firstChild = new Configuration();
                var secondChild = new Configuration();
                Random random = new Random();

                // number of points = sqrt(number of pieces) - 1
                int numberPointsRemaining = (int)Math.Sqrt(this.NumberPieces) - 1;

                // Mark certain indexes as crossover points
                int piecesMarked = 0; // Keeping track of how many pieces have been marked

                // We toggle this variable to know if we shoudl take from father or mother
                // It toggles after every segment
                bool toggle = true;

                while (piecesMarked != this.NumberPieces)
                {
                    int maxSegmentSize = this.NumberPieces - piecesMarked - numberPointsRemaining;
                    int segmentSize = numberPointsRemaining == 0 ? maxSegmentSize : random.Next(1, maxSegmentSize);

                    // add the segment to the children
                    for (int i = 0; i < segmentSize; ++i)
                    {
                        if (toggle)
                        {
                            firstChild.Pieces.Add(father.Pieces[piecesMarked + i]);
                            secondChild.Pieces.Add(mother.Pieces[piecesMarked + i]);
                        }
                        else
                        {
                            firstChild.Pieces.Add(mother.Pieces[piecesMarked + i]);
                            secondChild.Pieces.Add(father.Pieces[piecesMarked + i]);
                        }
                    }

                    piecesMarked = piecesMarked + segmentSize;
                    numberPointsRemaining--;
                    toggle = toggle ? false : true; // toggle the value
                }

                firstChild.CalculateFitness();
                secondChild.CalculateFitness();
                children.Add(firstChild);
                children.Add(secondChild);
            }


        }
    }
}
