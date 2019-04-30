using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Queries
{
    public interface IQuery<R>
    {
        Task<R> HandleAsync(IRequest request);
    }
}
