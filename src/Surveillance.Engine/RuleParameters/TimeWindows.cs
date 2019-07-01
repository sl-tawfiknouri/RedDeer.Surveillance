using System;
using Domain.Surveillance.Rules.Tuning;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.RuleParameters
{
    [Serializable]
    public class TimeWindows
    {
        public TimeWindows(
            string timeWindowId,
            TimeSpan backwardWindowSize,
            TimeSpan? futureWindowSize = null)
        {
            TimeWindowId = timeWindowId ?? string.Empty;
            BackwardWindowSize = backwardWindowSize;
            FutureWindowSize = futureWindowSize ?? TimeSpan.Zero;
        }

        [TuneableIdParameter]
        public string TimeWindowId { get; set; }

        [TuneableTimespanParameter]
        public TimeSpan BackwardWindowSize { get; set; }
        [TuneableTimespanParameter]
        public TimeSpan FutureWindowSize { get; set; }

        [TunedParam]
        TunedParameter<string> TunedParam { get; set; }

        public override int GetHashCode()
        {
            return 
                BackwardWindowSize.GetHashCode()
                * FutureWindowSize.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var castObj = obj as TimeWindows;
            if (castObj == null)
            {
                return false;
            }

            return BackwardWindowSize == castObj.BackwardWindowSize
                   && FutureWindowSize == castObj.FutureWindowSize;

        }
    }
}
