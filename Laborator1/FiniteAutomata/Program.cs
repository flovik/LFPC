using System;

namespace FiniteAutomata
{
    class Program
    {
        static void Main(string[] args)
        {
            FiniteAutomata dfa = new FiniteAutomata();

            //read all the lines from the source txt file
            string[] lines = System.IO.File.ReadAllLines(@"D:\UTM\Anul 2\Semestrul 4\LFPC\Laborator1\FiniteAutomata\source.txt");

            for (int i = 0; i < lines.Length; i++)
            {   
                //go through each line and add the state to the automaton
                char from = lines[i][0];
                string transition;
                if (lines[i].Length == 4)
                {
                    transition = lines[i][2].ToString();
                    transition += lines[i][3];
                }
                else
                {
                    transition = lines[i][2].ToString();
                }

                dfa.AddState(from, transition);
            }

            Console.WriteLine("Press N if you want to stop testing the automaton \nPress S to see the states of the automaton \nPress W to check a string");
            while (true)
            {
                string userInput = Console.ReadLine();
                if (userInput == "N")
                {
                    break;
                }
                else if (userInput == "S")
                {
                    dfa.GetStates();
                }
                else if (userInput == "W")
                {
                    Console.WriteLine("Press Q when you want to stop checking strings");
                    while (true)
                    {
                        Console.WriteLine("Type in a string  to check it: ");
                        string input = System.Console.ReadLine();
                        if (input == "Q")
                        {
                            break;
                        }
                        Console.WriteLine(dfa.CheckString(input));
                    }
                }
            }

            
        }
    }
}
