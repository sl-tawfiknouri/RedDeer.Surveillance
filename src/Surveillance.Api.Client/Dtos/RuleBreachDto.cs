﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Dtos
{
    public class RuleBreachDto
    {
        public int Id { get; set; }
        public string RuleId { get; set; }
        public List<OrderDto> Orders { get; set; }
    }
}
