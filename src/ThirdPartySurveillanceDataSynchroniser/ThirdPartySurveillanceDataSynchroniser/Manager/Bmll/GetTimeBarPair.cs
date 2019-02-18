using DataSynchroniser.Manager.Bmll.Interfaces;
using Firefly.Service.Data.BMLL.Shared.Requests;

namespace DataSynchroniser.Manager.Bmll
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
