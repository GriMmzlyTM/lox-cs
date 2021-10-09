using System;
using System.IO;
using System.Text;

namespace Lox
{
    internal static class Lox {

        private static bool _hadError = false;

        private static void Main(string[] args) {
            switch (args.Length) {
                case > 1:
                    Console.WriteLine("Usage: clox [script]"); 
                    Environment.Exit(64);
                    break;
                case 1: RunFile(args[0]); break;
                default: RunPrompt(); break;
            }

            Console.WriteLine("Hello World!");
        }

        private static void RunFile(string filePath) {
            var bytes = File.ReadAllBytes(Path.GetFullPath(filePath));
            Run(Encoding.Default.GetString(bytes));
            
            if (_hadError) Environment.Exit(65);
        }

        private static void RunPrompt() {
            for (;;) {
                Console.Write("> ");
                var currSource = Console.ReadLine();
                if (currSource == null) break;
                Run(currSource);
                _hadError = false;
            }
        }

        private static void Run(string source) {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens) {
                Console.WriteLine(token.Stringify());
            }
        }

        public static void Error(int line, string message) {
            Report(line, message, "");
            _hadError = true;
        }

        private static void Report(int line, string message, string where) =>
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");


    }
}
