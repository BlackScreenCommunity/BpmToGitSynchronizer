using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                Task task1 = Task.Run(() => TaskManager.RunManualCommiter());
                Task task2 = Task.Run(() => TaskManager.RunSheduledTask());
                Task.WaitAll(task1, task2);               
            }

            if (args.Length >= 1 && (new string[] { "ForceCommit", "Commit" }).Any(s => args[0].Contains(s)))
            {
                TaskManager.RunAutoCommiter();
            }
        }
    }
}
