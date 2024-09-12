namespace BpmToGitSynchronizer
{
    public class BpmSoftConfiguration
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsNetCore { get; set; }
        public bool EnableManualCommit { get; set; }
    }
}
