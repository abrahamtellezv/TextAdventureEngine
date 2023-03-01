using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class Player
    {
        readonly HashSet<Item> _inventory;
        const int MaxCapacity = 100;
        public int _currentCapacity = 2;
        bool _verbose;
        readonly List<string> impossibleToTakeMessages = new List<string>() { "You can't be serious. ", "That's... imposible.", "You tried and tried, but alas, you failed to take it.", "As you thought just before trying, that couldn't be done." };

        public Player()
        {
            Item wood = new("a small piece of wood", "You don't know why, but it may be a good idea to hold on to it.", "", true, 2, new HashSet<string> { "wood", "piece of wood", "wood piece" });
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

        public void SetVerbose(string[] words, bool beVerbose)
        {
            if (words.Length > 1)
            {
                Console.Write("I don't understand that command");
                return;
            }
            _verbose = beVerbose;
            string text = _verbose ? "Long descriptions enabled." : "Short descriptions enabled.";
            Console.Write($"{text}\n\n\n> ");
        }

        public void CheckInventory()
        {
            Console.Write("Inventory:");
            foreach (var item in _inventory)
            {
                Console.Write($"\n  {TextParser.RemoveArticle(item.Name)}");
                if (item is ContainerItem containerItem && (item as ContainerItem).CurrentCapacity > 0)
                {
                    Console.Write($", which contains:");
                    foreach (Item itemInside in containerItem.Items)
                    {
                        Console.Write($"\n    {itemInside.Name}");
                    }
                }
            }
            Console.Write("\n\n\n> ");
        }

        public void Look(Room room, bool readDescription)
        {
            Console.Write($"{room.Name}");
            string text = string.Empty;
            if (readDescription || _verbose)
                text += $"{room.Description} ";
            foreach (var item in room.Items.Where(item => !item.HasBeenTaken).Skip(1))
            {
                text += $"{item.OriginalLocationDescription} ";
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                Console.Write("\n");
                TextWriter.Write(text);
            }
            foreach (var item in room.Items.Where(item => item.HasBeenTaken))
            {
                Console.Write($"\nThere's {item.Name} here.");
            }
            Console.Write("\n\n\n> ");
        }

        public void TakeItem(string keyword, Room room)
        {
            if (_inventory.Any(item => item.Keywords.Contains(keyword)))
            {
                Console.Write($"You already have the {keyword}.\n\n\n> ");
                return;
            }

            Item? itemToTake = room.Items.FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToTake == null)
            {
                Console.Write($"There's no {keyword} around.\n\n\n> ");
                return;
            }

            if (itemToTake.Weight > MaxCapacity)
            {
                Random random = new();
                Console.Write(impossibleToTakeMessages[random.Next(0,impossibleToTakeMessages.Count)]);
                Console.Write("\n\n\n> ");
                return;
            }

            if (!itemToTake.IsTakeable)
            {
                Console.Write($"You can't take the {keyword}.\n\n\n> ");
                return;
            }

            if (itemToTake is ContainerItem containerItemToTake)
            {
                if (_currentCapacity + containerItemToTake.Weight + containerItemToTake.CurrentCapacity > MaxCapacity)
                {
                    Console.Write($"You're carrying too much to take the {keyword}.\n\n\n> ");
                    return;
                }
            }
            else if (_currentCapacity + itemToTake.Weight > MaxCapacity)
            {
                Console.Write($"You're carrying too much to take the {keyword}.\n\n\n> ");
                return;
            }
            
            _inventory.Add(itemToTake);
            _currentCapacity += itemToTake.Weight;
            room.Items.Remove(itemToTake);
            itemToTake.HasBeenTaken = true;
            if (itemToTake is ContainerItem containerItem2)
            {
                _currentCapacity += containerItem2.CurrentCapacity;
            }
            Console.Write($"You took the {TextParser.RemoveArticle(itemToTake.Name)}.\n\n\n> ");
        }

        public void TakeAllItems(Room room)
        {
            if (room.Items.Count <= 1)
            {
                Console.Write("There's nothing to take here.\n\n\n> ");
                return;
            }
            string text = "You took";
            foreach (Item item in room.Items.Where(item => item.IsTakeable))
            {
                if (item is ContainerItem containerItem)
                {
                    if (_currentCapacity + containerItem.Weight + containerItem.CurrentCapacity <= MaxCapacity)
                    {
                        _inventory.Add(containerItem);
                        _currentCapacity += containerItem.Weight;
                        _currentCapacity += containerItem.CurrentCapacity;
                        room.Items.Remove(containerItem);
                        containerItem.HasBeenTaken = true;
                        text += $" the {TextParser.RemoveArticle(containerItem.Name)},";
                    }
                }
                else if (_currentCapacity + item.Weight <= MaxCapacity)
                {
                    _inventory.Add(item);
                    _currentCapacity += item.Weight;
                    room.Items.Remove(item);
                    item.HasBeenTaken = true;
                    text += $" the {TextParser.RemoveArticle(item.Name)},";
                }
            }
            if (!text.Contains(','))
            {
                Console.Write("You couldn't take anything.");
            }
            else
            {
                text = $"{text[..^1]}.";
                if (text.Contains(','))
                {
                    text = string.Concat(text.AsSpan(0, text.LastIndexOf(",")), " and", text.AsSpan(text.LastIndexOf(",") + 1));
                }
                TextWriter.Write(text);
            }
            Console.Write("\n\n\n> ");
            return;
        }

        public void DropItem(string keyword, Room room)
        {
            Item? itemToDrop = _inventory.FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToDrop == null)
            {
                Console.Write($"You don't have that.\n\n\n> ");
                return;
            }
            else
            {
                _inventory.Remove(itemToDrop);
                _currentCapacity -= itemToDrop.Weight;
                room.Items.Add(itemToDrop);
                Console.Write($"You drop the {TextParser.RemoveArticle(itemToDrop.Name)}.\n\n\n> ");
                if (itemToDrop is ContainerItem containerItem) 
                { 
                    _currentCapacity -= containerItem.CurrentCapacity;
                }
                return;
            }
        }

        public void ExamineItem(string keyword, Room room)
        {
            Item? itemToExamine =  _inventory.Concat(room.Items).FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToExamine == null)
            {
                Console.WriteLine($"There's no {keyword} to examine.\n\n\n> ");
                return;
            }
            
            TextWriter.Write(itemToExamine.ExamineDescription);
            if (itemToExamine is ContainerItem containerItem && (itemToExamine as ContainerItem).IsOpen && (itemToExamine as ContainerItem).CurrentCapacity > 0)
            {
                Console.Write($"\nThe {TextParser.RemoveArticle(itemToExamine.Name)} contains:\n");
                foreach (Item item in containerItem.Items)
                {
                    Console.Write($" - {item.Name}");
                }
            }

            Console.Write("\n\n\n> ");
            return;
        }

    }
}
