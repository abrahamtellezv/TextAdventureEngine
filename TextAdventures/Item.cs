using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class Item
    {
        public string Name { get; private set; }
        public HashSet<string> Keywords { get; private set; }
        public string ExamineDescription { get; private set; }
        public string FirstEncounterDescription { get; private set; }
        public int Weight { get; private set; }
        public bool ShowFirstEncounterDescription { get; set; } = true;

        public Item(string name, HashSet<string> keywords, string examineDescription, string firstEncounterDescription, int weight)
        {
            Name = name;
            Keywords = keywords;
            ExamineDescription = examineDescription;
            FirstEncounterDescription = firstEncounterDescription;
            Weight = weight;
        }
    }

    internal class ContainerItem : Item
    {
        public HashSet<Item> Items { get; private set; }

        public int MaxCapacity { get; private set; }

        public int CurrentCapacity { get; private set; }

        public bool IsOpen { get; set; }

        public bool CanBeClosed { get; private set; }

        public ContainerItem(string name, HashSet<string> keywords, string examineDescription, string firstEncounterDescription, int weight, int maxCapacity, int currentCapacity, bool isOpen, bool canBeClosed) : base(name, keywords, examineDescription, firstEncounterDescription, weight)
        {
            Items = new HashSet<Item>();
            MaxCapacity = maxCapacity;
            CurrentCapacity = currentCapacity;
            IsOpen = isOpen;
            CanBeClosed = canBeClosed;
        }

        public void AddItems(params Item[] items)
        {
            foreach (Item item in items)
            {
                if (item.Weight > MaxCapacity)
                    Console.Write($"The {TextParser.RemoveArticle(Name)} can't hold the {item.Name}.\n");
                else if (CurrentCapacity + item.Weight > MaxCapacity)
                    Console.Write($"There are too many things in the {TextParser.RemoveArticle(Name)} to fit the {TextParser.RemoveArticle(item.Name)}\n");
                else
                {
                    CurrentCapacity += item.Weight;
                    Items.Add(item);
                }
            }
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
            CurrentCapacity -= item.Weight;
        }
    }
}
