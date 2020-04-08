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
                case "parkaccess":
                    ParkAccess();
                    break;
                case "logo":
                    CreateLogo(options);
                    break;
                default:
                    break;
            }

            return false;
        }

        private void CreateLogo(TurboChatOptions options)
        {
            var logo = @"
                 ,*,***,,,,,,,,,,,,*.                          
                            ,,,,,,,,,,,,,,,,,,,,,,*/(/*,,*.                     
                       *,,,,,,,,,,,,,,,,,,,,,,,,/#@&/*,,,,,,,                   
                    .,,,,,,,,,,,,,,,,,,,,,,,,/#@@@%/,,,,,,,,,*,.                
                .,,,,,,,,,,,,,,,,,,,,,,,,*(&@@@@@#*,,,,,,,,,,,,*/,              
              ,,,,,,,,,,,,,,,,,,,,,,,*(%&@@@@@@&(*,,,,,,,,,,,,,**//.            
           .,,,,,,,,,,,,,,,,,,,,*/(%@@@@@@@@@&#*,,,,,,,,,,,,,,,,**/(/*          
         ,,,,,,,,,,,,,,,,,,,/(%&@@@@@@@@@@@&#*,,,,,,,,,,,,,,,,,,,*/((((/.       
       .*,,,,,,,,,,,,,,*(#&@@@@@@@@@@@@@&#/*,,,,,,,,,,,,,,,,,,,,,**/((((/,      
     .,,,,,,,,,,,,,*(%&@@@@@@@@@@@@@&%(*,,,,,,,,,,,,,,,,,,,,,,,,,**/((((((/     
    .,,,,,,,,,,,*(&@@@@@@@@@@@@@@#/*,,,,,,,,,,,,,,,,,,,,,,,,,,,,,*//((((((((.   
  .*,,,,,,,,,*(%@@@@@@@@&%###&@&/,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,*/(((((((((*.  
 ,,,,,,,,,,*(&@@@@@&&@@%/,*/(##/,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,**/((((((((((,  
.,,,,,,,,*/%&&&%(/*(##(*,,/###/*,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,**/(((((((((((,  
,,,,,,,,*(&%#/*,*/#%#/*,,/(##(/*,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,**/((((((((((((,  
,,,,,,,/(%/*,,,*(#%#/*,,,(#%%(/*,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,*/(((((((((((((*  
*,,,,,*//,,,,,*#%%%(*,,,/#%%%#/*,,,,,,,,,,,,,,,,,,,,,,,,,,,,*/((((((((((((((*.  
,,,,,***,,,,*/#&&%#*,,,,(%%%%%(*,,,,,,,,,,,,,,,,,,,,,,,,,,**((((((((((((((((.   
,,,,,,,,,,,,/%@&&%/,,,,,#%&&&%%(*,,,,,,,,,,,,,,,,,,,,,,,,*/(((((((((((((((/     
*,,,,,,,,,,/%@&&&#*,,,,,#&&&&&&%#*,,,,,,,,,,,,,,,,,,,,**((##(((((((((((((/*     
*,,,,,,,,,*#&@&@&(*,,,,,(%&&&&&&&&#(*,,,,,,,,,,,,,,*/(########((((((((((/*      
,,,,,,,,,,(&@@@@&(,,,,,,*#&@&&&&&&&&%&%#((/////((#%%%###########((((((((,       
  .,,,,,,*%@@@@@&(,,,,,,,*#&@@&&&@@/  #@@@@@&&&&%%%%%%%###########(((/          
  .*(/,,,*%@@@@@&#*,,,,,,,*/&@@@@@@/    *&@@@&&&&&%%%%%%%##########(*           
      ((*,#&@@@@@&(,,,,,,,,,,*(%&@&/.((((/ %@@@&&&&&%%%%%%%######(              
       /&&%&@@@@@@@#*,,,,,,,,,,,,*/(######(%**@@@@&&&&%%%%%%%##*                
          %&&&@@@@@@@&###%%%%%&&&%%%%%#####(#%#  /&@&&&&%&*                     
           /%@&&@@@@@/,@&&&&&&&&%%%%%%%%########%/                              
             ,&@&&@@@/  (&@&&&&&&&&%%%%%%%%#######(                             
                (&@&&*    /&&&&&&&&&&%%%%%%%####%%%#.                           
                  ..#%(     .%@&&&&&&&&&&&&&&#...          

";
            using (var reader = new System.IO.StringReader(logo))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    line = line.Replace(" ", "_");
                    WriteMessage(options.Point, options.Name, line);
                    line = reader.ReadLine();
                }
            }
        }

        private void ParkAccess()
        {
            Console.Clear();
            for (var i = 0; i < 3; i++)
            {
                Console.Write("Enter passcode: ");
                Console.ReadLine();
                Console.WriteLine("Access Denied");
            }
            Thread.Sleep(1000);
            Console.Write("and......");
            Thread.Sleep(1000);
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("YOU DIDN'T SAY THE MAGIC WORD!!!");
            Thread.Sleep(1000);
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=RfiQYRn7fBg");
            while(true)
            {
                Thread.Sleep(100);
                Console.WriteLine("YOU DIDN'T SAY THE MAGIC WORD!");
                Console.Beep();
            }
        }

        private void DrawGauge(TurboChatOptions options, string point)
        {
            var thisUI = (TextUserInterface)ui;
            var line = thisUI.ReserveWorkArea(3) + 1;

            Console.SetCursorPosition(0, line);
//            Console.Write("Press any key to exit gauge mode...");

            var currentPoint = PIPoint.FindPIPoint(options.Point.Server, point);

            var attrs = currentPoint.GetAttributes("zero", "span");
            var zero = attrs["zero"].ToString();
            var span = attrs["span"].ToString();

            var zerodouble = double.Parse(zero);
            var spandouble = double.Parse(span);

            Console.CursorLeft = 0;
            Console.Write(zerodouble);
            Console.CursorLeft = 100;
            Console.Write(zerodouble + spandouble);
            Console.SetCursorPosition(0, line + 1);
            Console.CursorLeft = 0;
            Console.BackgroundColor = ConsoleColor.DarkCyan;

            while (!Console.KeyAvailable)
            {
                var gaugeValue = double.Parse(currentPoint.CurrentValue().ToString());
                if (gaugeValue > zerodouble + spandouble)
                {
                    gaugeValue = zerodouble + spandouble;
                }
                else if (gaugeValue < zerodouble)
                {
                    gaugeValue = zerodouble;
                }

                var position = 100 * (gaugeValue - zerodouble) / spandouble;

                Console.CursorLeft = 0;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                for (var i = 0; i < 115; i++) { Console.Write(" "); }
                Console.CursorLeft = 0;
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                for (var i = 0; i < position; i++) { Console.Write(" "); }
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write(gaugeValue);

                Thread.Sleep(1000);
            }

            Console.ReadKey(true);
            thisUI.ReleaseWorkArea();
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
