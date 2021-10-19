namespace Lox.Expressions.Visitors {
    public interface IVisitor<T> { 
        T VisitBinaryExpr(BinaryExpr expr);
        T VisitGroupingExpr(GroupingExpr expr);
        T VisitLiteralExpr(LiteralExpr expr);
        T VisitUnaryExpr(UnaryExpr expr);
    }
}
