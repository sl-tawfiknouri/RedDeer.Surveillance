using System;
using Domain.Surveillance.Judgements.Equity;
using Microsoft.Extensions.Logging;

namespace Surveillance.Engine.Rules.Judgements
{
    public class JudgementService
    {
        private readonly ILogger<JudgementService> _logger;

        public JudgementService(ILogger<JudgementService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Judgement(HighProfitJudgement highProfit)
        {
            if (highProfit == null)
            {
                _logger?.LogError($"High Profit Judgement was null");
                return;
            }

            // save high profit judgement
            // send high profit judgements onward to database
            // something to retrieve judgements -sort them into db



        }
    }
}
