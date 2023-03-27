using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    public class Surface
    {
        public string Name { get; private set; }
        public HashSet<string> Keywords { get; private set; }
        public int Weight { get; private set; }
        public string ExamineDescription { get; private set; }
        public HashSet<Item> Items { get; private set; }

        public Surface(string name, HashSet<string> keywords, string examineDescription, int weight)
        {
            Name = name;
            Keywords = keywords;
            ExamineDescription = examineDescription;
            Weight = weight;
            Items = new HashSet<Item>();
        }

        public void AddItems(params Item[] items)
        {
            foreach (Item item in items)
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }
    }
}
