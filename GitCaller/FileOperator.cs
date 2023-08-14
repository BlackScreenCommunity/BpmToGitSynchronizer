using System;
using System.IO;

namespace GitCaller
{
    // <summary>
    /// File system worker
    /// Reads and write configuration files
    /// </summary>
    public class FileOperator
    {
        /// <summary>
        /// Read file 
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>File content</returns>
        public string ReadTextFromFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"{filePath} file doesn't exists");
                return string.Empty;
            }
        }

        /// <summary>
        /// Write string to file
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="content">File's content 
        /// </param>
        public void WriteTextToFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"{filePath} file doesn't exists");
            }
        }
    }
}
