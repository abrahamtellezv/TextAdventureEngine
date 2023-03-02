using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class Item
    {
        public HashSet<string> Keywords { get; private set; }
        public bool IsTakeable { get; set; }
        public bool HasBeenTaken { get; set; } = false;
        public int Weight { get; private set; }
        public string Name { get; private set; }
        public string OriginalLocationDescription { get; private set; }
        public string ExamineDescription { get; private set; }
        public Item(string inventoryName, string examineDescription, string originalLocationDescription, bool isTakeable, int weight, HashSet<string> keywords)
        {
            Name = inventoryName;
            ExamineDescription = examineDescription;
            OriginalLocationDescription = originalLocationDescription;
            IsTakeable = isTakeable;
            Weight = weight;
            Keywords = keywords;
        }
    }

    internal class ContainerItem : Item
    {
        public HashSet<Item> Items { get; private set; }

        public int MaxCapacity { get; private set; }

        public int CurrentCapacity { get; private set; }

        public bool IsOpen { get; set; }

        public bool CanBeClosed { get; private set; }

        public ContainerItem(string inventoryName, string examineDescription, string originalLocationDescription, bool isTakeable, int weight, HashSet<string> keywords, int maxCapacity, int currentCapacity, bool isOpen, bool canBeClosed) : base(inventoryName, examineDescription, originalLocationDescription, isTakeable, weight, keywords)
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
