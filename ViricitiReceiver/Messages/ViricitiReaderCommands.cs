using System;

namespace ViricitiReceiver.Messages
{
    public class StartProcessing
    {
        public TimeSpan ConnectionTimeout { get; }

        /// <summary>
        /// Default connectiontimeout of 5 seconds
        /// </summary>
        public StartProcessing()
            : this (TimeSpan.FromSeconds(5))
        {
        }

        public StartProcessing(TimeSpan connectionTimeout)
        {
            ConnectionTimeout = connectionTimeout;
        }
    }

    public class ContinueProcessing
    {
        public TimeSpan ReceiveTimeout { get; }

        /// <summary>
        /// Default timeout of 30 seconds
        /// </summary>
        public ContinueProcessing()
            : this (TimeSpan.FromSeconds(30))
        {
        }

        public ContinueProcessing(TimeSpan receiveTimeout)
        {
            ReceiveTimeout = receiveTimeout;
        }
    }

    public class StopProcessing
    {
        public TimeSpan ConnectionTimeout { get; }

        /// <summary>
        /// Default connectiontimeout of 5 seconds
        /// </summary>
        public StopProcessing()
            : this(TimeSpan.FromSeconds(5))
        {
        }

        public StopProcessing(TimeSpan connectionTimeout)
        {
            ConnectionTimeout = connectionTimeout;
        }
    }
}
