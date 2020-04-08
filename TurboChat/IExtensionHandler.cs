using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboChat
{
    interface IExtensionHandler
    {
        void ProcessMessage(TurboChatOptions options, string commandLine);
    }
}
