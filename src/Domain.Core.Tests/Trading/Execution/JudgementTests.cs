namespace Domain.Core.Tests.Trading.Execution
{
    using Domain.Core.Trading.Execution;
    using NUnit.Framework;

    [TestFixture]
    public class JudgementTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var sentiment = PriceSentiment.Mixed;
            var judgement = new Judgement(sentiment);

            Assert.AreEqual(judgement.Sentiment, sentiment);
        }
    }
}
