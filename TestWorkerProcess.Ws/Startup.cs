using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace TestWorkerProcess.Ws
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<GlobalTime>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Console.SetOut(new StreamWriter($".\\logs-{new Random().Next()}.txt") { AutoFlush = true });

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                    Console.Out.WriteLine("Success");
                }
                catch (Exception exception)
                {
                    Console.Out.WriteLine($"ERROR {exception.ToString()}");
                }
            });

            app.Use(async (context, next) =>
            {
                try
                {
                    var globalTime = context.RequestServices.GetService<GlobalTime>();
                    var startTimeMs = globalTime.Sw.ElapsedMilliseconds;

                    context.Response.OnStarting(state =>
                    {
                        var httpContext = (HttpContext) state;
                        var endTimeMs = globalTime.Sw.ElapsedMilliseconds;
                        var elaspsedMs = endTimeMs - startTimeMs;
                        var timeDetailsJson = JsonConvert.SerializeObject(new { elaspsedMs, startTimeMs, endTimeMs });
                        httpContext.Response.Headers.Add("TimeDetails", timeDetailsJson);
                        return Task.FromResult(0);
                    }, context);

                    await next();
                }
                catch (Exception exception)
                {
                    Console.Out.WriteLine($"ERROR {exception.ToString()}");
                }
            });

            app.UseMvc();
        }
    }
}