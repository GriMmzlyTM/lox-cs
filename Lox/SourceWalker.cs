namespace Lox {
    public class SourceWalker {
        
        protected readonly string _source;

        protected int _start = 0;
        protected int _current = 0;
        protected int _endLength => _current - _start;
        protected int _line = 1;

        public SourceWalker(string source) {
            _source = source;
        }
        
        protected bool IsAtEnd(int depth = 0) => _current + depth >= _source.Length;
        protected char PeekAt(int depth = 0) => IsAtEnd(depth) ? '\0' : _source[_current + depth];
        protected char Advance() => _source[_current++];
        protected bool AdvanceOnMatch(char expected) {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;
            Advance();
            return true;
        }
        
    }
}