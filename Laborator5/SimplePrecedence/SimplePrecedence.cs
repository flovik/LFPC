using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePrecedence
{
    internal class SimplePrecedence
    {
        private List<string> Terminals;
        private List<string> NonTerminals;
        private Dictionary<char, int> Indexes = new();
        private char[,] Matrix;
        private Dictionary<string, List<string>> Transitions;
        private FirstLast FirstLast = new();
        public SimplePrecedence(Dictionary<string, List<string>> transitions, List<string> terminals, List<string> nonTerminals)
        {
            Transitions = transitions;
            Terminals = terminals;
            NonTerminals = nonTerminals;
        }

        private void PrintTransitions()
        {
            foreach (var (key, list) in Transitions)
            {
                Console.Write($"{key} -> ");
                for (int i = 0; i < list.Count; i++)
                {
                    if (i < list.Count - 1) Console.Write($"{list[i]} | ");
                    else Console.Write(list[i]);
                }

                Console.WriteLine();
            }

            Console.WriteLine("--------------");
        }

        private void PrintMatrix()
        {
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if(Matrix[i, j].Equals('\0')) Console.Write(" ");
                    Console.Write($"{Matrix[i, j]} ");
                }

                Console.WriteLine();
            }
        }

        public void Start()
        {
            FirstLast.Start(Transitions);
            FirstLast.Print(FirstLast.First);
            FirstLast.Print(FirstLast.Last);
            InitMatrix();
            Rule1();
            Rule2();
            Rule3();
            Rule4();
            Rule5();
            PrintMatrix();
        }

        private void InitMatrix()
        {
            var list = Terminals.Concat(NonTerminals).ToList();
            list.Add("$");
            Matrix = new char[list.Count + 1, list.Count + 1];
            for (int i = 0; i < list.Count; i++)
            {
                Indexes.Add(list[i][0], i + 1);
                Matrix[i + 1, 0] = list[i][0];
                Matrix[0, i + 1] = list[i][0];
            }
        }

        private void AddOperator(char Operator, int rowIndex, int columnIndex)
        {
            //adds an operator to the indicated rowIndex to columnIndex
            Matrix[rowIndex, columnIndex] = Operator;
        }

        private void Rule1()
        {
            //iterate over words and search for ones of len > 1, then equal the neighbors
            foreach (var (_, list) in Transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length > 1)
                    {
                        for (int j = 1; j < list[i].Length; j++)
                        {
                            char first = list[i][j - 1];
                            char second = list[i][j];
                            AddOperator('=', Indexes[first], Indexes[second]);
                        }
                    }
                }
            }
        }

        private void Rule2()
        {
            //terminal + NonTerminal, terminal < First(NonTerminal)
            foreach (var (_, list) in Transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length > 1)
                    {
                        for (int j = 1; j < list[i].Length; j++)
                        {
                            char terminal = list[i][j - 1];
                            char nonTerminal = list[i][j];
                            if (char.IsLower(terminal) && char.IsUpper(nonTerminal)) //the main condition
                            {
                                foreach (var character in FirstLast.First[nonTerminal]) //search in FIRST(NonTerminal)
                                {
                                    AddOperator('<', Indexes[terminal], Indexes[character]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Rule3()
        {
            //NonTerminal to Terminal
            foreach (var (_, list) in Transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length > 1)
                    {
                        for (int j = 1; j < list[i].Length; j++)
                        {
                            char nonTerminal = list[i][j - 1];
                            char terminal = list[i][j];
                            if (char.IsLower(terminal) && char.IsUpper(nonTerminal)) //the main condition
                            {
                                foreach (var character in FirstLast.Last[nonTerminal]) //search in LAST(nonTerminal)
                                {
                                    AddOperator('>', Indexes[character], Indexes[terminal]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Rule4()
        {
            //NonTerminal to NonTerminal
            foreach (var (_, list) in Transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length > 1)
                    {
                        for (int j = 1; j < list[i].Length; j++)
                        {
                            char nonTerminal1 = list[i][j - 1];
                            char nonTerminal2 = list[i][j];
                            if (char.IsUpper(nonTerminal1) && char.IsUpper(nonTerminal2)) //the main condition
                            {
                                foreach (var character in FirstLast.Last[nonTerminal1]) //search in LAST(nonTerminal1)
                                {
                                    foreach (var terminal in FirstLast.First[nonTerminal2]) //for all TERMINALS in FIRST(nonTerminal2)
                                    {
                                        if(char.IsLower(terminal)) AddOperator('>', Indexes[character], Indexes[terminal]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Rule5()
        {
            //add operators for $
            var list = Terminals.Concat(NonTerminals).ToList();
            foreach (var character in list)
            {
                AddOperator('<', Indexes['$'], Indexes[character[0]]);
                AddOperator('>', Indexes[character[0]], Indexes['$']);
            }
        }
    }
}
