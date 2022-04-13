using System;
using System.Text;

namespace SimplePrecedence
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            var lines = System.IO.File.ReadAllLines(@"D:\UTM\Anul 2\Semestrul 4\LFPC\Laborator5\SimplePrecedence\source.txt");
            var terminals = lines[0].Split(',').ToList();
            var nonTerminals = lines[1].Split(',').ToList();
            var transitions = Initialize(lines[2..]);
            var spp = new SimplePrecedence(transitions, terminals, nonTerminals);
            //spp.Start();
            //spp.CheckString("adabcd");
            //spp.CheckString("adbbecadbe");
            //spp.CheckString("adbecd");
            //spp.CheckString("adAecB");
            //spp.CheckString("adAeca");
            //spp.CheckString("DcbbbecC");

            Console.WriteLine("N - stop program");
            Console.WriteLine("M - print matrix");
            Console.WriteLine("T - print transitions");
            Console.WriteLine("F - print First/Last Set");
            Console.WriteLine("S - convert grammar to Simple Precedence");
            Console.WriteLine("Q - check string");

            while (true)
            {
                var userInput = Console.ReadLine();
                if (userInput == "N") break;

                switch (userInput)
                {
                    case "M":
                        spp.PrintMatrix();
                        break;
                    case "T":
                        spp.PrintTransitions();
                        break;
                    case "F":
                        spp.PrintFirstAndLast();
                        break;
                    case "S":
                        spp.Start();
                        break;
                    case "Q":
                        {
                            Console.WriteLine("Press Q when you want to stop checking strings");
                            while (true)
                            {
                                Console.WriteLine("Type in a string  to check it: ");
                                var input = Console.ReadLine();
                                if (input == null) continue;
                                if (input == "Q") break;
                                spp.CheckString(input);
                            }

                            break;
                        }
                }
            }
        }

        static Dictionary<string, List<string>> Initialize(string[] path)
        {
            //adds grammar from file to dictionary of arrays
            var transitions = new Dictionary<string, List<string>>();
            foreach (var line in path)
            {
                if (!transitions.ContainsKey(line[0].ToString()))
                {
                    transitions.Add(line[0].ToString(), new List<string>());
                    AddTransition(line);
                }
                else
                {
                    AddTransition(line);
                }
            }

            void AddTransition(string line)
            {
                string substr = line.Substring(line.IndexOf('>') + 1, line.Length - 2);
                transitions[line[0].ToString()].Add(substr);
            }

            return transitions;
        }
    }
}