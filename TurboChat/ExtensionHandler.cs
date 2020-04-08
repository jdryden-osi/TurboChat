﻿using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace TurboChat
{
    class ExtensionHandler : IExtensionHandler
    {
        private IChatStringDisplay ui;

        public ExtensionHandler(IChatStringDisplay ui)
        {
            this.ui = ui;
        }

        public void ProcessMessage(TurboChatOptions options, string commandLine)
        {
            if (commandLine.StartsWith("$/"))
            {
                CallExtension(commandLine.Substring(2), options);
            }
            else
            {
                commandLine = ReplaceInlinePoints(options.Point.Server, commandLine);
                WriteMessage(options.Point, options.Name, commandLine);
            }
        }

        public bool CallExtension(string commandLine, TurboChatOptions options)
        {
            var args = commandLine.Split('/');

            switch (args[0].ToLowerInvariant())
            {
                case "gauge":
                    DrawGauge(options, args[1]);
                    break;
                default:
                    break;
            }

            return false;
        }

        private void DrawGauge(TurboChatOptions options, string point)
        {
            Console.WriteLine();
            Console.WriteLine();
            var currentPoint = PIPoint.FindPIPoint(options.Point.Server, point);

            var attrs = currentPoint.GetAttributes("zero", "span");
            var zero = attrs["zero"].ToString();
            var span = attrs["span"].ToString();

            var zeroFloat = float.Parse(zero);
            var spanFloat = float.Parse(span);

            Console.CursorLeft = 0;
            Console.Write(zeroFloat);
            Console.CursorLeft = 100;
            Console.Write(zeroFloat + spanFloat);
            Console.WriteLine();
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.DarkCyan;

            while (!Console.KeyAvailable)
            {
                var gaugeValue = float.Parse(currentPoint.CurrentValue().ToString());
                if (gaugeValue > zeroFloat + spanFloat)
                {
                    gaugeValue = zeroFloat + spanFloat;
                }
                else if (gaugeValue < zeroFloat)
                {
                    gaugeValue = zeroFloat;
                }

                var position = 100 * gaugeValue / (spanFloat - zeroFloat);

                Console.CursorLeft = 0;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                for (var i = 0; i < 100; i++) { Console.Write(" "); }
                Console.CursorLeft = 0;
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                for (var i = 0; i < position; i++) { Console.Write(" "); }

                Thread.Sleep(1000);
            }

            Console.ReadKey(true);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine();
            Console.WriteLine();
        }

        static void WriteMessage(PIPoint point, string username, string message)
        {
            var value = new AFValue($"{username,-10}:{message}", AFTime.Now);
            point.UpdateValue(value, AFUpdateOption.Insert);
        }

        static string ReplaceInlinePoints(PIServer server, string message)
        {
            Regex rx = new Regex(@"\`(.*?)\`", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(message);

            if (matches.Count > 0)
            {
                var pointNames = matches.Cast<Match>().Select(m => m.Value.Substring(1, m.Value.Length - 2)).ToList();
                var tags = PIPoint.FindPIPoints(server, pointNames);
                var dict = new Dictionary<string, PIPoint>(StringComparer.OrdinalIgnoreCase);
                foreach (var tag in tags)
                {
                    dict[tag.Name] = tag;
                }

                for (var i = 0; i < pointNames.Count; i++)
                {
                    var name = pointNames[i];
                    var pointValue = dict.TryGetValue(name, out PIPoint tag) ? tag.CurrentValue().ToString() : "---";
                    message = message.Replace(matches[i].Value, pointValue);
                }
            }

            return message;
        }
    }
}
