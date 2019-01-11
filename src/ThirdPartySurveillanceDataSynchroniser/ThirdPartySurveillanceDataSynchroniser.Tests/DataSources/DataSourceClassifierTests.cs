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

            var result = classifier.Classify(null);

            Assert.AreEqual(result, DataSource.None);
        }

        [Test]
        public void Classify_NonNull_WithEquityCfi_Returns_Bmll()
        {
            var classifier = BuildClassifier();

            var result = classifier.Classify("entspb");

            Assert.AreEqual(result, DataSource.Bmll);
        }

        [Test]
        public void Classify_NonNull_WithDebtCfi_Returns_Markit()
        {
            var classifier = BuildClassifier();

            var result = classifier.Classify("DBF");

            Assert.AreEqual(result, DataSource.Markit);
        }

        private DataSourceClassifier BuildClassifier()
        {
            return new DataSourceClassifier();
        }
    }
}
