using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextAdventures
{
    class TextParser
    {
        static readonly string _incorrectCommand = "I don't understand that command.\n\n> ";

        public TextParser()
        {
        }

        public static string PrepareText(string input)
        {
            string pattern = @"\bthe\b";
            return Regex.Replace(input.ToLower(), pattern, "").Trim();
        }

        public void ParseText(string[] words, Player player, ref Room currentRoom)
        {
            switch (words[0])
            {
                case "test":
                    foreach (Item item in currentRoom.Items)
                    {
                        Console.WriteLine(item.Name);
                    }
                    break;
                case "h":
                case "help":
                    GiveHelp(words);
                    break;
                case "q":
                case "quit":
                    PromptQuit(words);
                    break;
                case "l":
                case "look":
                    HandleLooking(words, player, currentRoom);
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
                    HandleRoomChanging(words, player, ref currentRoom);
                    break;
                case "go":
                    if (words.Length == 1)
                        Console.Write("Go where?\n\n> ");
                    else
                        ParseText(words[1..], player, ref currentRoom);
                    break;
                case "r":
                case "restart":
                    PromptRestart(words);
                    break;
                case "brief":
                case "verbose":
                    player.SetVerbose(words, words[0] == "verbose");
                    break;
                case "t":
                case "take":
                    HandleTakingItem(words, player, currentRoom);
                    break;
                case "x":
                case "examine":
                    HandleExamining(words, player, currentRoom);
                    break;
                case "drop":
                    HandleDroppingItem(words, player, currentRoom);
                    break;
                default:
                    Console.Write(_incorrectCommand);
                    break;
            }
        }

        private static void HandleExamining(string[] words, Player player, Room currentRoom)
        {
            string item = RetrieveObjectFromInput(words, "Examine");
            if (string.IsNullOrWhiteSpace(item))
                return;
            player.ExamineItem(item, currentRoom);
        }

        private static void HandleTakingItem(string[] words, Player player, Room currentRoom)
        {
            string item = RetrieveObjectFromInput(words, "Take");
            if (string.IsNullOrWhiteSpace(item))
                return;
            player.TakeItem(item, currentRoom);
        }

        private static void HandleDroppingItem(string[] words, Player player, Room currentRoom)
        {
            string item = RetrieveObjectFromInput(words, "Drop");
            if (string.IsNullOrWhiteSpace(item))
                return;
            player.DropItem(item, currentRoom);
        }

        private static void GiveHelp(string[] words)
        {
            if (words.Length != 1)
            {
                Console.WriteLine(_incorrectCommand);
                return;
            }
            string help = "\nCommands I understand:\n" +
                "  h, help:                 provide brief instructions\n" +
                "  [d]irection,\n" +
                "  [direction],\n" +
                "  go [direction]:          move in one of ten possible directions: up, down,\n" +
                "                           north, west, southeast, etc.\n" +
                "  l, look, look around:    describe the room you're in and any objects in it\n" +
                "  take [item]:             take item from room or container if possible\n" +
                "  examine [item]:          get a description of said item\n" +
                "  i, inventory:            check your inventory\n" +
                "  verbose:                 describe rooms every time you enter\n" +
                "  brief:                   describe rooms only the first time you enter\n" +
                "  r, restart:              restart the game" +
                "  q, quit:                 exit the game\n\n";
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(help);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");
        }

        private static string RetrieveObjectFromInput(string[] words, string verb)
        {
            string? item = string.Empty;
            if (words.Length == 1)
            {
                Console.Write($"{verb} what?\n\n> ");
                item = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(item))
                    return "";
            }
            else
                foreach (string word in words.Skip(1))
                {
                    item += $"{word} ";
                }
            return PrepareText(item);
        }

        private static void HandleLooking(string[] words, Player player, Room currentRoom)
        {
            if (words.Length == 1 || (words.Length == 2 && words[1] == "around"))
                player.Look(currentRoom, readDescription: true);
            else
                Console.Write(_incorrectCommand);
        }

        private static void HandleRoomChanging(string[] words, Player player, ref Room currentRoom)
        {
            if (words.Length > 1)
            {
                Console.WriteLine(_incorrectCommand);
                return;
            }
            string direction;
            if (words[0].Length > 5)
                direction = $"{words[0][..1]}{words[0][5]}";
            else if (words[0].Length == 2 && words[0] != "up")
                direction = words[0];
            else
                direction = words[0][..1];
            currentRoom = player.ChangeRoom(direction, currentRoom);
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
                string? path = Environment.ProcessPath;
                if (string.IsNullOrWhiteSpace(path))
                {
                    Console.Write("There seems to be some problem restarting.\nIf you wish to restart, quit the game and start it again manually.\n\n> ");
                    return;
                }
                Process.Start(path);
                Environment.Exit(0);
            }
            else
                Console.Write("\n\n> ");
        }
    }
}
