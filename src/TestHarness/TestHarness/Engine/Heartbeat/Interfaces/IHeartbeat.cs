namespace TestHarness.Engine.Heartbeat.Interfaces
{
    using System;
    using System.Timers;

    public interface IHeartbeat : IDisposable
    {
        void OnBeat(ElapsedEventHandler handler);

        void Start();

        void Stop();
    }
}