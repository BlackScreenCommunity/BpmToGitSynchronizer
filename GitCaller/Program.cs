using System;
using System.Threading;
using System.Timers;

namespace GitCaller
{
    class Program
    {
        private static ManualResetEvent waitHandle = new ManualResetEvent(false);

        static void Main()
        {
            var aTimer = new System.Timers.Timer(60 * 60 * 1000);
            aTimer.Elapsed += new ElapsedEventHandler(RunTask);
            aTimer.Start();
            waitHandle.WaitOne();
        }

        private static void RunTask(object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"Run task at {DateTime.Now}");

            var configurations = ConfigurationManager.Load("caller_config.json");

            foreach (var configuration in configurations)
            {
                BpmSoftOperator sender = new BpmSoftOperator(configuration.BpmSoft.Url, configuration.BpmSoft.UserName, configuration.BpmSoft.Password);
                Console.WriteLine($"BpmSoftOperator: {sender.PullChangesToFileSystem()}");

                var gitOperator = new GitOperator(configuration.GitRepo.Path, configuration.GitRepo.UserName, configuration.GitRepo.Password, configuration.GitRepo.Branch);

                gitOperator.StageChanges();
                gitOperator.CommitChanges();
                gitOperator.PushChanges();
            }
        }
    }
}
