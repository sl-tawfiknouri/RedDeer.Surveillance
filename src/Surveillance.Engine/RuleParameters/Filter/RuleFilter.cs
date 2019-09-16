namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The rule filter.
    /// </summary>
    [Serializable]
    public class RuleFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleFilter"/> class.
        /// </summary>
        public RuleFilter()
        {
            this.Ids = new string[0];
        }

        /// <summary>
        /// Gets or sets the ids.
        /// </summary>
        public IReadOnlyCollection<string> Ids { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public RuleFilterType Type { get; set; }

        /// <summary>
        /// The none rule filter.
        /// </summary>
        /// <returns>
        /// The <see cref="RuleFilter"/>.
        /// </returns>
        public static RuleFilter None()
        {
            return new RuleFilter { Type = RuleFilterType.None, Ids = new string[0] };
        }
    }
}