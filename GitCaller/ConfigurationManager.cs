using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitCaller
{
    /// <summary>
    /// Reads and prepares bpmsoft and git-repo configurations 
    /// </summary>
    public static class ConfigurationManager
    {
        public static List<Configuration> Load(string pathToConfigFile = "caller_config.json")
        {
            var fileOperator = new FileOperator();
            // TODO implement loading of the configuration file from starting arguments
            var configurationFileContent = fileOperator.ReadTextFromFile(pathToConfigFile);
            return JsonConvert.DeserializeObject<List<Configuration>>(configurationFileContent);
        }
    }
}
