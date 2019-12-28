using Amazon.Runtime;
using System;
using System.Net;

namespace RedDeer.Etl.SqlSriptExecutor.Extensions
{
    public static class AmazonWebServiceResponseExtensions
    {
        public static HttpStatusCode EnsureSuccessStatusCode(this AmazonWebServiceResponse amazonWebServiceResponse)
        {
            if (amazonWebServiceResponse?.HttpStatusCode == null)
            {
                throw new ArgumentNullException(nameof(amazonWebServiceResponse));
            }

            return amazonWebServiceResponse.HttpStatusCode.EnsureSuccessStatusCode();
        }


        public static bool IsSuccessStatusCode(this AmazonWebServiceResponse amazonWebServiceResponse)
        {
            if (amazonWebServiceResponse?.HttpStatusCode == null)
            {
                throw new ArgumentNullException(nameof(amazonWebServiceResponse));
            }

            return amazonWebServiceResponse.HttpStatusCode.IsSuccessStatusCode();
        }
    }
}
