using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal? LimitPrice { get; set; }
    }
}
