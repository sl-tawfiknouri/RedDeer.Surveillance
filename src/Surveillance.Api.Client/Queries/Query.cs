using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Queries
{
    public abstract class Query<R>
    {
        internal abstract Task<R> HandleAsync(IRequest request);
    }
}
