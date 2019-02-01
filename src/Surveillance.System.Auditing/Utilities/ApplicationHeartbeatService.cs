using System;
using System.Timers;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Systems.Auditing.Utilities.Interfaces;

namespace Surveillance.Systems.Auditing.Utilities
{
    public class ApplicationHeartbeatService : IApplicationHeartbeatService
    {
        private readonly ISystemProcessContext _context;
        private const int HeartbeatFrequency = 15000; // milliseconds

        public ApplicationHeartbeatService(ISystemProcessContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Initialise()
        {
            var timer = new Timer(HeartbeatFrequency)
            {
                AutoReset = true,
                Interval = HeartbeatFrequency
            };

            timer.Elapsed += TimerOnElapsed;
            timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _context.UpdateHeartbeat();
        }
    }
}
