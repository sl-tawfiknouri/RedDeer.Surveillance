﻿using Surveillance.Recorders.Interfaces;
using System;
using Domain.Equity.Frames;

namespace Surveillance.Recorders
{
    // ReSharper disable once UnusedMember.Global
    public class RedDeerStubStockExchangeRecorder : IRedDeerStockExchangeRecorder
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ExchangeFrame value)
        {
        }
    }
}
