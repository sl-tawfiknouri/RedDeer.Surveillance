using System;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using Surveillance.Engine.Rules.RuleParameters.Services;

namespace Surveillance.Engine.Rules.Tests.RuleParameters.Manager
{
    [TestFixture]
    public class RuleParameterLeadingTimespanCalculatorTests
    {
        [Test]
        public void LeadingTimespan_Returns_Zero_For_Null()
        {
            var calculator = new RuleParameterAdjustedTimespanService();

            var result = calculator.LeadingTimespan(null);

            Assert.AreEqual(result, TimeSpan.Zero);
        }

        [Test]
        public void LeadingTimespan_Returns_Zero_For_Null_Collections()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto();

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.Zero);
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_Only_Cancelled()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                CancelledOrders = new CancelledOrderRuleParameterDto[]
                {
                    new CancelledOrderRuleParameterDto
                    {
                        WindowSize = TimeSpan.FromMinutes(5)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(5));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_Only_HighProfit()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                HighProfits = new HighProfitsRuleParameterDto[]
                {
                    new HighProfitsRuleParameterDto
                    {
                        WindowSize = TimeSpan.FromMinutes(6)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(6));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_Only_MarkingTheClose()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                MarkingTheCloses = new MarkingTheCloseRuleParameterDto[]
                {
                    new MarkingTheCloseRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(7)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(7));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_Only_Spoofing()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                Spoofings = new SpoofingRuleParameterDto[]
                {
                    new SpoofingRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(8)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(8));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_Only_Layering()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                Layerings = new LayeringRuleParameterDto[]
                {
                    new LayeringRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(9)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(9));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_Only_HighVolume()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                HighVolumes = new HighVolumeRuleParameterDto[]
                {
                    new HighVolumeRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(10)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(10));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_Only_WashTrade()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                WashTrades = new WashTradeRuleParameterDto[]
                {
                    new WashTradeRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(11)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(11));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_TwoAlternatives_WashTrade()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                WashTrades = new WashTradeRuleParameterDto[]
                {
                    new WashTradeRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(8)
                    }
                    ,
                    new WashTradeRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(11)
                    },
                    new WashTradeRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(2)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(11));
        }

        [Test]
        public void LeadingTimespan_Returns_Value_When_TwoAlternatives_WashTrade_And_HighVolume()
        {
            var calculator = new RuleParameterAdjustedTimespanService();
            var dto = new RuleParameterDto
            {
                WashTrades = new WashTradeRuleParameterDto[]
                {
                    new WashTradeRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(8)
                    }
                },
                HighVolumes = new HighVolumeRuleParameterDto[]
                {
                    new HighVolumeRuleParameterDto()
                    {
                        WindowSize = TimeSpan.FromMinutes(10)
                    }
                }
            };

            var result = calculator.LeadingTimespan(dto);

            Assert.AreEqual(result, TimeSpan.FromMinutes(10));
        }

    }
}
