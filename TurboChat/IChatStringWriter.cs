namespace Turbo_PI_Chat
{
    using System;

    /// <summary>
    /// Interface for object that will send a chat string
    /// </summary>
    interface IChatStringWriter
    {
        bool SendChatString(string text);
    }
}
