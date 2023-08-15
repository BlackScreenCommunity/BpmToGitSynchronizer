using Newtonsoft.Json;

namespace BpmToGitSynchronizer
{
    public class GitRepoConfiguration
    {
        public string Path { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Branch { get; set; }
        public string CommitMessage { get; set; }
    }
}
