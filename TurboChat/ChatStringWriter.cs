using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var msg = newMessage.Trim();

                if (options.ExtensionHandler != null && msg.StartsWith("$/"))
                {
                    options.ExtensionHandler.CallExtension(msg.Substring(2));
                }

                WriteMessage(this.options.Point, this.options.Name, newMessage);
            }

            return true;
        }

        static void WriteMessage(OSIsoft.AF.PI.PIPoint point, string username, string message)
        {
            var value = new AFValue($"{username,-10}:{message}", AFTime.Now);
            point.UpdateValue(value, AFUpdateOption.Insert);
        }
    }
}
