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
            foreach (var (key, list) in transitions)
            {
                foreach (var state in list)
                {
                    foreach (var ch in state)
                    {
                        if (char.IsUpper(ch)) nonTerminals.Add(ch.ToString());
                    }
                }
            }
        }

        internal void RemoveNonTerminals(Dictionary<string, List<string>> transitions)
        {
            //remove those that are not in the RHS
            foreach (var (key, list) in transitions)
            {
                if (!nonTerminals.Contains(key))
                {
                    transitions.Remove(key);
                }
            }
        }
    }
}
