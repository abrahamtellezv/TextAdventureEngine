using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    class TextParser
    {
        static readonly string _errorMessage = "I don't understand that command.\n\n> ";

        public TextParser()
        {

        }

        public string[] PrepareText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Array.Empty<string>();
            }
            string[] words = input.Replace("the",null).ToLower().Split(' ');
            return words;
        }

        public void ParseText(string[] words, Player player, ref Room currentRoom)
        {
            if (words.Length == 0)
            {
                Console.Write(_errorMessage);
                return;
            }

            switch (words[0])
            {
                case "h":
                case "help":
                    if (words.Length == 1)
                        GiveHelp();
                    else
                        Console.Write(_errorMessage);
                    break;
                case "q":
                case "quit":
                    if (words.Length == 1)
                    {
                        Console.WriteLine("Goodbye...");
                        Environment.Exit(0);
                    }
                    else
                        Console.Write(_errorMessage);
                    break;
                case "l":
                    player.Look(currentRoom, readDescription: true);
                    break;
                case "look":
                    if (words.Length == 1 || (words.Length == 2 && words[1] == "around"))
                        player.Look(currentRoom, readDescription: true);
                    else
                        Console.Write(_errorMessage);
                    break;
                case "i":
                case "inventory":
                    player.CheckInventory();
                    break;
                case "take":
                    break;
                case "examine":
                case "u":
                case "d":
                case "n":
                case "e":
                case "s":
                case "w":
                case "ne":
                case "nw":
                case "se":
                case "sw":
                case "up":
                case "down":
                case "north":
                case "east":
                case "south":
                case "northeast":
                case "northwest":
                case "southeast":
                case "southwest":
                case "west":
                    if (words.Length > 1)
                        Console.WriteLine(_errorMessage);
                    else
                        currentRoom = player.ChangeRoom(words[0], currentRoom);
                    break;
                case "go":
                    if (words.Length == 1)
                        Console.Write("Go where?\n\n> ");
                    else
                        ParseText(words[1..], player, ref currentRoom);
                    break;
                default:
                    Console.Write(_errorMessage);
                    break;
            }
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
                "  q, quit:                 exit the game\n\n> ";
            Console.Write(help);
        }
    }
}
