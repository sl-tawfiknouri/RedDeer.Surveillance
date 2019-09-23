namespace DataSynchroniser.Api.Bmll.Bmll
{
    public class SuccessOrFailureResult<T>
    {
        public SuccessOrFailureResult(bool success, T value)
        {
            this.Success = success;
            this.Value = value;
        }

        public bool Success { get; }

        public T Value { get; }
    }
}