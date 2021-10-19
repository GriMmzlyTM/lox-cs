﻿using Lox.Expressions.Visitors;

namespace Lox.Expressions {
    public interface IExpr {
        T Accept<T>(IVisitor<T> visitor);
    }
    public abstract record Expr : IExpr {
        public abstract T Accept<T>(IVisitor<T> visitor);
    }
    
    // Implementations
    public record BinaryExpr (IExpr Left, Token Operator, IExpr Right) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
    }
    public record GroupingExpr (IExpr Expression) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
    }
    public record LiteralExpr (object Value) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
    }
    public record UnaryExpr (Token Operator, IExpr Right) : Expr {
        public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
    }
}
