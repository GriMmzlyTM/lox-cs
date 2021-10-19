using System;
using System.Collections.Generic;
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

        private Token PreviousToken() {
            
        }

        private bool Match(TokenType type1, TokenType type2) {
            
        }
        
        private IExpr Comparison(){}
        
    }
}
