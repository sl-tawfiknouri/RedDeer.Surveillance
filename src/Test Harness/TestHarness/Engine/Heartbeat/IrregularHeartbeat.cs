using MathNet.Numerics.Distributions;
using System;
using System.Timers;
using TestHarness.Engine.Heartbeat.Interfaces;

namespace TestHarness.Engine.Heartbeat
{
    public class IrregularHeartbeat : IHeartbeat
    {
        private readonly double _sd;
        private readonly TimeSpan _coreHeartbeat;
        private readonly Timer _activeTimer;

        private readonly object _lock = new object();

        /// <summary>
        /// Ensure the SD is high enough we can observe the difference in the beats
        /// </summary>
        public IrregularHeartbeat(TimeSpan heartbeat, double sd)
        {
            _coreHeartbeat = heartbeat;

            _sd = sd;

            _activeTimer = new Timer();
            _activeTimer.Interval = heartbeat.TotalMilliseconds;
            _activeTimer.Elapsed += PaceMaker;
            _activeTimer.AutoReset = false;
        }

        public void Start()
        {
            lock (_lock)
            {
                _activeTimer.Enabled = true;
            }
        }

        private void PaceMaker(object sender, ElapsedEventArgs e)
        {
            var milliseconds = _coreHeartbeat.TotalMilliseconds;
            var subsequentBeat = Normal.Sample(milliseconds, _sd);
            subsequentBeat = Math.Max(subsequentBeat, 5);

            _activeTimer.Interval = subsequentBeat;
            _activeTimer.Enabled = true;
        }

        public void OnBeat(ElapsedEventHandler handler)
        {
            lock (_lock)
            {
                _activeTimer.Elapsed += handler;
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
            lock (_lock)
            {
                _activeTimer.Dispose();
            }
        }
    }
}
