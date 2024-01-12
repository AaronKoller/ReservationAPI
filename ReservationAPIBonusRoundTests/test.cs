using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ReservationAPIBonusRound.Models.AppSettings;
using TestInfrastructure;

namespace ReservationAPIBonusRoundTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        public AppSettingsOptions appSettings;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, builder) =>
            {
                var _configuration = TestConfigurationProvider.ConfigFromObject(appSettings);
                builder.AddConfiguration(_configuration);
            });
            builder.UseEnvironment("Development");
        }
    }
}
