using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace GitLogChurn
{
    class Program
    {
        static void Main(string[] args)
        {
            var startInfo = new ProcessStartInfo("git", "log --name-only  --pretty=format:\"%an|%cd\"")
            {
                WorkingDirectory = args[0],
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            string log;
            using (var process = Process.Start(startInfo)) 
                log = process.StandardOutput.ReadToEnd();

            var commitBlocks = log.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            var files = commitBlocks.AsParallel().SelectMany(commit =>
            {
                var lines = commit.Split('\n');

                var infoParts = lines.First().Split('|');
                var modifiedDate = DateTime.ParseExact(infoParts[1], "ddd MMM d HH:mm:ss yyyy zz00", new CultureInfo("en-US"));
                var author = infoParts[0];

                return lines.Skip(1).Select(name => new File { Name = name, Modifications = new List<Modification> { new Modification(modifiedDate, author) } });
            })
                             .GroupBy(f => f.Name).Select(fileGroup => new File { Name = fileGroup.Key, Modifications = fileGroup.SelectMany(file => file.Modifications).ToList() })
                             .ToList();

            var filesOrdered = files.OrderByDescending(f => f.Modifications.Count).Take(10).ToList();

            var max = Math.Min(Console.BufferWidth - 25, filesOrdered.Max(f => f.Name.Length));

            foreach (var file in filesOrdered)
                Console.WriteLine("{0} modified {1:00} times", file.Name.PadRight(max, ' '), file.Modifications.Count);
        }
    }
}
