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
        readonly HashSet<Item> _inventory;
        const int MaxWeight = 100;
        int _currentWeight = 2;
        bool _verbose;

        public Player()
        {
            _inventory = new HashSet<Item>{ new Item("small piece of wood", "You don't know why, but it may be a good idea to hold on to it.", "", true, 2, new HashSet<string>{ "wood", "piece of wood", "wood piece"}) };
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
                Console.WriteLine($"- {item.Name}");
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
            {
                if (item.OriginalLocationDescription != "")
                    text += $"{item.OriginalLocationDescription} ";
            }
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

        public void TakeItem(string keyword, Room room)
        {
            if (_inventory.Any(item => item.Keywords.Contains(keyword)))
            {
                Console.Write($"You already have the {keyword}.\n\n> ");
                return;
            }

            Item? itemToTake = room.Items.FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToTake != null)
            {
                if (!itemToTake.IsTakeable)
                {
                    Console.Write($"You can't take the {keyword}.\n\n> ");
                    return;
                }
                else if (_currentWeight + itemToTake.Weight > MaxWeight)
                {
                    Console.Write($"You're carrying too much weight to take the {itemToTake.Name}.\n\n> ");
                    return;
                }
                else
                {
                    _inventory.Add(itemToTake);
                    _currentWeight += itemToTake.Weight;
                    room.Items.Remove(itemToTake);
                    Console.Write($"You took the {itemToTake.Name}.\n\n> ");
                    return;
                }
            }
            else
                Console.Write($"There's no {keyword} around.\n\n> ");
        }

        public void ExamineItem(string keyword, Room room)
        {
            foreach (Item item in _inventory.Concat(room.Items))
            {
                if (item.Keywords.Contains(keyword))
                {
                    TextWriter.Write(item.ExamineDescription);
                    Console.Write("\n\n> ");
                    return;
                }
            }
            Console.WriteLine($"There's no {keyword} to examine.");
        }

    }
}
