﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Collections;

namespace publisher
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var instrumentationType = Type.GetType("Datadog.Trace.ClrProfiler.Instrumentation, SignalFx.Tracing.ClrProfiler.Managed");
                    var profilerAttached = instrumentationType?.GetProperty("ProfilerAttached", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ?? false;
                    var tracerAssemblyLocation = Type.GetType("SignalFx.Tracing.Tracer, SignalFx.Tracing")?.Assembly.Location;
                    var clrProfilerAssemblyLocation = instrumentationType?.Assembly.Location;
                    var nl = Environment.NewLine;

                    await context.Response.WriteAsync($"Profiler attached: {profilerAttached}{nl}");
                    await context.Response.WriteAsync($"SignalFx.Tracing: {tracerAssemblyLocation}{nl}");
                    await context.Response.WriteAsync($"Datadog.Trace.ClrProfiler.Managed: {clrProfilerAssemblyLocation}{nl}");

                    foreach (var envVar in GetEnvironmentVariables())
                    {
                        await context.Response.WriteAsync($"{envVar.Key}={envVar.Value}{nl}");
                    }

                    await context.Response.WriteAsync("Hello World Publisher: v4");
                });

                endpoints.MapGet("/bad-request", context =>
                {
                    throw new Exception("Hello World! Exception");
                });

                endpoints.MapGet("/status-code/{statusCode=200}", async context =>
                {
                    object statusCode = context.Request.RouteValues["statusCode"];
                    context.Response.StatusCode = int.Parse((string)statusCode);
                    await context.Response.WriteAsync($"Status Code: {statusCode}");
                });
            });
        }

        // Added to get environment variables
        private IEnumerable<KeyValuePair<string, string>> GetEnvironmentVariables()
        {
            var prefixes = new[]
                           {
                               "COR_",
                               "CORECLR_",
                               "DD_",
                               "DATADOG_"
                           };

            var envVars = from envVar in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>()
                          from prefix in prefixes
                          let key = (envVar.Key as string)?.ToUpperInvariant()
                          let value = envVar.Value as string
                          where key.StartsWith(prefix)
                          orderby key
                          select new KeyValuePair<string, string>(key, value);

            return envVars;
        }
    }
}
