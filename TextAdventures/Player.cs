using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class Player
    {
        readonly Dictionary<string, Item> _inventory;
        bool _verbose;

        public Player()
        {
            _inventory = new Dictionary<string, Item>
                {
                    { "wood", new Item("small piece of wood", "You don't know why, but it may be a good idea to hold on to it.", "", true) }
                };
        }

        public Room ChangeRoom(string direction, Room currentRoom)
        {
            if (!currentRoom.Exits.ContainsKey(direction))
            {
                Console.Write("You can't go in that direction.\n\n> ");
                Console.Title = currentRoom.Name;
                return currentRoom;
            }
            Room newRoom = currentRoom.Exits[direction];
            Look(newRoom, readDescription: !newRoom.Visited);
            newRoom.Visited = true;
            Console.Title = newRoom.Name;
            return newRoom;
        }

        public void CheckInventory()
        {
            Console.WriteLine("Inventory:");
            foreach (var item in _inventory)
            {
                Console.WriteLine($"- {item.Value.Name}");
            }
            Console.Write("\n\n> ");
        }

        public void Look(Room room, bool readDescription)
        {
            Console.WriteLine($"{room.Name}");
            string text = string.Empty;
            if (readDescription || _verbose)
                text += $"{room.Description} ";
            foreach (var item in room.Items)
                text += $"{item.Value.OriginalLocationDescription} ";
            TextWriter.Write(text);
            Console.Write("\n\n> ");
        }

        public void SetVerbose(string[] words, bool beVerbose)
        {
            if (words.Length > 1)
            {
                Console.Write("I don't understand that command");
                return;
            }
            _verbose = beVerbose;
            string text = _verbose ? "Long descriptions enabled." : "Short descriptions enabled.";
            Console.Write($"{text}\n\n> ");
        }

        public void TakeItem(string item, Room room)
        {
            if (_inventory.ContainsKey(item))
            {
                Console.Write($"You already have the {item}.\n\n> ");
                return;
            }
            if (!room.Items.ContainsKey(item))
            {
                Console.Write($"There's no '{item}' here.\n\n> ");
                return;
            }
            if (!room.Items[item].IsTakeable)
            {
                Console.Write($"You can't take the {item}.\n\n> ");
                return;
            }

            _inventory.Add(item, room.Items[item]);
            room.Items.Remove(item);
            Console.Write($"You took the {item}.\n\n> ");
        }

        public void ExamineItem(string item, Room room)
        {
            if (_inventory.ContainsKey(item))
            {
                TextWriter.Write(_inventory[item].ItemDescription);
                Console.Write("\n\n> ");
                return;
            }
            else if (room.Items.ContainsKey(item))
            {
                TextWriter.Write(room.Items[item].ItemDescription);
                Console.Write("\n\n> ");
                return;
            }
            Console.Write($"There's no {item} to examine.\n\n> ");
        }

    }
}
