namespace DataSynchroniser.Api.Bmll.Bmll.Interfaces
{
    using Firefly.Service.Data.BMLL.Shared.Requests;

    public interface IGetTimeBarPair
    {
        GetMinuteBarsRequest Request { get; }

        GetMinuteBarsResponse Response { get; }
    }
}