using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdventures
{
    internal class TextWriter
    {
        const int MaxCharacters = 80;
        public static void Write(string input)
        {
            // This function breaks long strings into shorter ones for easier reading, so short strings are still written with Console
            string[] words = input.Split(' ');
            string line = string.Empty;
            int index = 0;
            while (index < words.Length)
            {
                if (line.Length + words[index].Length >= MaxCharacters)
                {
                    Console.WriteLine(line);
                    line = $"{words[index]} ";
                }
                else
                {
                    line += $"{words[index]} ";
                }
                index++;
            }
            Console.Write($"{line.Trim()}");
        }
    }
}
