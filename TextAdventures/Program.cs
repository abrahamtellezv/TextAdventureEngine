using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace TextAdventures
{
    internal class Program
    {
        static void Main()
        {
            Player player = new();
            TextParser parser = new();
            Dictionary<string, Room> rooms = CreateWorld();
            Room currentRoom = rooms["outside"];
            Console.Title = "Hello...";
            Console.Write("Welcome to the text adventure game!");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("    (Type \"help\" for instructions)\n\n\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            TextWriter.Write("You stand before the slightly ajar door at 320 Real de Montes Urales. While it doesn't really seem that inviting, the chilly air is telling you to go inside.");
            Console.Write("\n\n\n> ");
            string? input;
            string[] words;
            while (true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write("No command was given.\n\n\n> ");
                    continue;
                }
                words = TextParser.PrepareText(input).Split(' ');
                parser.ParseText(words, player, ref currentRoom);
            }
        }

        private static Dictionary<string, Room> CreateWorld()
        {
            Dictionary<string, Room> rooms = new();
            Room outside = new("Outside", "You're outside the house, it's too dark to go wandering any further, you should go back inside.");
            Room entrance = new("Entrance", "The entrance to the house, you can see one of my threaded rainbow pieces here. There's a flight of stairs leading up to the study, while a small flight leads down to a bathroom. The living room lies to the north. The smell of the kitchen comes from the east.");
            Room downstairsBathroom = new("Downstairs bathroom", "This small bathroom was, somehow, one of the best rooms in the house. The stairs lead up to the entrance.");
            Room garden = new("Garden", "Somehow the grass is still green in some patches, sadly, the potted plants and the trees have all died. The living and dining rooms lie to the southwest and southeast respectively");
            Room kitchen = new("Kitchen", "You can smell the years of cooking that have happened here. The entrance lies to the west. North is the dining room. The laundry room can be seen to the south.");
            Room laundryRoom = new("Laundry room", "The trusty old washing machine takes most of the space here. The kitchen is to the north. A door leads to the service area to the south.");
            Room livingRoom = new("Living room", "The couch has definitely seen better times and the tv doesn't seem to work anymore. You see the dining room to the east and the entrance to the south.");
            Room diningRoom = new("Dinning room", "There's food on the table, not tha t you should eat it, who knows how long it's been there. Walking west leads to the living room. The kitchen is to the south.");
            Room serviceArea = new("Service area", "There's a lot of junk here. The laundry room is up north.");
            Room study = new("Study", "There's so many things on the desks it looks like a stationary shop. At least dusty and messy one. There's a bathroom to the north. You can see a faint green hue coming from the door to the east. Your bedroom was behind the eastern door. There's a door to the balcony south, next to the stairs leading down to the entrance.");
            Room balcony = new("Balcony", "Normally you'd be able to see the small park in front of the house, but for some reason, it's too dark to see right now.");
            Room bedroom = new("Bedroom", "The unmade bed has the lingering smell of the three of us...");
            Room bigBedroom = new("Big bedroom", "She had all sorts of stuff all over the place, she left it that way.");
            Room bathroom = new("Bathroom", "Still steamy... somehow...");
            rooms.Add("outside", outside);
            rooms.Add("entrance", entrance);
            rooms.Add("downstairsBathroom", downstairsBathroom);
            rooms.Add("garden", garden);
            rooms.Add("kitchen", kitchen);
            rooms.Add("laundry room", laundryRoom);
            rooms.Add("living room", livingRoom);
            rooms.Add("dining room", diningRoom);
            rooms.Add("service area", serviceArea);
            rooms.Add("study", study);
            rooms.Add("balcony", balcony);
            rooms.Add("bedroom", bedroom);
            rooms.Add("big bedroom", bigBedroom);
            rooms.Add("bathroom", bathroom);

            outside.AddExit("n", entrance);
            entrance.AddExit("s", outside);
            entrance.AddExit("u", study);
            entrance.AddExit("n", livingRoom);
            entrance.AddExit("e", kitchen);
            entrance.AddExit("d", downstairsBathroom);
            downstairsBathroom.AddExit("u", entrance);
            livingRoom.AddExit("s", entrance);
            livingRoom.AddExit("e", diningRoom);
            livingRoom.AddExit("n", garden);
            diningRoom.AddExit("w", livingRoom);
            diningRoom.AddExit("s", kitchen);
            diningRoom.AddExit("n", garden);
            garden.AddExit("sw", livingRoom);
            garden.AddExit("se", diningRoom);
            kitchen.AddExit("n", diningRoom);
            kitchen.AddExit("w", entrance);
            kitchen.AddExit("s", laundryRoom);
            laundryRoom.AddExit("n", kitchen);
            laundryRoom.AddExit("s", serviceArea);
            serviceArea.AddExit("n", laundryRoom);
            study.AddExit("d", entrance);
            study.AddExit("n", bathroom);
            study.AddExit("e", bedroom);
            study.AddExit("s", balcony);
            study.AddExit("w", bigBedroom);
            bigBedroom.AddExit("e", study);
            balcony.AddExit("n", study);
            bedroom.AddExit("w", study);
            bathroom.AddExit("s", study);

            Item keys = new("a set of keys", "A set of keys, to the front door and who knows what else.", "There's a set of keys on the white table next to the door.", isTakeable: true, 3, new HashSet<string> { "key", "keys", "key set", "set of keys" });
            Item soda = new("a can of soda", "A can of soda, \"Red Cola\" reads the label.", "You see a can of soda on the floor.", isTakeable: true, 8, new HashSet<string> { "can", "soda", "can of soda", "soda can" });
            Item cake = new("a cake", "The remains of a chocolate cake, the resemblance to the one on Matilda is remarkable.", "There's some cake on the counter.", isTakeable: true, 23, new HashSet<string> { "cake" });
            Item pen = new("a brown pen", "A brown ballpoint pen, I've always liked this ink color, how didn't people think of this before.", "A pen is sitting on the table.", isTakeable: true, 2, new HashSet<string> { "pen" });
            ContainerItem bag = new("a paper bag", "A simple paper bag with a logo printed on.", "You see a paper bag lying on the couch.", isTakeable: true, 1, new HashSet<string> { "bag", "paper bag" }, 10, 0, isOpen: false, canBeClosed: true);
            Item ball = new("a ball", "a ball", "", isTakeable: true, 4, new HashSet<string> { "ball" });
            bag.AddItems(ball);
            
            entrance.AddItems(keys, pen);
            kitchen.AddItems(cake);
            livingRoom.AddItems(bag, soda);

            return rooms;
        }

        public static HashSet<Room> LoadRooms()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string roomsFile = "TextAdventures.Rooms.json";

            using Stream? stream = assembly.GetManifestResourceStream(roomsFile);
            
            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {roomsFile} not found");
            }

            using StreamReader reader = new(stream);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<HashSet<Room>>(json);
        }
    }
}