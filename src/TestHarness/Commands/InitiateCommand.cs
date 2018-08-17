using System;

namespace TestHarness.Commands
{
    public class InitiateCommand
    {
        public InitiateCommand(
            bool generateSecurityMarket,
            bool generateSecurityTrades,
            bool initiateOnStartup,
            TimeSpan frequency)
        {
            GenerateSecurityMarket = generateSecurityMarket;
            GenerateSecurityTrades = generateSecurityTrades;
            InitiateOnStartup = initiateOnStartup;
            Frequency = frequency;
        }

        public bool GenerateSecurityMarket { get; }

        public bool GenerateSecurityTrades { get; }

        public bool InitiateOnStartup { get; }

        public TimeSpan Frequency { get; }
    }
}
