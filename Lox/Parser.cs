using System;
using System.Collections.Generic;
using System.Linq;
using Lox.Expressions;

namespace Lox {
    public class Parser {
        private class ParseError : SystemException {}
        
        private readonly List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens) {
            _tokens = tokens;
        }

        public IExpr Parse() {
            try {
                return Expression();
            }
            catch (ParseError) {
                return null;
            }
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
            return Equality();
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
