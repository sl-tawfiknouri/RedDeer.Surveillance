﻿using Domain.Surveillance.Rules.Interfaces;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Surveillance.Api.App.Infrastructure.Interfaces;
using Surveillance.Api.App.Types;

namespace Surveillance.Api.App.Infrastructure
{
    public class SurveillanceQuery : ObjectGraphType
    {
        public SurveillanceQuery(
            ISurveillanceAuthorisation authorisation,
            IHttpContextAccessor ctx,
            IActiveRulesService ruleService)
        {
            if (!authorisation.IsAuthorised(ctx.HttpContext.User))
            {
                return;
            }          

            Field<ListGraphType<RulesTypeEnumGraphType>>(
                "rules",
                "The category of the rule",
                resolve: context => ruleService.EnabledRules());
        }
    }
}
