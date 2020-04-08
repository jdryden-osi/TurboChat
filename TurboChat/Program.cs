
namespace TurboChat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using OSIsoft.AF.Asset;
    using OSIsoft.AF.Data;
    using OSIsoft.AF.PI;
    using OSIsoft.AF.Time;

    class Program
    {
        static void Main(string[] args)
        {
            var options = new TurboChatOptions();
            var writer = new ChatStringWriter(options);

            using (var ui = new TextUserInterface(writer))
            {
                ui.SplashScreen();

                // ask the TURBOCHAT user to enter their TURBOCHAT name
                options.Name = GetUserName();

                // pick a TURBOCHAT room to digitally chill out in
                var server = (new PIServers())["CSPIBUILD.dev.osisoft.int"];
                options.Point = GetRoomSelection(server);

                options.ExtensionHandler = new ExtensionHandler(ui);

                ui.DrawApplicationChrome();

                // print last 50 messages in the "room"
                var initialMessages = options.Point.RecordedValuesByCount(AFTime.Now, 50, false, AFBoundaryType.Inside, null, false);
                foreach (var msg in initialMessages.OrderBy(m => m.Timestamp))
                {
                    PrintMessage(ui, msg);
                }

                // start a background task to print new "messages" in the "room" every "1 second"
                Task.Run(() =>
                {
                    var pipe = new PIDataPipe(AFDataPipeType.Snapshot);
                    pipe.AddSignups(new List<PIPoint> { options.Point });

                    while (true)
                    {
                        var updates = pipe.GetUpdateEvents(100)
                            .Where(u => u.Action == AFDataPipeAction.Update)
                            .OrderBy(v => v.Value.Timestamp);

                        foreach (var update in updates)
                        {
                            PrintMessage(ui, update.Value);
                        }
                        Thread.Sleep(1000);
                    }
                });

                ui.Run();
            }
        }

        static string GetUserName()
        {
            Console.Write("Enter your TURBOCHAT name: ");
            while (true)
            {
                var username = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.Write("I couldn't hear that, speak up: ");
                }                
                else if (!CheckLength(username))
                {
                    return username.Trim();
                }
            }
        }

        static bool CheckLength(string name)
        {
            if (name.Length > 10)
            {
                Console.Write("You think storage is free? Pick a shorter name: ");
                return true;
            }

            return false;
        }

        static PIPoint GetRoomSelection(PIServer server)
        {
            var rooms = PIPoint.FindPIPoints(server, "TurboChat*").ToList();
            Console.WriteLine("Available rooms: ");
            for (var i = 1; i <= rooms.Count; i++)
            {
                if (!rooms[i-1].IsAttributeLoaded("Descriptor"))
                {
                    rooms[i - 1].LoadAttributes(new string[] { "Descriptor" });
                }

                var desc = rooms[i - 1].GetAttribute("Descriptor") as string;
                Console.WriteLine($"{i,3} {(string.IsNullOrWhiteSpace(desc) ? rooms[i-1].Name : desc)}");
            }

            Console.Write("Select your TURBOCHAT room (/N to create new room): ");
            while (true)
            {
                var selection = Console.ReadLine();
                if (int.TryParse(selection, out var index) && index > 0 && index <= rooms.Count)
                {
                    return rooms[index - 1];
                }
                else if (selection == "/N")
                {
                    return CreateNewRoom(server);
                }
                else
                {
                    Console.Write("WRONG! Try again, dummy: ");
                }
            }
        }

        static PIPoint CreateNewRoom(PIServer server)
        {
            while (true)
            {
                Console.Write("Enter room name: ");
                var newRoom = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newRoom))
                {
                    Console.WriteLine("Come on! Say something!");
                }
                else if (!CheckLength(newRoom))
                {
                    if (PIPoint.TryFindPIPoint(server, "TurboChat-" + newRoom, out _))
                    {
                        Console.WriteLine("You can't steal another chat room name! Duh!");
                    }
                    else
                    {
                        return server.CreatePIPoint("TurboChat-" + newRoom, new Dictionary<string, object> { { "PointSource", "CHAT" }, {"PointType", PIPointType.String }, { "Descriptor", newRoom } });
                    }
                }
            }
        }

        static void PrintMessage(IChatStringDisplay display, AFValue afValue)
        {
            string userName = string.Empty;
            string value = afValue.Value.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                var colon = value.IndexOf(':');
                if (colon >= 0)
                {
                    userName = value.Substring(0, colon).Trim();
                    value = value.Substring(colon + 1).Trim();
                }
            }

            display.AddChatString(afValue.Timestamp, userName, value);
        }
    }
}
