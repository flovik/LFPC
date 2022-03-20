using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class CFG
    {
        private Dictionary<string, List<string>> Transitions;
        public CFG(Dictionary<string, List<string>> transitions)
        {
            Transitions = transitions;
        }

        public void ConvertCfGtoCnf()
        {
            PrintTransitions();
            RemoveNullTransitions();
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
        }

        private void RemoveNullTransitions()
        {

        }
    }
}
