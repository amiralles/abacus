
namespace Abacus {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using static System.String;
	using static Abacus.Assert;
	using static Abacus.Error;

	public abstract class SyntaxNode {

		List<SyntaxNode> _childs;

		public bool IsEmpty;

		public readonly static SyntaxNode Empty = new Const(null);

		public void Add(SyntaxNode ch) {
			if (_childs == null)
				_childs = new List<SyntaxNode>();
			_childs.Add(ch);
		}

		public SyntaxNode[] GetChilds() 
			=> _childs?.ToArray();

		public virtual SyntaxNode Reduce()
			=> CanReduce ? Reduce() : this;

		public virtual bool CanReduce => false;

		public abstract Expression Accept(SyntaxWalker walker);

		protected Expression NotImplemented() {
			throw new NotImplementedException();
		}
	}

	public class SyntaxTree : SyntaxNode { 
		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class Const  : SyntaxNode {

		public Const(object val) : this (val, typeof(object)) {
		}

		public Const(object val, Type t) {
		}

		public override Expression Accept(SyntaxWalker walker) 
			=> NotImplemented();

	}

	public class GetLocal  : SyntaxNode {
		public readonly string Name;
		public GetLocal(string name){
			DbgDieIf(IsNullOrEmpty(name), "name can't be null or empty.");
			Name = name;
		}

		public override Expression Accept(SyntaxWalker walker) 
			=> NotImplemented();
	}


	public class FuncCall : SyntaxNode {
		public readonly SyntaxNode   Target;
		public readonly string         FuncName;
		public readonly SyntaxNode[] Argv;

		public FuncCall(
				SyntaxNode target, string funcName, SyntaxNode[] argv) {

			DbgDieIf(IsNullOrEmpty(funcName),
					"funcName can't be null o empty.");

			Target = target;
			FuncName = funcName;
			Argv = argv;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class EqualExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public EqualExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker) 
			=> NotImplemented();

	}

	public class NotEqualExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public NotEqualExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class LessThanEqExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public LessThanEqExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class NotExpression : SyntaxNode {
		public readonly SyntaxNode Expr;

		public NotExpression(SyntaxNode expr) {
			Expr = expr;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}


	public class OrExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public OrExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}


	public class AndExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public AndExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}


	public class LessThanExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public LessThanExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class GreaterThanExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public GreaterThanExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class GreaterThanEqExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public GreaterThanEqExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class UnaryExpression : SyntaxNode {
		public readonly Operator Op;
		public readonly SyntaxNode Expr;

		public UnaryExpression(Operator op, SyntaxNode expr) {
			Op = op;
			Expr = expr;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class BinaryExpression : SyntaxNode {
		public readonly Operator Op;
		public readonly SyntaxNode Lhs, Rhs;

		public BinaryExpression(
				BinaryOperator op, SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

	public class ParenExpression : SyntaxNode {
		public readonly SyntaxNode Expr;

		public ParenExpression(SyntaxNode expr) {
			Expr = expr;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> NotImplemented();
	}

}

