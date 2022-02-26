using System;
using System.Collections.Generic;
using System.Text;

namespace FiniteAutomata
{
    public class DFA
    {
        private readonly List<string> _states;
        private readonly string _startState;
        private string _currentState;
        private readonly List<string> _finalStates;
        private readonly List<string> _alphabet;
        private readonly Dictionary<Tuple<string, string>, string> _transitions;

        public DFA(List<string> states, string startState, string currentState, List<string> finalStates, List<string> alphabet, Dictionary<Tuple<string, string>, string> transitions)
        {
            _states = states;
            _startState = startState;
            _currentState = currentState;
            _finalStates = finalStates;
            _alphabet = alphabet;
            _transitions = transitions;
        }

        public void PrintAutomaton()
        {   

            //analytical representation
            Console.Write("States: ");
            foreach (var state in _states)
            {
                Console.Write($"{state} ");
            }

            Console.WriteLine();

            Console.WriteLine($"Starting state: {_startState}");
            Console.WriteLine($"Current state: {_currentState}");
            Console.Write("Final states are:");

            foreach (var finals in _finalStates)
            {
                Console.Write($"{finals} ");
            }

            Console.WriteLine();
            Console.Write("Alphabet: ");

            foreach (var symbol in _alphabet)
            {
                Console.Write($"{symbol} ");
            }

            Console.WriteLine();

            foreach (var ((from, symbol), to) in _transitions)
            {
                Console.WriteLine($"{from},{symbol} -> {to}");
            }

            //transition table
            Console.WriteLine();
            string[,] table = new string[_states.Count + 1, _alphabet.Count + 1];
            for (int i = 0; i < _states.Count; i++)
            {
                table[i + 1, 0] = _states[i];
            }

            for (int i = 0; i < _alphabet.Count; i++)
            {
                table[0, i + 1] = _alphabet[i];
            }

            for (int i = 1; i < table.GetLength(0); i++)
            {
                for (int j = 1; j < table.GetLength(1); j++)
                {
                    if (_transitions.ContainsKey(new Tuple<string, string>(table[i, 0], table[0, j])))
                    {
                        var temp = new Tuple<string, string>(table[i, 0], table[0, j]);
                        table[i,j] = _transitions[temp];
                    }
                    else
                    {
                        table[i, j] = "-";
                    }
                }
            }

            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    Console.Write(String.Format("{0}\t", table[i, j]));
                }

                Console.WriteLine();

                
            }

        }

        public string CheckString(string input)
        {
            _currentState = _startState;
            foreach (var symbol in input)
            {
                var temp = new Tuple<string, string>(_currentState, symbol.ToString());
                if (!_transitions.ContainsKey(temp)) //no such letter in the alphabet
                    return "No";
                
                _currentState = _transitions[temp];
            }

            if (_finalStates.Contains(_currentState))
                return "Yes";

            return "No";
        }
    }
}
