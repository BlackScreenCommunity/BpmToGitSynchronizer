using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BpmToGitSynchronizer
{
    /// <summary>
    /// Reads and prepares bpmsoft and git-repo configurations 
    /// </summary>
    public static class PushPullConfigurationManager
    {
        private static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
                return builder.Build();
            }
        }
        public static List<PushPullConfiguration> Load()
        {
            return Configuration.GetSection("PushPullConfiguration").Get<List<PushPullConfiguration>>();
        }
    }
}
