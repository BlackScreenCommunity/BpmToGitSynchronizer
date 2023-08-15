using System;
using System.IO;
using System.Threading;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace GitCaller
{
    /// <summary>
    /// Handle user tasks
    /// Runs single time or plans periodically tasks 
    /// </summary>
    public class TaskManager
    {
        private static ManualResetEvent waitHandle = new ManualResetEvent(false);

        private static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
                return builder.Build();
            }
        }

        /// <summary>
        /// Schedule periodic task to pull-push changes
        /// </summary>
        public static void RunSheduledTask()
        {
            var operationPeriodInHour = int.Parse(Configuration["PushPullPeriodInHours"]);
            Console.WriteLine($"Run task every {operationPeriodInHour} hours");
            var aTimer = new System.Timers.Timer(operationPeriodInHour * 60 * 60 * 1000);
            aTimer.Elapsed += new ElapsedEventHandler(RunTask);
            aTimer.Start();
            waitHandle.WaitOne();
        }

        /// <summary>
        /// Event handler - runs pull-push task
        /// </summary>
        private static void RunTask(object source, ElapsedEventArgs e)
        {
            RunAutoCommiter();
        }

        /// <summary>
        /// Runs common scenario
        /// Pulls changes from bpmsoft to filesystem
        /// Prepares commits and pushes them to git repository
        /// </summary>
        public static void RunAutoCommiter()
        {
            Console.WriteLine($"Run task at {DateTime.Now}");

            var configurations = PushPullConfigurationManager.Load();

            foreach (var configuration in configurations)
            {
                BpmSoftOperator sender = new BpmSoftOperator(configuration.BpmSoft.Url, configuration.BpmSoft.UserName, configuration.BpmSoft.Password, configuration.BpmSoft.IsNetCore);
                Console.WriteLine($"BpmSoftOperator: {sender.PullChangesToFileSystem()}");

                var gitOperator = new GitOperator(
                    configuration.GitRepo.Path, 
                    configuration.GitRepo.UserName, 
                    configuration.GitRepo.Password, 
                    configuration.GitRepo.Branch,
                    configuration.GitRepo.CommitMessage
                );

                gitOperator.StageChanges();
                gitOperator.CommitChanges();
                gitOperator.PushChanges();
            }
        }
    }
}
