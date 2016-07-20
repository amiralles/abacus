

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
			throw new NotImplementedException(
					$"Not Implemented Exception at {GetType().Name}.");
		}
	}

	public class SyntaxTree : SyntaxNode { 
		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}

	public class Const  : SyntaxNode {
		public readonly object Val;
		public readonly Type Type;

		public Const(object val) :
		   	this (val, typeof(object)) {
		}

		public Const(object val, Type t) {
			Val = val;
			Type = t;
		}

		public override Expression Accept(SyntaxWalker walker) 
			=> walker.Walk(this);

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

	// The easiest way to implement comparisons is to delegate all 
	// calls to a single helper method that performs converts and
	// comparisons. By doing that it's only a matter to check for:
	// -1 lessThan
	//  0 equal
	//  1 greaterThan
	//  Since we have a lot of implicit convertions (something like js) unless
	//  performance states otherwise, all the operations will be solved by
	//  the same method that will centralized convertions, coertions, and so
	//  on and so forth.....
	public class EqualExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public EqualExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker) 
			=> walker.Walk(this);

	}

	public class NotEqualExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public NotEqualExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}

	public class LessThanEqExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public LessThanEqExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}

	public class LessThanExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public LessThanExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}

	public class GreaterThanExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public GreaterThanExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}

	public class GreaterThanEqExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public GreaterThanEqExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}

	public class NotExpression : SyntaxNode {
		public readonly SyntaxNode Expr;

		public NotExpression(SyntaxNode expr) {
			Expr = expr;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}


	public class OrExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public OrExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}


	public class AndExpression : SyntaxNode {
		public readonly SyntaxNode Lhs, Rhs;

		public AndExpression(SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}



	public class UnaryExpression : SyntaxNode {
		public readonly Operator Op;
		public readonly SyntaxNode Expr;

		public UnaryExpression(Operator op, SyntaxNode expr) {
			Op = op;
			Expr = expr;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
	}

	public class BinExpression : SyntaxNode {
		public readonly Operator Op;
		public readonly SyntaxNode Lhs, Rhs;

		public BinExpression(
				BinaryOperator op, SyntaxNode lhs, SyntaxNode rhs) {
			DbgEnsureLhsRhs(lhs, rhs);
			Lhs = lhs;
			Rhs = rhs;
			Op = op.Operator;
		}

		public override Expression Accept(SyntaxWalker walker)
			=> walker.Walk(this);
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

