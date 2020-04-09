using OSIsoft.AF.Asset;
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
                    ParkAccess(options);                    
                    break;
                case "logo":
                    CreateLogo(options);
                    break;
                default:
                    break;
            }

            return false;
        }

        private void PrintAscii(string logo, TurboChatOptions options)
        {
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
            PrintAscii(logo, options);
        }

        private void ParkAccess(TurboChatOptions options)
        {
            for (var i = 0; i < 3; i++)
            {
                Console.Write("Enter passcode: ");
                var line = Console.ReadLine();
                if (line == "unixsystem")
                {
                    var dino = @"
	
$$$$$$$$$$$$$$$$$$$$$$$$$$$**""""""""`` ````""""""#*R$$$$$$$$$$$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$$$$$$*""""..........      `""#$$$$$$$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$$$#""    .ue@$$$********$$$$Weu.   `""*$$$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$#""   ue$$*#""""              `""""*$$No.   ""R$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$P""   u@$*""`                         ""#$$o.  ^*$$$$$$$$$$$$$$
$$$$$$$$$$$P""  .o$R"". .WN.           ""#$Nu  `#$$$$$$$$$$$$
$$$$$$$$$$""  .@$#`       'ou  .oeW$$$$$$$$W            ""$$u  ""$$$$$$$$$$$
$$$$$$$$#   o$#`      ueL  $$$$$$$$$$$$$$$$ku.           ""$$u  ""$$$$$$$$$
$$$$$$$""  x$P`        `""$$u$$$$$$$$$$$$$$""#$$$L            ""$o *$$$$$$$
$$$$$$""  d$""        #$u.2$$$$$$$$$$$$$$$$  #$$$Nu            $$.  #$$$$$$
$$$$$""  @$""          $$$$$$$$$$$$$$$$$$$$k  $$#*$$u           #$L  #$$$$$
$$$$""  d$         #Nu@$$$$$$$$$$$$$$$$$$""  x$$L #$$$o.         #$c  #$$$$
$$$F  d$          .$$$$$$$$$$$$$$$$$$$$N  d$$$$  ""$$$$$u        #$L  #$$$
$$$  :$F..`$$$$$$$$$$$$$$$$$$$$$$$$$$$`    R$$$$$eu.     $$   $$$
$$!  $$        . R$$$$$$$$$$$$$$$$$$$$$$$$$$$$$.   @$$$$$$$$Nu   '$N  `$$
$$  x$""        Re$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$uu@""``""$$$$$$$i   #$:  $$
$E  $$       c 8$$$$$$$$$$$$$$$$$$$$$G(   ``^*$$$$$$$WW$$$$$$$$N   $$  4$
$~ :$$N. tL i)$$$$$$$$$$$$$$$$$$$$$$$$$N       ^#R$$$$$$$$$$$$$$$  9$  '$
$  t$$$$u$$W$$$$$$$$$$$$$$!$$$$$$$$$$$$$&       . c?""*$$$R$$$$$$$  '$k  $
$  @$$$$$$$$$$$$$$$$$$$$""E F!$$$$$$$$$$.""        +.""@\* x .""""*$$""   $B  $
$  $$$$$$$$$$$$$$$$""$)#F     $$$$$$$$$$$           `  -d>x""*=.""`    $$  $
$  $$$$$$$$$$?$$R'$ `#d$""""    #$$$$$$$$$ > .                ""       $$  $
$  $$$$$$$($$@$""` P *@$.@#""!    ""*$$$$$$$L!.                        $$  $
$  9$$$$$$$L#$L  ! "" <$$`          ""*$$$$$NL:""z  f                  $E  $
$> ?$$$$ $$$b$^      .$c .ueu.        `""$$$$b""x""#  ""               x$!  $
$k  $$$$N$ ""$$L:$oud$$$` d$ .u.         ""$$$$$o."" #f.              $$   $
$$  R$""""$$o.$""$$$$"""""" ue$$$P""`""c          ""$$$$$$Wo'              :$F  t$
$$: '$&  $*$$u$$$$u.ud$R"" `    ^            ""#*****               @$   $$
$$N  #$: E 3$$$$$$$$$""                                           d$""  x$$
$$$k  $$   F *$$$$*""                                            :$P   $$$
$$$$  '$b                                                      .$P   $$$$
$$$$b  `$b                                                    .$$   @$$$$
$$$$$N  ""$N                                                  .$P   @$$$$$
$$$$$$N  '*$c                                               u$#  .$$$$$$$
$$$$$$$$.  ""$N.                                           .@$""  x$$$$$$$$
$$$$$$$$$o   #$N.                                       .@$#  .@$$$$$$$$$
$$$$$$$$$$$u  `#$Nu                                   u@$#   u$$$$$$$$$$$
$$$$$$$$$$$$$u   ""R$o.                             ue$R""   u$$$$$$$$$$$$$
$$$$$$$$$$$$$$$o.  ^#$$bu.                     .uW$P""`  .u$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$$u   `""#R$$Wou..... ....uueW$$*#""   .u@$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$$$$Nu.    `""""#***$$$$$***""""""`    .o$$$$$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$$$$$$$$$eu..               ...ed$$$$$$$$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$NWWeeeeedW@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
";
                    PrintAscii(dino, options);
                    return;
                }
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
            var line = thisUI.ReserveWorkArea(3);

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
