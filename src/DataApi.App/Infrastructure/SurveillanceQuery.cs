using Domain.Surveillance.Rules.Interfaces;
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
           // IHttpContextAccessor ctx,
            IActiveRulesService ruleService)
        {
            //if (!authorisation.CanReadApi(ctx.HttpContext.User))
            //{
            //    return;
            //}          

            Field<ListGraphType<RulesTypeEnumGraphType>>(
                "rules",
                "The category of the rule",
                resolve: context => ruleService.EnabledRules());
        }

        //public SurveillanceQuery(
        //    IActiveRulesService ruleService,
        //    IDataLoaderContextAccessor dataAccessor,
        //    IHttpContextAccessor ctx,
        //    ISurveillanceAuthorisation authorisation)
        //{
        //    if (!authorisation.CanReadApi(ctx.HttpContext.User))
        //    {
        //        return;
        //    }          
        //}
    }
}
