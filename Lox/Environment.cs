using System.Collections.Generic;

namespace Lox {
    public class Environment {

        private readonly Environment _enclosing;
        private readonly Dictionary<string, object> values = new();

        public Environment() {
            _enclosing = null;
        }

        public Environment(Environment enclosing) {
            _enclosing = enclosing;
        }
        
        public void Assign(Token name, object val) {
            if (values.ContainsKey(name.Lexeme)) {
                values[name.Lexeme] = val;
                return;
            }
            
            if (_enclosing != null) {
                _enclosing.Assign(name, val);
                return;
            }
            
            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");

        }
        
        public void Define(Token name, object value) {
            if (values.TryAdd(name.Lexeme, value)) return;
            throw new RuntimeError(name, $"Variable already defined.");
        }

        public object Get(Token name) {
            if (values.TryGetValue(name.Lexeme, out var variable)) return variable;
            if (_enclosing != null) return _enclosing.Get(name);
            
            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");
        }
    }
}
