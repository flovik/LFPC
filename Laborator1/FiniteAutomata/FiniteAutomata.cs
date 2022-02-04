using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FiniteAutomata
{
    class FiniteAutomata
    {
        private Dictionary<char, List<string>> _map;

        public FiniteAutomata()
        {
            _map = new Dictionary<char, List<string>>();
        }

        public void AddState(char state, string transition)
        {
            var list = new List<string>();
 
            list.Add(transition);
            if (_map.ContainsKey(state))
            {
                _map[state].Add(transition);
            }
            else
            {
                _map.Add(state, list);
            }
            

        }

        public void GetStates()
        {
            foreach (var pair in _map)
            {
                foreach (var transition in pair.Value)
                {
                    if (transition.Length == 2)
                    {
                        Console.WriteLine($"{pair.Key} -> {transition[0]}{transition[1]}");
                    }
                    else
                    {
                        Console.WriteLine($"{pair.Key} -> {transition}");
                    }
                }
            }
        }

        public string CheckString(string input)
        {
            if (input == "")
            {
                return "String not accepted";
            }

            char currentKey = 'S';
            for (int i = 0; i < input.Length; i++)
            {
                bool existingRoad = false;
                char currentChar = input[i];
                foreach (var pair in _map[currentKey])
                {
                    if (currentChar.Equals(pair[0]) && pair.Length == 2)
                    {
                        if (i == input.Length - 1)
                        {
                            return "String not accepted";
                        }

                        existingRoad = true;
                        currentKey = pair[1];
                        break;
                    }
                    if (currentChar.Equals(pair[0]) && pair.Length == 1)
                    {
                        if (i == input.Length - 1)
                        {
                            return "String accepted";
                        }
                        
                        return "String not accepted";
                        
                    }
                }

                if (!existingRoad)
                {
                    return "String not accepted";
                }
  
            }
 


            return "String not accepted";
        }
    }
}
