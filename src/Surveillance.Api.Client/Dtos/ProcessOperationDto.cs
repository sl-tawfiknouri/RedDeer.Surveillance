using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class ProcessOperationDto
    {
        public int Id { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int OperationState { get; set; }

        public DateTime OperationStart => DateTime.Parse(Start, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime? OperationEnd => string.IsNullOrEmpty(End) ? (DateTime?)null : DateTime.Parse(End, CultureInfo.GetCultureInfo("en-GB"));
    }
}
