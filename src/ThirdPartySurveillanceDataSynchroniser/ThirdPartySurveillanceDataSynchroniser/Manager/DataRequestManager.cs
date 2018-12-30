using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager
{
    public class DataRequestManager : IDataRequestManager
    {
        private readonly IBmllDataRequestRepository _dataRequestRepository;
        private readonly ILogger<DataRequestManager> _logger;

        public DataRequestManager(
            IBmllDataRequestRepository dataRequestRepository,
            ILogger<DataRequestManager> logger)
        {
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Handle(string ruleRunId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext)
        {
            if (string.IsNullOrWhiteSpace(ruleRunId))
            {
                _logger.LogError($"DataRequestManager asked to handle a rule run that had a null or empty id");
                dataRequestContext.EventError($"DataRequestManager RuleRunId was null");
                return;
            }

            _logger.LogInformation($"DataRequestManager handling request with id {ruleRunId}");

            // get fi for rule run id




            // ok so we need to first go get all the relevant rows matching the rule run id
            // if none, return
            // if has rows, group by data source maybe some kind of DataSourceFinancialInstrument object
            // then submit to our own handler
            // log warnings (?)

            // how to handle unable to fetch market data issues and feed back to the client as well?
            // lets make a table and just insert it into there for now
            // chat to dev ops about making it an error





            _logger.LogInformation($"DataRequestManager completed handling request with id {ruleRunId}");
        }
    }
}
