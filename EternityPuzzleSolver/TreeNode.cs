using System;
using System.Collections.Generic;

namespace EternityPuzzleSolver
{
    public class TreeNode
    {
        private double ucb1;

        public TreeNode Root { get; set; }
        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; set; }
        public int NumberPlays { get; set; }
        public double TotalReward { get; set; }
        public PuzzlePiece PuzzlePiece { get; set; }

        public double UCB1 
        {
            get
            {
                this.CalculateUCB1();
                return this.ucb1;
            }
        }

        public bool IsUCB1Infinity { get; set; }

        public TreeNode()
        {
            this.Children = new List<TreeNode>();
            this.NumberPlays = 0;
            this.TotalReward = 0;
            this.IsUCB1Infinity = true;
            this.ucb1 = 0;
        }

        public void CalculateUCB1()
        {
            this.IsUCB1Infinity = this.NumberPlays == 0;
            if (!this.IsUCB1Infinity)
            {
                this.ucb1 = this.TotalReward / this.NumberPlays + 2 * Math.Sqrt(Math.Log(this.Root.NumberPlays / this.NumberPlays));
            }
        }
    }
}
