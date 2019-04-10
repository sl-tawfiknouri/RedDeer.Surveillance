using System.ComponentModel.DataAnnotations;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class RuleBreachOrder : IRuleBreachOrder
    {
        [Key]
        public int RuleBreachId { get; set; }
        public int OrderId { get; set; }
    }
}
