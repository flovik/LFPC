using System;
using System.Collections.Generic;
using System.Linq;

namespace NFA
{
    class Program
    {
        static void Main(string[] args)
        {
            //read all the lines from the source txt file
            var lines = System.IO.File.ReadAllLines(@"D:\UTM\Anul 2\Semestrul 4\LFPC\Laborator2\NFAtoDFA\NFA\source.txt");

            List<string> states = new List<string>();
            List<string> finalStates = new List<string>();
            List<string> alphabet = new List<string>();
            Dictionary<Tuple<string, string>, string> transitions = new Dictionary<Tuple<string, string>, string>();
            bool isDeterministic = true;
   
            var separatedStates = lines[0].Split(',');
            foreach (var strState in separatedStates)
            {
                states.Add(strState);
            }

            //S is the starting point and the currentState
            var startState = "q0";
            var currentState = "q0";

            
            var separatedAlphabet = lines[1].Split(',');
            foreach (var symbol in separatedAlphabet)
            {
                alphabet.Add(symbol);
            }

            var separatedFinalStates = lines[2].Split(',');
            foreach (var final in separatedFinalStates)
            {
                finalStates.Add(final);
            }

            
            //fill the transition function
            for (var i = 3; i < lines.Length; i++)
            {
                var fromState = lines[i].Substring(0, lines[i].IndexOf(':'));
                var symbol = lines[i].Substring(lines[i].IndexOf(':') + 1, 1);
                var toState = lines[i].Substring(lines[i].IndexOf('>') + 1, 2);

                if (!transitions.ContainsKey(new Tuple<string, string>(fromState, symbol)))
                {
                    transitions.Add(new Tuple<string, string>(fromState, symbol), toState);
                    continue;
                }

                transitions[new Tuple<string, string>(fromState, symbol)] += toState;
                isDeterministic = false;
            }

            var dfa = new NFA(states, startState, currentState, finalStates, alphabet, transitions, isDeterministic);

            Console.WriteLine("Press N if you want to stop testing the automaton " +
                              "\nPress S to see the states of the automaton " +
                              "\nPress W to check a string" +
                              "\nPress E to convert NFA to DFA");
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
                            if (dfa.IsDeterministic())
                            {
                                Console.WriteLine("Press Q when you want to stop checking strings");
                                while (true)
                                {
                                    Console.WriteLine("Type in a string  to check it: ");
                                    var input = Console.ReadLine();
                                    if (input == "Q") break;
                                    Console.WriteLine(dfa.CheckString(input));
                                }
                            }

                            Console.WriteLine("Your automaton is not deterministic! Convert it");
                            break;
                        }
                    case "E":
                    {   
                        dfa.ConvertNFA();
                        break;
                    }
                }
            }
        }
    }
}
