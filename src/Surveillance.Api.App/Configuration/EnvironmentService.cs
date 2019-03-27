using Surveillance.Api.App.Configuration.Interfaces;
using System;
using System.Linq;

namespace Surveillance.Api.App.Configuration
{
    public class EnvironmentService : IEnvironmentService
    {
        public bool IsUnitTest()
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));
        }

        public bool IsEc2Instance()
        {
            return 
                IsUnitTest() == false
                && Amazon.Util.EC2InstanceMetadata.InstanceId != null;
        }
    }
}
