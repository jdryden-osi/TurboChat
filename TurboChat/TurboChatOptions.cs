using OSIsoft.AF.PI;

namespace TurboChat
{
    class TurboChatOptions
    {
        public string Name { get; private set; }
        public PIPoint Point { get; private set; }
        public IExtensionHandler ExtensionHandler { get; set; }

        public TurboChatOptions(string name, PIPoint point)
        {
            this.Name = name;
            this.Point = point;
        }
    }
}
