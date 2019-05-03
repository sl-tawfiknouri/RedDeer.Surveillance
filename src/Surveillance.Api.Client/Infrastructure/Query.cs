using SAHB.GraphQLClient.QueryGenerator;
using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Infrastructure
{
    public abstract class Query<R> : NodeParent
    {
        public Query() : base(null) { }
        internal abstract Task<R> HandleAsync(IRequest request);
    }
}
