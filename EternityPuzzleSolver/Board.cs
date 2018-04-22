using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void StartMonteCarloTreeSearch(ref Configuration mostFit)
        {
            // First step: create the first tree node
            // This node has no puzzle piece associated to it because it represents the empty state
            TreeNode initialNode = new TreeNode();
            initialNode.Root = initialNode;
            TreeNode currentNode = initialNode;

            // Next we need to add the available actions to the initial state
            this.Expand(ref currentNode, ref initialNode, mostFit);

            int piecesLeft = this.NumberPieces - mostFit.Pieces.Count;
            int runtime = 0;

            if (piecesLeft > 39)
            {
                runtime = 25000;
            }
            else if (piecesLeft > 29)
            {
                runtime = 20000;
            }
            else if (piecesLeft > 19)
            {
                runtime = 15000;
            }
            else if (piecesLeft > 9)
            {
                runtime = 10000;
            }
            else
            {
                runtime = 5000;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Now we start the loop, ends either when we run out of time or no more moves to make
            while (stopwatch.ElapsedMilliseconds < runtime) 
            {
                // Check if current node is a leaf node
                if (currentNode.Children.Count == 0)
                {
                    // Check if the number of passes is 0
                    if (currentNode.NumberPlays == 0)
                    {
                        // Rollout
                        this.Rollout(ref currentNode, mostFit);

                        // Go back to top
                        currentNode = initialNode;
                    }
                    else
                    {
                        if (currentNode.Parent.Children.Count != 3)
                        {
                            this.Expand(ref currentNode, ref initialNode, mostFit);

                            // Go to first child node
                            currentNode = currentNode.Children[0];

                            // Rollout
                            this.Rollout(ref currentNode, mostFit);
                        }

                        // Go back to top
                        currentNode = initialNode;
                    }
                }
                else
                {
                    // Go to child node with highest UCB1
                    currentNode.Children = currentNode.Children.OrderByDescending(x => x.UCB1).ToList();

                    // Check if there's an infinity score
                    if (currentNode.Children.Exists(x => x.IsUCB1Infinity))
                    {
                        currentNode = currentNode.Children.Find(x => x.IsUCB1Infinity);
                    }
                    else
                    {
                        currentNode = currentNode.Children[0];
                    }
                }
            }

            currentNode = initialNode;
            currentNode.Children = currentNode.Children.OrderByDescending(x => x.UCB1).ToList();
            mostFit.Pieces.Add(currentNode.Children[0].PuzzlePiece);
        }

        public void DeterminePlacedPieces(TreeNode currentNode, out List<int> ids)
        {
            ids = new List<int>();

            while (currentNode.Parent != null)
            {
                ids.Add(currentNode.PuzzlePiece.ID);
                currentNode = currentNode.Parent;
            }
        }

        public void Expand(ref TreeNode currentNode, ref TreeNode rootNode, Configuration currentConfiguration)
        {
            // Expansion
            // Add a child for each possible action from here
            List<TreeNode> childNodes = new List<TreeNode>();

            // Find the pieces that haven't been placed by going through the tree
            this.DeterminePlacedPieces(currentNode, out List<int> ids);
            foreach (var piece in currentConfiguration.Pieces)
            {
                ids.Add(piece.ID);
            }
            List<PuzzlePiece> placedPieces = new List<PuzzlePiece>();
            foreach (var id in ids)
            {
                placedPieces.Add(this.PuzzlePieces.Find(x => x.ID == id));
            }

            List<PuzzlePiece> remainingPieces = this.PuzzlePieces.Except(placedPieces).ToList();

            // Add a child node for every piece
            foreach (var piece in remainingPieces)
            {
                // Add all three arrangements
                // Get the arrangements
                PuzzlePiece piece1 = piece.Duplicate();
                piece1.CurrentArrangement = piece1.Arrangements[0];
                PuzzlePiece piece2 = piece.Duplicate();
                piece2.CurrentArrangement = piece2.Arrangements[1];
                PuzzlePiece piece3 = piece.Duplicate();
                piece3.CurrentArrangement = piece3.Arrangements[2];

                // Create the children Nodes
                TreeNode treeNode1 = new TreeNode
                {
                    PuzzlePiece = piece1,
                    Root = rootNode,
                    Parent = currentNode
                };
                TreeNode treeNode2 = new TreeNode
                {
                    PuzzlePiece = piece2,
                    Root = rootNode,
                    Parent = currentNode
                };
                TreeNode treeNode3 = new TreeNode
                {
                    PuzzlePiece = piece3,
                    Root = rootNode,
                    Parent = currentNode
                };

                // Add the nodes
                childNodes.Add(treeNode1);
                childNodes.Add(treeNode2);
                childNodes.Add(treeNode3);
            }

            currentNode.Children = childNodes;
        }

        public void Rollout(ref TreeNode currentNode, Configuration currentConfiguration)
        {
            Configuration configuration = new Configuration();
            TreeNode leafNode = currentNode;

            // Create a configuration based on the pieces of the parents and the node's piece
            while (currentNode.Parent != null)
            {
                configuration.Pieces.Add(currentNode.PuzzlePiece);
                currentNode = currentNode.Parent;
            }
            configuration.Pieces.Reverse();

            Configuration tempConfiguration = new Configuration();
            foreach(var piece in currentConfiguration.Pieces)
            {
                tempConfiguration.Pieces.Add(piece.Duplicate());
            }
            tempConfiguration.Pieces.AddRange(configuration.Pieces);

            currentNode = leafNode;

            // Run the simulation of filling out the config and looking at average fitness
            List<Configuration> simulatedConfigurations = new List<Configuration>();
            for (int i = 0; i < 1000; ++i)
            {
                simulatedConfigurations.Add(this.FillOutConfiguration(tempConfiguration));
            }

            // Find out what is the average fitness of that move
            double averageFitness = simulatedConfigurations.Average(x => x.Fitness);

            // Back propagation
            while (currentNode.Parent != null)
            {
                currentNode.TotalReward += averageFitness;
                currentNode.NumberPlays++;
                currentNode = currentNode.Parent;
            }

            // Do it for the root node as well
            currentNode.TotalReward += averageFitness;
            currentNode.NumberPlays++;

            // Calculate the ucb1 score for everyone affected
            currentNode = leafNode;
            while (currentNode.Parent != null)
            {
                currentNode.CalculateUCB1();
                currentNode = currentNode.Parent;
            }
            currentNode.CalculateUCB1();

            // reset
            currentNode = leafNode;
        }

        public Configuration FillOutConfiguration(Configuration configuration)
        {
            Configuration filledOut = new Configuration();
            List<PuzzlePiece> copyPuzzlePieces = new List<PuzzlePiece>(this.PuzzlePieces);

            // Copy over the existing pieces
            foreach (var piece in configuration.Pieces)
            {
                filledOut.Pieces.Add(piece.Duplicate());
            }

            // Remove all pieces with the ID of a piece already in the configuration
            foreach(var piece in configuration.Pieces)
            {
                copyPuzzlePieces.RemoveAll(x => x.ID == piece.ID);
            }

            // Fill out the configuration with the pieces that are left
            while (filledOut.Pieces.Count < this.NumberPieces)
            {
                // Add a piece in random order
                Random random = new Random();
                int index = random.Next(copyPuzzlePieces.Count);
                filledOut.Pieces.Add(copyPuzzlePieces[index]);

                copyPuzzlePieces.RemoveAt(index);
            }

            filledOut.CalculateFitness();
            return filledOut;
        }
    }
}
