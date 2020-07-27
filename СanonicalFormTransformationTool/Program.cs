using System;
using System.IO;

namespace СanonicalFormTransformationTool
{
    class Program
    {
        static readonly TermSumParser Parser = new TermSumParser();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var input = args[0];
                var dir = Path.GetDirectoryName(input);
                var output = Path.Combine(dir, Path.GetFileNameWithoutExtension(input) + ".out");

                using var sr = new StreamReader(input);
                using var sw = new StreamWriter(output);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var res = string.IsNullOrWhiteSpace(line)? "" : Parse(line);
                    sw.WriteLine(res);
                }
            }
            else
            {
                while (true)
                {
                    Console.WriteLine("Enter text");
                    var line = Console.ReadLine();
                    Console.WriteLine(Parse(line));
                }
            }
        }

        static string Parse(string text)
        {
            try
            {
                return Parser.ToCanonicalForm(text);
            }
            catch
            {
                return "ERROR!";
            }
        }
    }
}
