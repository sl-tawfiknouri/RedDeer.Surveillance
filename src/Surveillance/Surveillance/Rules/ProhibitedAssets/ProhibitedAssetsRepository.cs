using System;
using System.Collections.Generic;
using Surveillance.Rules.ProhibitedAssets.Interfaces;

namespace Surveillance.Rules.ProhibitedAssets
{
    /// <summary>
    /// In memory repository for prohibited assets
    /// </summary>
    public class ProhibitedAssetsRepository : IProhibitedAssetsRepository
    {
        public ProhibitedAssetsRepository()
        {
            ProhibitedEquities = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
            {
                "Lehman Bros",
                "Northern Rock",
                "Bear Stearns",
                "HBOS",
                "Lloyds TSB",
                "RBS"
            };
        }

        public HashSet<string> ProhibitedEquities { get; }
    }
}
