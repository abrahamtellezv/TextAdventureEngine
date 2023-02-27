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
            Item wood = new("small piece of wood", "You don't know why, but it may be a good idea to hold on to it.", "", true, 2, new HashSet<string> { "wood", "piece of wood", "wood piece" });
            wood.HasBeenTaken = true;
            _inventory = new HashSet<Item>{ wood };
        }

        public Room ChangeRoom(string direction, Room currentRoom)
        {
            if (!currentRoom.Exits.ContainsKey(direction))
            {
                Console.Write("You can't go in that direction.\n\n> ");
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
                text += $"{room.Description}";
            foreach (var item in room.Items.Where(item => !item.HasBeenTaken))
            {
                text += $"{item.OriginalLocationDescription} ";
            }
            TextWriter.Write(text);
            Console.Write("\n");
            foreach (var item in room.Items.Where(item => item.HasBeenTaken))
            {
                Console.WriteLine($"There's a {item.Name} here.");
            }
            Console.Write("\n> ");
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
            if (keyword == "all")
            {
                if (room.Items.Count <= 1)
                {
                    Console.Write("There's nothing to take here.");
                    return;
                }
                string text = "You took";
                foreach (Item item in room.Items.Where(item => item.IsTakeable))
                {
                    if (_currentWeight + item.Weight <= MaxWeight)
                    {
                        _inventory.Add(item);
                        _currentWeight += item.Weight;
                        room.Items.Remove(item);
                        item.HasBeenTaken = true;
                        text += $" the {item.Name},";
                    }
                }
                if (text.IndexOf(',') != -1) 
                {
                    TextWriter.Write($"{text.Substring(0,text.Length - 1)}.");
                    Console.Write("\n\n> ");
                }
                else
                {
                    Console.Write("You couldn't take anything\n\n> ");
                }
                return;
            }

            if (_inventory.Any(item => item.Keywords.Contains(keyword)))
            {
                Console.Write($"You already have the {keyword}.\n\n> ");
                return;
            }

            Item? itemToTake = room.Items.FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToTake == null)
            {
                Console.Write($"There's no {keyword} around.\n\n> ");
                return;
            }

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
                itemToTake.HasBeenTaken = true;
                Console.Write($"You took the {itemToTake.Name}.\n\n> ");
                return;
            }
        }

        public void DropItem(string keyword, Room room)
        {
            Item? itemToDrop = _inventory.FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToDrop == null)
            {
                Console.Write($"You don't have that.\n\n> ");
                return;
            }
            else
            {
                _inventory.Remove(itemToDrop);
                _currentWeight -= itemToDrop.Weight;
                room.Items.Add(itemToDrop);
                Console.Write($"You drop the {itemToDrop.Name}.\n\n> ");
                return;
            }
        }

        public void ExamineItem(string keyword, Room room)
        {
            Item? itemToExamine =  _inventory.Concat(room.Items).FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToExamine == null)
            {
                Console.WriteLine($"There's no {keyword} to examine.");
                return;
            }
            
            TextWriter.Write(itemToExamine.ExamineDescription);
            Console.Write("\n\n> ");
            return;
        }

    }
}
