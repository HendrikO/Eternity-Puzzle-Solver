using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EternityPuzzleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the board
            Initialize(out Board eternityBoard);

            Configuration mostFit = new Configuration();

            while (mostFit.Pieces.Count < eternityBoard.NumberPieces)
            {
                eternityBoard.StartMonteCarloTreeSearch(ref mostFit);
            }

            // print output
            using (System.IO.StreamWriter outputFile =
                   new System.IO.StreamWriter("/Users/hendrikoosenbrug/Developer/EternityPuzzleSolver/PuzzleDraw-master/puzzle.csv"))
            { 
                foreach (var piece in mostFit.Pieces)
                {
                    string outputLine = string.Empty;
                    outputLine = string.Format("{0},{1},{2}", piece.CurrentArrangement.Item1, piece.CurrentArrangement.Item2, piece.CurrentArrangement.Item3);
                    outputFile.WriteLine(outputLine);
                }
            }
        }

        static void Initialize(out Board eternityBoard)
        {
            eternityBoard = new Board();

            // Open input file
            string inputFile = Directory.GetCurrentDirectory();
            inputFile = Path.Combine(inputFile, "puzzle.txt");

            int counter = 0;
            string line = string.Empty;

            // Read the file and initialize the board  
            StreamReader file = new StreamReader(inputFile);
            while ((line = file.ReadLine()) != null)
            {
                if (counter == 0)
                {
                    // this means we're at the first line
                    var firstLine = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string number = Regex.Match(firstLine[0], @"\d+").Value;

                    // Set the number of pieces
                    eternityBoard.NumberPieces = Int32.Parse(number) * Int32.Parse(number);

                    number = Regex.Match(firstLine[1], @"\d+").Value;

                    // Set the number of colors
                    eternityBoard.NumberColors = Int32.Parse(number);
                }
                else
                {
                    // Puzzle piece information
                    if (!string.IsNullOrEmpty(line))
                    {
                        var puzzleInfo = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        PuzzlePiece puzzlePiece =
                            new PuzzlePiece(Int32.Parse(puzzleInfo[0]), Int32.Parse(puzzleInfo[1]), Int32.Parse(puzzleInfo[2]), eternityBoard.PuzzlePieces.Count);
                        eternityBoard.PuzzlePieces.Add(puzzlePiece);
                    }
                }
                counter++;
            }

            file.Close();
        }
    }
}
