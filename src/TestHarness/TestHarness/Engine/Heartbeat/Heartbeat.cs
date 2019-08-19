

// ReSharper disable InconsistentlySynchronizedField
namespace TestHarness.Engine.Heartbeat
{
    using System;
    using System.Timers;

    using TestHarness.Engine.Heartbeat.Interfaces;

    public class Heartbeat : IHeartbeat
    {
        private readonly Timer _activeTimer;

        private readonly object _lock = new object();

        public Heartbeat(TimeSpan beatFrequency)
        {
            this._activeTimer = new Timer { Interval = beatFrequency.TotalMilliseconds, AutoReset = true };
        }

        public void Dispose()
        {
            this._activeTimer.Dispose();
        }

        public void OnBeat(ElapsedEventHandler handler)
        {
            if (this._activeTimer != null) this._activeTimer.Elapsed += handler;
        }

        public void Start()
        {
            lock (this._lock)
            {
                this._activeTimer.Enabled = true;
            }
        }

        public void Stop()
        {
            lock (this._lock)
            {
                this._activeTimer.Enabled = false;
            }
        }
    }
}