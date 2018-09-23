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
                "Bear Stearns",
            };
        }

        public HashSet<string> ProhibitedEquities { get; }
    }
}
