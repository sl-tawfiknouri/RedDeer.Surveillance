using Surveillance.Rules.ProhibitedAssets.Interfaces;
using System;
using System.Collections.Generic;

namespace Surveillance.Rules.BarredAssets
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
