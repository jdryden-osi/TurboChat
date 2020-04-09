namespace TurboChat
{
    using OSIsoft.AF.Time;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Xml.Schema;

    /// <summary>
    /// Class that provides all UI for the main application
    /// </summary>
    class TextUserInterface : IDisposable, IChatStringDisplay
    {
        private const string logo = @"
 ________  ___          _________  ___  ___  ________  ________  ________  ________  ___  ___  ________  _________   
|\   __  \|\  \        |\___   ___\\  \|\  \|\   __  \|\   __  \|\   __  \|\   ____\|\  \|\  \|\   __  \|\___   ___\ 
\ \  \|\  \ \  \       \|___ \  \_\ \  \\\  \ \  \|\  \ \  \|\ /\ \  \|\  \ \  \___|\ \  \\\  \ \  \|\  \|___ \  \_| 
 \ \   ____\ \  \           \ \  \ \ \  \\\  \ \   _  _\ \   __  \ \  \\\  \ \  \    \ \   __  \ \   __  \   \ \  \  
  \ \  \___|\ \  \           \ \  \ \ \  \\\  \ \  \\  \\ \  \|\  \ \  \\\  \ \  \____\ \  \ \  \ \  \ \  \   \ \  \ 
   \ \__\    \ \__\           \ \__\ \ \_______\ \__\\ _\\ \_______\ \_______\ \_______\ \__\ \__\ \__\ \__\   \ \__\
    \|__|     \|__|            \|__|  \|_______|\|__|\|__|\|_______|\|_______|\|_______|\|__|\|__|\|__|\|__|    \|__|";

        private const string applicationName = "PI TurboChat";
        private ConsoleColor originalBackground;
        private ConsoleColor originalForeground;
        private ConsoleColor defaultBackground;
        private ConsoleColor defaultForeground;
        private string originalTitle;
        private int originalWidth;
        private int originalHeight;

        private object consoleLock = new object();

        private Dictionary<string, ConsoleColor> colorMap = new Dictionary<string, ConsoleColor>();
        private IChatStringWriter writer;
        private bool exitProgram = false;

        private int dataTop;
        private int dataLeft;
        private int dataRight;
        private int dataBottom;

        private int workAreaHeight = 0; // 0 means no reserved work area

        private ConsoleColor[] validColors = {
            ConsoleColor.White,
            ConsoleColor.Yellow,
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Magenta,
            ConsoleColor.Red,
            ConsoleColor.Gray,
            ConsoleColor.DarkCyan,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkGray,
        };
        private int nextColor = 0;

        public int CurrentLine { get; private set; }

        public TextUserInterface(IChatStringWriter writer, TurboChatOptions options)
        {
            this.writer = writer;

            this.originalBackground = Console.BackgroundColor;
            this.originalForeground = Console.ForegroundColor;
            this.originalTitle = Console.Title;
            this.originalWidth = Console.WindowWidth;
            this.originalHeight = Console.WindowHeight;

            this.defaultBackground = ConsoleColor.DarkBlue;
            this.defaultForeground = ConsoleColor.White;


            if (Console.WindowHeight < 50)
            {
                Console.WindowHeight = 50;
            }

            if (Console.WindowWidth < 130)
            {
                Console.WindowWidth = 130;
            }
        }

        public void SplashScreen()
        {
            Console.BackgroundColor = this.defaultBackground;
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Remove Window Scroll bar
            //Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            Console.Clear();
            Console.Title = applicationName;

            Console.SetCursorPosition(1, 15);
            Console.WriteLine(logo);
            Console.SetCursorPosition(1, 25);
            CenterText("The fastest, easiest way to chat using PI System data (tm)");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            CenterText("Copyright (c) 2020 EspressoTeam, LOL. All Rights Reserved.");
            Console.SetCursorPosition(1, Console.WindowHeight - 1);
            Console.Write("Press any key to continue . . .");
            Console.ReadKey();
            Console.Clear();
        }

        public void DrawApplicationChrome()
        {
            Console.Clear();

            Console.BackgroundColor = this.defaultBackground;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            this.dataLeft = 1;
            this.dataTop = 2;
            this.dataRight = Console.WindowWidth - 2;
            this.dataBottom = Console.WindowHeight - 3;
            this.CurrentLine = this.dataTop;

            BoxWindow();

            Console.CursorTop = 0;
            CenterText("╡ " + applicationName + " ╞");

            this.DrawHelpBar();
        }

        public void Run()
        {
            this.exitProgram = false;

            // get the next dumb message to send
            while (!this.exitProgram)
            {
                var newMessage = ReadLine();
                if (!this.exitProgram && !string.IsNullOrWhiteSpace(newMessage))
                {
                    writer.SendChatString(newMessage);
                }
            }
        }

        public void AddChatString(AFTime time, string id, string text)
        {
            lock (consoleLock)
            {
                int oldX = Console.CursorLeft;
                int oldY = Console.CursorTop;

                Console.BackgroundColor = this.defaultBackground;
                Console.SetCursorPosition(this.dataLeft, this.CurrentLine);

                if (CurrentLine < this.dataBottom)
                {
                    ++this.CurrentLine;
                }
                else
                {
                    // Scroll the data area
                    Console.MoveBufferArea(this.dataLeft, this.dataTop + 1, this.dataRight - this.dataLeft, this.dataBottom - this.dataTop - 1, this.dataLeft, this.dataTop);
                    Console.CursorTop = this.CurrentLine - 1;
                }

                ConsoleColor textColor;
                if (!this.colorMap.TryGetValue(id, out textColor))
                {
                    textColor = this.validColors[this.nextColor++ % this.validColors.Length];
                    this.colorMap[id] = textColor;
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(time.ToString("hh:mm:ss") + " ");
                Console.ForegroundColor = textColor;
                Console.Write(id);
                Console.Write(":");
                Console.WriteLine(text);

                Console.CursorLeft = oldX;
                Console.CursorTop = oldY;
            }
        }

        public void AddChatRoomName(string roomName)
        {
            lock (this.consoleLock)
            {
                var fg = Console.ForegroundColor;
                var bg = Console.BackgroundColor;

                Console.SetCursorPosition(1, 1);
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Cyan;
                var blanks = new string(' ', this.dataRight - this.dataLeft);
                Console.Write(blanks);
                Console.CursorLeft = this.dataLeft;
                Console.Write($"Room: {roomName}");

                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
            }
        }

        public int ReserveWorkArea(int numberOfLines)
        {
            if (numberOfLines <= 0)
            {
                return -1;
            }


            lock (this.consoleLock)
            {
                Console.BackgroundColor = this.defaultBackground;
                this.workAreaHeight = Math.Min(numberOfLines, 4);
                int newTop = this.dataTop + this.workAreaHeight;
                Console.MoveBufferArea(Console.WindowLeft, newTop, Console.WindowWidth, this.dataBottom - dataTop, Console.WindowLeft, this.dataTop);

                this.dataBottom -= numberOfLines;
                Console.CursorVisible = false;
            }

            return this.dataBottom + 3;
        }

        public void ReleaseWorkArea()
        {
            if (this.workAreaHeight > 0)
            {
                lock (this.consoleLock)
                {
                    int oldX = Console.CursorLeft;
                    int oldY = Console.CursorTop;

                    Console.BackgroundColor = this.defaultBackground;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.MoveBufferArea(Console.WindowLeft, this.dataBottom, Console.WindowWidth, this.workAreaHeight + 1, Console.WindowLeft, this.dataBottom + this.workAreaHeight);

                    for (int i = 0; i <= this.workAreaHeight; ++i)
                    {
                        int y = Console.WindowTop + Console.WindowHeight - this.workAreaHeight - 4 + i;
                        Console.SetCursorPosition(0, y);
                        Console.Write("║");
                        Console.SetCursorPosition(Console.WindowWidth - 1, y);
                        Console.Write("║");
                    }

                    this.dataBottom += this.workAreaHeight;
                    this.CurrentLine -= this.workAreaHeight;
                    this.workAreaHeight = 0;

                    this.DrawHelpBar();

                    Console.CursorLeft = oldX;
                    Console.CursorTop = oldY;
                    Console.CursorVisible = true;
                    Console.ForegroundColor = this.defaultForeground;
                }
            }
        }

        /// <summary>
        /// Write text centered in the current line on the console
        /// </summary>
        /// <param name="text"></param>
        private static void CenterText(string text)
        {
            Console.CursorLeft = (Console.WindowWidth - text.Length) / 2;
            Console.Write(text);
        }

        private static void Box(int left, int top, int right, int bottom)
        {
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;

            Console.SetCursorPosition(left, top);
            Console.Write("╔");
            for (int x = left+1; x < right - 1; ++x)
            {
                Console.Write("═");
            }
            Console.Write("╗");

            for(int y = top + 1; y < bottom - 1; ++y)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("║");
                Console.SetCursorPosition(right - 1, y);
                Console.Write("║");
            }

            Console.SetCursorPosition(left, bottom - 1);
            Console.Write("╚");
            for (int x = left + 1; x < right - 1; ++x)
            {
                Console.Write("═");
            }
            Console.Write("╝");

            Console.CursorLeft = oldX;
            Console.CursorTop = oldY;
        }

        private static void BoxWindow()
        {
            Box(0, 0, Console.WindowWidth, Console.WindowHeight - 1);
        }

        private string ReadLine()
        {
            int maxWidth = Console.WindowWidth - 3;

            Console.BackgroundColor =this.defaultBackground;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(this.dataLeft, this.dataBottom);

            // Blank out the input line
            Console.Write(new String(' ', this.dataRight - this.dataLeft));
            Console.CursorLeft = this.dataLeft;

            string inputString = string.Empty;
            ConsoleKeyInfo keyInfo;
            while (!this.exitProgram)
            {
                keyInfo = Console.ReadKey(true);

                // Ignore if Alt pressed
                if ((keyInfo.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
                    continue;

                // Ignore if most Ctrl Keys
                if ((keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                {
                    if (keyInfo.Key == ConsoleKey.B)
                    {
                        Console.Beep();
                    }
                    continue;
                }

                // Ignore if KeyChar value is \u0000.
                if (keyInfo.KeyChar == '\u0000')
                    continue;

                // Ignore tab key.
                if (keyInfo.Key == ConsoleKey.Tab) 
                    continue;

                // Handle backspace.
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    // Are there any characters to erase?
                    if (inputString.Length > 0)
                    {
                        lock (consoleLock)
                        {
                            // Determine where we are in the console buffer.
                            int cursorCol = Console.CursorLeft - 1;
                            int oldLength = inputString.Length;

                            inputString = inputString.Substring(0, oldLength - 1);
                            Console.CursorLeft = this.dataLeft;
                            Console.Write(inputString + new String(' ', oldLength - inputString.Length));
                            Console.CursorLeft = cursorCol;
                        }
                    }
                    continue;
                }

                // Handle Escape key.
                if (keyInfo.Key == ConsoleKey.Escape) {
                    inputString = string.Empty;
                    this.exitProgram = true;
                    break;
                }

                // Handle the Enter key
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (inputString.Length >= maxWidth - 1)
                {
                    Console.Beep();
                } 
                else
                {
                    // Handle key by adding it to input string.
                    lock (consoleLock)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.CursorLeft = this.dataLeft;
                        inputString += keyInfo.KeyChar;
                        Console.Write(inputString);
                    }
                }
            }

            // Blank out the input line
            Console.CursorLeft = this.dataLeft;
            Console.Write(new String(' ', this.dataRight - this.dataLeft));
            Console.CursorLeft = this.dataLeft;

            return inputString;
        }

        private void DrawHelpBar()
        {
            var fg = Console.ForegroundColor;
            var bg = Console.BackgroundColor;
            var oldX = Console.CursorLeft;
            var oldY = Console.CursorTop;

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            var fill = new string(' ', Console.WindowWidth - 1);
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(fill);
            Console.CursorLeft = 6;
            CenterText("Quit: Ctrl+C     New Room: Esc    Beep: Ctrl+B");

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.CursorLeft = oldX;
            Console.CursorTop = oldY;
        }

#region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Console.BackgroundColor = this.originalBackground;
                    Console.ForegroundColor = this.originalForeground;
                    Console.Title = this.originalTitle;
                    Console.WindowWidth = this.originalWidth;
                    Console.WindowHeight = this.originalHeight;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
#endregion
    }
}
