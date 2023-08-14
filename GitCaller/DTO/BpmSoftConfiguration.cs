using Newtonsoft.Json;

namespace GitCaller
{
    public class BpmSoftConfiguration
    {
        [JsonProperty("Url")]
        public string Url;

        [JsonProperty("UserName")]
        public string UserName;

        [JsonProperty("Password")]
        public string Password;
    }
}
