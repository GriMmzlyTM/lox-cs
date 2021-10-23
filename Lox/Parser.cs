using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Lox.Expressions;

namespace Lox {
    public class Parser {
        private readonly List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens) {
            _tokens = tokens;
        }

        private IExpr Expression() {
            return Equality();
        }

        private IExpr Equality() {
            var expr = Comparison();
            
            while(Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
                var op = PreviousToken();
                var right = Comparison();
                expr = new BinaryExpr(expr, op, right);
            }
            return expr;
        }

        bool Match(params TokenType[] types) =>
            types.Any(Check) && Advance();

        private bool Advance()
        {
            
        }
        
        private bool Check(TokenType type)
        {
            return false;
        }

        private Token PreviousToken()
        {
            return null;
        }

        private bool Match(TokenType type1, TokenType type2)
        {
            return true;
        }

        private IExpr Comparison()
        {
            return null;
        }
    }
}
