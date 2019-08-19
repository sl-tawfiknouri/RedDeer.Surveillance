namespace Surveillance.Engine.Rules.RuleParameters
{
    using System;

    using Domain.Surveillance.Rules.Tuning;

    [Serializable]
    public class TimeWindows
    {
        public TimeWindows(string timeWindowId, TimeSpan backwardWindowSize, TimeSpan? futureWindowSize = null)
        {
            this.TimeWindowId = timeWindowId ?? string.Empty;
            this.BackwardWindowSize = backwardWindowSize;
            this.FutureWindowSize = futureWindowSize ?? TimeSpan.Zero;
        }

        [TuneableTimespanParameter]
        public TimeSpan BackwardWindowSize { get; set; }

        [TuneableTimespanParameter]
        public TimeSpan FutureWindowSize { get; set; }

        [TuneableIdParameter]
        public string TimeWindowId { get; set; }

        [TunedParam]
        public TunedParameter<string> TunedParam { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var castObj = obj as TimeWindows;
            if (castObj == null) return false;

            return this.BackwardWindowSize == castObj.BackwardWindowSize
                   && this.FutureWindowSize == castObj.FutureWindowSize;
        }

        public override int GetHashCode()
        {
            return this.BackwardWindowSize.GetHashCode() * this.FutureWindowSize.GetHashCode();
        }
    }
}