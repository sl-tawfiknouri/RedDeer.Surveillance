namespace DataSynchroniser.Api.Bmll.Bmll
{
    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;

    using Firefly.Service.Data.BMLL.Shared.Requests;

    public class GetTimeBarPair : IGetTimeBarPair
    {
        public GetTimeBarPair(GetMinuteBarsRequest request, GetMinuteBarsResponse response)
        {
            this.Request = request;
            this.Response = response;
        }

        public GetMinuteBarsRequest Request { get; }

        public GetMinuteBarsResponse Response { get; }
    }
}