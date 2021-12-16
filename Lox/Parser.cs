using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Lox.Syntax;

namespace Lox {
    public class Parser {
        private class ParseError : SystemException {}
        
        private readonly List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens) {
            _tokens = tokens;
        }

        public List<IStmt> Parse() {
            var statements = new List<IStmt>();

            while (!IsAtEnd())
                statements.Add(Declaration());

            return statements;
        }

        private IStmt Declaration() {
            try {
                return Match(TokenType.VAR) ? VarDeclaration() : Statement();
            }
            catch (ParseError error) {
                Synchronize();
                return null;   
            }
        }

        private Stmt VarDeclaration() {
            var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            IExpr initializer = null;
            if (Match(TokenType.EQUAL)) {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new VarStmt(name, initializer);
        }

        private IStmt Statement() {
            if (Match(TokenType.IF))
                return IfStatement();
            
            if (Match(TokenType.PRINT))
                return PrintStatement();

            if (Match(TokenType.LEFT_BRACE))
                return new BlockStmt(Block());

            return ExpressionStatement();
        }

        private IStmt IfStatement() {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            var thenBranch = Statement();
            IStmt elseBranch = null;

            if (Match(TokenType.ELSE))
                elseBranch = Statement();

            return new IfStmt(condition, thenBranch, elseBranch);
        }

        private IEnumerable<IStmt> Block() {
            var statements = new List<IStmt>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private IStmt ExpressionStatement() {
            var expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new ExpressionStmt(expr);
        }
        
        private IStmt PrintStatement() {
            var value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new PrintStmt(value);
        }

        private void Synchronize() {
            Advance();

            while (!IsAtEnd()) {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type) {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }

        private IExpr Expression() {
            return Assignment();
        }

        private IExpr Assignment() {

            var expr = Or();

            if (!Match(TokenType.EQUAL)) return expr;
            
            var equals = Previous();
            var value = Assignment();

            if (expr is VariableExpr varExpr) {
                var name = varExpr.Name;
                return new AssignExpr(name, value);
            }

            Error(equals, "Invalid assignment target.");

            return expr;
        }

        private IExpr Or() {
            var expr = And();

            while (Match(TokenType.OR)) {
                var @operator = Previous();
                var right = And();
                expr = new LogicalExpr(expr, @operator, right);
            }

            return expr;
        }

        private IExpr And() {
            var expr = Equality();
            
            while(Match(TokenType.AND))
            {
                var @operator = Previous();
                var right = Equality();
                expr = new LogicalExpr(expr, @operator, right);
            }

            return expr;
        }

        private IExpr Equality() {
            var expr = Comparison();
            
            while(Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
                var op = Previous();
                var right = Comparison();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }
        
        private IExpr Comparison() {
            var expr = Term();
            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
                var op = Previous();
                var right = Term();
                expr = new BinaryExpr(expr, op, right);
            }
            
            return expr;
        }

        private IExpr Term() {
            var expr = Factor();
            while (Match(TokenType.MINUS, TokenType.PLUS)) {
                var op = Previous();
                var right = Factor();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private IExpr Factor() {
            var expr = Unary();

            while (Match(TokenType.SLASH,TokenType.STAR)) {
                var op = Previous();
                var right = Unary();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        private IExpr Unary() {
            if (Match(TokenType.BANG, TokenType.MINUS)) {
                var op = Previous();
                var right = Unary();

                return new UnaryExpr(op, right);
            }

            return Primary();
        }

        private IExpr Primary() {
            if (Match(TokenType.FALSE)) return new LiteralExpr(false);
            if (Match(TokenType.TRUE)) return new LiteralExpr(true);
            if (Match(TokenType.NIL)) return new LiteralExpr(null);

            if (Match(TokenType.NUMBER, TokenType.STRING)) return new LiteralExpr(Previous().Literal);

            if (Match(TokenType.IDENTIFIER)) return new VariableExpr(Previous());
            
            if (Match(TokenType.LEFT_PAREN)) {
                var expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");

                return new GroupingExpr(expr);
            }

            throw Error(Peek(), "Expected expression.");
        }

        private Token Consume(TokenType type, string message) {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message) {
            Lox.Error(token, message);

            return new ParseError();
        }
        
        bool Match(params TokenType[] types) {
            if (!types.Any(Check)) return false;
            Advance();
            return true;
        } 
        
        private Token Advance() {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;

        private bool IsAtEnd() => Peek().Type == TokenType.EOF;

        private Token Peek() => _tokens[_current];

        private Token Previous() => _tokens[_current - 1];

    }
}
