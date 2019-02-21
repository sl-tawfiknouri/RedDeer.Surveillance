namespace DataSynchroniser.DataSources.Interfaces
{
    public interface IDataSourceClassifier
    {
        DataSource Classify(string cfi);
    }
}