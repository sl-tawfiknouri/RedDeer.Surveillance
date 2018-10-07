using System.Data;

namespace Surveillance.DataLayer.Aurora.Interfaces
{
    public interface IConnectionStringFactory
    {
        IDbConnection BuildConn();
    }
}