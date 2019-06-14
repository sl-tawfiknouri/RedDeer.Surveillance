﻿using System.Timers;
using TestHarness.Engine.Heartbeat.Interfaces;

namespace TestHarness.Engine.Heartbeat
{
    public class PulsatingHeartbeat : IPulsatingHeartbeat
    {
        private ElapsedEventHandler _handler;
        private readonly object _lock = new object();

        public void Pulse()
        {
            lock (_lock)
            {
                _handler?.Invoke(this, null);
            }
        }

        public void OnBeat(ElapsedEventHandler handler)
        {
            lock (_lock)
            {
                _handler = handler;
            }
        }
 
        public void Start()
        {}

        public void Stop()
        {}

        public void Dispose()
        {}
    }
}