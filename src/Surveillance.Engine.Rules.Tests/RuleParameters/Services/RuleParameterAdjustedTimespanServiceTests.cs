namespace Surveillance.Engine.Rules.Tests.RuleParameters.Services
{
    using System;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;

    using Surveillance.Engine.Rules.RuleParameters.Services;

    /// <summary>
    /// The rule parameter adjusted timespan service tests.
    /// </summary>
    [TestFixture]
    public class RuleParameterAdjustedTimespanServiceTests
    {
        /// <summary>
        /// The leading timespan null is timespan zero.
        /// </summary>
        [Test]
        public void LeadingTimespanNullIsTimespanZero()
        {
            var service = this.BuildService();

            var trail = service.LeadingTimespan(null);

            Assert.AreEqual(TimeSpan.Zero, trail);
        }

        /// <summary>
        /// The leading timespan empty is timespan zero.
        /// </summary>
        [Test]
        public void LeadingTimespanEmptyIsTimespanZero()
        {
            var service = this.BuildService();

            var trail = service.LeadingTimespan(new RuleParameterDto());

            Assert.AreEqual(TimeSpan.Zero, trail);
        }

        /// <summary>
        /// The leading timespan is timespan twelve days.
        /// </summary>
        [Test]
        public void LeadingTimespanIsTimespanTwelveDays()
        {
            var service = this.BuildService();

            var dto = new RuleParameterDto
              {
                  CancelledOrders = new[] { new CancelledOrderRuleParameterDto { WindowSize = TimeSpan.FromDays(1) } },
                  HighProfits = new[] { new HighProfitsRuleParameterDto { WindowSize = TimeSpan.FromDays(2) } },
                  MarkingTheCloses = new[] { new MarkingTheCloseRuleParameterDto { WindowSize = TimeSpan.FromDays(3) } },
                  Spoofings = new[] { new SpoofingRuleParameterDto { WindowSize = TimeSpan.FromDays(4) } },
                  Layerings = new[] { new LayeringRuleParameterDto { WindowSize = TimeSpan.FromDays(5) } },
                  HighVolumes = new[] { new HighVolumeRuleParameterDto { WindowSize = TimeSpan.FromDays(6) } },
                  WashTrades = new[] { new WashTradeRuleParameterDto { WindowSize = TimeSpan.FromDays(7) } },
                  Rampings = new[] { new RampingRuleParameterDto { WindowSize = TimeSpan.FromDays(8) } },
                  PlacingOrders = new[] { new PlacingOrdersWithNoIntentToExecuteRuleParameterDto { WindowSize = TimeSpan.FromDays(9) } },
                  FixedIncomeWashTrades = new[] { new FixedIncomeWashTradeRuleParameterDto { WindowSize = TimeSpan.FromDays(10) } },
                  FixedIncomeHighProfits = new[] { new FixedIncomeHighProfitRuleParameterDto { WindowSize = TimeSpan.FromDays(11) } },
                  FixedIncomeHighVolumeIssuance = new[] { new FixedIncomeHighVolumeIssuanceRuleParameterDto { WindowSize = TimeSpan.FromDays(12) } }
              };

            var trail = service.LeadingTimespan(dto);

            Assert.AreEqual(TimeSpan.FromDays(12), trail);
        }

        /// <summary>
        /// The trailing timespan null is timespan zero.
        /// </summary>
        [Test]
        public void TrailingTimespanNullIsTimespanZero()
        {
            var service = this.BuildService();

            var trail = service.TrailingTimeSpan(null);

            Assert.AreEqual(TimeSpan.Zero, trail);
        }

        /// <summary>
        /// The trailing timespan three days is timespan three days.
        /// </summary>
        [Test]
        public void TrailingTimespanThreeDaysIsTimespanThreeDays()
        {
            var service = this.BuildService();

            var dto = new RuleParameterDto
              {
                  HighProfits = new HighProfitsRuleParameterDto[]
                    {
                        new HighProfitsRuleParameterDto
                        {
                            ForwardWindow = TimeSpan.FromDays(2)
                        },
                        new HighProfitsRuleParameterDto
                        {
                            ForwardWindow = TimeSpan.FromDays(3)
                        },
                        new HighProfitsRuleParameterDto
                        {
                            ForwardWindow = TimeSpan.FromDays(1)
                        }
                    },
              };

            var trail = service.TrailingTimeSpan(dto);

            Assert.AreEqual(TimeSpan.FromDays(3), trail);
        }

        /// <summary>
        /// The trailing timespan three days only in cancelled order is timespan three days.
        /// </summary>
        [Test]
        public void TrailingTimespanThreeDaysOnlyInCancelledOrderIsTimespanThreeDays()
        {
            var service = this.BuildService();

            var dto = new RuleParameterDto
              {
                  CancelledOrders = new CancelledOrderRuleParameterDto[]
                    {
                        new CancelledOrderRuleParameterDto
                            {
                                WindowSize = TimeSpan.FromDays(3)
                            }
                    }
              };

            var trail = service.TrailingTimeSpan(dto);

            Assert.AreEqual(TimeSpan.FromDays(0), trail);
        }

        /// <summary>
        /// The build service.
        /// </summary>
        /// <returns>
        /// The <see cref="RuleParameterAdjustedTimespanService"/>.
        /// </returns>
        private RuleParameterAdjustedTimespanService BuildService()
        {
            return new RuleParameterAdjustedTimespanService();
        }
    }
}
