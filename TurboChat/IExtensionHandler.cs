using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboChat
{
    interface IExtensionHandler
    {
        bool CallExtension(string commandLine);
    }
}
