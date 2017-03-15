using Halcyon.Web.HAL.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halcyon.HAL.Attributes;

namespace Halcyon.Tests.SelfHost.Mvc {
    public class Startup {
        public Startup(IHostingEnvironment env) {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Add framework services.
            services
                .AddMvc()
                .AddMvcOptions(c => {
                    c.OutputFormatters.RemoveType<JsonOutputFormatter>();
                    c.OutputFormatters.Add(new JsonHalOutputFormatter(
                        new string[] { "application/hal+json", "application/vnd.example.hal+json", "application/vnd.example.hal.v1+json" },
                        converters: new HALAttributeConverter()
                    ));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                .AddDebug();
                
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
