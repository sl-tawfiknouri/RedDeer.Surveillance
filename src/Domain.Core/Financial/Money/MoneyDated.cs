namespace Domain.Core.Financial.Money
{
    using System;

    /// <summary>
    ///     Extended money amount with a date for exchange rate conversions
    /// </summary>
    public class MoneyDated
    {
        public MoneyDated(decimal value, string currency, DateTime date)
        {
            this.Amount = new Money(value, new Currency(currency ?? string.Empty));
            this.Date = date;
        }

        public Money Amount { get; }

        public DateTime Date { get; }
    }
}