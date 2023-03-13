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
        public HashSet<Surface> Surfaces { get; private set; }
        public bool Visited { get; set; }

        public Room(string name, string description)
        {
            Name = name;
            Description = description;
            Exits = new Dictionary<string, Room>();
            Items = new HashSet<Item>() { };
            Surfaces = new HashSet<Surface>() { new("floor", new HashSet<string> { "floor", "ground" }, "Nothing remarkable about the ground here.", 999) };
        } 

        public void AddExit(string direction, Room room)
        {
            Exits.Add(direction, room);
        }

        public void AddSurface(Surface surface)
        {
            Surfaces.Add(surface);
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
