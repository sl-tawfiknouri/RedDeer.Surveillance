namespace RedDeer.Surveillance.Api.Client.Filters
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class RuleBreachFilter<T> : Filter<RuleBreachFilter<T>, T>
        where T : Parent
    {
        public RuleBreachFilter(T node)
            : base(node)
        {
        }

        public RuleBreachFilter<T> ArgumentId(int id)
        {
            return this.AddArgument("id", id);
        }
    }
}