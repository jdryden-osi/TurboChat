
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
                ui.ColorScheme = GetColorScheme(ui);
                options.Name = GetUserName();
                while (SetupChatRoom(ui, options));
            }
        }

        static bool SetupChatRoom(TextUserInterface ui, TurboChatOptions options)
        {
            // pick a TURBOCHAT room to digitally chill out in
            var server = (new PIServers())["CSPIBUILD.dev.osisoft.int"];
            var point = GetRoomSelection(server, ui);
            if (point == null)
            {
                return false;
            }

            options.Point = point;
            options.ExtensionHandler = new ExtensionHandler(ui);

                ui.DrawApplicationChrome();
                ui.AddChatRoomName(RoomName(options.Point));

            // print last 50 messages in the "room"
            var initialMessages = options.Point.RecordedValuesByCount(AFTime.Now, 50, false, AFBoundaryType.Inside, null, false);
            foreach (var msg in initialMessages.OrderBy(m => m.Timestamp))
            {
                PrintMessage(ui, msg);
            }

            var canceled = false;
            // start a background task to print new "messages" in the "room" every "1 second"
            Task.Run(() =>
            {
                var pipe = new PIDataPipe(AFDataPipeType.Snapshot);
                pipe.AddSignups(new List<PIPoint> { options.Point });

                while (!canceled)
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

                pipe.Close();
            });

            ui.Run();
            canceled = true;
            Console.Clear();
            return true;
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

        static ColorScheme GetColorScheme(TextUserInterface ui)
        {
            Console.WriteLine("Available color schemes: ");
            var count = 1;
            foreach (string key in ui.ColorSchemeOptions.Keys)
            {
                ColorScheme tmp = ui.ColorSchemeOptions[key];
                Console.ForegroundColor = tmp.Foreground;
                Console.BackgroundColor = tmp.Background;
                Console.WriteLine($"{count,3} {key}");
                count++;
            }
            Console.ForegroundColor = ui.ColorScheme.Foreground;
            Console.BackgroundColor = ui.ColorScheme.Background;
            Console.Write("Select your color scheme: ");

            while (true)
            {
                var selection = Console.ReadLine();
                if (int.TryParse(selection, out var index) && index > 0 && index <= count)
                {
                    return ui.ColorSchemeOptions.ElementAt(index - 1).Value;
                }
                else if (selection == "/Q")
                {
                    return null;
                }
                else
                {
                    Console.Write("WRONG! Do you even get colors?: ");
                }
            }
        }

        static PIPoint GetRoomSelection(PIServer server, TextUserInterface ui)
        {
            Console.ForegroundColor = ui.ColorScheme.Foreground;
            Console.BackgroundColor = ui.ColorScheme.Background;
            Console.Clear();
            var rooms = PIPoint.FindPIPoints(server, "TurboChat*").ToList();
            Console.WriteLine("Available rooms: ");
            for (var i = 1; i <= rooms.Count; i++)
            {
                Console.WriteLine($"{i,3} {RoomName(rooms[i - 1])}");
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
                else if (selection == "/Q")
                {
                    return null;
                }
                else
                {
                    Console.Write("WRONG! Try again, dummy: ");
                }
            }
        }

        static PIPoint CreateNewRoom(PIServer server)
        {
            Console.Write("Enter room name: ");
            while (true)
            {
                var newRoom = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newRoom))
                {
                    Console.WriteLine("Come on! Say something!");
                    Console.Write("Go ahead. I'm listening: ");
                }
                else if (!CheckLength(newRoom))
                {
                    if (PIPoint.TryFindPIPoint(server, "TurboChat-" + newRoom, out _))
                    {
                        Console.WriteLine("You can't steal another chat room name! Duh!");
                        Console.Write("Try again: ");
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

        /// <summary>
        /// To determine the room name associated with the input PI Point
        /// </summary>
        static string RoomName(PIPoint tag)
        {
            if (!tag.IsAttributeLoaded("Descriptor"))
            {
                tag.LoadAttributes(new string[] { "Descriptor" });
            }

            var desc = tag.GetAttribute("Descriptor") as string;
            return string.IsNullOrWhiteSpace(desc) ? tag.Name : desc;
        }
    }
}
