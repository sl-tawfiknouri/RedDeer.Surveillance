namespace TestHarness.Engine.Heartbeat
{
    using System;
    using System.Timers;

    using MathNet.Numerics.Distributions;

    using TestHarness.Engine.Heartbeat.Interfaces;

    public class IrregularHeartbeat : IHeartbeat
    {
        private readonly Timer _activeTimer;

        private readonly TimeSpan _coreHeartbeat;

        private readonly object _lock = new object();

        private readonly double _sd;

        /// <summary>
        ///     Ensure the SD is high enough we can observe the difference in the beats
        /// </summary>
        public IrregularHeartbeat(TimeSpan heartbeat, double sd)
        {
            this._coreHeartbeat = heartbeat;

            this._sd = sd;

            this._activeTimer = new Timer { Interval = heartbeat.TotalMilliseconds };
            this._activeTimer.Elapsed += this.PaceMaker;
            this._activeTimer.AutoReset = false;
        }

        public void Dispose()
        {
            lock (this._lock)
            {
                this._activeTimer.Dispose();
            }
        }

        public void OnBeat(ElapsedEventHandler handler)
        {
            lock (this._lock)
            {
                this._activeTimer.Elapsed += handler;
            }
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

        private void PaceMaker(object sender, ElapsedEventArgs e)
        {
            var milliseconds = this._coreHeartbeat.TotalMilliseconds;
            var subsequentBeat = Normal.Sample(milliseconds, this._sd);
            subsequentBeat = Math.Max(subsequentBeat, 5);

            this._activeTimer.Interval = subsequentBeat;
            this._activeTimer.Enabled = true;
        }
    }
}