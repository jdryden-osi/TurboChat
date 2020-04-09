using OSIsoft.AF.PI;
using System;

namespace TurboChat
{
    class TurboChatOptions
    {
        public string Name { get; set; }
        public PIPoint Point { get; set; }
        public IExtensionHandler ExtensionHandler { get; set; }
        public TurboChatOptions()
        {
        }
    }
}
