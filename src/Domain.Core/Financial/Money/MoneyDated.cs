using System;

namespace Domain.Core.Financial.Money
{
    /// <summary>
    /// Extended money amount with a date for exchange rate conversions
    /// </summary>
    public class MoneyDated
    {
        public MoneyDated(decimal value, string currency, DateTime date)
        {
            Amount = new Money(value, new Currency(currency ?? string.Empty));
            Date = date;
        }

        public MoneyDated(decimal value, Currency currency, DateTime date)
        {
            Amount = new Money(value, currency);
            Date = date;
        }

        public DateTime Date { get; }

        public Money Amount { get; }
    }
}
