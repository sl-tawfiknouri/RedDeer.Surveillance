namespace Surveillance.Auditing.Utilities
{
    using System;
    using System.Timers;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.Utilities.Interfaces;

    public class ApplicationHeartbeatService : IApplicationHeartbeatService
    {
        private const int HeartbeatFrequency = 15000; // milliseconds

        private readonly ISystemProcessContext _context;

        public ApplicationHeartbeatService(ISystemProcessContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Initialise()
        {
            var timer = new Timer(HeartbeatFrequency) { AutoReset = true, Interval = HeartbeatFrequency };

            timer.Elapsed += this.TimerOnElapsed;
            timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            this._context.UpdateHeartbeat();
        }
    }
}