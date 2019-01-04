using Firefly.Service.Data.BMLL.Shared.Requests;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IGetTimeBarPair
    {
        GetMinuteBarsRequest Request { get; }
        GetMinuteBarsResponse Response { get; }
    }
}