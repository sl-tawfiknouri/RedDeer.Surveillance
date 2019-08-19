namespace Domain.Core.Tests.Financial.Money
{
    using Domain.Core.Financial.Money;

    using NUnit.Framework;

    [TestFixture]
    public class CurrencyTests
    {
        [Test]
        public void Constructor_Assigns_Correctly()
        {
            var code = "GBP";
            var shortName = "STERLING";
            var longName = "POUND STERLING";
            var subDivisionCode = "GBX";
            var symbol = "GBP";

            var currency = new Currency(code, shortName, longName, subDivisionCode, symbol);

            Assert.AreEqual(currency.Code, code);
            Assert.AreEqual(currency.ShortName, shortName);
            Assert.AreEqual(currency.LongName, longName);
            Assert.AreEqual(currency.SubDivisionCode, subDivisionCode);
            Assert.AreEqual(currency.Symbol, symbol);
        }

        [Test]
        public void Constructor_Handles_NullAllArgs()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new Currency(null, null, null, null, null));
        }

        [Test]
        public void Constructor_Handles_NullCode()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new Currency(null));
        }

        [TestCase("a", "a", true)]
        [TestCase("a", "A", true)]
        [TestCase("A", "a", true)]
        [TestCase("a", "b", false)]
        [TestCase("b", "a", false)]
        [TestCase("B", "a", false)]
        [TestCase("a", "B", false)]
        [TestCase("a", "", false)]
        [TestCase("", "B", false)]
        public void Equals_ReturnsExpected_ForCodes(string codeA, string codeB, bool expectedResult)
        {
            var currencyA = new Currency(codeA);
            var currencyB = new Currency(codeB);

            var result = currencyA.Equals(currencyB);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Equals_ReturnsFalse_WhenIsNull()
        {
            var currency = new Currency("a-currency");

            var result = currency.Equals(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ReturnsFalse_WhenNotCurrency()
        {
            var currency = new Currency("a-currency");

            var result = currency.Equals("not-a-currency-struct");

            Assert.IsFalse(result);
        }

        [Test]
        public void GetHashCode_ReturnsCodeHashCode_ForCurrency()
        {
            var currency = new Currency("a-currency");

            var hashCode = currency.GetHashCode();

            Assert.AreEqual("A-CURRENCY".GetHashCode(), hashCode);
        }

        [Test]
        public void ToString_ShouldReturnCode_AsExpected()
        {
            var currency = new Currency("A CODE");

            var result = currency.ToString();

            Assert.AreEqual("A CODE", result);
        }

        [Test]
        public void ToString_ShouldReturnCodeForNull_AsExpected()
        {
            var currency = new Currency(null);

            var result = currency.ToString();

            Assert.AreEqual(string.Empty, result);
        }
    }
}