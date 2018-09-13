﻿using System;
using Domain.Equity.Interfaces;

namespace Domain.Equity
{
    public struct SecurityIdentifiers : ISecurityIdentifiers
    {
        public SecurityIdentifiers(
            string clientIdentifier,
            string sedol,
            string isin,
            string figi)
        {
            ClientIdentifier = clientIdentifier ?? string.Empty;
            Sedol = sedol ?? string.Empty;
            Isin = isin ?? string.Empty;
            Figi = figi ?? string.Empty;
        }

        public string ClientIdentifier { get; }
        public string Sedol { get; }
        public string Isin { get; }
        public string Figi { get; }

        public override int GetHashCode()
        {
            // we need to manually check every collision
            // this is a bit undesirable for any dictionaries etc using hash codes
            // we can replace this if we have our own internal identifier for securities
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SecurityIdentifiers))
            {
                return false;
            }

            var otherId = (SecurityIdentifiers)obj;

            if (string.Equals(ClientIdentifier, otherId.ClientIdentifier, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (string.Equals(Sedol, otherId.Sedol, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (string.Equals(Isin, otherId.Isin, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (string.Equals(Figi, otherId.Figi, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ClientIdentifier: {ClientIdentifier} | Sedol {Sedol} | Isin {Isin} | Figi {Figi}";
        }
    }
}