using Microsoft.Extensions.Logging;
using System;
using Utilities.Network_IO.Interfaces;

namespace Relay.Network_IO
{
    public class LoggingMessageWriter : IMessageWriter
    {
        ILogger _logger;

        public LoggingMessageWriter(ILogger<LoggingMessageWriter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Write(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _logger.LogInformation(message);
        }
    }
}
