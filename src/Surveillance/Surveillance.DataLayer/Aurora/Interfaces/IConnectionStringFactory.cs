namespace Surveillance.DataLayer.Aurora.Interfaces
{
    using System.Data;

    public interface IConnectionStringFactory
    {
        IDbConnection BuildConn();
    }
}