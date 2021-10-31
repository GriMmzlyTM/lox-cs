using System.Collections.Generic;

namespace Lox {
    public class Scanner {
        private readonly string _source;
        private readonly List<Token> _tokens = new();

        private int _start = 1;
        private int _current = 1;
        private int _endLength => _current - _start;
        private int _line = 1;
        
        public Scanner(string source) {
            _source = source;
        }

        public IEnumerable<Token> ScanTokens() {
            while (!IsAtEnd()) {
                _start = _current;
                ScanToken();
            }
            
            _tokens.Add(new Token(_line, null, "", TokenType.EOF));
            return _tokens;
        }

        private void ScanToken() {
            var c = Advance();

            switch (c) {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS);break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;

                case '!': AddToken(AdvanceOnMatch('=') ? TokenType.BANG_EQUAL : TokenType.BANG);break;
                case '=': AddToken(AdvanceOnMatch('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);break;
                case '<': AddToken(AdvanceOnMatch('=') ? TokenType.LESS_EQUAL : TokenType.LESS);break;
                case '>': AddToken(AdvanceOnMatch('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);break;

                case '/':
                    if (AdvanceOnMatch('/')) while (PeekAt() != '\n' && !IsAtEnd()) Advance();
                    else AddToken(TokenType.SLASH);
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    _line++;
                    break;
                case '"': ParseString(); break;
                default:
                    if (IsDigit(c))
                        ParseNumber();
                    else if (IsAlpha(c))
                        ParseIdentifier();
                    else
                        Lox.Error(_line, $"Unexpected character: {c}");
                    break;
            }
        }
        
        // ////////////////////
        #region Token manipulation
        private void AddToken(TokenType type) => AddToken(type, null);
        private void AddToken(TokenType type, object literal) => 
            _tokens.Add(new Token(_line, literal, _source.Substring(_start, _endLength), type));
        
        #endregion
        
        // ////////////////////
        #region Char/source manipulation
        
        private bool IsAtEnd(int depth = 0) => _current + depth >= _source.Length;
        private char PeekAt(int depth = 0) => IsAtEnd(depth) ? '\0' : _source[_current + depth];
        private char Advance() => _source[_current++];
        private bool AdvanceOnMatch(char expected) {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;
            Advance();
            return true;
        }
        #endregion
        
        // ////////////////////
        #region Static char validation
        
        private static bool IsAlpha(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_';
        private static bool IsDigit(char c) => c is >= '0' and <= '9';
        private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);
        
        #endregion
        
        // ///////////////////
        #region Token value type logic
        
        private void ParseIdentifier() {
            while (IsAlphaNumeric(PeekAt())) Advance();

            var text = _source.Substring(_start, _endLength);
            TokenType type;
            if (!ReservedWords.Keywords.TryGetValue(text, out type))
                type = TokenType.IDENTIFIER;
            
            AddToken(type);
        }

        private void ParseNumber() {
            while (IsDigit(PeekAt())) Advance();

            if (PeekAt() == '.' && IsDigit(PeekAt(1))) {
                // consume '.'
                Advance();
                while (IsDigit(PeekAt())) Advance();
            }
            
            AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
        }

        private void ParseString() {
            while (PeekAt() != '"' && !IsAtEnd()) {
                if (PeekAt() == '\n') _line++;
                Advance();
            }
            if (IsAtEnd()) {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            Advance();

            var val = _source.Substring(_start + 1, _endLength - 2 );
            AddToken(TokenType.STRING, val);
        }
        #endregion
        
    }
}
