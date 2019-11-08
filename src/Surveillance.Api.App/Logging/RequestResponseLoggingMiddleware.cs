namespace Surveillance.Api.App.Logging
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class RequestResponseLoggingMiddleware : IMiddleware
    {
        private const int ReadChunkBufferLength = 4096;

        private const string XCorrelationIDHeader = "X-Correlation-ID";

        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger)
        {
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = await FormatRequest(context);

            this._logger.LogInformation(request);

            var originalBody = context.Response.Body;

            using (var newResponseBody = new MemoryStream())
            {
                context.Response.Body = newResponseBody;

                var sw = Stopwatch.StartNew();

                await next(context);

                sw.Stop();

                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalBody);

                newResponseBody.Seek(0, SeekOrigin.Begin);
                var response = FormatResponse(context, newResponseBody, sw.Elapsed);

                this._logger.LogInformation(response);
            }
        }

        private static string FormatCorrelationId(HttpContext context)
        {
            return
                $"X-Correlation-ID: {string.Join(",", context.Request.Headers.GetCommaSeparatedValues(XCorrelationIDHeader))}";
        }

        private static string FormatHeaders(IHeaderDictionary headers)
        {
            return headers?.Count > 0
                       ? $"Headers: {string.Join(",", headers?.Select(s => $"'{s.Key}': '{s.Value.ToString()}'"))}"
                       : null;
        }

        private static async Task<string> FormatRequest(HttpContext context)
        {
            var sb = new StringBuilder()
                .AppendLine(
                    $"Http Request Information {FormatUrl(context.Request)}, {FormatCorrelationId(context)}, TraceIdentifier: {context.TraceIdentifier}, ContentType: {context.Request.ContentType}")
                .AppendLine(FormatHeaders(context.Request.Headers))
                .AppendLine($"Request Body: {await GetRequestBody(context.Request)}");

            return sb.ToString();
        }

        private static string FormatResponse(HttpContext context, MemoryStream newResponseBody, TimeSpan elapsed)
        {
            var sb = new StringBuilder()
                .AppendLine(
                    $"Http Response Information {FormatUrl(context.Request)}, {FormatCorrelationId(context)}, TraceIdentifier: {context.TraceIdentifier}, StatusCode: {context.Response.StatusCode}, Elapsed: {elapsed},")
                .AppendLine(FormatHeaders(context.Response.Headers))
                .AppendLine($"Response Body: {ReadStreamInChunks(newResponseBody)}");

            return sb.ToString();
        }

        private static string FormatUrl(HttpRequest request)
        {
            return $"{request.Method} {request.Scheme}://{request.Host}{request.Path} {request.QueryString}";
        }

        private static async Task<string> GetRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            

            using (var requestStream = new MemoryStream())
            {
                await request.Body.CopyToAsync(requestStream);
                request.Body.Seek(0, SeekOrigin.Begin);
                return ReadStreamInChunks(requestStream);
            }
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;
            using (var textWriter = new StringWriter())
            using (var reader = new StreamReader(stream))
            {
                var readChunk = new char[ReadChunkBufferLength];
                int readChunkLength;

                // do while: is useful for the last iteration in case readChunkLength < chunkLength
                do
                {
                    readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
                    textWriter.Write(readChunk, 0, readChunkLength);
                }
                while (readChunkLength > 0);

                result = textWriter.ToString();
            }

            return result;
        }
    }
}