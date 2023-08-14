using Newtonsoft.Json;

namespace GitCaller
{
    public class GitRepoConfiguration
    {
        [JsonProperty("UserName")]
        public string UserName;

        [JsonProperty("Password")]
        public string Password;

        [JsonProperty("Branch")]
        public string Branch;
    }
}
