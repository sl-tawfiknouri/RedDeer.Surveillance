using System;

namespace Domain.Finance
{
    public struct Currency
    {
        public Currency(string value)
        {
            Value = value ?? string.Empty;
        }

        public string Value { get; }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            var typeCheck = obj is Currency;

            if (!typeCheck)
            {
                return false;
            }

            var currencyObj = (Currency) obj;

            return string.Equals(currencyObj.Value, Value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
