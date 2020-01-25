namespace Marketing.FunctionalTests
{
    using Microsoft.AspNetCore.TestHost;
    using System;
    using System.Net.Http;

    internal static class HttpClientExtensions
    {
        internal static HttpClient CreateIdempotentClient(this TestServer server)
        {
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());
            return client;
        }
    }
}