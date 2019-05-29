using System;
using System.Timers;

namespace TestHarness.Engine.Heartbeat.Interfaces
{
    public interface IHeartbeat : IDisposable
    {
        void OnBeat(ElapsedEventHandler handler);
        void Start();
        void Stop();
    }
}