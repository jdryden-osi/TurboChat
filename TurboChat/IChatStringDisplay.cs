namespace Turbo_PI_Chat
{
    using System;

    /// <summary>
    /// Interface for an object that displays chat strings
    /// </summary>
    interface IChatStringDisplay
    {
        void AddChatString(DateTime time, string id, string text);
    }
}
