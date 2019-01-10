using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Shell.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Shell
{
    public class ShellRepo : IShellRepo
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ShellRepo> _logger;

        public ShellRepo(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ShellRepo> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<bool> CanHitDb(CancellationTokenSource cts)
        {
            _logger.LogInformation($"ShellRepo starting can hit db method");

            try
            {
                var timeTask = Task.Run(() =>
                {
                    var conn = _dbConnectionFactory.BuildConn();

                    if (conn == null)
                    {
                        _logger.LogError($"ShellRepo connection was null! Returning!!");
                        return false;
                    }

                    _logger.LogInformation($"ShellRepo opening connection to {conn.ConnectionString}");

                    conn.Open();
                    Thread.Sleep(5 * 1000);
                    conn.Close();
                    _logger.LogInformation($"ShellRepo closing connection to {conn.ConnectionString}");

                    return true;
                });

                timeTask.Wait(cts.Token);

                if (cts.Token.IsCancellationRequested)
                {
                    _logger.LogInformation($"ShellRepo timed out connecting to the database");
                    return false;
                }

                _logger.LogInformation($"ShellRepo starting completed hit db method");
                return timeTask.Result;
            }
            catch (Exception e)
            {
                _logger?.LogError($"ShellRepo caught an exception! {e.Message} {e.InnerException?.Message} {_dbConnectionFactory?.BuildConn()?.ConnectionString}", e);

                return false;
            }
        }
    }
}
