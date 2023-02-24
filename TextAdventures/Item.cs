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
}
