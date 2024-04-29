using System.Linq;

namespace BpmToGitSynchronizer
{
    /// <summary>
    /// Runs program
    /// No arguments - does single commit and runs scheduled task to perform commits periodically
    /// ForceCommit - single time run. Performs pull to file system from bpmsoft and push commits to git
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                TaskManager.RunAutoCommiter();
                TaskManager.RunSheduledTask();
            }

            if (args.Length >= 1 && (new string[] { "ForceCommit", "Commit" }).Any(s => args[0].Contains(s)))
            {
                TaskManager.RunAutoCommiter();
            }
        }
    }
}
