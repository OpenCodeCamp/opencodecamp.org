namespace Marketing.FunctionalTests
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using OpenCodeCamp.BuildingBlocks.IntegrationEventLogEF;
    using OpenCodeCamp.Services.Marketing.Api.Application.Commands;
    using OpenCodeCamp.Services.Marketing.Api.Infrastructure;
    using OpenCodeCamp.Services.Marketing.Infrastructure;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class MarketingScenarios : IClassFixture<CustomWebApplicationFactory<OpenCodeCamp.Services.Marketing.Api.Startup>>
    {
        private readonly CustomWebApplicationFactory<OpenCodeCamp.Services.Marketing.Api.Startup> _factory;

        public MarketingScenarios(CustomWebApplicationFactory<OpenCodeCamp.Services.Marketing.Api.Startup> factory)
        {
            this._factory = factory;
        }

        private static class PostUrls
        {
            internal static string NewsletterSubscriptions = "api/v1/newslettersubscriptions";

            internal static string SubscribeToNewsletter = NewsletterSubscriptions + "/create";
        }

        //[Fact]
        //public async Task Subscribe_to_newsletter_invalid_email_created_bad_request_response()
        //{
        //    // Arrange
        //    var content = new StringContent(this.BuildNewsletterSubscriptionCreationDto(validEmail: false), UTF8Encoding.UTF8, "application/json");
        //    var client = this._factory.CreateClient();

        //    // Act
        //    var response = await client.PostAsync(PostUrls.SubscribeToNewsletter, content);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Subscribe_to_newsletter_invalid_email_lang_bad_request_response()
        //{
        //    // Arrange
        //    var content = new StringContent(this.BuildNewsletterSubscriptionCreationDto(validLang: false), UTF8Encoding.UTF8, "application/json");
        //    var client = this._factory.CreateClient();

        //    // Act
        //    var response = await client.PostAsync(PostUrls.SubscribeToNewsletter, content);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //}

        [Fact]
        public async Task BasicIntegrationTest()
        {
            // Arrange
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    // Add TestServer
                    webHost.UseTestServer();

                    // Specify the environment
                    webHost.UseEnvironment("Test");

                    webHost.Configure(app => app.Run(async ctx => await ctx.Response.WriteAsync("Hello World!")));
                });

            // Create and start up the host
            var host = await hostBuilder.StartAsync();

            // Create an HttpClient which is setup for the test host
            var client = host.GetTestClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            //responseString.Should().Be("Hello World!");
            Assert.Equal("Hello World!", responseString);
        }

        //[Fact]
        //public async Task BasicEndPointTest()
        //{
        //    //// Arrange
        //    //var hostBuilder = new HostBuilder()
        //    //    .ConfigureWebHost(webHost =>
        //    //    {
        //    //// Add TestServer
        //    //webHost.UseTestServer();
        //    //        webHost.UseStartup<OpenCodeCamp.Services.Marketing.Api.Startup>();
        //    //    });

        //    //// Create and start up the host
        //    //var host = await hostBuilder.StartAsync();

        //    //// Create an HttpClient which is setup for the test host
        //    //var client = host.GetTestClient();

        //    //// Act
        //    //var response = await client.GetAsync("/Home/Test");

        //    //// Assert
        //    //var responseString = await response.Content.ReadAsStringAsync();
        //    //Assert.Equal("Hello World!", responseString);
        //    string path = Assembly.GetAssembly(typeof(MarketingScenarioBase))
        //            .Location;
        //    IHost host = hshshs.CreateHostBuilder2(path).Build();

        //    using (var scope = host.Services.CreateScope())
        //    {
        //        var services = scope.ServiceProvider;
        //        var marketingContextLogger = services.GetRequiredService<ILogger<MarketingContext>>();
        //        var marketingContext = services.GetService<MarketingContext>();

        //        marketingContext.Database.EnsureCreated();
        //        marketingContext.Database.Migrate();
        //        new MarketingContextSeed().SeedAsync(marketingContext).Wait();

        //        var integrationEventLogContextLogger = services.GetRequiredService<ILogger<IntegrationEventLogContext>>();
        //        var integrationEventLogContext = services.GetService<IntegrationEventLogContext>();

        //        integrationEventLogContext.Database.EnsureCreated();
        //    }

        //    var testServer = host.GetTestServer();
        //    //host.Run();
        //    var client = testServer.CreateClient();

        //}

        string BuildNewsletterSubscriptionCreationDto(bool validEmail = true, bool validLang = true)
        {
            var command = new SubscribeToNewsletterCommand()
            {
                EmailAddress = validEmail ? "marketing@unit-tests.com" : "marketing-unit-tests.com",
                Language = validLang ? "fr" : "english-language"
            };
            return JsonConvert.SerializeObject(command);
        }
    }
}