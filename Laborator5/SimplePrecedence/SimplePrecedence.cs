using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePrecedence
{
    internal class SimplePrecedence
    {
        private Dictionary<string, List<string>> Transitions;
        private FirstLast FirstLast = new();
        public SimplePrecedence(Dictionary<string, List<string>> transitions)
        {
            Transitions = transitions;
        }

        private void PrintTransitions()
        {
            foreach (var (key, list) in Transitions)
            {
                Console.Write($"{key} -> ");
                for (int i = 0; i < list.Count; i++)
                {
                    if (i < list.Count - 1) Console.Write($"{list[i]} | ");
                    else Console.Write(list[i]);
                }

                Console.WriteLine();
            }

            Console.WriteLine("--------------");
        }

        public void Start()
        {
            FirstLast.Start(Transitions);
            FirstLast.Print(FirstLast.First);
            FirstLast.Print(FirstLast.Last);
        }
    }
}
