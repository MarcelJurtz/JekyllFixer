using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace JekyllFixer
{
    class Program
    {
        const string DEFAULT_PATH = "/home/marcel/Development/Web/blog/_site";

        static string jekyllPath;
        const int MAX_INPUTS = 3;
        static List<string> excludes = new List<string>(){"index.html", "search.html"};

        static void Main(string[] args)
        {
            Console.WriteLine($"Use default path: {DEFAULT_PATH} ? (Y/n)");

            if(Console.ReadKey().Key == ConsoleKey.Enter || Console.ReadKey().Key == ConsoleKey.Y) 
            {
                if(Directory.Exists(DEFAULT_PATH))
                    jekyllPath = DEFAULT_PATH;
                else
                    jekyllPath = RequestDirectory(MAX_INPUTS);
            } else 
            {
                jekyllPath = RequestDirectory(MAX_INPUTS);
            }

            bool automaticMode = false;
            Console.WriteLine("Use automatic mode? y/N?");
            if(Console.ReadKey().Key == ConsoleKey.Y)
                automaticMode = true;

            // Search for posts
            var files = FindInvalidJekyllFiles(jekyllPath);
            Console.WriteLine($"Found {files.Count} invalid files");
            foreach(var file in files)
            {
                if(excludes.Contains(file))
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(file);

                if(!automaticMode)
                    Console.WriteLine($"Move {fileName} to {fileName}/index.html? (Y/n)");

                if(automaticMode || Console.ReadKey().Key == ConsoleKey.Enter || Console.ReadKey().Key == ConsoleKey.Y) {
                    // Move file to folder and rename to index.html
                    string newFolderPath = Path.Combine(Path.GetDirectoryName(file), fileName);
                    Directory.CreateDirectory(newFolderPath);
                    File.Move(file, Path.Combine(newFolderPath, "index.html"));
                    Console.WriteLine($"Moving {fileName} to {Path.Combine(newFolderPath, "index.html")}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Done. Press any key to exit");
            Console.Read();
        }

        private static List<string> FindInvalidJekyllFiles(string path)
        {
            return Directory.GetFiles(path, "*.html", SearchOption.AllDirectories)
                .Where(x => !excludes.Contains(Path.GetFileName(x))).ToList();
        }

        private static string RequestDirectory(int maxTries)
        {
            int inputs = 0;

            do{
                Console.WriteLine("Enter Jekyll _site path: ");
                jekyllPath = Console.ReadLine();
                inputs++;
            }
            while(!Directory.Exists(jekyllPath) && inputs < maxTries);

            if(!Directory.Exists(jekyllPath)) {
                Console.WriteLine("Invalid path entered. Press any key to terminate.");
                Console.Read();
                return null;
            }

            return jekyllPath;
        }
    }
}
