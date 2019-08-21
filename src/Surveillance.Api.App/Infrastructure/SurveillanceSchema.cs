namespace Surveillance.Api.App.Infrastructure
{
    using GraphQL;
    using GraphQL.Types;

    public class SurveillanceSchema : Schema
    {
        public SurveillanceSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            this.Query = resolver.Resolve<SurveillanceQuery>();
        }
    }
}