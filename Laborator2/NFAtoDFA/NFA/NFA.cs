using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFA
{
    class NFA
    {
        private readonly List<string> _states;
        private readonly string _startState;
        private string _currentState;
        private readonly List<string> _finalStates;
        private readonly List<string> _alphabet;
        private Dictionary<Tuple<string, string>, string> _transitions;
        private bool _isDeterministic;

        public NFA(List<string> states, string startState, string currentState, List<string> finalStates, List<string> alphabet, Dictionary<Tuple<string, string>, string> transitions, bool isDeterministic)
        {
            _states = states;
            _startState = startState;
            _currentState = currentState;
            _finalStates = finalStates;
            _alphabet = alphabet;
            _transitions = transitions;
            _isDeterministic = isDeterministic;
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
                        table[i, j] = _transitions[temp];
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
                    Console.Write(String.Format("{0}\t\t", table[i, j]));
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

        public void ConvertNFA()
        {
            Queue<string> queue = new Queue<string>();
            Dictionary<Tuple<string, string>, string> newTransitions = new Dictionary<Tuple<string, string>, string>();
            HashSet<string> presentStates = new HashSet<string>(); //add q0 as the already seen state
            
            InitiateConversion(ref presentStates, ref queue);

            while (queue.Count > 0)
            {
                if (_states.Contains(queue.Peek())) //if that state already was present or it is a new state, in order to check the existent transition table
                {
                    foreach (var symbol in _alphabet)
                    {
                        AddNewState(ref queue, ref newTransitions, ref presentStates, symbol);
                    }

                    queue.Dequeue();
                }
                else
                {
                    //rearrange to normal, break down q1q3 into q1, q3
                    BreakDownQueueState(ref queue, out List<string> brokenDownStatesFromQueue);

                    foreach (var symbol in _alphabet) //firstly we check each state (q1 and q3) for a, then (q1 and q3) for b ...
                    {

                        AddNewComposedState(out HashSet<string> brokenDownStatesFromTable, ref newTransitions, symbol, brokenDownStatesFromQueue);

                        //when for one symbol we are done, we check if any words were formed
                        if (brokenDownStatesFromTable.Count > 0)
                        {
                            var sortedStatesFromTable = brokenDownStatesFromTable.OrderBy(x => x).ToList(); //sort the set of found states
                            string finalState = sortedStatesFromTable[0]; //create that final state

                            for (int i = 1; i < sortedStatesFromTable.Count; i++)
                            {
                                finalState += sortedStatesFromTable[i];
                            }

                            Tuple<string, string> tuple = new Tuple<string, string>(queue.Peek(), symbol);
                            Console.WriteLine($"Found a new transition {queue.Peek()} to {finalState} via {symbol}");
                            newTransitions[tuple] = finalState; //add it to the transition table
                            if (!presentStates.Contains(finalState)) //in case it is not present in the transition table, add it to the queue and add to the presentStates to not consider it again
                            {
                                Console.WriteLine($"Found a fresh state {finalState} that is about to be investigated!");
                                queue.Enqueue(finalState);
                                presentStates.Add(finalState);
                            }
                        }
                    }

                    queue.Dequeue();
                }
            }

            UpdateAutomaton(ref newTransitions);

        }

        public void InitiateConversion(ref HashSet<string> presentStates, ref Queue<string> queue)
        {
            Console.WriteLine("Initializing conversion. Adding q0 to the new transition table");
            presentStates.Add("q0");

            //push the initial state to queue
            queue.Enqueue(_startState);
        }

        public void UpdateAutomaton(ref Dictionary<Tuple<string, string>, string> input)
        {
            //update NFA class

            _states.Clear();
            foreach (var state in input)
            {
                if (!_states.Contains(state.Key.Item1)) //and the ones that go anywhere, if some states disappeared, they will not be included
                {
                    _states.Add(state.Key.Item1);
                }

                if (!_states.Contains(state.Value)) //in case of final states
                {
                    _states.Add(state.Value);
                }
            }


            _transitions = input;

            int len = _finalStates.Count;
            for (int i = 0; i < len; i++)
            {
                foreach (var state in _states)
                {
                    if (state.Contains(_finalStates[i]) && state != _finalStates[i]) //adds the final states, not considering the one that is already there
                    {
                        _finalStates.Add(state);
                    }
                }
            }

            //finally can check strings :)
            _isDeterministic = true;
        }

        public void AddNewState(ref Queue<string> queue, ref Dictionary<Tuple<string, string>, string> newTransitions, ref HashSet<string> presentStates, string symbol)
        {
            var tuple = new Tuple<string, string>(queue.Peek(), symbol);
            if (_transitions.ContainsKey(tuple)) //if that tuple is found in the original transition table
            {
                Console.WriteLine($"Found a new transition {queue.Peek()} to {_transitions[tuple]} via {symbol}");
                newTransitions[tuple] = _transitions[tuple];//migrate the transition to the new table
                if (!presentStates.Contains(_transitions[tuple])) //check the presentStates if that state is already in the new transition table
                {   
                    //because it is a new state, add it to the queue and to the presentStates
                    Console.WriteLine($"Found a fresh state {_transitions[tuple]} that is about to be investigated!");
                    queue.Enqueue(_transitions[tuple]);
                    presentStates.Add(_transitions[tuple]);
                }
            }
        }

        public void BreakDownQueueState(ref Queue<string> queue, out List<string> brokenDownStatesFromQueue)
        {
            brokenDownStatesFromQueue = new List<string>();
            for (int i = 0; i < queue.Peek().Length; i += 2)
            {
                brokenDownStatesFromQueue.Add(queue.Peek().Substring(i, 2)); //retrieve the state from big string
            }

        }

        public void AddNewComposedState(out HashSet<string> brokenDownStatesFromTable, ref Dictionary<Tuple<string, string>, string> newTransitions, string symbol, List<string> brokenDownStatesFromQueue) 
        {
            brokenDownStatesFromTable = new HashSet<string>();
            foreach (var state in brokenDownStatesFromQueue)
            {
                var tuple = new Tuple<string, string>(state, symbol);
                if (newTransitions.ContainsKey(tuple)) //in the already built transition table search the states
                {
                    var temp = new List<string>(); //we can face composed states so we will decompose them
                    for (int i = 0; i < newTransitions[tuple].Length; i += 2) //in case the pair is composed
                    {
                        temp.Add(newTransitions[tuple].Substring(i, 2));
                    }

                    foreach (var s in temp) //then every state in decomposed list are going into the Set
                    {
                        brokenDownStatesFromTable.Add(s); //for no duplicates
                    }

                }
                else if (_transitions.ContainsKey(tuple)) //also search in the old transition table in case there is no that state already
                {
                    var temp = new List<string>(); //we can face composed states so we will decompose them
                    for (int i = 0; i < _transitions[tuple].Length; i += 2) //in case the pair is composed
                    {
                        temp.Add(_transitions[tuple].Substring(i, 2));
                    }

                    foreach (var s in temp) //then every state in decomposed list are going into the Set
                    {
                        brokenDownStatesFromTable.Add(s);
                    }
                }
            }
        }

        public bool IsDeterministic()
        {
            return _isDeterministic;
        }
    }
}
