using System.Collections.Generic;

namespace Surveillance.Rules.BarredAssets
{
    public interface IProhibitedAssetsRepository
    {
        HashSet<string> ProhibitedEquities { get; }
    }
}