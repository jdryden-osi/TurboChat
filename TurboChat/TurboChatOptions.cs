using OSIsoft.AF.PI;

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
