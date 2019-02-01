using System.Data;

namespace Surveillance.Systems.DataLayer.Interfaces
{
    public interface IConnectionStringFactory
    {
        IDbConnection BuildConn();
    }
}