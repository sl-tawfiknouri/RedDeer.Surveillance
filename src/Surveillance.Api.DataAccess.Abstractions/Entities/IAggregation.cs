using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IAggregation
    {
        string Key { get; }
        int Count { get; }
    }
}
