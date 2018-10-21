﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;
using Surveillance.Utility.Interfaces;

namespace Surveillance.App.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ISystemProcessRepository _systemProcessRepository;
        private readonly ISystemProcessOperationRepository _systemProcessOperationRepository;
        private readonly ISystemProcessOperationRuleRunRepository _systemProcessRuleRunRepository;
        private readonly ISystemProcessOperationDistributeRuleRepository _systemProcessDistributeRepository;
        private readonly IApiHeartbeat _apiHeartbeat;

        public DashboardModel(
            ISystemProcessRepository systemProcessRepository,
            ISystemProcessOperationRuleRunRepository systemProcessRuleRunRepository,
            ISystemProcessOperationRepository systemProcessOperationRepository,
            ISystemProcessOperationDistributeRuleRepository systemProcessDistributeRepository,
            IApiHeartbeat apiHeartbeat)
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
            _apiHeartbeat =
                apiHeartbeat
                ?? throw new ArgumentNullException(nameof(apiHeartbeat));
        }

        public async Task OnGet()
        {
            // awesome! now I think we have enough to get this ball rolling.
            // lets go fetch things like uptime etc
            
            // ok that's enough cool stuff now we also need the audit logs
            // and a snap of the AWS queues too
            

            ProcessId = Process.GetCurrentProcess().Id.ToString();
            ProcessPeakWorkingSet = Process.GetCurrentProcess().PeakWorkingSet64.ToString();
            ProcessStartTime = Process.GetCurrentProcess().StartTime.ToString();
            ProcessMachineName = Process.GetCurrentProcess().MachineName;
            ProcessProcessorTime = $"{Process.GetCurrentProcess().TotalProcessorTime.Days} DAYS {Process.GetCurrentProcess().TotalProcessorTime.Hours} HOURS {Process.GetCurrentProcess().TotalProcessorTime.Minutes} MINUTES {Process.GetCurrentProcess().TotalProcessorTime.Seconds} SECONDS";

            SystemProcesses = await _systemProcessRepository.GetDashboard();
            SystemProcessOperation = await _systemProcessOperationRepository.GetDashboard();
            SystemProcessRuleRun = await _systemProcessRuleRunRepository.GetDashboard();
            SystemProcessDistribute = await _systemProcessDistributeRepository.GetDashboard();
            ApiHeartbeat = await _apiHeartbeat.HeartsBeating();
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


    }
}