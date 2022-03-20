using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class HelperClass
    {
        internal bool HasEpsilon(Dictionary<string, List<string>> transitions)
        {
            //checks if grammar has an epsilon
            foreach (var (_, list) in transitions)
            {
                if (list.Any(state => state.Equals("ε"))) return true;
            }

            return false;
        }

        internal List<string> GenerateStates(string input, string epsilonState)
        {
            //decompose the state into smaller states, by removing one A at a time (substring)
            //ABAAs
            //ABAs, BAAs
            //ABs, BAs
            //Bs
            var list = new List<string>();
            var queue = new Queue<string>();
            queue.Enqueue(input);
            while (queue.Count != 0)
            {
                string state = queue.Peek();
                for (int i = 0; i < state.Length; i++)
                {
                    if (state[i] == char.Parse(epsilonState))
                    {
                        //take substring before that A and after that A
                        string subState = state[..i] + state[++i..];
                        if (!list.Contains(subState))
                        {
                            //handles duplicates
                            queue.Enqueue(subState); 
                            list.Add(subState);
                        }
                    }
                }

                queue.Dequeue();
            }

            return list;
        }

        internal bool HasUnit(Dictionary<string, List<string>> transitions)
        {
            //checks if any state is of length 1 and is uppercase
            foreach (var (_, list) in transitions)
            {
                if (list.Any(state => state.Length == 1 && state.Any(char.IsUpper))) return true;
            }

            return false;
        }
    }
}
