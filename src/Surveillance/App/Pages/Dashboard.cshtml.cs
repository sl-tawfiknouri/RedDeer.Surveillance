using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Exceptions;
using Surveillance.Auditing.DataLayer.Repositories.Exceptions.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Engine.Rules.Utility.Interfaces;

namespace RedDeer.Surveillance.App.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ISystemProcessRepository _systemProcessRepository;
        private readonly ISystemProcessOperationRepository _systemProcessOperationRepository;
        private readonly ISystemProcessOperationRuleRunRepository _systemProcessRuleRunRepository;
        private readonly ISystemProcessOperationDistributeRuleRepository _systemProcessDistributeRepository;
        private readonly ISystemProcessOperationUploadFileRepository _systemProcessUploadFileRepository;
        private readonly IExceptionRepository _exceptionRepository;
        private readonly IApiHeartbeat _apiHeartbeat;
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(
            ISystemProcessRepository systemProcessRepository,
            ISystemProcessOperationRuleRunRepository systemProcessRuleRunRepository,
            ISystemProcessOperationRepository systemProcessOperationRepository,
            ISystemProcessOperationDistributeRuleRepository systemProcessDistributeRepository,
            IExceptionRepository exceptionRepository,
            IApiHeartbeat apiHeartbeat, 
            ISystemProcessOperationUploadFileRepository systemProcessUploadFileRepository,
            ILogger<DashboardModel> logger)
        {
            _systemProcessRepository =
                systemProcessRepository
                ?? throw new ArgumentNullException(nameof(systemProcessRepository));
            _systemProcessRuleRunRepository =
                systemProcessRuleRunRepository
                ?? throw new ArgumentNullException(nameof(systemProcessRuleRunRepository));
            _systemProcessOperationRepository =
                systemProcessOperationRepository
                ?? throw new ArgumentNullException(nameof(systemProcessOperationRepository));
            _systemProcessDistributeRepository =
                systemProcessDistributeRepository
                ?? throw new ArgumentNullException(nameof(systemProcessDistributeRepository));
            _exceptionRepository =
                exceptionRepository
                ?? throw new ArgumentNullException(nameof(exceptionRepository));
            _apiHeartbeat =
                apiHeartbeat
                ?? throw new ArgumentNullException(nameof(apiHeartbeat));
            _systemProcessUploadFileRepository =
                systemProcessUploadFileRepository
                ?? throw new ArgumentNullException(nameof(systemProcessUploadFileRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnGet()
        {
            try
            {
                ProcessId = Process.GetCurrentProcess().Id.ToString();
                ProcessPeakWorkingSet = Process.GetCurrentProcess().PeakWorkingSet64.ToString();
                ProcessStartTime = Process.GetCurrentProcess().StartTime.ToString();
                ProcessMachineName = Process.GetCurrentProcess().MachineName;
                ProcessProcessorTime = $"{Process.GetCurrentProcess().TotalProcessorTime.Days} DAYS {Process.GetCurrentProcess().TotalProcessorTime.Hours} HOURS {Process.GetCurrentProcess().TotalProcessorTime.Minutes} MINUTES {Process.GetCurrentProcess().TotalProcessorTime.Seconds} SECONDS";

                SystemProcesses = await _systemProcessRepository.GetDashboard();
                SystemProcessOperation = await _systemProcessOperationRepository.GetDashboard();
                SystemProcessRuleRun = await _systemProcessRuleRunRepository.GetDashboard();
                SystemProcessDistribute = await _systemProcessDistributeRepository.GetDashboard();
                SystemProcessUploadFile = await _systemProcessUploadFileRepository.GetDashboard();
                ApiHeartbeat = await _apiHeartbeat.HeartsBeating();
                Exceptions = await _exceptionRepository.GetForDashboard();
            }
            catch (Exception e)
            {
                _logger.LogError($"DashboardModel caught and swallowed an exception", e);
            }
        }

        public string ProcessId { get; set; }

        public string ProcessMachineName { get; set; }

        public string ProcessPeakWorkingSet { get; set; }

        public string ProcessStartTime { get; set;}

        public string ProcessProcessorTime { get; set; }

        public bool ApiHeartbeat { get; set; }

        public IReadOnlyCollection<ISystemProcess> SystemProcesses { get; set; }

        public IReadOnlyCollection<ISystemProcessOperationDistributeRule> SystemProcessDistribute { get; set; }

        public IReadOnlyCollection<ISystemProcessOperationRuleRun> SystemProcessRuleRun { get; set; }

        public IReadOnlyCollection<ISystemProcessOperation> SystemProcessOperation { get; set; }

        public IReadOnlyCollection<ISystemProcessOperationUploadFile> SystemProcessUploadFile { get; set; }

        public IReadOnlyCollection<ExceptionDto> Exceptions { get; set; }
    }
}