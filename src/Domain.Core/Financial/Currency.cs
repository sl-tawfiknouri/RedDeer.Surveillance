using System;

namespace Domain.Core.Financial
{
    public struct Currency
    {
        public Currency(string value)
        {
            Value = value?.ToUpper()?.Trim() ?? string.Empty;
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

        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }
}
