namespace Lox {
    
    public record Token(int Line, object Literal, string Lexeme, TokenType Type);

    public static class TokenExtensions {
        public static string Stringify(this Token token) => $"{token.Type} {token.Lexeme} {token.Literal ?? "NULL"}";
    }
}
