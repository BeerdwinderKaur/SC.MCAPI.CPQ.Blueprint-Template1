using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(config =>
                    {
                        var appConfigUri = config.Build().GetConnectionString("appConfiguration");
                        if (!string.IsNullOrWhiteSpace(appConfigUri))
                        {
                            config.AddAzureAppConfiguration(options =>
                            {
                                options.Connect(new Uri(appConfigUri), new DefaultAzureCredential());
                                options.ConfigureRefresh(refresh =>
                                {
                                    refresh
                                        .Register("refreshAll", true)
                                        .SetCacheExpiration(TimeSpan.FromSeconds(5));
                                });
                            });
                        }
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
