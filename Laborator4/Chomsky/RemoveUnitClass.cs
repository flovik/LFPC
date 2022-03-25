using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chomsky
{
    internal class RemoveUnit
    {
        private Dictionary<string, List<string>> unitList;
        internal bool HasUnit(Dictionary<string, List<string>> transitions)
        {
            //checks if any state is of length 1 and is uppercase
            foreach (var (_, list) in transitions)
            {
                if (list.Any(state => state.Length == 1 && state.Any(char.IsUpper))) return true;
            }

            return false;
        }
        internal void AddUnits(Dictionary<string, List<string>> transitions)
        {
            unitList = new Dictionary<string, List<string>>(); //in case S -> A, S -> B
            foreach (var (key, list) in transitions)
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
        }

        internal void AddStates(Dictionary<string, List<string>> transitions)
        {
            foreach (var (unit, list) in unitList)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    //remove that unit from Transitions
                    //S -> B remove
                    transitions[unit].Remove(list[i]);

                    //S -> A, A -> ... many states, add all states from A to S
                    //for every outgoing state, add to the unit all the states
                    foreach (var state in transitions[list[i]]) //iterate the list of B
                    {
                        //don't add if we have duplicates
                        if (!transitions[unit].Contains(state))
                        {
                            transitions[unit].Add(state); //add in S all states of B
                        }
                    }
                }
            }
        }
    }
}
