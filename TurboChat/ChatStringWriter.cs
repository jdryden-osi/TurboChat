namespace TurboChat
{
    class ChatStringWriter : IChatStringWriter
    {
        private TurboChatOptions options;

        public ChatStringWriter(TurboChatOptions options)
        {
            this.options = options;
        }

        public bool SendChatString(string newMessage)
        {
            if (!string.IsNullOrWhiteSpace(newMessage))
            {
                if (options.ExtensionHandler != null)
                {
                    options.ExtensionHandler.ProcessMessage(options, newMessage.Trim());
                }
            }

            return true;
        }
    }
}
