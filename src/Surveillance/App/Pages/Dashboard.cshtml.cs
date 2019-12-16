namespace RedDeer.Surveillance.App.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using global::Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using global::Surveillance.Auditing.DataLayer.Repositories.Exceptions;
    using global::Surveillance.Auditing.DataLayer.Repositories.Exceptions.Interfaces;
    using global::Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using global::Surveillance.Engine.Rules.Utility.Interfaces;

    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    public class DashboardModel : PageModel
    {
        private readonly IApiHeartbeat _apiHeartbeat;

        private readonly IExceptionRepository _exceptionRepository;

        private readonly ILogger<DashboardModel> _logger;

        private readonly ISystemProcessOperationDistributeRuleRepository _systemProcessDistributeRepository;

        private readonly ISystemProcessOperationRepository _systemProcessOperationRepository;

        private readonly ISystemProcessRepository _systemProcessRepository;

        private readonly ISystemProcessOperationRuleRunRepository _systemProcessRuleRunRepository;

        private readonly ISystemProcessOperationUploadFileRepository _systemProcessUploadFileRepository;

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
            this._systemProcessRepository = systemProcessRepository
                                            ?? throw new ArgumentNullException(nameof(systemProcessRepository));
            this._systemProcessRuleRunRepository = systemProcessRuleRunRepository
                                                   ?? throw new ArgumentNullException(
                                                       nameof(systemProcessRuleRunRepository));
            this._systemProcessOperationRepository = systemProcessOperationRepository
                                                     ?? throw new ArgumentNullException(
                                                         nameof(systemProcessOperationRepository));
            this._systemProcessDistributeRepository = systemProcessDistributeRepository
                                                      ?? throw new ArgumentNullException(
                                                          nameof(systemProcessDistributeRepository));
            this._exceptionRepository =
                exceptionRepository ?? throw new ArgumentNullException(nameof(exceptionRepository));
            this._apiHeartbeat = apiHeartbeat ?? throw new ArgumentNullException(nameof(apiHeartbeat));
            this._systemProcessUploadFileRepository = systemProcessUploadFileRepository
                                                      ?? throw new ArgumentNullException(
                                                          nameof(systemProcessUploadFileRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool ApiHeartbeat { get; set; }

        public IReadOnlyCollection<ExceptionDto> Exceptions { get; set; }

        public string ProcessId { get; set; }

        public string ProcessMachineName { get; set; }

        public string ProcessPeakWorkingSet { get; set; }

        public string ProcessProcessorTime { get; set; }

        public string ProcessStartTime { get; set; }

        public IReadOnlyCollection<ISystemProcessOperationDistributeRule> SystemProcessDistribute { get; set; }

        public IReadOnlyCollection<ISystemProcess> SystemProcesses { get; set; }

        public IReadOnlyCollection<ISystemProcessOperation> SystemProcessOperation { get; set; }

        public IReadOnlyCollection<ISystemProcessOperationRuleRun> SystemProcessRuleRun { get; set; }

        public IReadOnlyCollection<ISystemProcessOperationUploadFile> SystemProcessUploadFile { get; set; }

        public async Task OnGet()
        {
            try
            {
                this.ProcessId = Process.GetCurrentProcess().Id.ToString();
                this.ProcessPeakWorkingSet = Process.GetCurrentProcess().PeakWorkingSet64.ToString();
                this.ProcessStartTime = Process.GetCurrentProcess().StartTime.ToString();
                this.ProcessMachineName = Process.GetCurrentProcess().MachineName;
                this.ProcessProcessorTime =
                    $"{Process.GetCurrentProcess().TotalProcessorTime.Days} DAYS {Process.GetCurrentProcess().TotalProcessorTime.Hours} HOURS {Process.GetCurrentProcess().TotalProcessorTime.Minutes} MINUTES {Process.GetCurrentProcess().TotalProcessorTime.Seconds} SECONDS";

                this.SystemProcesses = await this._systemProcessRepository.GetDashboard();
                this.SystemProcessOperation = await this._systemProcessOperationRepository.GetDashboard();
                this.SystemProcessRuleRun = await this._systemProcessRuleRunRepository.GetDashboard();
                this.SystemProcessDistribute = await this._systemProcessDistributeRepository.GetDashboard();
                this.SystemProcessUploadFile = await this._systemProcessUploadFileRepository.GetDashboard();
                this.ApiHeartbeat = await this._apiHeartbeat.HeartsBeating();
                this.Exceptions = await this._exceptionRepository.GetForDashboard();
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "DashboardModel caught and swallowed an exception");
            }
        }
    }
}