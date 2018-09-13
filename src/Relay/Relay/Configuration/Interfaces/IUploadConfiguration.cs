namespace Relay.Configuration.Interfaces
{
    public interface IUploadConfiguration
    {
        string RelayTradeFileUploadDirectoryPath { get; }
        string RelayEquityFileUploadDirectoryPath { get; }
    }
}
