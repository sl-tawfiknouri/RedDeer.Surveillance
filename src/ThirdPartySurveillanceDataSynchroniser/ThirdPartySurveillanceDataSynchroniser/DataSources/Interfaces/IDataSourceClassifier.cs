using DomainV2.Financial;

namespace ThirdPartySurveillanceDataSynchroniser.DataSources.Interfaces
{
    public interface IDataSourceClassifier
    {
        DataSource Classify(FinancialInstrument instrument);
    }
}