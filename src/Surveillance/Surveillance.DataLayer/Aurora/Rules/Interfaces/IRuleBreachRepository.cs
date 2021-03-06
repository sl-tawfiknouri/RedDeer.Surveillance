﻿namespace Surveillance.DataLayer.Aurora.Rules.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Rules;

    public interface IRuleBreachRepository
    {
        Task<long?> Create(RuleBreach message);

        Task<RuleBreach> Get(string id);

        Task<bool> HasDuplicate(string ruleId);

        Task<bool> HasDuplicateBackTest(string ruleId, string correlationId);
    }
}