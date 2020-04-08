namespace TurboChat
{
    using OSIsoft.AF.Time;
    using System;

    /// <summary>
    /// Interface for an object that displays chat strings
    /// </summary>
    interface IChatStringDisplay
    {
        void AddChatString(AFTime time, string id, string text);
    }
}
