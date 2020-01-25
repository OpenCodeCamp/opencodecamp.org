namespace Marketing.FunctionalTests
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OpenCodeCamp.Services.Marketing.Api;

    public class MarketingTestsStartup : Startup
    {
        public MarketingTestsStartup(IConfiguration env) : base(env)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Added to avoid the Authorize data annotation in test environment. 
            // Property "SuppressCheckForUnhandledSecurityMetadata" in appsettings.json
            services.Configure<RouteOptions>(Configuration);
            base.ConfigureServices(services);
        }
    }
}