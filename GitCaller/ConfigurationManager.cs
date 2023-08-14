using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitCaller
{
    public static class ConfigurationManager
    {
        public static List<Configuration> Load(string pathToConfigFile)
        {
            var fileOperator = new FileOperator();
            var configurationFileContent = fileOperator.ReadTextFromFile("caller_config.json");
            return JsonConvert.DeserializeObject<List<Configuration>>(configurationFileContent);
        }
    }
}
