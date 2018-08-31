using System;
using Domain.Equity.Trading.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Spoofing.Interfaces;

namespace Surveillance.Rules.Spoofing
{
    public class SpoofingRule : ISpoofingRule
    {
        private ILogger _logger;

        public SpoofingRule(ILogger<SpoofingRule> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
            _logger.LogInformation("Spoofing rule stream completed.");
        }

        public void OnError(Exception error)
        {
            if (error == null)
            {
                return;
            }

            _logger.LogError($"An error occured in the spoofing rule stream {error.ToString()}");
        }

        public void OnNext(TradeOrderFrame value)
        {
            if (value == null)
            {
                return;
            }

            // add to collection of orders...
            // we need something that just accepts orders into list of lists and has an update on X% of sample window 
            // after it does the update shuffle, execute the spoofing check against the "active" window part of the list
            // keep the other part of the list active in memory for the user setting different values of their sample window
            // retire items from memory after the hard spoofing check limit i.e. 1 week?
        }
    }
}
