using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;

namespace TurboChat
{
    class Program
    {
        static void Main(string[] args)
        {
            // ask the TURBOCHAT user to enter their TURBOCHAT name
            var username = GetUserName();

            // pick a TURBOCHAT room to digitally chill out in
            var server = (new PIServers())["CSPIBUILD"];
            var point = GetRoomSelection(server);

            // print last 50 messages in the "room"
            var initialMessages = point.RecordedValuesByCount(AFTime.Now, 50, false, AFBoundaryType.Inside, null, false);
            foreach (var msg in initialMessages.OrderBy(m => m.Timestamp))
            {
                PrintMessage(msg);
            }

            // start a background task to print new "messages" in the "room" every "1 second"
            Task.Run(() =>
            {
                var pipe = new PIDataPipe(AFDataPipeType.Snapshot);
                pipe.AddSignups(new List<PIPoint> { point });

                while (true)
                {
                    var updates = pipe.GetUpdateEvents(100)
                        .Where(u => u.Action == AFDataPipeAction.Update)
                        .OrderBy(v => v.Value.Timestamp);

                    foreach (var update in updates)
                    {
                        PrintMessage(update.Value);
                    }
                    Thread.Sleep(1000);
                }
            });

            // get the next dumb message to send
            while (true)
            {
                var newMessage = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newMessage))
                {
                    WriteMessage(point, username, newMessage);
                }
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
                else if (username.Length > 10)
                {
                    Console.Write("You think storage is free? Pick a shorter name: ");
                }
                else
                {
                    return username.Trim();
                }
            }
        }

        static PIPoint GetRoomSelection(PIServer server)
        {
            var rooms = PIPoint.FindPIPoints(server, "TurboChat*").ToList();
            Console.WriteLine("Available rooms: ");
            for (var i = 1; i <= rooms.Count; i++)
            {
                Console.WriteLine($"{i,3} {rooms[i-1].Name}");
            }
            Console.Write("Select your TURBOCHAT room: ");
            while (true)
            {
                var selection = Console.ReadLine();
                if (int.TryParse(selection, out var index) && index > 0 && index <= rooms.Count)
                {
                    return rooms[index - 1];
                }
                else
                {
                    Console.Write("WRONG! Try again, dummy: ");
                }
            }
        }

        static void PrintMessage(AFValue afValue)
        {
            Console.WriteLine($"[{afValue.Timestamp}] {afValue.Value}");
        }

        static void WriteMessage(PIPoint point, string username, string message)
        {
            var value = new AFValue($"{username,-10} : {message}", AFTime.Now);
            point.UpdateValue(value, AFUpdateOption.Insert);
        }
    }
}
