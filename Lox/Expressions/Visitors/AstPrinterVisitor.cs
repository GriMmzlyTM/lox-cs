using System.Text;

namespace Lox.Expressions.Visitors {
    public class AstPrinterVisitor : IVisitor<string> {

        public string  Print(IExpr expr) => expr.Accept(this);

        private string Parenthesize(string name, params IExpr[] exprs) {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append('(').Append(name);

            foreach (var expr in exprs) {
                stringBuilder.Append(' ');
                stringBuilder.Append(expr.Accept(this));
            }
            stringBuilder.Append(')');
            return stringBuilder.ToString();
        }

        public string VisitBinaryExpr(BinaryExpr expr) => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        public string VisitGroupingExpr(GroupingExpr expr) => Parenthesize("group", expr.Expression);
        public string VisitLiteralExpr(LiteralExpr expr) => expr.Value == null ? "nil" : expr.Value.ToString();
        public string VisitUnaryExpr(UnaryExpr expr) => Parenthesize(expr.Operator.Lexeme, expr.Right);

    }
}
