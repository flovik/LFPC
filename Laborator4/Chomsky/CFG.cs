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
        private RemoveNull RemoveNull = new();
        private RemoveUnit RemoveUnit = new();
        private RemoveNonProductive RemoveNonProductive = new();
        private RemoveInaccessible RemoveInaccessible = new();
        private ChomskyNormalForm ChomskyNormalForm = new();

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
            CNF();
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
            while (RemoveNull.HasEpsilon(Transitions))
            {
                RemoveNull.GenerateEpsilonStates(Transitions);
                RemoveNull.AddNewEpsilonStatesToTransitions(Transitions);
            }
        }

        private void RemoveUnitTransitions()
        {
            //check if Transitions still have unit 
            while (RemoveUnit.HasUnit(Transitions))
            {
                RemoveUnit.AddUnits(Transitions);
                RemoveUnit.AddStates(Transitions);
            }
        }

        private void RemoveNonProductiveTransactions()
        {
            RemoveNonProductive.AddNonTerminals(Transitions);
            RemoveNonProductive.CheckTransitions(Transitions);
            RemoveNonProductive.RemoveOccurrencesKey(Transitions);
        }

        private void RemoveInaccessibleTransactions()
        {
            RemoveInaccessible.FindNonTerminals(Transitions);
            RemoveInaccessible.RemoveNonTerminals(Transitions);
        }

        private void CNF()
        {
           ChomskyNormalForm.NormalizeTerminalAndNonTerminal(Transitions);
           ChomskyNormalForm.NormalizeManySymbols(Transitions);
        }

    }
}
