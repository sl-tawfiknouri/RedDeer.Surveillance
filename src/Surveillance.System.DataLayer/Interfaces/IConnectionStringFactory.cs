using System.Data;

namespace Surveillance.Auditing.DataLayer.Interfaces
{
    public interface IConnectionStringFactory
    {
        IDbConnection BuildConn();
    }
}