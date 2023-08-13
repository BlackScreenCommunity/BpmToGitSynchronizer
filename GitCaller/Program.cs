using System;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitCaller
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string testRepo = @"C:\Users\nkuzmin\Desktop\git\demorepository\";
            var gitOperator = new GitOperator(testRepo, "nickkuzmin1301@gmail.com", "", "master");

            gitOperator.StageChanges();
            gitOperator.CommitChanges();
            gitOperator.PushChanges();
        }
    }

    class GitOperator
    {
        string repoPath;
        string username;
        string branch;
        string password;
        string commitMessage;

        public string RepoPath { get => repoPath; set => repoPath = value; }
        public string UserName { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Branch { get => branch; set => branch = value; }
        public string CommitMessage { get => commitMessage; set => commitMessage = value; }

        public GitOperator(string repoPath, string username, string password, string branch, string commitMessage = "Commit")
        {
            RepoPath = repoPath;
            UserName = username;
            Password = password;
            Branch = branch;
            CommitMessage = commitMessage;
        }

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

        public void CommitChanges()
        {
            try
            {
                using (var repo = new Repository(RepoPath))
                {
                    repo.Commit($"{DateTime.Now.ToString("yyyy-MM-dd")} {CommitMessage}", new Signature(UserName, Branch, DateTimeOffset.Now),
                    new Signature(UserName, Branch, DateTimeOffset.Now));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("CommitChanges " + e.Message);
            }
        }

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
                Console.WriteLine("PushChanges " + e.Message);
            }
        }
    }
}
