using System;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IHeartbeatSelector
    {
        ICompleteSelector Regular(TimeSpan frequency);
        ICompleteSelector Irregular(TimeSpan frequency, int sd);
    }
}
