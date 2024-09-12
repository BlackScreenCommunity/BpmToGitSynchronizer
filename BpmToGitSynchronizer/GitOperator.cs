using System;
using System.Xml.Serialization;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace BpmToGitSynchronizer
{
    /// <summary>
    /// Performs Git-repository operations
    /// </summary>
    class GitOperator
    {
        string repoPath;
        string username;
        string branch;
        string password;
        string commitMessage;

        /// <summary>
        /// Path to folder with repository
        /// </summary>
        public string RepoPath { get => repoPath; set => repoPath = value; }
        /// <summary>
        /// VCS Username
        /// </summary>
        public string UserName { get => username; set => username = value; }
        /// <summary>
        /// VCS Password
        /// </summary>
        public string Password { get => password; set => password = value; }
        /// <summary>
        /// Branch name where changes will be committed
        /// </summary>
        public string Branch { get => branch; set => branch = value; }
        /// <summary>
        /// Default part of tme message.
        /// The message is formed according to the following template: "YYYY-MM-DD {CommitMessage}"
        /// </summary>
        public string CommitMessage { get => commitMessage; set => commitMessage = value; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="repoPath">Path to folder with repository</param>
        /// <param name="username">VCS Username</param>
        /// <param name="password">VCS Password</param>
        /// <param name="branch">Branch name where changes will be committed</param>
        /// <param name="commitMessage">Commit message</param>
        public GitOperator(string repoPath, string username, string password, string branch, string commitMessage = "Commit")
        {
            RepoPath = repoPath;
            UserName = username;
            Password = password;
            Branch = branch;
            CommitMessage = commitMessage;
        }
        
        /// <summary>
        /// Constructor
        /// Create GitOperator from GitRepoConfiguration object
        /// </summary>
        /// <param name="repoConfiguration">Git repository configuration from appsettings</param>
        public GitOperator(GitRepoConfiguration repoConfiguration) {
            RepoPath = repoConfiguration.Path;
            UserName = repoConfiguration.UserName;
            Password = repoConfiguration.Password;
            Branch = repoConfiguration.Branch;
            CommitMessage = repoConfiguration.CommitMessage;
        }
        
        /// <summary>
        /// Stage all the Changes/Untracked/Deleted files
        /// </summary>
        public void StageChanges()
        {
            try
            {
                using (var repo = new Repository(RepoPath))
                {
                    Commands.Stage(repo, "*");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StageChanges " + ex.Message);
            }
        }

        /// <summary>
        /// Perform commit
        /// </summary>
        public void CommitChanges()
        {
            try
            {
                using (var repo = new Repository(RepoPath))
                {
                    repo.Commit($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm")} {CommitMessage}", new Signature(UserName, Branch, DateTimeOffset.Now),
                    new Signature(UserName, Branch, DateTimeOffset.Now));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("CommitChanges " + e.Message);
            }
        }

        /// <summary>
        /// Push changes to Remote repo
        /// </summary>
        public void PushChanges()
        {
            try
            {
                using (var repo = new Repository(RepoPath))
                {
                    PushOptions options = new PushOptions();
                    options.CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials()
                            {
                                Username = UserName,
                                Password = Password 
                            });
                    repo.Network.Push(repo.Branches[Branch], options);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when pushing changes " + e.Message);
                throw new Exception("Ошибка выполнения PUSH в репозиторий", e);
            }
        }
    }
}
