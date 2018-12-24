using System;

namespace ViricitiReceiver.Messages
{
    public class MessageReceived
    {
        public string Message { get; }
        public DateTime ReceivedDateUtc { get; }

        public MessageReceived(string message, DateTime receivedDateUtc)
        {
            Message = message;
            ReceivedDateUtc = receivedDateUtc.ToUniversalTime(); // Just in case it's not yet UTC
        }
    }
}
