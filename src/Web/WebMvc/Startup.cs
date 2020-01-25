namespace OpenCodeCamp.WebMvc
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Localization.Routing;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using OpenCodeCamp.WebMvc.Infrastructure;
    using OpenCodeCamp.WebMvc.Services;

    public class LocalizationPipeline
    {
        public void Configure(IApplicationBuilder app, RequestLocalizationOptions options)
        {
            app.UseRequestLocalization(options);
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppSettings>(Configuration);

            services.AddLocalization(opts =>
                opts.ResourcesPath = "Resources"
            );

            services
                .AddControllersWithViews()
                //.AddMvc()
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .Services
                .AddHttpClientServices(Configuration)
            ;

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("en"),
                new CultureInfo("fr-FR"),
                new CultureInfo("fr")
            };

            var options = new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            options.RequestCultureProviders = new[]
            {
                 new RouteDataRequestCultureProvider() { Options = options }
            };
            options.RequestCultureProviders = new[] {
    new RouteDataRequestCultureProvider()
        {
            RouteDataStringKey = "lang",
            Options = options
        }
    };

            services.AddSingleton(options);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                //ApplyCurrentCultureToResponseHeaders = true
            });

            // Matches request to an endpoint.
            app.UseRouting();

            // Endpoint aware middleware. 
            // Middleware can use metadata from the matched endpoint.
            app.UseAuthorization();

            // Execute the matched endpoint.
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //   name: "Test1",
                //   pattern: "test1",
                //   defaults: new { lang = "en" },
                //   constraints: new { Controller = "Marketing", action = "Test" });

                //endpoints.MapControllerRoute(
                //   name: "Test2",
                //   pattern: "test2",
                //   defaults: new { lang = "en", Controller = "Marketing", action = "Test" },
                //   constraints: new { });

                endpoints.MapControllerRoute(
                    name: "NewsletterSubscriptionConfirmation",
                    pattern: "confirm-newsletter-subscription/{email}/{token}",
                    defaults: new { lang = "en", Controller = "Marketing", action = "ConfirmNewsletterSubscription" },
                    constraints: new { });

                endpoints.MapControllerRoute(
                   name: "Localized",
                   pattern: "{lang}/{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                   name: "LocalizedDefault",
                   pattern: "{lang=en}/{controller}/{action}/{id?}");

                endpoints.MapControllerRoute(
                   name: "IndexDefault",
                   pattern: "{lang}/{controller}/{action}",
                   defaults: new { lang = "en", Controller = "Home", action = "Index" });
            });
        }
    }

    static class ServiceCollectionExtensions
    {
        // Adds all Http client services
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register delegating handlers
            //services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<HttpClientRequestIdDelegatingHandler>();

            services.AddHttpClient<IMarketingService, MarketingService>()
                 //.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                 .AddHttpMessageHandler<HttpClientRequestIdDelegatingHandler>()
                 //.AddDevspacesSupport()
                 ;


            //add custom application services
            //services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            return services;
        }
    }
}