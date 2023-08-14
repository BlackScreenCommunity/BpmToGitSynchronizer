using Newtonsoft.Json;

namespace GitCaller
{
    public class Configuration
    {
        [JsonProperty("BpmSoft")]
        public BpmSoftConfiguration BpmSoft;

        [JsonProperty("GitRepo")]
        public GitRepoConfiguration GitRepo;
    }
}
