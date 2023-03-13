using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
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
        readonly List<string> impossibleToTakeMessages = new() { "You can't be serious.", "That's... imposible.", "You tried and tried, but alas, you failed to take it.", "As you thought just before trying, that couldn't be done." };
        readonly List<string> implausibleToTakeMessages = new() { "That's just too heavy to be lugging around.", "That's... not a very good idea.", "Yeah... but why???", "Doing that would be terribly inconvenient." };

        public Player()
        {
            Item wood = new("a small piece of wood", new HashSet<string> { "wood", "piece of wood", "wood piece" }, "You don't know why, but it may be a good idea to hold on to it.", "", 2);
            wood.ShowFirstEncounterDescription = false;
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
                if (item is ContainerItem containerItem && containerItem.CurrentCapacity > 0)
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
            string roomDescriptionText = string.Empty;
            string otherItemsText = string.Empty;
            string containersText = string.Empty;
            if (readDescription || _verbose)
                roomDescriptionText += $"{room.Description} ";

            HashSet<Item> allItems = GetAllItemsToLookAt(room);
            foreach (Item item in allItems)
            {
                if (item.ShowFirstEncounterDescription)
                    roomDescriptionText += $"{item.FirstEncounterDescription} ";
                else
                    otherItemsText += $"\nThere's {item.Name} here.";
                if (item is ContainerItem container && container.IsOpen && container.CurrentCapacity > 0)
                {
                    containersText += $"\nThe {TextParser.RemoveArticle(item.Name)} contains:";
                    foreach (var itemInside in container.Items)
                        containersText += $"\n  {itemInside.Name}";
                }
            }
            if (!string.IsNullOrWhiteSpace(roomDescriptionText))
            {
                Console.Write("\n");
                TextWriter.Write(roomDescriptionText);
            }
            Console.Write(otherItemsText);
            Console.Write(containersText);
            Console.Write("\n\n\n> ");
        }

        private HashSet<Item> GetAllItemsToLookAt(Room room)
        {
            HashSet<Item> items = new(room.Items);
            foreach (var surface in room.Surfaces)
            {
                items.UnionWith(surface.Items);
            }
            return items;
        }

        private bool ItemIsASurface(string keyword, Room room)
        {
            Surface? surface = (from s in room.Surfaces
                                 where s.Keywords.Contains(keyword)
                                 select s).FirstOrDefault();

            if (surface == null)
                return false;

            Random random = new();
            if (surface.Weight == 999)
                Console.Write(impossibleToTakeMessages[random.Next(0, impossibleToTakeMessages.Count)]);
            else
                Console.Write(implausibleToTakeMessages[random.Next(0, implausibleToTakeMessages.Count)]);
            Console.Write("\n\n\n> ");
            return true;       
        }

        private bool ItemIsInInventory(string keyword)
        {
            if (_inventory.Any(item => item.Keywords.Contains(keyword)))
            {
                Console.Write($"You already have the {keyword}.\n\n\n> ");
                return true;
            }

            foreach (Item item in _inventory.Where(item => item is ContainerItem container && container.IsOpen && container.CurrentCapacity > 0))
            {
                ContainerItem container = (ContainerItem)item;
                Item? itemToTake = container.Items.FirstOrDefault(item => item.Keywords.Contains(keyword));
                if (itemToTake != null)
                {
                    _inventory.Add(itemToTake);
                    container.RemoveItem(itemToTake);
                    Console.Write($"You took the {TextParser.RemoveArticle(itemToTake.Name)}.\n\n\n> ");
                    return true;
                }
            }

            return false;
        }

        private bool ItemIsInsideContainer(string keyword, Room room)
        {
            ContainerItem? containerItem = null;
            Item? itemToTake = room.Items.FirstOrDefault(item => item.Keywords.Contains(keyword));
            foreach (var item in room.Items.Where(item => item is ContainerItem container && container.IsOpen && container.CurrentCapacity > 0))
            {
                containerItem = (ContainerItem)item;
                itemToTake = containerItem.Items.FirstOrDefault(item => item.Keywords.Contains(keyword));
                if (itemToTake != null && itemToTake.Weight + _currentCapacity <= MaxCapacity)
                {
                    _inventory.Add(itemToTake);
                    _currentCapacity += itemToTake.Weight;
                    containerItem.RemoveItem(itemToTake);
                    item.ShowFirstEncounterDescription = false;
                    Console.Write($"You took the {TextParser.RemoveArticle(itemToTake.Name)}.\n\n\n> ");
                    return true;
                }
                else if (itemToTake != null && itemToTake.Weight + _currentCapacity > MaxCapacity)
                {
                    Console.Write($"You're carrying too much to take the {keyword}.\n\n\n> ");
                    return true;
                }
            }
            return false;
        }

        private bool ItemIsOnSurface(string keyword, Room room)
        {
            Item? item = (from surface in room.Surfaces
                        from i in surface.Items
                        where i.Keywords.Contains(keyword)
                        select i).FirstOrDefault();

            if (item == null) return false;
            return true;
        }

        private bool CanYouTakeTheItem(Item item, string keyword)
        {
            if (item.Weight > MaxCapacity)
            {
                Random random = new();
                if (item.Weight == 999)
                    Console.Write(impossibleToTakeMessages[random.Next(0, impossibleToTakeMessages.Count)]);
                else
                    Console.Write(impossibleToTakeMessages[random.Next(0, implausibleToTakeMessages.Count)]);
                Console.Write("\n\n\n> ");
                return false;
            }

            if (item is ContainerItem containerItem)
            {
                if (_currentCapacity + containerItem.Weight + containerItem.CurrentCapacity > MaxCapacity)
                {
                    Console.Write($"You're carrying too much to take the {keyword}.\n\n\n> ");
                    return false;
                }
            }

            else if (_currentCapacity + item.Weight > MaxCapacity)
            {
                Console.Write($"You're carrying too much to take the {keyword}.\n\n\n> ");
                return false;
            }
            return true;
        }

        public void TryToTakeItem(string keyword, Room room)
        {
            if (ItemIsInInventory(keyword)) return;

            if (ItemIsASurface(keyword, room)) return;

            if (ItemIsOnSurface(keyword, room)) return;

            if (ItemIsInsideContainer(keyword, room)) return;
            
            Item? itemToTake = room.Items.FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToTake == null)
            {
                Console.Write($"There's no {keyword} around.\n\n\n> ");
                return;
            }

            if (!CanYouTakeTheItem(itemToTake, keyword)) return;

            TakeItem(itemToTake, room);
        }

        private void TakeItem(Item item, Room room)
        {
            _inventory.Add(item);
            _currentCapacity += item.Weight;
            room.Items.Remove(item);
            item.ShowFirstEncounterDescription = false;
            if (item is ContainerItem containerItem2)
            {
                _currentCapacity += containerItem2.CurrentCapacity;
            }
            Console.Write($"You took the {TextParser.RemoveArticle(item.Name)}.\n\n\n> ");
        }

        public void TryToTakeAllItems(Room room)
        {
            if (room.Items.Count < 1)
            {
                Console.Write("There's nothing to take here.\n\n\n> ");
                return;
            }
            string text = "You took";
            foreach (Item item in room.Items.Where(item => item.Weight < MaxCapacity))
            {
                if (item is ContainerItem containerItem)
                {
                    if (_currentCapacity + containerItem.Weight + containerItem.CurrentCapacity <= MaxCapacity)
                    {
                        _inventory.Add(containerItem);
                        _currentCapacity += containerItem.Weight;
                        _currentCapacity += containerItem.CurrentCapacity;
                        room.Items.Remove(containerItem);
                        containerItem.ShowFirstEncounterDescription = false;
                        text += $" the {TextParser.RemoveArticle(containerItem.Name)},";
                    }
                }
                else if (_currentCapacity + item.Weight <= MaxCapacity)
                {
                    _inventory.Add(item);
                    _currentCapacity += item.Weight;
                    room.Items.Remove(item);
                    item.ShowFirstEncounterDescription = false;
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

        public void PutItem(string[] items, Room room)
        {
            //probablemente podriamos buscar primero que existan los items en la habitacion (incluidos contenedores) o en el inventario
            //en caso que no pues ya ahi muere. 
            //si si estan 
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

        public void OpenOrCloseItem(string keyword, Room room, bool openItem)
        {
            string verb = openItem ? "open" : "close";
            string pastTenseVerb = openItem ? "opened" : "closed";
            Item? itemToOpen = _inventory.Concat(room.Items).FirstOrDefault(item => item.Keywords.Contains(keyword));
            if (itemToOpen == null)
            {
                Console.Write($"There's no {keyword} to {verb}.\n\n\n> ");
                return;
            }
            if (itemToOpen is not ContainerItem containerItem)
            {
                Console.Write($"You can't {verb} that.\n\n\n> ");
                return;
            }
            else if (!containerItem.CanBeClosed)
            {
                Console.Write($"The {keyword} can't be {pastTenseVerb}.\n\n\n> ");
                return;
            }
            containerItem.IsOpen = openItem;
            Console.Write($"You {pastTenseVerb} the {TextParser.RemoveArticle(containerItem.Name)}.\n\n\n> ");
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
