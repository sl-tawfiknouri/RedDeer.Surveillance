namespace Surveillance.Api.App.Configuration
{
    using System;
    using System.Linq;

    using Amazon.Util;

    using Surveillance.Api.App.Configuration.Interfaces;

    public class EnvironmentService : IEnvironmentService
    {
        public bool IsEc2Instance()
        {
            return this.IsUnitTest() == false && EC2InstanceMetadata.InstanceId != null;
        }

        public bool IsUnitTest()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));
        }
    }
}