﻿using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands.Market_Abuse_Commands
{
    public class TradeProhibitedSecurityCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        public TradeProhibitedSecurityCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return
                string.Equals(command, "run prohibited trade", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(command, "buy prohibited security", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(command, "buy lehman bros", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            _appFactory.ProhibitedSecurityHeartbeat?.Pulse();
        }
    }
}
