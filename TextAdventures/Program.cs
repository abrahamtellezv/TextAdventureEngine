using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace TextAdventures
{
    internal class Program
    {
        static void Main()
        {
            Player player = new();
            TextParser parser = new();
            Console.Title = "Hello...";
            Dictionary<string, Room> rooms = InitializeRooms();
            Room currentRoom = rooms["outside"];
            Console.Write("Welcome to the text adventure game!");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("    (Type \"help\" for instructions)\n\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("You stand before the slightly ajar door at 320 Real de Montes Urales. While it doesn't really seem that inviting, the chilly air is telling you to go inside.\n\n> ");
            while (true)
            {
                string[] words = parser.PrepareText(Console.ReadLine());
                parser.ParseText(words, player, ref currentRoom);
                //ParseText(Console.ReadLine(), player, ref currentRoom);
            }
        }



        private static Dictionary<string, Room> InitializeRooms()
        {
            Dictionary<string, Room> rooms = new();
            Room outside = new("Outside", "You're outside the house, it's too dark to go wandering any further, you should go back inside.");
            rooms.Add("outside", outside);
            Room entrance = new("Entrance", "The entrance to the house, you can see one of my threaded rainbow pieces here. There's a flight of stairs leading up to the study, while a small flight leads down to a bathroom. The living room lies to the north. To the east there's the kitchen.");
            rooms.Add("entrance", entrance);
            Room downstairsBathroom = new("Downstairs bathroom", "This small bathroom was, somehow, one of the best rooms in the house. The stairs lead up to the entrance.");
            rooms.Add("downstairsBathroom", downstairsBathroom);
            Room garden = new("Garden", "Somehow the grass is still green in some patches, sadly, the potted plants and the trees have all died. The living and dining rooms lie to the southwest and southeast respectively");
            rooms.Add("garden", garden);
            Room kitchen = new("Kitchen", "You can smell the years of cooking that have happened here. The entrance lies to the west. North is the dining room. The laundry room can be seen to the south.");
            rooms.Add("kitchen", kitchen);
            Room laundryRoom = new("Laundry room", "The trusty old washing machine takes most of the space here. The kitchen is to the north. A door leads to the service area to the south.");
            rooms.Add("laundry room", laundryRoom);
            Room livingRoom = new("Living room", "The couch has definitely seen better times and the tv doesn't seem to work anymore. You can see the dining room to the east and the entrance to the south.");
            rooms.Add("living room", livingRoom);
            Room diningRoom = new("Dinning room", "There's food on the table, not that you should eat it, who knows how long it's been there. Walking west leads to the living room. The kitchen is to the south.");
            rooms.Add("dining room", diningRoom);
            Room serviceArea = new("Service area", "There's a lot of junk here. The laundry room is up north.");
            rooms.Add("service area", serviceArea);
            Room study = new("Study", "There's so many things on the desks it looks like a stationary shop. At least dusty and messy one. There's a bathroom to the north. You can see a faint green hue coming from the door to the east. Your bedroom was behind the eastern door. There's a door to the balcony south, next to the stairs leading down to the entrance.");
            rooms.Add("study", study);
            Room balcony = new("Balcony", "Normally you'd be able to see the small park in front of the house, but for some reason, it's too dark to see right now.");
            rooms.Add("balcony", balcony);
            Room bedroom = new("Bedroom", "The unmade bed has the lingering smell of the three of us...");
            rooms.Add("bedroom", bedroom);
            Room bigBedroom = new("Big bedroom", "She had all sorts of stuff all over the place, she left it that way.");
            rooms.Add("big bedroom", bigBedroom);
            Room bathroom = new("Bathroom", "Still steamy... somehow...");
            rooms.Add("bathroom", bathroom);

            outside.Exits.Add("n", entrance);
            entrance.Exits.Add("s", outside);
            entrance.Exits.Add("u", study);
            entrance.Exits.Add("n", livingRoom);
            entrance.Exits.Add("e", kitchen);
            entrance.Exits.Add("d", downstairsBathroom);
            downstairsBathroom.Exits.Add("u", entrance);
            livingRoom.Exits.Add("s", entrance);
            livingRoom.Exits.Add("e", diningRoom);
            livingRoom.Exits.Add("n", garden);
            diningRoom.Exits.Add("w", livingRoom);
            diningRoom.Exits.Add("s", kitchen);
            diningRoom.Exits.Add("n", garden);
            garden.Exits.Add("sw", livingRoom);
            garden.Exits.Add("se", diningRoom);
            kitchen.Exits.Add("n", diningRoom);
            kitchen.Exits.Add("w", entrance);
            kitchen.Exits.Add("s", laundryRoom);
            laundryRoom.Exits.Add("n", kitchen);
            laundryRoom.Exits.Add("s", serviceArea);
            serviceArea.Exits.Add("n", laundryRoom);
            study.Exits.Add("d", entrance);
            study.Exits.Add("n", bathroom);
            study.Exits.Add("e", bedroom);
            study.Exits.Add("s", balcony);
            study.Exits.Add("w", bigBedroom);
            bigBedroom.Exits.Add("e", study);
            balcony.Exits.Add("n", study);
            bedroom.Exits.Add("w", study);
            bathroom.Exits.Add("s", study);

            entrance.Items.Add("keys", new Item("keys", "A set of keys, to the front door and who know what else", "There's a set of keys on the white table next to the door"));

            return rooms;
        }
        //public static void ParseText(string? input, Player player, ref Room currentRoom)
        //{
        //    string errorMessage = "I don't understand that command.\n\n> ";

        //    if (string.IsNullOrWhiteSpace(input))
        //    {
        //        Console.Write(errorMessage);
        //        return;
        //    }

        //    string abridgedInput = input.Replace("the", null).ToLower();
        //    string[] words = abridgedInput.Split(' ');
        //    switch (words[0])
        //    {
        //        case "h":
        //        case "help":
        //            if (words.Length == 1)
        //                GiveHelp();
        //            else
        //                Console.Write(errorMessage);
        //            break;
        //        case "q":
        //        case "quit":
        //            if (words.Length == 1)
        //            {
        //                Console.WriteLine("Goodbye...");
        //                Environment.Exit(0);
        //            }
        //            else
        //                Console.Write(errorMessage);
        //            break;
        //        case "l":
        //            player.Look(currentRoom, readDescription: true);
        //            break;
        //        case "look":
        //            if (words.Length == 1 || (words.Length == 2 && words[1] == "around"))
        //                player.Look(currentRoom, readDescription: true);
        //            else
        //                Console.Write(errorMessage);
        //            break;
        //        case "i":
        //        case "inventory":
        //            player.CheckInventory();
        //            break;
        //        case "take":
        //            break;
        //        case "examine":
        //        case "u":
        //        case "d":
        //        case "n":
        //        case "e":
        //        case "s":
        //        case "w":
        //        case "ne":
        //        case "nw":
        //        case "se":
        //        case "sw":
        //        case "up":
        //        case "down":
        //        case "north":
        //        case "east":
        //        case "south":
        //        case "northeast":
        //        case "northwest":
        //        case "southeast":
        //        case "southwest":
        //        case "west":
        //            if (words.Length > 1)
        //                Console.WriteLine(errorMessage);
        //            else
        //                currentRoom = player.ChangeRoom(words[0], currentRoom);
        //            break;
        //        case "go":
        //            if (words.Length == 1)
        //                Console.Write("Go where?\n\n> ");
        //            else
        //                Console.WriteLine();
        //            break;
        //        default:
        //            Console.Write(errorMessage);
        //            break;
        //    }
        //}
        //public static void GiveHelp()
        //{
        //    string help = "Commands I understand:\n" +
        //        "  h, help:                 provide brief instructions\n" +
        //        "  [d]irection,\n" +
        //        "  [direction],\n" +
        //        "  go [direction]:          move in one of ten possible directions (up, down, north, west, southeast...)\n" +
        //        "  l, look, look around:    look around the room, describing it and any objects in it\n" +
        //        "  take [item]:             take item from room if possible\n" +
        //        "  examine [item]:          get a description of said item\n" +
        //        "  i, inventory:            check your inventory\n" +
        //        "  q, quit:                 exit the game\n\n> ";
        //    Console.Write(help);
        //}
    }
}