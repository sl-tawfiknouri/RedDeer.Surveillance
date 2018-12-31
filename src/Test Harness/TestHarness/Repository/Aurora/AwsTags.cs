using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace TestHarness.Repository.Aurora
{
    public static class AwsTags
    {
        public static string Environment = "Environment";

        public static bool IsEc2Instances()
        {
            return Amazon.Util.EC2InstanceMetadata.InstanceId != null;
        }

        public static bool IsLiveEc2Instance()
        {
            if (!IsEc2Instances())
            {
                return false;
            }

            var env = GetTag(Environment);

            return string.Equals(env, "live", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetTag(string name)
        {
            var instanceId = Amazon.Util.EC2InstanceMetadata.InstanceId;
            var client = new AmazonEC2Client(new AmazonEC2Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            });

            var tags = client.DescribeTagsAsync(new DescribeTagsRequest
            {
                Filters = new List<Filter>
                {
                    new Filter("resource-id", new List<string> { instanceId }),
                    new Filter("key", new List<string> { name }),
                }
            }).Result.Tags;

            return tags?.FirstOrDefault()?.Value;
        }
    }
}
