using System;
using System.Timers;
using TestHarness.Engine.Heartbeat.Interfaces;

namespace TestHarness.Engine.Heartbeat
{
    public class Heartbeat : IHeartbeat
    {
        private TimeSpan _beatFrequency;
        private Timer _activeTimer;

        private object _lock = new object();

        public Heartbeat(TimeSpan beatFrequency)
        {
            _beatFrequency = beatFrequency;

            _activeTimer = new Timer();
            _activeTimer.Interval = _beatFrequency.TotalMilliseconds;
            _activeTimer.AutoReset = true;
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
