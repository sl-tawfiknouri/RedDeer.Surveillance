// ReSharper disable UnusedMember.Global

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    using System;

    public interface IHeartbeatSelector
    {
        ICompleteSelector Irregular(TimeSpan frequency, int sd);

        ICompleteSelector Regular(TimeSpan frequency);
    }
}