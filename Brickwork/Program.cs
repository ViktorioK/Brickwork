namespace Brickwork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Program
    {
        private const int MAX_PERMISSIBLE_VALUE = 100;
        private const int MIN_PERMISSIBLE_VALUE = 2;

        private const string INVALID_LAYER_STRUCTURE_MESSAGE = "Layer is invalid (Brick structure should be rectangles of size 1 x 2 with exactly the same numerical values)! Try again.";

        static void Main()
        {
            Brick[][] firstLayerOfBricks;
            Brick[][] secondLayerOfBricks;

            Console.Write("Enter height and width of the area (They should be non-zero even numbers not exceeding 100, separated by a blank line): ");

            int[] inputValues;
            while (!DimensionsAreValid(inputValues = Console.ReadLine().Split().Select(e => int.Parse(e)).ToArray()))
            {
                Console.WriteLine("Invalid input values! Try again.");
            }

            int rows = inputValues[0]; //height
            int cols = inputValues[1]; //width

            Console.WriteLine($"Enter proper set of values to fill the layer ({rows} rows and {cols} columns, separated by a blank line): ");

            do
            {
                firstLayerOfBricks = InitializeInputLayer(rows, cols);
            }
            while (!LayerIsValid(firstLayerOfBricks));

            secondLayerOfBricks = BuildSecondLayerOfBricks(firstLayerOfBricks, rows, cols);

            if (!LayerIsValid(secondLayerOfBricks))
            {
                Console.WriteLine(-1 + ": There is no solution.");
            }

            PrintLayerOfBricks(secondLayerOfBricks);

        }

        private static Brick[][] BuildSecondLayerOfBricks(Brick[][] firstLayerOfBricks, int rows, int cols)
        {
            Brick[][] secondLayerOfBricks = new Brick[rows][];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    secondLayerOfBricks[row] = new Brick[cols];
                }
            }

            int brickValueCounter = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (secondLayerOfBricks[row][col] == null)
                    {
                        secondLayerOfBricks[row][col] = new Brick(++brickValueCounter)
                        {
                            IsParent = true
                        };
                    }
                    if (!secondLayerOfBricks[row][col].IsChild) //Then it's parent
                    {
                        // Check to the right to link pair of bricks (parent and child)
                        if (col < cols - 1 && firstLayerOfBricks[row][col].Number != firstLayerOfBricks[row][col + 1].Number)
                        {
                            secondLayerOfBricks[row][col + 1] = new Brick(brickValueCounter)
                            {
                                IsChild = true,
                                ParentRow = row,
                                ParentColumn = col,
                            };
                            secondLayerOfBricks[row][col].ChildRow = row;
                            secondLayerOfBricks[row][col].ChildColumn = col + 1;
                        }
                        // Check to the bottom to link pair of bricks (parent and child)
                        else if (row < rows - 1 && firstLayerOfBricks[row][col].Number != firstLayerOfBricks[row + 1][col].Number)
                        {
                            secondLayerOfBricks[row + 1][col] = new Brick(brickValueCounter)
                            {
                                IsChild = true,
                                ParentRow = row,
                                ParentColumn = col,
                            };
                            secondLayerOfBricks[row][col].ChildRow = row + 1;
                            secondLayerOfBricks[row][col].ChildColumn = col;
                        }
                    }
                }
            }

            return secondLayerOfBricks;
        }

        private static Brick[][] InitializeInputLayer(int rows, int cols)
        {
            Brick[][] firstLayerOfBricks = new Brick[rows][];
            for (int row = 0; row < rows; row++)
            {
                firstLayerOfBricks[row] = new Brick[cols];

                int[] colValues = Console.ReadLine().Split().Select(e => int.Parse(e)).ToArray();

                if (colValues.Length != cols)
                {
                    Console.WriteLine("Invalid input values count! Try again.");
                    row--;
                    continue;
                }

                for (int col = 0; col < cols; col++)
                {
                    firstLayerOfBricks[row][col] = new Brick(colValues[col]);
                }
            }

            return firstLayerOfBricks;
        }

        private static bool DimensionsAreValid(int[] inputValues)
        {
            int valuesCount = inputValues.Length;

            if (valuesCount != 2
                || inputValues[0] % 2 != 0
                || inputValues[1] % 2 != 0
                || inputValues[0] < MIN_PERMISSIBLE_VALUE
                || inputValues[0] > MAX_PERMISSIBLE_VALUE
                || inputValues[1] < MIN_PERMISSIBLE_VALUE
                || inputValues[1] > MAX_PERMISSIBLE_VALUE)
            {
                return false;
            }

            return true;
        }

        private static bool LayerIsValid(Brick[][] layer)
        {
            if (layer == null)
            {
                Console.WriteLine(INVALID_LAYER_STRUCTURE_MESSAGE);
                return false;
            }

            var valueOccurences = new Dictionary<int, int>();

            for (int row = 0; row < layer.Length; row++)
            {
                for (int col = 0; col < layer[row].Length; col++)
                {
                    int currentBrickValue = layer[row][col].Number;
                    if (!valueOccurences.ContainsKey(currentBrickValue))
                    {
                        valueOccurences[currentBrickValue] = 0;
                    }

                    valueOccurences[currentBrickValue]++;
                }
            }

            foreach (var kvp in valueOccurences)
            {
                if (kvp.Value != 2)
                {
                    Console.WriteLine(INVALID_LAYER_STRUCTURE_MESSAGE);
                    return false;
                }
            }

            return true;
        }

        private static void PrintLayerOfBricks(Brick[][] layer)
        {
            Console.WriteLine(new string('-', layer[0].Length * 2 + 1));
            for (int row = 0; row < layer.Length; row++)
            {
                for (int col = 0; col < layer[row].Length; col++)
                {
                    Console.Write(col == 0 ? "-" : string.Empty);
                    Console.Write((col < layer[row].Length - 1 && layer[row][col].Number != layer[row][col + 1].Number)
                        ? layer[row][col].Number + "|"
                        : layer[row][col].Number + "-");
                }
                Console.WriteLine();
                if (row < layer.Length - 1)
                {
                    for (int col = 0; col < layer[row].Length; col++)
                    {
                        Console.Write(col == 0 ? "-" : string.Empty);
                        Console.Write((layer[row][col].Number != layer[row + 1][col].Number)
                            ? IsTwoDigitNumber(layer, row, col) ? "---" : "--"
                            : (col == layer[row].Length - 1) ? IsTwoDigitNumber(layer, row, col) ? "-  " : "- " : (col == 0) ? IsTwoDigitNumber(layer, row, col) ? "  " : " " : IsTwoDigitNumber(layer, row, col) ? "-  " : "- ");
                    }
                    Console.WriteLine("-");
                }
                
            }
            Console.WriteLine(new string('-', layer[0].Length * 2 + 1));

            static bool IsTwoDigitNumber(Brick[][] layer, int row, int col)
            {
                return layer[row][col].Number.ToString().Length == 2;
            }
        }
    }
}