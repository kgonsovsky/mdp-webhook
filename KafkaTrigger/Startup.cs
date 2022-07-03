using System;
using EventGrid;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(KafkaTrigger.Startup))]

namespace KafkaTrigger
{
    class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string cs = Environment.GetEnvironmentVariable("MdpSettings");
            builder.ConfigurationBuilder.AddAzureAppConfiguration(cs);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<MdpSettings>().Configure<IConfiguration>((settings, configuration) => configuration.GetSection("MdpSettings").Bind(settings));

        }
    }
}