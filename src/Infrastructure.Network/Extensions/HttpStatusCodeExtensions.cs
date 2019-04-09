﻿using Amazon.Runtime;
using System;
using System.Net;
using System.Net.Http;

namespace Infrastructure.Network.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static HttpStatusCode EnsureSuccessStatusCode(this HttpStatusCode statusCode)
            => new HttpResponseMessage(statusCode).EnsureSuccessStatusCode().StatusCode;

        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
            => new HttpResponseMessage(statusCode).IsSuccessStatusCode;
    }

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
