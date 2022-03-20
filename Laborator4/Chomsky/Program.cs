using System;
using System.ComponentModel;
using System.Text;

namespace Chomsky
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            var lines = System.IO.File.ReadAllLines(@"D:\UTM\Anul 2\Semestrul 4\LFPC\Laborator4\Chomsky\source2.txt");
            var transitions = Initialize(lines);
            var cfg = new CFG(transitions);
            cfg.ConvertCfGtoCnf();
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