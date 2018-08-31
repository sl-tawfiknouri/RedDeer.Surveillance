using System.Collections.Generic;

namespace Surveillance.Rules.ProhibitedAssets.Interfaces
{
    public interface IProhibitedAssetsRepository
    {
        HashSet<string> ProhibitedEquities { get; }
    }
}