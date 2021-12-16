using System;
using System.IO;
using System.Linq;
using System.Text;
using Lox.Syntax.Visitors;

namespace Lox
{
    internal static class Lox {

        private static readonly InterpreterVisitor _interpreterVisitor = new InterpreterVisitor();

        private static bool _hadError = false;
        private static bool _hadRuntimeError = false;

        private static void Main(string[] args) {
            switch (args.Length) {
                case > 1:
                    Console.WriteLine("Usage: clox [script]"); 
                    System.Environment.Exit(64);
                    break;
                case 1: RunFile(args[0]); break;
                default: RunPrompt(); break;
            }
        }

        private static void RunFile(string filePath) {
            var bytes = File.ReadAllBytes(Path.GetFullPath(filePath));
            Run(Encoding.Default.GetString(bytes));
            
            if (_hadError) System.Environment.Exit(65);
            if (_hadRuntimeError) System.Environment.Exit(70);
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

            var parser = new Parser(tokens.ToList());
            var statements = parser.Parse();

            if (_hadError) return;
            
            _interpreterVisitor.Interpret(statements);
        }

        public static void Error(int line, string message) {
            Report(line, message, "");
            _hadError = true;
        }

        public static void RuntimeError(RuntimeError err) {
            Console.Error.WriteLine(err.Message + $"\n[line {err.Token.Line}]");
            _hadRuntimeError = true;
        }
        
        public static void Error(Token token, string message) {
            if (token.Type == TokenType.EOF)
                Report(token.Line, " at end", message);
            else
                Report(token.Line, " at '" + token.Lexeme + "'", message);
        }

        private static void Report(int line, string message, string where) =>
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");


    }
}
