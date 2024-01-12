using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TestInfrastructure.AppSettings.InMemoryFileProvider;

namespace TestInfrastructure
{
    public static class TestConfigurationProvider
    {
        public static IConfigurationRoot ConfigFromObject(object objectConfig, string inMemoryAppSettingsName = "AppSettings.Test.json")
        {
            var jsonSerializationSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var fullJsonConfig = JsonConvert.SerializeObject(objectConfig, Formatting.Indented, jsonSerializationSettings);
            return ConfigFromJson(fullJsonConfig, inMemoryAppSettingsName);
        }

        public static IConfigurationRoot ConfigFromJson(string fullJsonConfig, string inMemoryAppSettingsName)
        {
            var memoryFileProvider = new InMemoryFileProvider(fullJsonConfig);
            return new ConfigurationBuilder().AddJsonFile(memoryFileProvider, inMemoryAppSettingsName, false, false).Build();
            //return new ConfigurationBuilder().AddJsonFile(memoryFileProvider, "AppSettings.Test.json", false, false).Build();
        }
    }
}
