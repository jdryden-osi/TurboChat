namespace TurboChat
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
