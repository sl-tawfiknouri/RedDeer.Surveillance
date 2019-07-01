using Surveillance.Engine.Rules.RuleParameters.Filter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IReferenceDataFilterable
    {
        RuleFilter Regions { get; set; }
        RuleFilter Countries { get; set; }
        RuleFilter Sectors { get; set; }
        RuleFilter Industries { get; set; }

        bool HasReferenceDataFilters();
    }
}
