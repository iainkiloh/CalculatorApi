using AspNetCoreRateLimit;
using CalculatorApi.Infrastructure.Services;
using CalculatorApi.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace CalculatorApi
{
    /// <summary>
    /// Represents the startup process for the application.
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The current hosting environment.</param>
        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               //no secrets fie in use
               //.AddJsonFile($"secrets/appsettings.{env.EnvironmentName}.secrets.json", optional: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();

        }

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        /// <value>The current application configuration.</value>
        public IConfigurationRoot Configuration { get; }
        
        /// <summary>
        /// Configures services for the application.
        /// </summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public void ConfigureServices(IServiceCollection services)
        {

            // needed to load configuration from appsettings.json
            services.AddOptions();

            //Rate Limiting
            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            //inject the ICalculator implementation - scoped means new instance on each request
            services.AddScoped<ICalculator, Calculator>();

            //load mvc framework
            services.AddMvc();

            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            services.AddMvcCore().AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });


            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddSwaggerGen(
                options =>
                {
                    // resolve the IApiVersionDescriptionProvider service
                    // note: that we have to build a temporary service provider here because one has not been created yet
                    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                    // add a swagger document for each discovered API version
                    // note: you might choose to skip or document deprecated API versions differently
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    }

                    //this api is not using Bearer Authentication
                    //options.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                    //options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    //    { "Bearer", Enumerable.Empty<string>() },
                    //});

                    //note that password grants OAuth fow contains a bug in SwaggerUI which meakes them unusable at this point
                    //we therefore are not using the sign in functionality at the moment
                    //https://github.com/swagger-api/swagger-ui/issues/4192
                    //options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    //{
                    //     Type = "oauth2", Flow = "password", TokenUrl = "http://notimplemented"
                    //});

                    //get string valus for enums in swagger interface
                    options.DescribeAllEnumsAsStrings();

                    // integrate xml comments (Not implemented in this api)
                    // options.IncludeXmlComments(XmlCommentsFilePath);
                });
        }

        /// <summary>
        /// Configures the application using the provided builder, hosting environment, and logging factory.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        /// <param name="env">The current hosting environment.</param>
        /// <param name="loggerFactory">The logging factory used for instrumentation.</param>
        /// <param name="provider">The API version descriptor provider used to enumerate defined API versions.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider, IApplicationLifetime applicationLifetime)
        {
            app.UseIpRateLimiting();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                });
        }

        static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"CalculatorApi {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "CalculatorApi",
                Contact = new Contact() { Name = "Iain Kiloh", Email = "iainkiloh@gmail.com" },
                TermsOfService = "Private",
                License = new License() { Name = "None", Url = "https://kilohsoftware.com" }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

    }

}
