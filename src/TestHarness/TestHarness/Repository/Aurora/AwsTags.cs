namespace TestHarness.Repository.Aurora
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;
    using Amazon.Util;

    public static class AwsTags
    {
        public static string Environment = "Environment";

        public static bool IsLiveEc2Instance()
        {
            try
            {
                if (!IsEc2Instances()) return false;

                var env = GetTag(Environment);

                return string.Equals(env, "live", StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception)
            {
                return true;
            }
        }

        private static string GetTag(string name)
        {
            var instanceId = EC2InstanceMetadata.InstanceId;
            var client = new AmazonEC2Client(
                new AmazonEC2Config
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            var tags = client.DescribeTagsAsync(
                new DescribeTagsRequest
                    {
                        Filters = new List<Filter>
                                      {
                                          new Filter("resource-id", new List<string> { instanceId }),
                                          new Filter("key", new List<string> { name })
                                      }
                    }).Result.Tags;

            return tags?.FirstOrDefault()?.Value;
        }

        private static bool IsEc2Instances()
        {
            return EC2InstanceMetadata.InstanceId != null;
        }
    }
}