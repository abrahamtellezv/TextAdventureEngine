using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class Room
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Dictionary<string, Room> Exits { get; private set; }
        public HashSet<Item> Items { get; private set; }
        public bool Visited { get; set; }

        public Room(string name, string description)
        {
            Name = name;
            Description = description;
            Exits = new Dictionary<string, Room>();
            Items = new HashSet<Item>() { };
            Item floor = new("floor", "Nothing remarkable about the ground here.", "", 999, new HashSet<string> { "floor", "ground" });
            AddItems(floor);
        } 

        public void AddExit(string direction, Room room)
        {
            Exits.Add(direction, room);
        }

        public void AddItems(params Item[] items)
        {
            foreach (Item item in items)
            {
                Items.Add(item);
            }
        }
    }
}
