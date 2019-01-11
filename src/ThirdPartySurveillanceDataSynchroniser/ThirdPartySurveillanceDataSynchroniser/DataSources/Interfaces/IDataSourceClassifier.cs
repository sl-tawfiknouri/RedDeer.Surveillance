namespace ThirdPartySurveillanceDataSynchroniser.DataSources.Interfaces
{
    public interface IDataSourceClassifier
    {
        DataSource Classify(string cfi);
    }
}