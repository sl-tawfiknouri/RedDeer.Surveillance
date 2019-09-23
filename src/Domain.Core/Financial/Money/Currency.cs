namespace Domain.Core.Financial.Money
{
    using System;

    public struct Currency
    {
        public Currency(string code)
        {
            this.Code = code?.ToUpper()?.Trim() ?? string.Empty;
            this.ShortName = string.Empty;
            this.LongName = string.Empty;
            this.SubDivisionCode = string.Empty;
            this.Symbol = string.Empty;
        }

        public Currency(string code, string shortName, string longName, string subDivisionCode, string symbol)
        {
            this.Code = code?.ToUpper()?.Trim() ?? string.Empty;
            this.ShortName = shortName?.ToUpper()?.Trim() ?? string.Empty;
            this.LongName = longName?.ToUpper()?.Trim() ?? string.Empty;
            this.SubDivisionCode = subDivisionCode?.ToUpper()?.Trim() ?? string.Empty;
            this.Symbol = symbol?.ToUpper()?.Trim() ?? string.Empty;
        }

        /// <summary>
        ///     3 letter code
        ///     GBP for pound sterling
        /// </summary>
        public string Code { get; }

        public string ShortName { get; }

        public string LongName { get; }

        /// <summary>
        ///     GBX for pence
        /// </summary>
        public string SubDivisionCode { get; }

        /// <summary>
        ///     Currency symbol
        /// </summary>
        public string Symbol { get; }

        public override int GetHashCode()
        {
            return this.Code?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            var typeCheck = obj is Currency;

            if (!typeCheck) return false;

            var currencyObj = (Currency)obj;

            return string.Equals(currencyObj.Code, this.Code, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return this.Code ?? string.Empty;
        }
    }
}