using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
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
        static readonly string _incorrectCommand = "I don't understand that command.\n\n\n> ";

        public TextParser()
        {
        }

        public static string PrepareText(string input)
        {
            string pattern = @"\bthe\b";
            return Regex.Replace(input.ToLower(), pattern, "").Trim();
        }

        public static string RemoveArticle(string input)
        {
            return string.Join(" ", (input.Split(' ')).Skip(1));
        }

        public void ParseText(string[] words, Player player, ref Room currentRoom)
        {
            switch (words[0])
            {
                case "test":
                    Console.WriteLine(player._currentCapacity);
                    break;
                case "help":
                case "h":
                    GiveHelp(words);
                    break;
                case "detailed":
                    GiveDetailedHelp(words);
                    break;
                case "exit":
                case "quit":
                case "q":
                    PromptQuit(words);
                    break;
                case "look":
                case "l":
                    HandleLooking(words, player, currentRoom);
                    break;
                case "inventory":
                case "i":
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
                        Console.Write("Go where?\n> ");
                    else
                        ParseText(words[1..], player, ref currentRoom);
                    break;
                case "restart":
                case "r":
                    PromptRestart(words);
                    break;
                case "brief":
                case "verbose":
                    player.SetVerbose(words, words[0] == "verbose");
                    break;
                case "take":
                case "t":
                    HandleTakingItem(words, player, currentRoom);
                    break;
                case "examine":
                case "x":
                    HandleExamining(words, player, currentRoom);
                    break;
                case "drop":
                    HandleDroppingItem(words, player, currentRoom);
                    break;
                case "open":
                case "close":
                    HandleOpeningAndClosingItem(words, player, currentRoom);
                    break;
                default:
                    Console.Write(_incorrectCommand);
                    break;
            }
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
                "  detailed help:           provide extended instructions\n" +
                "  [d]irection,\n" +
                "  [direction],\n" +
                "  go [direction]:          move in one of ten possible directions: up, down,\n" +
                "                           north, west, southeast, etc.\n" +
                "  l, look, look around:    describe the room you're in and any objects in it\n" +
                "  take [item]:             take item from room or container if possible\n" +
                "  examine [item]:          get a description of said item\n" +
                "  i, inventory:            check your inventory\n" +
                "  r, restart:              restart the game\n" +
                "  q, quit:                 exit the game\n\n\n";
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(help);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("> ");
        }

        private static void GiveDetailedHelp(string[] words)
        {
            if (!(words.Length == 2 && words[1] == "help")) 
            {
                Console.WriteLine(_incorrectCommand);
                return;
            }
            string detailedHelp =
                "\n - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - \n\n" +
                "Here's a the complete list of commands I understand:\n\n" +
                " help, h:\n" +
                "       Provide a shortened set of instructions.\n\n" +
                " detailed help:\n" +
                "       Provide comprehensive instructions on all possible commands.\n\n" +
                " go [direction], [direction], [d]irection:\n" +
                "       Move in one of ten possible directions: up, down, east, south, north,\n" +
                "       west, southeast, northeast, northwest and southwest. You can use\n" +
                "       abbreviations to travel faster: u, d, n, w, ne, sw...\n\n" +
                " look around, look, l:\n" +
                "       Reads you the description of the room and enlists all the objects\n" +
                "       in the room that can be taken.\n\n" +
                " take [item], t [item]:\n" +
                "       Take's the specified item from the room or container if possible.\n" +
                "       Type 'take all' to try to pick every available item in the room.\n" +
                "       Keep in mind that you can only carry so many things at a time.\n\n" +
                " examine [item], x [item]:\n" +
                "       Get a detailed description of the specified item. Rooms may have some\n" +
                "       non-takeable items that can be examined.\n\n" +
                " drop [item]:\n" +
                "       Drop the specified item from your inventory.\n\n" +
                " inventory, i:\n" +
                "       Gives a list of all the items you are currently holding.\n\n" +
                " brief:\n" +
                "       Describes rooms in detail only the first time you enter them.\n" +
                "       This is the default description mode.\n\n" +
                " verbose:\n" +
                "       Describes rooms in full every time you enter them.\n\n" +
                " restart, r:\n" +
                "       Promts you to restart the game.\n\n" +
                " quit, q:\n" +
                "       Prompts you to quit the game.\n\n" +
                " - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - \n\n\n";
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(detailedHelp);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("> ");
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
            if (string.IsNullOrWhiteSpace(item)) return;

            if (item == "all")
                player.TakeAllItems(currentRoom);
            else
                player.TakeItem(item, currentRoom);
        }

        private static void HandleDroppingItem(string[] words, Player player, Room currentRoom)
        {
            string item = RetrieveObjectFromInput(words, "Drop");
            if (string.IsNullOrWhiteSpace(item)) return;

            player.DropItem(item, currentRoom);
        }

        private static void HandleOpeningAndClosingItem(string[] words, Player player, Room currentRoom)
        {
            string verb = words[0] == "open" ? "Open" : "Close";
            string item = RetrieveObjectFromInput(words, verb);
            if (string.IsNullOrWhiteSpace(item)) return;

            player.OpenOrCloseItem(item, currentRoom, words[0] == "open");
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
                Console.Write("Ok.\n\n\n> ");
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
                Console.Write("Ok.\n\n\n> ");
        }

        private static string RetrieveObjectFromInput(string[] words, string verb)
        {
            string? item = string.Empty;
            if (words.Length == 1)
            {
                Console.Write($"{verb} what?\n> ");
                item = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(item))
                {
                    Console.Write($"You {verb.ToLower()} nothing.\n\n\n> ");
                    return "";
                }
            }
            else
                foreach (string word in words.Skip(1))
                {
                    item += $"{word} ";
                }
            return PrepareText(item);
        }
    }
}
