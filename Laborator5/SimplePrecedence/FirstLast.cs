using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePrecedence
{
    internal class FirstLast
    {
        public Dictionary<char, List<char>> First = new();
        public Dictionary<char, List<char>> Last = new();
        private Dictionary<string, List<string>> _transitions;

        public void Start(Dictionary<string, List<string>> transitions)
        {
            _transitions = transitions;
            PopulateFirstSet();
            PopulateLastSet();
            AddStates(First);
            AddStates(Last);
        }

        private void PopulateFirstSet()
        {
            foreach (var (key, list) in _transitions)
            {
                foreach (var word in list)
                {
                    if (First.ContainsKey(key[0]))
                    {
                        //handle duplicatess
                        if (!First[key[0]].Contains(word[0])) First[key[0]].Add(word[0]);
                    }
                    else
                    {
                        First.Add(key[0], new List<char> {word[0]});
                    }

                }
            }

        }

        private void PopulateLastSet()
        {
            foreach (var (key, list) in _transitions)
            {
                foreach (var word in list)
                {
                    if (Last.ContainsKey(key[0]))
                    {
                        //handle duplicatess
                        if (!Last[key[0]].Contains(word[^1])) Last[key[0]].Add(word[^1]);
                    }
                    else
                    {
                        Last.Add(key[0], new List<char> {word[^1]});
                    }
                }
            }
        }

        private void AddStates(Dictionary<char, List<char>> map)
        {
            int iteration = 1;
            while (iteration > 0)
            {
                foreach (var (key, list) in map)
                {
                    foreach (var character in list.ToList())
                    {
                        //check if char we want to iterate is not the same as key and if it is an upper
                        if (!key.Equals(character) && char.IsUpper(character))
                        {
                            foreach (var tempChar in map[character])
                            {
                                if (!map[key].Contains(tempChar))
                                {
                                    map[key].Add(tempChar);
                                    if (char.IsUpper(tempChar)) iteration++;
                                }
                            }
                        }
                    }
                }

                iteration--;
            }
        }

        public void Print(Dictionary<char, List<char>> map)
        {
            foreach (var (key, list) in map)
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
    }
}

