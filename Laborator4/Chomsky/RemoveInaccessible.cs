using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class RemoveInaccessible
    {
        private HashSet<string> nonTerminals = new() {"S"}; //S is accessible by default

        internal void FindNonTerminals(Dictionary<string, List<string>> transitions)
        {
            //find NonTerminals in the RHS
            int iteration = 1;
            while (iteration != 0)
            {
                //iterate every reachable state if you can reach other states, start from S
                int size = nonTerminals.Count;
                foreach (var reachableTerminal in nonTerminals.ToList())
                {
                    foreach (var potentialReachable in transitions[reachableTerminal]) //check just the list of reachable states!
                    {
                        foreach (var ch in potentialReachable) 
                        {
                            if (char.IsUpper(ch) && !nonTerminals.Contains(ch.ToString()))
                            {
                                nonTerminals.Add(ch.ToString());
                                iteration++;
                            }
                        }
                    }
                }

                iteration--;
            }
        }

        internal void RemoveNonTerminals(Dictionary<string, List<string>> transitions)
        {
            //remove those that are not in the RHS
            foreach (var (key, _) in transitions)
            {
                if (!nonTerminals.Contains(key))
                {
                    transitions.Remove(key);
                }
            }
        }
    }
}
