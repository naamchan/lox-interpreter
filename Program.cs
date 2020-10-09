using System;
using static System.Console;
using static interpreter.TokenType;

namespace interpreter
{
    class Program
    {
        private static bool hadError = false;

        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    RunFile(args[0]);
                    break;
                case 0:
                    RunRepl();
                    break;
                default:
                    WriteLine("Usage: interpreter [entry script]");
                    System.Environment.Exit(64);
                    return;
            }
        }

        static void RunFile(string filePath)
        {
            Run(System.IO.File.ReadAllText(filePath));
            if (hadError)
            {
                System.Environment.Exit(65);
            }
        }

        static void RunRepl()
        {
            while (true)
            {
                Write("> ");
                var line = ReadLine();
                if (line is null)
                {
                    break;
                }
                Run(line);
                hadError = false;
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            foreach (var token in scanner.scanTokens(source))
            {
                WriteLine(token);
            }
        }

        internal static void Error(int line, int col, string message)
        {
            Report(line, col, message);
        }

        private static void Report(int line, int col, string message)
        {
            Console.Error.WriteLine($"[line {line} col {col}] Error: {message}");
            hadError = true;
        }
    }
}
