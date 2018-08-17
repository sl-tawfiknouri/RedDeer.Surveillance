using System;

namespace TestHarness.Commands
{
    public class InitiateCommand
    {
        public InitiateCommand(
            bool generateSecurityMarket,
            bool generateSecurityTrades,
            bool initiateOnStartup,
            bool outputDisplay,
            TimeSpan frequency)
        {
            GenerateSecurityMarket = generateSecurityMarket;
            GenerateSecurityTrades = generateSecurityTrades;
            InitiateOnStartup = initiateOnStartup;
            OutputDisplay = outputDisplay;
            Frequency = frequency;
        }

        public bool GenerateSecurityMarket { get; }

        public bool GenerateSecurityTrades { get; }

        public bool InitiateOnStartup { get; }

        public bool OutputDisplay { get; }

        public TimeSpan Frequency { get; }

    }
}
