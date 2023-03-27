using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    public class Item
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

    public class ContainerItem : Item
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
                string container = string.Join(" ", (Name.Split(' ')).Skip(1));
                string itemToStore = string.Join(" ", (item.Name.Split(' ')).Skip(1));
                if (item.Weight > MaxCapacity)
                    Console.Write($"The {container} can't hold the {itemToStore}.\n");
                else if (CurrentCapacity + item.Weight > MaxCapacity)
                    Console.Write($"There are too many things in the {container} to fit the {itemToStore}\n");
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
