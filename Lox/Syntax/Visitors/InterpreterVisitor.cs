using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Markup;

namespace Lox.Syntax.Visitors {
    public class InterpreterVisitor : IVisitor<object> {

        private Environment _environment = new();
        
        public void Interpret(List<IStmt> stmts) {
            try {
                foreach (var stmt in stmts) {
                    Execute(stmt);
                }
            }
            catch (RuntimeError err) {
                Lox.RuntimeError(err);
            }
        }

        private void Execute(IStmt stmt) => stmt.Accept(this);

        private string Stringify(object val) {
            if (val == null) return "nil";

            if (val is double valDouble) {
                var text = valDouble.ToString(CultureInfo.InvariantCulture);
                if (text.EndsWith(".0"))
                    text = text.Substring(0, text.Length - 2);

                return text;
            }

            return val.ToString();
        }
        
        private object Evaluate(IExpr expr) => expr.Accept(this);

        private bool IsTruthy(object expr) =>
            expr switch
            {
                null => false,
                bool b => b,
                _ => true
            };

        private bool IsEqual(object left, object right) {
            return left switch {
                null when right == null => true,
                null => false,
                _ => left.Equals(right)
            };

        }

        private void CheckNumberOperands(Token op, params object[] operand) {
            if (operand.All(x => x is double)) return;
            throw new RuntimeError(op, "Operands must be double values.");
        }

        public object VisitAssignExpr(AssignExpr expr) {
            var val = Evaluate(expr.Value);
            _environment.Assign(expr.Name, val);

            return val;
        }
        public object VisitBinaryExpr(BinaryExpr expr) {
            var (left1, @operator, right1) = expr;
            var left = Evaluate(left1);
            var right = Evaluate(right1);

            switch (@operator.Type) {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:

                    if (left is double leftDouble && right is double rightDouble) return leftDouble + rightDouble;

                    if (left is string || right is string) return left.ToString() + right.ToString();

                    throw new RuntimeError(expr.Operator, "Operands must be numbers or strings.");

                    break;
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
                default: return null;
            }
        }

        public object VisitGroupingExpr(GroupingExpr expr) => Evaluate(expr.Expression);
        public object VisitLiteralExpr(LiteralExpr expr) => expr.Value;
        public object VisitUnaryExpr(UnaryExpr expr) {
            var (@operator, right1) = expr;
            var right = Evaluate(right1);

            switch (@operator.Type) {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
                default:
                    return null;
            }

        }
        public object VisitVariableExpr(VariableExpr expr) => _environment.Get(expr.Name);
        public object VisitLogicalExpr(LogicalExpr expr) {
            var left = Evaluate(expr.Left);
            
            if (expr.Op.Type == TokenType.OR) {
                if (IsTruthy(left))
                    return left;
            }
            else {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        public void VisitExpressionStmt(ExpressionStmt stmt) {
            Evaluate(stmt.Expression);
        }
        
        public void VisitPrintStmt(PrintStmt stmt) {
            var value = Evaluate(stmt.Expression);
            Console.WriteLine(Stringify(value));
        }
        
        public void VisitVarStmt(VarStmt stmt) {
            object value = null;
            if (stmt.initializer != null) value = Evaluate(stmt.initializer);

            _environment.Define(stmt.name, value);
        }
        public void VisitBlockStmt(BlockStmt stmt) {
            ExecuteBlock(stmt.Statements, new Environment(_environment));
        }
        
        public void VisitIfStmt(IfStmt stmt) {
            if (IsTruthy(Evaluate(stmt.condition)))
                Execute(stmt.thenBranch);
            else if (stmt.elseBranch != null)
                Execute(stmt.elseBranch);
        }

        public void ExecuteBlock(IEnumerable<IStmt> statements, Environment environment) {
            var previousEnv = _environment;

            try {
                _environment = environment;
                foreach (var stmt in statements) {
                    Execute(stmt);
                }
            }
            finally {
                _environment = previousEnv;
            }
        }
    }
}
