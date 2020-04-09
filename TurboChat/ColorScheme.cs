using System;

namespace TurboChat
{
    class ColorScheme
    {
        private ConsoleColor _defaultBackground = ConsoleColor.DarkBlue;
        private ConsoleColor _defaultForeground = ConsoleColor.Yellow;
        public ConsoleColor Background { get; set; }
        public ConsoleColor Foreground { get; set; }

        public ColorScheme()
        {
            Background = _defaultBackground;
            Foreground = _defaultForeground;
        }

        public ColorScheme(ConsoleColor back, ConsoleColor fore)
        {
            Background = back;
            Foreground = fore;
        }
    }

}
