using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Shell.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser
{
    public class Mediator : IMediator
    {
        private readonly IShellFactset _shellFactset;
        private readonly IShellBmll _shellBmll;
        private readonly IShellRepo _shellRepo;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IShellFactset shellFactset,
            IShellBmll shellBmll,
            IShellRepo shellRepo,
            ILogger<Mediator> logger)
        {
            _shellFactset = shellFactset;
            _shellBmll = shellBmll;
            _shellRepo = shellRepo;
            _logger = logger;
        }

        public void Initiate()
        {
            while (true)
            {
                _logger.LogInformation($"   D A T A   S Y N C H R O N I S E R  | process-id {Process.GetCurrentProcess().Id} | started-at {Process.GetCurrentProcess().StartTime}");

                try
                {
                    var cts1 = new CancellationTokenSource(1000 * 15);
                    var task1 = _shellFactset.HeartBeating(cts1.Token);
                    task1.Wait();
                    var task1Result = task1.Result;
                    _logger.LogInformation($"Mediator factset shell heartbeat has a result of {task1Result}");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Mediator factset shell encountered an exception! {e.Message} {e.InnerException?.Message}", e);
                }

                try
                {
                    var cts2 = new CancellationTokenSource(1000 * 15);
                    var task2 = _shellBmll.HeartBeating(cts2.Token);
                    task2.Wait();
                    var task2Result = task2.Result;
                    _logger.LogInformation($"Mediator bmll shell heartbeat has a result of {task2Result}");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Mediator bmll shell encountered an exception! {e.Message} {e.InnerException?.Message}", e);
                }

                try
                {
                    var cts3 = new CancellationTokenSource(1000 * 15);
                    var task3 = _shellRepo.CanHitDb(cts3);
                    task3.Wait();
                    var task3Result = task3.Result;
                    _logger.LogInformation($"Mediator repository shell heartbeat has a result of {task3Result}");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Mediator repository shell encountered an exception! {e.Message} {e.InnerException?.Message}", e);
                }

                Thread.Sleep(15 * 1000);
            }
        }
    }
}
