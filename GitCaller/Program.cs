namespace GitCaller
{
    /// <summary>
    /// Runs program
    /// No arguments - runs scheduled task to perform commits periodically
    /// ForceCommit - single time run. Performs pull to file system from bpmsoft and push commits to git
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                TaskManager.RunSheduledTask();
            }

            if (args.Length >= 1 && args[0] == "ForceCommit")
            {
                TaskManager.RunAutoCommiter();
            }
        }
    }
}
