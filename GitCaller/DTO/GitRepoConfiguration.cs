using Newtonsoft.Json;

namespace GitCaller
{
    public class GitRepoConfiguration
    {
        [JsonProperty("Path")]
        public string Path;

        [JsonProperty("UserName")]
        public string UserName;

        [JsonProperty("Password")]
        public string Password;

        [JsonProperty("Branch")]
        public string Branch;

        [JsonProperty("CommitMessage")]
        public string CommitMessage;
    }
}
