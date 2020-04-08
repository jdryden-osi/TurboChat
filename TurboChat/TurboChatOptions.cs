using OSIsoft.AF.PI;
using System;

namespace TurboChat
{
    class TurboChatOptions
    {
        public string Name { get; set; }
        public PIPoint Point { get; set; }
        public IExtensionHandler ExtensionHandler { get; set; }
        private ColorScheme colors;
        public ColorScheme Colors
        {
            get { return colors; }
            set { 
                colors = value;
                Console.ForegroundColor = colors.foreground;
                Console.BackgroundColor = colors.background;
                Console.Clear();
            }
        }
        public TurboChatOptions()
        {
        }
    }
}
