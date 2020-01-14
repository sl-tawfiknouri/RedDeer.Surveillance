using System.IO;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface ICSVService
    {
        int ReplaceNewLines(string fileName, Stream sourceStream, Stream destinationStream);
    }
}