using DomainV2.Financial;
using DomainV2.Financial.Cfis;
using ThirdPartySurveillanceDataSynchroniser.DataSources.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.DataSources
{
    public class DataSourceClassifier : IDataSourceClassifier
    {       
        public DataSource Classify(FinancialInstrument instrument)
        {
            if (instrument == null)
            {
                return DataSource.None;
            }

            if (string.IsNullOrWhiteSpace(instrument.Cfi))
            {
                return DataSource.None;
            }

            var cfi = new Cfi(instrument.Cfi);

            switch (cfi.CfiCategory)
            {
                case CfiCategory.Equities:
                    return DataSource.Bmll;
                case CfiCategory.DebtInstrument:
                    return DataSource.Markit;
                case CfiCategory.Entitlements:
                    return DataSource.None;
                case CfiCategory.Futures:
                    return DataSource.None;
                case CfiCategory.None:
                    return DataSource.None;
                case CfiCategory.Options:
                    return DataSource.None;
                case CfiCategory.Others:
                    return DataSource.None;
                default:
                    return DataSource.None;
            }
        }
    }
}
