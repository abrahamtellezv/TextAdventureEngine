using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class Item
    {
        public string Name { get; set; }
        public string OriginalLocationDescription { get; set; }
        public string ItemDescription { get; set; }
        public Item(string name, string itemDescription, string originalLocationDescription)
        {
            Name = name;
            ItemDescription = itemDescription;
            OriginalLocationDescription = originalLocationDescription;
        }
    }
}
