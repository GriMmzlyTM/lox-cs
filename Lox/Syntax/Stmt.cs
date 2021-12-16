using System.Collections;
using System.Collections.Generic;
using Lox.Syntax.Visitors;

namespace Lox.Syntax {
    
    public interface IStmt {
        void Accept<T>(IVisitor<T> visitor);
    }
    
    public abstract record Stmt : IStmt {
        public abstract void Accept<T>(IVisitor<T> visitor);
    }

    public record BlockStmt(IEnumerable<IStmt> Statements) : Stmt {

        public override void Accept<T>(IVisitor<T> visitor) => visitor.VisitBlockStmt(this);
    }

    public record ExpressionStmt(IExpr Expression) : Stmt { 
        public override void Accept<T>(IVisitor<T> visitor) => visitor.VisitExpressionStmt(this);
    }

    public record PrintStmt(IExpr Expression) : Stmt {
        public override void Accept<T>(IVisitor<T> visitor) => visitor.VisitPrintStmt(this);
    }

    public record VarStmt(Token name, IExpr initializer) : Stmt {
        public override void Accept<T>(IVisitor<T> visitor) => visitor.VisitVarStmt(this);
    }
    
    public record IfStmt(IExpr condition, IStmt thenBranch, IStmt elseBranch) : Stmt {
        public override void Accept<T>(IVisitor<T> visitor) => visitor.VisitIfStmt(this);
    }
}
