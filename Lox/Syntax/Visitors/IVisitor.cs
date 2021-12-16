namespace Lox.Syntax.Visitors {
    public interface IVisitor<T> {

        T VisitAssignExpr(AssignExpr expr);
        T VisitBinaryExpr(BinaryExpr expr);
        T VisitGroupingExpr(GroupingExpr expr);
        T VisitLiteralExpr(LiteralExpr expr);
        T VisitUnaryExpr(UnaryExpr expr);
        T VisitVariableExpr(VariableExpr expr);
        T VisitLogicalExpr(LogicalExpr expr);

        void VisitExpressionStmt(ExpressionStmt stmt);
        void VisitPrintStmt(PrintStmt stmt);
        void VisitVarStmt(VarStmt stmt);
        void VisitBlockStmt(BlockStmt stmt);
        void VisitIfStmt(IfStmt stmt);
    }
}
