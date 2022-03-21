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
        private char constant = '\u03B1';
        public CFG(Dictionary<string, List<string>> transitions)
        {
            Transitions = transitions;
        }

        public void ConvertCfGtoCnf()
        {
            Console.WriteLine("Context free grammar: ");
            PrintTransitions();
            RemoveNullTransitions();
            Console.WriteLine("Grammar after removing null transitions: ");
            PrintTransitions();
            RemoveUnitTransitions();
            Console.WriteLine("Grammar after removing unit transitions: ");
            PrintTransitions();
            RemoveNonProductiveTransactions();
            Console.WriteLine("Grammar after removing non productive transitions: ");
            PrintTransitions();
            RemoveInaccessibleTransactions();
            Console.WriteLine("Grammar after removing inaccessible transitions: ");
            PrintTransitions();
            ChomskyNormalForm();
            Console.WriteLine("Chomsky normal form: ");
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

        private void RemoveUnitTransitions()
        {
            //check if Transitions still have unit 
            while (Helper.HasUnit(Transitions))
            {
                var unitList = new Dictionary<string, List<string>>(); //in case S -> A, S -> B
                foreach (var (key, list) in Transitions)
                {
                    //adding the states into the array
                    int size = list.Count;
                    for (int i = 0; i < size; i++)
                    {
                        if (list[i].Length == 1 && char.IsUpper(char.Parse(list[i]))) 
                        {
                            if (!unitList.ContainsKey(key))
                            {
                                unitList.Add(key, new List<string>() { list[i] });
                            }
                            else
                            {
                                unitList[key].Add(list[i]);
                            }
                            
                        }
                    }
                }

                foreach (var (unit, list) in unitList)
                {

                    for (int i = 0; i < list.Count; i++)
                    {
                        //remove that unit from Transitions
                        Transitions[unit].Remove(list[i]);

                        //S -> A, A -> ... many states, add all states from A to S
                        //for every outgoing state, add to the unit all the states
                        foreach (var state in Transitions[list[i]])
                        {
                            //don't add if we have duplicates
                            if (!Transitions[unit].Contains(state))
                            {
                                Transitions[unit].Add(state);
                            }
                        }

                    }
                }

            }
        }

        private void RemoveNonProductiveTransactions()
        {
            //add NonTerminals that will be used to check non productive
            var nonTerminals = new HashSet<string>();
            foreach (var (key, list) in Transitions)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Length == 1 && char.IsLower(char.Parse(list[i])))
                    {
                        nonTerminals.Add(key);
                    }
                }
            }

            //check Transitions once more, substituting known nonTerminals in unknown nonTerminals
            //if new are found, do another iteration in case other unknowns are found
            int iteration = 1;
            while (iteration != 0)
            {
                foreach (var (key, list) in Transitions)
                {
                    if(nonTerminals.Contains(key)) continue;

                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < list[i].Length; j++)
                        {
                            if (nonTerminals.Contains(list[i][j].ToString()))
                            {
                                nonTerminals.Add(key);
                                iteration++;
                                break;
                            }
                        }
                    }
                }

                iteration--;
            }

            //remove all occurrences of that key
            foreach (var (key, list) in Transitions)
            {
                if (!nonTerminals.Contains(key))
                {
                    //remove the key itself
                    Transitions.Remove(key);

                    //now remove occurrences
                    foreach (var (tempKey, tempList) in Transitions)
                    {
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            //removes every occurrence of non productive state
                            Transitions[tempKey].RemoveAll(state => state.Contains(key));
                        }
                    }
                }
            }
        }

        private void RemoveInaccessibleTransactions()
        {
            //find NonTerminals in the RHS
            var nonTerminals = new HashSet<string> {"S"}; //S is accessible by default
            foreach (var (key, list) in Transitions)
            {
                foreach (var state in list)
                {
                    foreach (var ch in state)
                    {
                        if (char.IsUpper(ch)) nonTerminals.Add(ch.ToString());
                    }
                }
            }

            //remove those that are not in the RHS
            foreach (var (key, list) in Transitions)
            {
                if (!nonTerminals.Contains(key))
                {
                    Transitions.Remove(key);
                }
            }
        }

        private void ChomskyNormalForm()
        {
            NormalizeTerminalAndNonTerminal();
            NormalizeManySymbols();
        }

        private void NormalizeTerminalAndNonTerminal()
        {
            //change transactions with terminals and non terminals
            var rules = new Dictionary<string, string>();
            foreach (var (key, list) in Transitions)
            {
                for(int i = 0; i < list.Count; i++) 
                {
                    if (list[i].Length > 1) //consider states with length >= 2
                    {
                        for (int j = 0; j < list[i].Length; j++)
                        {
                            if (char.IsLower(list[i][j]))
                            {
                                //check if we have in rules that substitution 
                                if (rules.ContainsKey(list[i][j].ToString()))
                                {
                                    //take substring before terminal, the changed terminal and after terminal substring
                                    string subState = list[i][..j] +
                                                      rules[list[i][j].ToString()] +
                                                      list[i][++j..];
                                    Transitions[key][i] = subState;
                                }
                                else
                                {
                                    rules[list[i][j].ToString()] = $"{constant++}";
                                    string subState = list[i][..j] +
                                                      rules[list[i][j].ToString()] +
                                                      list[i][++j..];
                                    Transitions[key][i] = subState;
                                }
                            }
                        }
                    }
                }
            }

            //add every new rule to the transition table
            foreach (var (key, value) in rules)
            {
                Transitions[value] = new List<string> {key};
            }
        }
        private void NormalizeManySymbols()
        {
            //change transactions with more than 2 symbols
            var rules = new Dictionary<string, string>();
            while (Helper.HasLong(Transitions))
            {
                foreach (var (key, list) in Transitions.ToList())
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
                                var subState = list[i].Substring(j, 2);
                                subStates.Add(subState);
                            }

                            foreach (var state in subStates)
                            {
                                if (!rules.ContainsKey(state))
                                {
                                    //add the new rule and replace old Varible with new
                                    rules.Add(state, constant.ToString());
                                    string changedState = list[i].Replace(state, rules[state]);
                                    Transitions[key][i] = changedState;
                                    constant++;
                                }
                                else
                                {
                                    //replace 2 Terminal with 1 Terminal
                                    string changedState = list[i].Replace(state, rules[state]);
                                    Transitions[key][i] = changedState;
                                }
                            }
                        }
                    }
                }
            }

            //add the remaining rules to the transitions
            foreach (var (key, value) in rules)
            {
                Transitions.Add(value, new List<string>(){key});
            }
        }
    }
}
