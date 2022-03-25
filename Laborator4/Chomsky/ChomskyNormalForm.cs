using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class ChomskyNormalForm
    {
        private char constant = '\u03B1';
        internal void NormalizeTerminals(Dictionary<string, List<string>> transitions)
        {
            //change transactions with terminals and non terminals
            var rules = new Dictionary<string, string>();
            foreach (var (key, list) in transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length > 1) //consider states with length >= 2
                    {
                        for (int j = 0; j < list[i].Length; j++)
                        {
                            if (char.IsLower(list[i][j])) //change all terminals, all
                            {
                                //check if we have in rules that substitution 
                                if (rules.ContainsKey(list[i][j].ToString()))
                                {
                                    //take substring before terminal, the changed terminal and after terminal substring
                                    string subState = list[i][..j] +
                                                      rules[list[i][j].ToString()] +
                                                      list[i][++j..];
                                    transitions[key][i] = subState;
                                }
                                else
                                {
                                    //a -> X1 (greek letter in my case)
                                    rules[list[i][j].ToString()] = $"{constant++}";
                                    string subState = list[i][..j] +
                                                      rules[list[i][j].ToString()] +
                                                      list[i][++j..];
                                    transitions[key][i] = subState;
                                }
                            }
                        }
                    }
                }
            }

            //add every new rule to the transition table
            foreach (var (key, value) in rules)
            {
                transitions[value] = new List<string> { key };
            }
        }
        internal void NormalizeManySymbols(Dictionary<string, List<string>> transitions)
        {
            //change transactions with more than 2 symbols
            var rules = new Dictionary<string, string>();
            while (HasLong(transitions))
            {
                foreach (var (key, list) in transitions.ToList())
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Length > 2) //consider states with length >= 3
                        {
                            //take substrings of len 2
                            //in case state is odd len, ignore last char
                            int size = list[i].Length - (list[i].Length % 2);
                            var subStates = new List<string>();
                            for (int j = 0; j < size; j += 2)
                            {
                                //take all the pairs of big state
                                var subState = list[i].Substring(j, 2);
                                subStates.Add(subState);
                            }

                            foreach (var state in subStates) //now change all that pairs
                            {
                                if (!rules.ContainsKey(state))
                                {
                                    //add the new rule and replace old Variable with new
                                    rules.Add(state, constant.ToString());
                                    string changedState = list[i].Replace(state, rules[state]);
                                    transitions[key][i] = changedState;
                                    constant++;
                                }
                                else
                                {
                                    //replace 2 Terminal with 1 Terminal
                                    string changedState = list[i].Replace(state, rules[state]);
                                    transitions[key][i] = changedState;
                                }
                            }
                        }
                    }
                }
            }

            //add the remaining rules to the transitions
            foreach (var (key, value) in rules)
            {
                transitions.Add(value, new List<string>() { key });
            }
        }
        internal bool HasLong(Dictionary<string, List<string>> transitions)
        {
            //checks if grammar has states longer than 2
            foreach (var (_, list) in transitions)
            {
                if (list.Any(state => state.Length > 2)) return true;
            }

            return false;
        }
    }
}
