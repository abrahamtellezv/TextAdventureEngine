﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class Player
    {
        readonly Dictionary<string, Item> _inventory;

        public Player()
        {
            _inventory = new Dictionary<string, Item>
                {
                    { "wood", new Item("small piece of wood", "You don't know why, but it may be a good idea to hold on to it", "") }
                };
        }

        public void CheckInventory()
        {
            Console.WriteLine("Inventory:");
            foreach (var item in _inventory)
            {
                Console.WriteLine($"A {item.Value.Name}");
            }
            Console.Write("\n\n> ");
        }

        public void ExamineItem(string item)
        {
            Console.WriteLine($"{_inventory[item].ItemDescription}");
            Console.Write("\n\n> ");
        }

        public void TakeItem(string item, Room room)
        {
            if (!room.Items.ContainsKey(item))
            {
                Console.WriteLine($"There's no {item} here.");
                return;
            }

            _inventory.Add(item, room.Items[item]);
            room.Items.Remove(item);
        }

        public void Look(Room room, bool readDescription)
        {
            Console.WriteLine($"{room.Name}");
            if (readDescription)
                Console.Write($"{room.Description}");
            foreach (var item in room.Items) 
            {
                Console.Write($"{item.Value.OriginalLocationDescription}. ");
            }
            Console.Write("\n\n> ");
        }

        public Room ChangeRoom(string direction, Room currentRoom)
        {
            if (direction.Length > 5)
                direction = direction[..2];
            else if (direction.Length > 2)
                direction = direction[..1];

            if (!currentRoom.Exits.ContainsKey(direction))
            {
                Console.Write("You can't go in that direction.\n\n> ");
                Console.Title = currentRoom.Name;
                return currentRoom;
            }
            Room newRoom = currentRoom.Exits[direction];
            Look(newRoom, readDescription: !newRoom.Visited);
            if (!newRoom.Visited)
                newRoom.Visited = true;
            Console.Title = newRoom.Name;
            return newRoom;
        }
    }
}