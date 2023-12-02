/*
 * Parcel Logistics Service
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.22.1
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using B3B4G7.SKS.Package.Services.Filters;
using B3B4G7.SKS.Package.Services.OpenApi;
using B3B4G7.SKS.Package.Services.Formatters;
using B3B4G7.SKS.Package.Services.Helper;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using B3B4G7.SKS.Package.DataAccess.Sql.Context;
using B3B4G7.SKS.Package.BusinessLogic;
using B3B4G7.SKS.Package.BusinessLogic.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Interfaces;
using B3B4G7.SKS.Package.DataAccess.Sql;
using B3B4G7.SKS.Package.ServiceAgents.Interfaces;
using B3B4G7.SKS.Package.ServiceAgents;
using Newtonsoft.Json;
using B3B4G7.SKS.Package.WebhookManager.Interfaces;
using B3B4G7.SKS.Package.WebhookManager;
using System.Diagnostics.CodeAnalysis;

namespace B3B4G7.SKS.Package.Services
{
    /// <summary>
    /// Startup
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //read connectin string from configruation
            services
                .AddDbContext<TracknTraceContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite()));

            // Add framework services.
            services
                // Don't need the full MVC stack for an API, see https://andrewlock.net/comparing-startup-between-the-asp-net-core-3-templates/
                .AddControllers(options => {
                    options.InputFormatters.Insert(0, new InputFormatterStream());
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    });
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services
                .AddAutoMapper(typeof(AutoMapperProfiles).Assembly)
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            services
                .AddSwaggerGen(c =>
                {
                    c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                    
                    c.SwaggerDoc("1.22.1", new OpenApiInfo
                    {
                        Title = "Parcel Logistics Service",
                        Description = "Parcel Logistics Service (ASP.NET Core 6.0)",
                        TermsOfService = new Uri("https://github.com/openapitools/openapi-generator"),
                        Contact = new OpenApiContact
                        {
                            Name = "SKS",
                            Url = new Uri("http://www.technikum-wien.at/"),
                            Email = ""
                        },
                        License = new OpenApiLicense
                        {
                            Name = "NoLicense",
                            Url = new Uri("http://localhost")
                        },
                        Version = "1.22.1",
                    });
                    c.CustomSchemaIds(type => type.FriendlyId(true));

                    if(Assembly.GetEntryAssembly().GetName().Name != "testhost") {
                        c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");
                    }
                    
                    // Include DataAnnotation attributes on Controller Action parameters as OpenAPI validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });
                services
                    .AddSwaggerGenNewtonsoftSupport();

                services.AddTransient<ILogisticsPartnerLogic, LogisticsPartnerLogic>();
                services.AddTransient<IRecipientLogic, RecipientLogic>();
                services.AddTransient<ISenderLogic, SenderLogic>();
                services.AddTransient<IStaffLogic, StaffLogic>();
                services.AddTransient<IWarehouseManagementLogic, WarehouseManagementLogic>();
                services.AddTransient<IParcelWebHookLogic, ParcelWebHookLogic>();

                services.AddTransient<IParcelRepository, SqlParcelRepository>();
                services.AddTransient<IWarehouseRepository, SqlWarehouseRepository>();
                services.AddTransient<IWebhookRepository, SqlWebhookRepository>();

                services.AddTransient<IGeoEncodingAgent, OpenStreetEncodingAgent>();
                services.AddTransient<IWebHookManager, WebHookManager>();
                services.AddHttpClient();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TracknTraceContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger(c =>
                {
                    c.RouteTemplate = "openapi/{documentName}/openapi.json";
                })
                .UseSwaggerUI(c =>
                {
                    // set route prefix to openapi, e.g. http://localhost:8080/openapi/index.html
                    c.RoutePrefix = "openapi";
                    //TODO: Either use the SwaggerGen generated OpenAPI contract (generated from C# classes)
                    c.SwaggerEndpoint("/openapi/1.22.1/openapi.json", "Parcel Logistics Service");

                    //TODO: Or alternatively use the original OpenAPI contract that's included in the static files
                    // c.SwaggerEndpoint("/openapi-original.json", "Parcel Logistics Service Original");
                });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}