namespace Turbo_PI_Chat
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class that provides all UI for the main application
    /// </summary>
    class TextUserInterface : IDisposable, IChatStringDisplay
    {
        private const string logo = @"
 ________  ___          _________  ___  ___  ________  ________  ________          ________  ___  ___  ________  _________   
|\   __  \|\  \        |\___   ___\\  \|\  \|\   __  \|\   __  \|\   __  \        |\   ____\|\  \|\  \|\   __  \|\___   ___\ 
\ \  \|\  \ \  \       \|___ \  \_\ \  \\\  \ \  \|\  \ \  \|\ /\ \  \|\  \       \ \  \___|\ \  \\\  \ \  \|\  \|___ \  \_| 
 \ \   ____\ \  \           \ \  \ \ \  \\\  \ \   _  _\ \   __  \ \  \\\  \       \ \  \    \ \   __  \ \   __  \   \ \  \  
  \ \  \___|\ \  \           \ \  \ \ \  \\\  \ \  \\  \\ \  \|\  \ \  \\\  \       \ \  \____\ \  \ \  \ \  \ \  \   \ \  \ 
   \ \__\    \ \__\           \ \__\ \ \_______\ \__\\ _\\ \_______\ \_______\       \ \_______\ \__\ \__\ \__\ \__\   \ \__\
    \|__|     \|__|            \|__|  \|_______|\|__|\|__|\|_______|\|_______|        \|_______|\|__|\|__|\|__|\|__|    \|__|";

        private ConsoleColor originalBackground;
        private ConsoleColor originalForeground;
        private string originalTitle;
        private int originalWidth;
        private int originalHeight;

        private object consoleLock = new object();

        private Dictionary<string, ConsoleColor> colorMap = new Dictionary<string, ConsoleColor>();

        public TextUserInterface()
        {
            this.originalBackground = Console.BackgroundColor;
            this.originalForeground = Console.ForegroundColor;
            this.originalTitle = Console.Title;
            this.originalWidth = Console.WindowWidth;
            this.originalHeight = Console.WindowHeight;
            
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
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Remove Window Scroll bar
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            Console.Clear();
            Console.Title = "TURBO PI-CHAT";

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
        }

        public void DrawApplicationChrome()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            BoxWindow();

            Console.ReadKey();
            Console.Beep();
        }

        public void AddChatString(DateTime time, string id, string text)
        {
            lock (consoleLock)
            {
                ConsoleColor textColor;
                if (!this.colorMap.TryGetValue(id, out textColor))
                {
                    textColor = ConsoleColor.White; //?? get unique color
                    this.colorMap[id] = textColor;
                }

                Console.Write(time.ToString("hh:mm:ss") + ": ");
                Console.WriteLine(text);
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
            Box(0, 0, Console.WindowWidth - 1, Console.WindowHeight - 1);
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
