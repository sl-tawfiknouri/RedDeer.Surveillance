using Firefly.Service.Data.BMLL.Shared.Requests;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class GetTimeBarPair : IGetTimeBarPair
    {
        public GetTimeBarPair(
            GetMinuteBarsRequest request,
            GetMinuteBarsResponse response)
        {
            Request = request;
            Response = response;
        }

        public GetMinuteBarsRequest Request { get; }
        public GetMinuteBarsResponse Response { get;  }
    }
}
