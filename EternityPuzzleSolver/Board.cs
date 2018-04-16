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

                configuration.Pieces.Add(piece);
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
            tempConfigs.AddRange(children);
            tempConfigs = tempConfigs.OrderBy(x => x.Fitness).ToList();

            this.ClearBoard();

            for (int i = 0; i < Constants.PopulationSize / 2; ++i)
            {
                this.Configurations.Add(tempConfigs[i]);
            }

            while (this.Configurations.Count < Constants.PopulationSize)
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

            //*Order 1 Crossover*
            while (parentStack.Count > 0)
            {
                // Select two parents at random
                var father = parentStack.Pop();
                var mother = parentStack.Pop();

                // Breed and generate 2 children
                children.Add(this.GenerateChild(father, mother));
                children.Add(this.GenerateChild(mother, father));
            }

            // Mutate Children
            this.MutateChildren(ref children);
        }

        private Configuration GenerateChild(Configuration father, Configuration mother)
        {
            Configuration child = new Configuration();

            // swath size depends on the number of pieces that we have
            int swathSize = (int)Math.Sqrt(this.NumberPieces);

            // find a random index to start
            Random random = new Random();
            int index = random.Next(this.NumberPieces);

            // Easier done with arrays
            PuzzlePiece[] fatherArray = father.Pieces.ToArray();
            PuzzlePiece[] motherArray = mother.Pieces.ToArray();
            PuzzlePiece[] childArray = new PuzzlePiece[this.NumberPieces];

            // Copy swath from father to first child
            for (int i = 0; i < swathSize; ++i)
            {
                childArray[index] = fatherArray[index];

                if (index == this.NumberPieces - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
            }

            // Copy the allelles from the mother now
            int allelesRemaining = this.NumberPieces - swathSize;
            int motherIndex = index;

            while (allelesRemaining > 0)
            {
                // Check to see if the child doesn't already contains the allele in question
                if (!childArray.Any(x => x!= null && x.ID == motherArray[motherIndex].ID))
                {
                    // We can add it
                    childArray[index] = motherArray[motherIndex];
                    index++;
                    motherIndex++;
                    allelesRemaining--;
                }
                else
                {
                    motherIndex++;
                }

                // Adjust index if it's too big
                index = index == this.NumberPieces ? 0 : index;
                motherIndex = motherIndex == this.NumberPieces ? 0 : motherIndex;
            }

            child.Pieces = childArray.ToList();
            child.CalculateFitness();

            return child;
        }

        private void MutateChildren(ref List<Configuration> children)
        {
            foreach (var child in children)
            {
                // pick the pieces for mutation
                List<int> mutationPieceIndexes = new List<int>();
                Random random = new Random();
                while (mutationPieceIndexes.Count < 1)
                {
                    int index = random.Next(this.NumberPieces);
                    if (!mutationPieceIndexes.Contains(index))
                    {
                        mutationPieceIndexes.Add(index);
                    }
                }

                // Mutation pieces at those indexes by finding new configuration
                foreach (var index in mutationPieceIndexes)
                {
                    child.Pieces[index].SwapArrangement();
                }

                child.CalculateFitness();
            }
        }
    }
}
