using NUnit.Framework;
using ThirdPartySurveillanceDataSynchroniser.DataSources;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.DataSources
{
    [TestFixture]
    public class DataSourceClassifierTests
    {
        [Test]
        public void Classify_Null_Instrument_Returns_None()
        {
            var classifier = BuildClassifier();

            var result = classifier.Classify(null);

            Assert.AreEqual(result, DataSource.None);
        }

        [Test]
        public void Classify_NonNull_WithNullCfi_Returns_None()
        {
            var classifier = BuildClassifier();
            var instrument = TestHelpers.Helpers.FinancialInstrument();
            instrument.Cfi = null;

            var result = classifier.Classify(instrument);

            Assert.AreEqual(result, DataSource.None);
        }

        [Test]
        public void Classify_NonNull_WithEquityCfi_Returns_Bmll()
        {
            var classifier = BuildClassifier();
            var instrument = TestHelpers.Helpers.FinancialInstrument();
            instrument.Cfi = "entspb"; // equity shares

            var result = classifier.Classify(instrument);

            Assert.AreEqual(result, DataSource.Bmll);
        }

        [Test]
        public void Classify_NonNull_WithDebtCfi_Returns_Markit()
        {
            var classifier = BuildClassifier();
            var instrument = TestHelpers.Helpers.FinancialInstrument();
            instrument.Cfi = "DBF"; // fixed interest rate bond

            var result = classifier.Classify(instrument);

            Assert.AreEqual(result, DataSource.Markit);
        }

        private DataSourceClassifier BuildClassifier()
        {
            return new DataSourceClassifier();
        }
    }
}
