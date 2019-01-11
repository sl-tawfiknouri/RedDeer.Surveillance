using DomainV2.Financial.Cfis;
using ThirdPartySurveillanceDataSynchroniser.DataSources.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.DataSources
{
    public class DataSourceClassifier : IDataSourceClassifier
    {       
        public DataSource Classify(string cfiStr)
        {
            if (string.IsNullOrWhiteSpace(cfiStr))
            {
                return DataSource.None;
            }

            var cfi = new Cfi(cfiStr);

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
