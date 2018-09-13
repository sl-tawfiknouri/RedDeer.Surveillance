﻿using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IEquitiesFileRelayProcessFactory
    {
        IEquityDataGenerator Create(string filePath);
    }
}