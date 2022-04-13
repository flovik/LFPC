using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePrecedence
{
    internal class SimplePrecedence
    {
        private List<string> _terminals;
        private List<string> _nonTerminals;
        private Dictionary<char, int> _indexes = new();
        private char[,] _matrix;
        private Dictionary<string, List<string>> _transitions;
        private FirstLast _firstLast = new();
        private Stack<string> _stack = new();
        public SimplePrecedence(Dictionary<string, List<string>> transitions, List<string> terminals, List<string> nonTerminals)
        {
            _transitions = transitions;
            _terminals = terminals;
            _nonTerminals = nonTerminals;
        }

        public void PrintTransitions()
        {
            Console.WriteLine("Transitions: ");
            foreach (var (key, list) in _transitions)
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

        public void PrintMatrix()
        {
            Console.WriteLine("Matrix: ");
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    if (_matrix[i, j].Equals('\0')) Console.Write(" ");
                    Console.Write($"{_matrix[i, j]} ");
                }

                Console.WriteLine();
            }
        }

        public void PrintFirstAndLast()
        {
            Console.WriteLine("First set: ");
            _firstLast.Print(_firstLast.First);
            Console.WriteLine("Last set: ");
            _firstLast.Print(_firstLast.Last);
        }

        public void Start()
        {
            _firstLast.Start(_transitions);
            InitMatrix();
            Rule1();
            Rule2();
            Rule3();
            Rule4();
        }

        private void InitMatrix()
        {
            var list = _terminals.Concat(_nonTerminals).ToList();
            list.Add("$");
            _matrix = new char[list.Count + 1, list.Count + 1];
            for (int i = 0; i < list.Count; i++)
            {
                _indexes.Add(list[i][0], i + 1);
                _matrix[i + 1, 0] = list[i][0];
                _matrix[0, i + 1] = list[i][0];
            }
        }

        private void AddOperator(char Operator, int rowIndex, int columnIndex)
        {
            //adds an operator to the indicated rowIndex to columnIndex
            _matrix[rowIndex, columnIndex] = Operator;
        }

        private void Rule1()
        {
            //iterate over words and search for ones of len > 1, then equal the neighbors
            foreach (var (_, list) in _transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length > 1)
                    {
                        for (int j = 1; j < list[i].Length; j++)
                        {
                            char first = list[i][j - 1];
                            char second = list[i][j];
                            AddOperator('=', _indexes[first], _indexes[second]);
                        }
                    }
                }
            }
        }

        private void Rule2()
        {
            //terminal + NonTerminal, terminal < First(NonTerminal)
            foreach (var (_, list) in _transitions)
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
                                foreach (var character in _firstLast.First[nonTerminal]) //search in FIRST(NonTerminal)
                                {
                                    AddOperator('<', _indexes[terminal], _indexes[character]);
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
            foreach (var (_, list) in _transitions)
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
                                foreach (var character in _firstLast.Last[nonTerminal]) //search in LAST(nonTerminal)
                                {
                                    AddOperator('>', _indexes[character], _indexes[terminal]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Rule4()
        {
            //add operators for $
            var list = _terminals.Concat(_nonTerminals).ToList();
            foreach (var character in list)
            {
                AddOperator('<', _indexes['$'], _indexes[character[0]]);
                AddOperator('>', _indexes[character[0]], _indexes['$']);
            }
        }

        public void CheckString(string input)
        {
            var word = ParseInput("$<" + input);
            if (word == null)
            {
                Console.WriteLine("Rejected");
                return;
            }

            //initialize top of stack with $
            _stack.Push("$");
            int current = 1;
            while (word[current] != '$')
            {
                if (word[current] == '<')
                {
                    //we get a new combination that should be pushed in stack
                    _stack.Push("<");
                }
                else if (word[current] == '>')
                {
                    //we get a combination here
                    string topOfStack = _stack.Pop()[1..];
                    if (topOfStack == "S") //finish the program if got to S
                    {
                        Console.WriteLine("Accepted");
                        return;
                    }
                    //if single letter state, just search in transitions, if not, form the transition and then search
                    string replace = topOfStack.Length == 1 ? GetReplaceForPop(topOfStack) : ComposeBigState(topOfStack);
                    if (replace == "")
                    {
                        Console.WriteLine("Rejected");
                        return;
                    }
                    //now insert the needed operators in the word to continue parsing
                    //take the last char of the current top of stack, and the index of replaced char, find their operator in matrix
                    char leftOperator = _matrix[_indexes[_stack.Peek()[^1]], _indexes[replace[0]]];
                    //now right operator, index of replaced char and the next char in word
                    char rightOperator = _matrix[_indexes[replace[0]], _indexes[word[current + 1]]];

                    if (leftOperator == '<')
                    {
                        //push to the stack again that replaced char
                        //be careful with indexes of the word, change CURRENT index in the word with the operator
                        //then go one back to analyze that char again
                        _stack.Push("<" + replace);
                        word[current--] = rightOperator;
                    }
                    else if (leftOperator == '>')
                    {
                        //set the right Operator
                        word[current--] = rightOperator;
                        //set the replaced in the word
                        word[current--] = replace[0];
                        //put the > symbol in case it will be parsed
                        word[current--] = '>';
                    }
                    else
                    {
                        //equals
                        //add the modified transition to the top of the stack
                        string temp = _stack.Pop() + "=" + replace;
                        _stack.Push(temp);
                        word[current--] = rightOperator;
                    }
                }
                else
                {
                    //append next char to top of stack
                    var temp = _stack.Pop() + word[current];
                    _stack.Push(temp);
                }

                current++;
            }
        }

        private StringBuilder? ParseInput(string input)
        {
            //construct initial string with operators
            for (int i = 3; i < input.Length; i++)
            {
                //handle case when input is incorrect
                if (!(_nonTerminals.Contains(input[i - 1].ToString()) ^ _terminals.Contains(input[i - 1].ToString())))
                {
                    return null;
                }

                if (!(_nonTerminals.Contains(input[i].ToString()) ^ _terminals.Contains(input[i].ToString())))
                {
                    return null;
                }

                int first = _indexes[input[i - 1]];
                int second = _indexes[input[i]];
                if (_matrix[first, second] == '\0') return null; //not a valid relation
                input = input.Insert(i, _matrix[first, second].ToString());
                i++;
            }

            input += ">$";
            StringBuilder result = new StringBuilder(input);
            return result;
        }

        private string GetReplaceForPop(string topOfStack)
        {
            foreach (var (key, list) in _transitions)
            {
                //get the state we need to replace with
                foreach (var transition in list)
                {
                    if (transition.Equals(topOfStack))
                    {
                        return key;
                    }
                }
            }

            //if not found that transition, then input string is not correct
            return "";
        }

        private string ComposeBigState(string topOfStack)
        {
            var miniStates = topOfStack.Split('=').ToList();
            string state = string.Concat(miniStates);
            return GetReplaceForPop(state);
        }
    }
}
