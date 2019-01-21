namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class SuccessOrFailureResult<T>
    {
        public SuccessOrFailureResult(bool success, T value)
        {
            Success = success;
            Value = value;
        }

        public bool Success { get; }
        public T Value { get; }
    }
}
