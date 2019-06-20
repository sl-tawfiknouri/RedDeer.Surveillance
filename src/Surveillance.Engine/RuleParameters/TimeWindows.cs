using System;

namespace Surveillance.Engine.Rules.RuleParameters
{
    public class TimeWindows
    {
        public TimeWindows(
            TimeSpan backwardWindowSize,
            TimeSpan? futureWindowSize = null)
        {
            BackwardWindowSize = backwardWindowSize;
            FutureWindowSize = futureWindowSize;
        }

        public TimeSpan BackwardWindowSize { get; }
        public TimeSpan? FutureWindowSize { get; }
    }
}
