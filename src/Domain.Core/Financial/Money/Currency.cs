using System;

namespace Domain.Core.Financial
{
    public struct Currency
    {
        public Currency(string code)
        {
            Code = code?.ToUpper()?.Trim() ?? string.Empty;
            ShortName = string.Empty;
            LongName = string.Empty;
            SubDivisionCode = string.Empty;
            Symbol = string.Empty;
        }

        public Currency(string code, string shortName, string longName, string subDivisionCode, string symbol)
        {
            Code = code?.ToUpper()?.Trim() ?? string.Empty;
            ShortName = shortName?.ToUpper()?.Trim() ?? string.Empty;
            LongName = longName?.ToUpper()?.Trim() ?? string.Empty;
            SubDivisionCode = subDivisionCode?.ToUpper()?.Trim() ?? string.Empty;
            Symbol = symbol?.ToUpper()?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// 3 letter code 
        /// GBP for pound sterling
        /// </summary>
        public string Code { get; }

        public string ShortName { get; }

        public string LongName { get; }

        /// <summary>
        /// GBX for pence
        /// </summary>
        public string SubDivisionCode { get; }

        /// <summary>
        /// Currency symbol
        /// </summary>
        public string Symbol { get; }

        public override int GetHashCode()
        {
            return Code?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            var typeCheck = obj is Currency;

            if (!typeCheck)
            {
                return false;
            }

            var currencyObj = (Currency) obj;

            return string.Equals(currencyObj.Code, Code, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return Code ?? string.Empty;
        }
    }
}
