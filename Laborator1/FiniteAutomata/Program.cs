using System;
using System.Collections.Generic;
using System.Linq;

namespace FiniteAutomata
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //read all the lines from the source txt file
            var lines = System.IO.File.ReadAllLines(@"D:\UTM\Anul 2\Semestrul 4\LFPC\Laborator1\FiniteAutomata\source.txt");

            List<string> states = new List<string>();
            List<string> finalStates = new List<string>();
            List<string> alphabet = new List<string>();
            Dictionary<Tuple<string, string>, string> transitions = new Dictionary<Tuple<string, string>, string>();

            //add states to the dictionary
            Dictionary<string, string> stateDictionary = new Dictionary<string, string>();
            var separatedStates = lines[0].Split(',');
            foreach (var strState in separatedStates)
            {
                stateDictionary.Add(strState, $"q{stateDictionary.Count}");
            }

            //S is the starting point and the currentState
            var startState = stateDictionary["S"];
            var currentState = stateDictionary["S"];

            foreach (var (_, value) in stateDictionary)
            {
                states.Add(value);
            }

            var separatedAlphabet = lines[1].Split(',');
            foreach (var symbol in separatedAlphabet)
            {
                alphabet.Add(symbol);
            }


            //fill the transition function
            for (var i = 2; i < lines.Length; i++)
            {
                var separatedTransition = lines[i].Split('>');
                //if only road without an existent state
                if (separatedTransition[1].Length == 1)
                {   
                    states.Add($"q{states.Count}");
                    finalStates.Add(states.Last());
                    transitions.Add(new Tuple<string, string>(stateDictionary[separatedTransition[0]], separatedTransition[1][0].ToString()),
                        finalStates.Last());

                    continue;
                }

                
                transitions.Add(new Tuple<string, string>(stateDictionary[separatedTransition[0]], separatedTransition[1][0].ToString()), 
                    stateDictionary[separatedTransition[1][1].ToString()]);

            }

            var dfa = new DFA(states, startState, currentState, finalStates, alphabet, transitions);

            Console.WriteLine("Press N if you want to stop testing the automaton \nPress S to see the states of the automaton \nPress W to check a string");
            while (true)
            {
                var userInput = Console.ReadLine();
                if (userInput == "N") break;
                
                switch (userInput)
                {
                    case "S":
                        dfa.PrintAutomaton();
                        break;
                    case "W":
                    {
                        Console.WriteLine("Press Q when you want to stop checking strings");
                        while (true)
                        {
                            Console.WriteLine("Type in a string  to check it: ");
                            var input = Console.ReadLine();
                            if (input == "Q") break;
                            Console.WriteLine(dfa.CheckString(input));
                        }

                        break;
                    }
                }
            }


        }
    }
}
