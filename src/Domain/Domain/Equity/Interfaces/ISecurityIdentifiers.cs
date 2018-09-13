﻿namespace Domain.Equity.Interfaces
{
    public interface ISecurityIdentifiers
    {
        string ClientIdentifier { get; }
        string Figi { get; }
        string Isin { get; }
        string Sedol { get; }
    }
}