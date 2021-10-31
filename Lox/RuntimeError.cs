using System;
using System.Transactions;

namespace Lox {
    public class RuntimeError : SystemException {
        public readonly Token Token;

        public RuntimeError(Token token, string msg) : base(msg) {
            Token = token;
        }
    }
}
