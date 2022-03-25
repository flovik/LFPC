using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class RemoveNull
    {
        private List<string> epsilonStates;
        internal bool HasEpsilon(Dictionary<string, List<string>> transitions)
        {
            //checks if grammar has an epsilon
            foreach (var (_, list) in transitions)
            {
                if (list.Any(state => state.Equals("ε"))) return true;
            }

            return false;
        }

        internal void GenerateEpsilonStates(Dictionary<string, List<string>> transitions)
        {
            epsilonStates = new List<string>();
            //add Non Terminal states that have epsilon
            foreach (var (key, list) in transitions)
            {
                if (list.Any(state => state.Equals("ε")))
                    epsilonStates.Add(key);
            }
        }

        internal void AddNewEpsilonStatesToTransitions(Dictionary<string, List<string>> transitions)
        {
            //add new states that had an epsilonState
            foreach (var epsilon in epsilonStates)
            {
                //remove the epsilon from epsilonState
                transitions[epsilon].Remove("ε");

                //epsilonState = A;
                //add new states that have epsilon
                foreach (var (key, list) in transitions)
                {
                    int size = list.Count;
                    for (int i = 0; i < size; i++)
                    {
                        //S -> A, A -> ε
                        //in case the state maps to the epsilonState
                        if (list[i].Equals(epsilon))
                        {
                            //make it an epsilonState
                            transitions[key][i] = "ε";
                        }
                        else if (list[i].Contains(epsilon))
                        {
                            //S -> aA, A -> ε
                            //have to check in case we have multiple epsilonStates in the currentString
                            //how many A's we have
                            var compositeEpsilon = list[i].Count(ch => ch == char.Parse(epsilon));
                            if (compositeEpsilon == 1)
                            {
                                string add = list[i].Replace(epsilon, "");
                                //avoid duplicates
                                if(!transitions[key].Contains(add))
                                    transitions[key].Add(add);
                            }
                            else
                            {
                                //S -> ABsA, A -> ε
                                //break down a big state into smaller
                                var smallerStates = GenerateStates(list[i], epsilon);
                                foreach (var state in smallerStates)
                                {
                                    transitions[key].Add(state);
                                }
                            }
                        }
                    }
                }
            }
            
        }
        internal List<string> GenerateStates(string input, string epsilonState)
        {
            //decompose the state into smaller states, by removing one A at a time (substring)
            //ABAAs
            //ABAs, BAAs
            //ABs, BAs
            //Bs
            var list = new List<string>(); //temp list that will store all decomposed states
            var queue = new Queue<string>();
            queue.Enqueue(input);
            while (queue.Count != 0)
            {
                string state = queue.Peek();
                for (int i = 0; i < state.Length; i++)
                {
                    if (state[i] == char.Parse(epsilonState)) //if currentChar is A
                    {
                        //take substring before that A and after that A
                        string subState = state[..i] + state[++i..];
                        if (subState.Length == 0) //handle if multiple A's result in a empty string
                        {
                            list.Add("ε");
                        }
                        else if (!list.Contains(subState)) //handles duplicates
                        {
                            queue.Enqueue(subState);
                            list.Add(subState);
                        }
                        
                    }
                }

                queue.Dequeue();
            }

            return list;
        }
    }
}
