using System;

namespace DomainV2.Financial
{
    /// <summary>
    /// Extended currency amount with a date for conversions
    /// </summary>
    public class CurrencyAmountDated
    {
        public CurrencyAmountDated(decimal value, string currency, DateTime date)
        {
            Amount = new CurrencyAmount(value, new Currency(currency ?? string.Empty));
            Date = date;
        }

        public CurrencyAmountDated(decimal value, Currency currency, DateTime date)
        {
            Amount = new CurrencyAmount(value, currency);
            Date = date;
        }

        public DateTime Date { get; }

        public CurrencyAmount Amount { get; }
    }
}
