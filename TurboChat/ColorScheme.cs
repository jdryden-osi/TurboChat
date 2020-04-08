using System;

namespace TurboChat
{
    class ColorScheme
    {
        public ConsoleColor background { get; set; }
        public ConsoleColor foreground { get; set; }

        public ColorScheme(ConsoleColor back, ConsoleColor fore)
        {
            background = back;
            foreground = fore;
        }
    }

}
