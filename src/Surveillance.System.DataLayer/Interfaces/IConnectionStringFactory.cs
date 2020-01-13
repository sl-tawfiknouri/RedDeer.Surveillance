namespace Surveillance.Auditing.DataLayer.Interfaces
{
    using System.Data;

    public interface IConnectionStringFactory
    {
        IDbConnection BuildConn();
        string OverrideMigrationsFolder();
    }
}