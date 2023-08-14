using System;

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
}
