using System;
using System.Globalization;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

namespace Lox.Expressions.Visitors {
    public class InterpreterVisitor : IVisitor<object> {

        public void Interpret(IExpr expression) {
            try {
                var value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError err) {
                Lox.RuntimeError(err);
            }
        }

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
        
        public object VisitBinaryExpr(BinaryExpr expr) {
            var (left1, @operator, right1) = expr;
            var left = Evaluate(left1);
            var right = Evaluate(right1);

            switch (@operator.Type) {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    return (left is double leftDouble && right is double rightDouble) 
                        ? leftDouble + rightDouble 
                        : (left is string leftString && right is string rightString) 
                            ? leftString + rightString
                            : throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");
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
    }
}
