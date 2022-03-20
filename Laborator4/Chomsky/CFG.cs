﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class CFG
    {
        private Dictionary<string, List<string>> Transitions;
        private HelperClass Helper = new();
        public CFG(Dictionary<string, List<string>> transitions)
        {
            Transitions = transitions;
        }

        public void ConvertCfGtoCnf()
        {
            PrintTransitions();
            RemoveNullTransitions();
            PrintTransitions();
        }

        private void PrintTransitions()
        {
            foreach (var (key, list) in Transitions)
            {
                Console.Write($"{key} -> ");
                for (int i = 0; i < list.Count; i++)
                {
                    if(i < list.Count - 1) Console.Write($"{list[i]} | ");
                    else Console.Write(list[i]);
                }

                Console.WriteLine();
            }

            Console.WriteLine("--------------");
        }

        private void RemoveNullTransitions()
        {
            //repeat the process of removing epsilons while any appear
            while (Helper.HasEpsilon(Transitions))
            {
                var epsilonsStates = new List<string>();
                //add Non Terminal states that have epsilon
                foreach (var (key, list) in Transitions)
                {
                    if (list.Any(state => state.Equals("ε"))) 
                        epsilonsStates.Add(key); 
                }

                //add new states that had an epsilonState
                foreach (var epsilon in epsilonsStates)
                {
                    //remove the epsilon from epsilonState
                    Transitions[epsilon].Remove("ε");

                    //epsilonState = A;

                    //add new states that have epsilon
                    foreach (var (key, list) in Transitions)
                    {
                        int size = list.Count;
                        for (int i = 0; i < size; i++)
                        {
                            //S -> A, A -> ε
                            //in case the state maps to the epsilonState
                            if (list[i].Equals(epsilon))
                            {
                                //make it an epsilonState
                                Transitions[key][i] = "ε";
                            }
                            else if (list[i].Contains(epsilon))
                            {
                                //S -> aA, A -> ε
                                //have to check in case we have multiple epsilonStates in the currentString
                                var compositeEpsilon = list[i].Count(ch => ch == char.Parse(epsilon));
                                if (compositeEpsilon == 1)
                                {
                                    string add = list[i].Replace(epsilon, "");
                                    Transitions[key].Add(add);
                                }
                                else
                                {
                                    //S -> ABsA, A -> ε
                                    //break down a big state into smaller
                                    var smallerStates = Helper.GenerateStates(list[i], epsilon);
                                    foreach (var state in smallerStates)
                                    {
                                       Transitions[key].Add(state); 
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
