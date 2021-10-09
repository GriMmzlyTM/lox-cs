using Lox.Expressions.Visitors;

namespace Lox.Expressions {
    public interface IExpr {
        T Accept<T>(IVisitor<T> visitor);
    }
    public abstract record Expr : IExpr {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
    
    // Implementations
    public record Binary (Token Operator, Expr Left, Expr Right) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
    }
    public record Grouping (Expr Expression) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
    }
    public record Literal (object Value) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
    }
    public record Unary (Token Operator, Expr Right) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
    }
}
