using System.Linq;

namespace Domain.Financial.Cfis
{
    /// <summary>
    /// Categories cfi by first two characters (category and group)
    /// Does not use attributes (1-4)
    /// </summary>
    public class Cfi
    {
        public Cfi(string underlyingValue)
        {
            Value = underlyingValue?.ToLower();

            if (string.IsNullOrWhiteSpace(Value))
            {
                return;
            }

            CfiCategory = InferCategory(Value);
            CfiGroup = InferGroup(Value, CfiCategory);
        }

        public string Value { get; }
        public CfiCategory CfiCategory { get; }
        public CfiGroup CfiGroup { get; }

        private CfiCategory InferCategory(string cfi)
        {
            if (string.IsNullOrWhiteSpace(cfi))
            {
                return CfiCategory.None;
            }

            var leadingCharacter = cfi.FirstOrDefault();

            switch (leadingCharacter)
            {
                case 'e':
                    return CfiCategory.Equities;
                case 'd':
                    return CfiCategory.DebtInstrument;
                case 'r':
                    return CfiCategory.Entitlements;
                case 'o':
                    return CfiCategory.Options;
                case 'f':
                    return CfiCategory.Futures;
                case 'm':
                    return CfiCategory.Others;
                default:
                    return CfiCategory.None;
            }
        }

        private CfiGroup InferGroup(string cfi, CfiCategory cfiCategory)
        {
            if (string.IsNullOrWhiteSpace(cfi)
                || cfiCategory == CfiCategory.None)
            {
                return CfiGroup.None;
            }

            switch (cfiCategory)
            {
                case CfiCategory.Equities:
                    return InferEquityGroup(cfi);
                case CfiCategory.DebtInstrument:
                    return InferDebtGroup(cfi);
                case CfiCategory.Entitlements:
                    return InferEntitlementsGroup(cfi);
                case CfiCategory.Options:
                    return InferOptionsGroup(cfi);
                case CfiCategory.Futures:
                    return InferFuturesGroup(cfi);
                case CfiCategory.Others:
                    return InferOthersGroup(cfi);
                default:
                    return CfiGroup.None;
            }
        }

        private CfiGroup InferEntitlementsGroup(string cfi)
        {
            if (cfi.Length < 2)
            {
                return CfiGroup.None;
            }

            var groupChar = cfi.Skip(1).First();

            switch (groupChar)
            {
                case 'a':
                    return CfiGroup.AllotmentRights;
                case 'm':
                    return CfiGroup.Others;
                case 's':
                    return CfiGroup.SubscriptionRights;
                case 'w':
                    return CfiGroup.Warrants;
                default:
                    return CfiGroup.None;
            }
        }

        private CfiGroup InferOptionsGroup(string cfi)
        {
            if (cfi.Length < 2)
            {
                return CfiGroup.None;
            }

            var groupChar = cfi.Skip(1).First();

            switch (groupChar)
            {
                case 'c':
                    return CfiGroup.CallOptions;
                case 'p':
                    return CfiGroup.PutOptions;
                case 'm':
                    return CfiGroup.Others;
                default:
                    return CfiGroup.None;
            }
        }

        private CfiGroup InferFuturesGroup(string cfi)
        {
            if (cfi.Length < 2)
            {
                return CfiGroup.None;
            }

            var groupChar = cfi.Skip(1).First();

            switch (groupChar)
            {
                case 'c':
                    return CfiGroup.CommoditiesFutures;
                case 'f':
                    return CfiGroup.FinancialFutures;
                default:
                    return CfiGroup.None;
            }
        }

        private CfiGroup InferOthersGroup(string cfi)
        {
            if (cfi.Length < 2)
            {
                return CfiGroup.None;
            }

            var groupChar = cfi.Skip(1).First();

            switch (groupChar)
            {
                case 'm':
                    return CfiGroup.Others;
                case 'r':
                    return CfiGroup.ReferentialInstruments;
                default:
                    return CfiGroup.None;
            }
        }


        private CfiGroup InferDebtGroup(string cfi)
        {
            if (cfi.Length < 2)
            {
                return CfiGroup.None;
            }

            var groupChar = cfi.Skip(1).First();

            switch (groupChar)
            {
                case 'b':
                    return CfiGroup.Bonds;
                case 'c':
                    return CfiGroup.ConvertibleBonds;
                case 'm':
                    return CfiGroup.Others;
                case 't':
                    return CfiGroup.MediumTermNotes;
                case 'w':
                    return CfiGroup.BondsWithWarrantsAttached;
                case 'y':
                    return CfiGroup.MoneyMarketInstruments;
                default:
                    return CfiGroup.None;
            }
        }

        private CfiGroup InferEquityGroup(string cfi)
        {
            if (cfi.Length < 2)
            {
                return CfiGroup.None;
            }

            var groupChar = cfi.Skip(1).First();

            switch (groupChar)
            {
                case 's':
                    return CfiGroup.Shares;
                case 'p':
                    return CfiGroup.PreferredShares;
                case 'r':
                    return CfiGroup.PreferenceShares;
                case 'c':
                    return CfiGroup.ConvertibleShares;
                case 'f':
                    return CfiGroup.PreferredConvertibleShares;
                case 'v':
                    return CfiGroup.PreferenceConvertibleShares;
                case 'u':
                    return CfiGroup.Units;
                case 'm':
                    return CfiGroup.Others;
                default:
                    return CfiGroup.None;
            }
        }
    }

    public enum CfiCategory
    {
        None,
        DebtInstrument,
        Equities,
        Entitlements,
        Options,
        Futures,
        Others
    }

    public enum CfiGroup
    {
        None,

        Shares,
        PreferredShares,
        PreferenceShares,
        ConvertibleShares,
        PreferredConvertibleShares,
        PreferenceConvertibleShares,
        Units,
        Others,

        Bonds,
        ConvertibleBonds,
        MediumTermNotes,
        BondsWithWarrantsAttached,
        MoneyMarketInstruments,

        AllotmentRights,
        SubscriptionRights,
        Warrants,

        CallOptions,
        PutOptions,

        CommoditiesFutures,
        FinancialFutures,

        ReferentialInstruments
    }
}
