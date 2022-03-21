using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class RemoveNonProductive
    {
        private HashSet<string> nonTerminals = new();

        internal void AddNonTerminals(Dictionary<string, List<string>> transitions)
        {
            foreach (var (key, list) in transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length == 1 && char.IsLower(char.Parse(list[i])))
                    {
                        nonTerminals.Add(key);
                    }
                }
            }
        }

        internal void CheckTransitions(Dictionary<string, List<string>> transitions)
        {
            //check Transitions once more, substituting known nonTerminals in unknown nonTerminals
            //if new are found, do another iteration in case other unknowns are found
            int iteration = 1;
            while (iteration != 0)
            {
                foreach (var (key, list) in transitions)
                {
                    if (nonTerminals.Contains(key)) continue;

                    for (int i = 0; i < list.Count; i++)
                    {
                        var toBeCheckedNonTerminals = new List<string>();
                        for (int j = 0; j < list[i].Length; j++)
                        {
                            //to be sure the whole word is ok, need to check every Non Terminal letter
                            if(char.IsUpper(list[i][j])) toBeCheckedNonTerminals.Add(list[i][j].ToString());
                        }

                        //checks if all are Known Non Terminals!
                        if (toBeCheckedNonTerminals.All(nonTerminal => char.IsUpper(nonTerminal[0])))
                        {
                            nonTerminals.Add(key);
                            iteration++;
                        }
                    }
                }

                iteration--;
            }
        }

        internal void RemoveOccurrencesKey(Dictionary<string, List<string>> transitions)
        {
            //remove all occurrences of that key
            foreach (var (key, list) in transitions)
            {
                if (!nonTerminals.Contains(key))
                {
                    //remove the key itself
                    transitions.Remove(key);

                    //now remove occurrences in all other states
                    foreach (var (tempKey, tempList) in transitions)
                    {
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            //removes every occurrence of non productive state
                            transitions[tempKey].RemoveAll(state => state.Contains(key));
                        }
                    }
                }
            }
        }
    }
}
