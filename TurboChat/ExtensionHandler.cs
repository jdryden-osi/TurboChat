namespace TurboChat
{
    class ExtensionHandler : IExtensionHandler
    {
        private IChatStringDisplay ui;

        public ExtensionHandler(IChatStringDisplay ui)
        {
            this.ui = ui;
        }

        public bool CallExtension(string commandLine)
        {
            // Parse out the command

            // Call the appropriate handler

            // Return true if handled, false if not handled
            return false;
        }
    }
}
