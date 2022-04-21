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
            if (string.IsNullOrEmpty(word.ToString()))
            {
                Console.WriteLine("Rejected");
                return;
            }

            Console.WriteLine(word);
            Parse(word);
        }

        public void Parse(StringBuilder input)
        {
            while (true)
            {
                string tempInput = input.ToString();
                //base case when we get to S, string is accepted
                if (tempInput.Contains("S"))
                {
                    Console.WriteLine("Accepted");
                    return;
                }

                //now string checking is going from backwards
                int left = input.Length - 1, right = input.Length - 1;
                while (input[left] != '<')
                {
                    if (input[left] == '>') right = left;
                    left--;
                }
                
                //take substring of that state
                var state = tempInput.Substring(left + 1, right - left - 1);

                //search for all substitutions possible
                var substitutions = state.Length == 1 ? GetReplaceForPop(state) : ComposeBigState(state);
                if (substitutions.Count == 0) //no transitions
                {
                    Console.WriteLine("Rejected");
                    return;
                }

                //in case we have only one transition
                if (substitutions.Count == 1) state = substitutions[0];
                else
                {
                    //ambiguous case
                    //as I remember from the class, right = has highest priority, then 
                    //goes left = and anything else
                    //make an initial state in case we have only < and >
                    var changeToState = substitutions[0];
                    var symbols = new char[2];
                    symbols[0] = _matrix[_indexes[input[left - 1]], _indexes[changeToState[0]]];
                    symbols[1] = _matrix[_indexes[changeToState[0]], _indexes[input[right + 1]]];
                    foreach (var substitution in substitutions)
                    {
                        if(substitution.Equals(changeToState)) continue;
                        var tempLeft = _matrix[_indexes[input[left - 1]], _indexes[substitution[0]]];
                        var tempRight = _matrix[_indexes[substitution[0]], _indexes[input[right + 1]]];
                        if (tempRight == '=') //biggest priority found
                        {
                            changeToState = substitution;
                            symbols[0] = tempLeft;
                            symbols[1] = tempRight;
                        }
                        else if (tempLeft == '=' && !symbols[1].Equals('=')) //if got a left = and still no right equal found
                        {
                            changeToState = substitution;
                            symbols[0] = tempLeft;
                            symbols[1] = tempRight;
                        }
                    }

                    state = changeToState;
                }

                //change the state inside parantheses
                input.Remove(left + 1, right - left - 1);
                input.Insert(left + 1, state); //insert into the string the modified state
                right = left + 2; //we know that after left index we add only a char, so > will be 2 indexes away (in case insert long state)

                // now insert the needed operators in the word to continue parsing
                char leftOperator = _matrix[_indexes[input[left - 1]], _indexes[input[left + 1]]];
                char rightOperator = _matrix[_indexes[input[right - 1]], _indexes[input[right + 1]]];
                input[left] = leftOperator;
                input[right] = rightOperator;
                Console.WriteLine(input);
                ;
            }
        }

        private StringBuilder ParseInput(string input)
        {
            //construct initial string with operators
            for (int i = 3; i < input.Length; i++)
            {
                //handle case when input is incorrect
                if (!(_nonTerminals.Contains(input[i - 1].ToString()) ^ _terminals.Contains(input[i - 1].ToString())))
                {
                    return new StringBuilder();
                }

                if (!(_nonTerminals.Contains(input[i].ToString()) ^ _terminals.Contains(input[i].ToString())))
                {
                    return new StringBuilder();
                }

                int first = _indexes[input[i - 1]];
                int second = _indexes[input[i]];
                if (_matrix[first, second] == '\0') return new StringBuilder(); //not a valid relation
                input = input.Insert(i, _matrix[first, second].ToString());
                i++;
            }

            input += ">$";
            var result = new StringBuilder(input);
            return result;
        }

        private List<string> GetReplaceForPop(string topOfStack)
        {
            var result = new List<string>();
            foreach (var (key, list) in _transitions)
            {
                //get the state we need to replace with
                foreach (var transition in list)
                {
                    if (transition.Equals(topOfStack))
                    {
                        result.Add(key);
                    }
                }
            }

            //if not found that transition, then input string is not correct
            return result.Count != 0 ? result : new List<string>();
        }

        private List<string> ComposeBigState(string topOfStack)
        {
            var miniStates = topOfStack.Split('=').ToList();
            string state = string.Concat(miniStates);
            return GetReplaceForPop(state);
        }
    }
}
