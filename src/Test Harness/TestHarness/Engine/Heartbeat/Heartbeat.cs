using System;
using System.Timers;
using TestHarness.Engine.Heartbeat.Interfaces;
// ReSharper disable InconsistentlySynchronizedField

namespace TestHarness.Engine.Heartbeat
{
    public class Heartbeat : IHeartbeat
    {
        private readonly Timer _activeTimer;

        private readonly object _lock = new object();

        public Heartbeat(TimeSpan beatFrequency)
        {
            _activeTimer = new Timer
            {
                Interval = beatFrequency.TotalMilliseconds,
                AutoReset = true
            };
        }

        public void OnBeat(ElapsedEventHandler handler)
        {
            if (_activeTimer != null)
            {
                _activeTimer.Elapsed += handler;
            }
        }

        public void Start()
        {
            lock (_lock)
            {
                _activeTimer.Enabled = true;
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                _activeTimer.Enabled = false;
            }
        }

        public void Dispose()
        {
            _activeTimer.Dispose();
        }
    }
}
