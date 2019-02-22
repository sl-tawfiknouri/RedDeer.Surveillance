﻿using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Firefly.Service.Data.BMLL.Shared.Requests;

namespace DataSynchroniser.Api.Bmll.Bmll
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