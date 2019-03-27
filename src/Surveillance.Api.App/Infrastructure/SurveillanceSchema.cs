using GraphQL;
using GraphQL.Types;

namespace Surveillance.Api.App.Infrastructure
{
    public class SurveillanceSchema : Schema
    {
        public SurveillanceSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<SurveillanceQuery>();
        }
    }
}
