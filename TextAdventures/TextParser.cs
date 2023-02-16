using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    class TextParser
    {
        static readonly string _incorrectCommand = "I don't understand that command.\n\n> ";
        static readonly string _noCommand = "No command was given.\n\n> ";

        public TextParser()
        {

        }

        public string[] PrepareText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Array.Empty<string>();
            }
            string[] words = input.ToLower().Replace(" the "," ").Split(' ');
            return words;
        }

        public void ParseText(string[] words, Player player, ref Room currentRoom)
        {
            if (words.Length == 0)
            {
                Console.Write(_noCommand);
                return;
            }

            switch (words[0])
            {
                case "h":
                case "help":
                    if (words.Length == 1)
                        GiveHelp();
                    else
                        Console.Write(_incorrectCommand);
                    break;
                case "q":
                case "quit":
                    PromptQuit(words);
                    break;
                case "l":
                    player.Look(currentRoom, readDescription: true);
                    break;
                case "look":
                    if (words.Length == 1 || (words.Length == 2 && words[1] == "around"))
                        player.Look(currentRoom, readDescription: true);
                    else
                        Console.Write(_incorrectCommand);
                    break;
                case "i":
                case "inventory":
                    player.CheckInventory();
                    break;
                case "u":
                case "d":
                case "n":
                case "e":
                case "s":
                case "w":
                case "up":
                case "down":
                case "north":
                case "east":
                case "south":
                case "west":
                case "ne":
                case "nw":
                case "se":
                case "sw":
                case "northeast":
                case "northwest":
                case "southeast":
                case "southwest":
                    if (words.Length > 1)
                        Console.WriteLine(_incorrectCommand);
                    else
                    HandleRoomChanging(words, player, ref currentRoom);
                    break;
                case "go":
                    if (words.Length == 1)
                        Console.Write("Go where?\n\n> ");
                    else
                        ParseText(words[1..], player, ref currentRoom);
                    break;
                case "t":
                case "take":
                    HandleItemTaking(words, player, currentRoom);
                    break;
                case "r":
                case "restart":
                    PromptRestart(words);
                    break;
                default:
                    Console.Write(_incorrectCommand);
                    break;
            }
        }

        private static void PromptQuit(string[] words)
        {
            if (words.Length > 1)
            {
                Console.WriteLine(_incorrectCommand);
                return;
            }
            Console.Write("Are you sure you want to quit? Press 'y' to do so.\n> ");
            string? answer = Console.ReadLine();
            if (answer == "Y" || answer == "y")
            {
                Console.Write("Goodbye.\n\n\n\n\n");
                Environment.Exit(0);
            }
            else
                Console.Write("\n\n> ");
        }

        private static void PromptRestart(string[] words)
        {
            if (words.Length > 1)
            {
                Console.WriteLine(_incorrectCommand);
                return;
            }
            Console.Write("Are you sure you want to restart? Press 'y' to do so.\n> ");
            string? answer = Console.ReadLine();
            if (answer == "Y" || answer == "y")
            {
                Console.Clear();
                Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                Environment.Exit(0);
            }
            else
                Console.Write("\n\n> ");
        }

        private static void HandleItemTaking(string[] words, Player player, Room currentRoom)
        {
            string? item = string.Empty;
            if (words.Length == 1)
            {
                Console.Write("Take what?\n\n> ");
                item = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(item))
                {
                    Console.Write(_incorrectCommand);
                    return;
                }
                item = item.ToLower().Trim();
                if (item[..4] == "the ")
                    item = item[4..];
            }
            else
                foreach (string word in words.Skip(1))
                {
                    item += $"{word} ";
                }
            player.TakeItem(item.Trim(), currentRoom);
        }

        private static void HandleRoomChanging(string[] words, Player player, ref Room currentRoom)
        {
            string direction;
            if (words[0].Length > 5)
                direction = $"{words[0][..1]}{words[0][5]}";
            else if (words[0].Length == 2 && words[0] != "up")
                direction = words[0];
            else
                direction = words[0][..1];
            currentRoom = player.ChangeRoom(direction, currentRoom);
        }

        private static void GiveHelp()
        {
            string help = "Commands I understand:\n" +
                "  h, help:                 provide brief instructions\n" +
                "  [d]irection,\n" +
                "  [direction],\n" +
                "  go [direction]:          move in one of ten possible directions (up, down, north, west, southeast...)\n" +
                "  l, look, look around:    look around the room, describing it and any objects in it\n" +
                "  take [item]:             take item from room if possible\n" +
                "  examine [item]:          get a description of said item\n" +
                "  i, inventory:            check your inventory\n" +
                "  q, quit:                 exit the game\n\n";
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(help);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");
        }
    }
}
