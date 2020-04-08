using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TurboChat
{
    class ExtensionHandler : IExtensionHandler
    {
        private IChatStringDisplay ui;

        public ExtensionHandler(IChatStringDisplay ui)
        {
            this.ui = ui;
        }

        public bool ProcessMessage(TurboChatOptions options, string commandLine)
        {
            if (commandLine.StartsWith("$/"))
            {
                CallExtension(commandLine);
            }
            else
            {
                commandLine = ReplaceInlinePoints(options.Point.Server, commandLine);
            }

            WriteMessage(options.Point, options.Name, commandLine);

            return true;
        }

        public bool CallExtension(string commandLine)
        {
            // Parse out the command

            // Call the appropriate handler

            // Return true if handled, false if not handled
            return false;
        }

        static void WriteMessage(OSIsoft.AF.PI.PIPoint point, string username, string message)
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
