
namespace Abacus {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BF = System.Reflection.BindingFlags;
    // using LinqE = System.Linq.Expressions.Expression;
    using static System.Linq.Expressions.Expression;
    using static Abacus.Error;
    // using static Abacus.Operator;

	//TODO: Explore different way to handle syntax errors. I think
	//      for this particular component throwing exceptions
	//      is not an option (performance wise).
    public class SyntaxWalker {
		static readonly Type WALKER_TYPE = typeof(SyntaxWalker);
		static readonly BF STAPRIVATE = BF.Static | BF.NonPublic | BF.InvokeMethod | BF.DeclaredOnly;

		static readonly MethodInfo POW = WALKER_TYPE.GetMethod(
				"__Pow", STAPRIVATE);

		static readonly MethodInfo TRUNC = WALKER_TYPE.GetMethod(
				"__Trunc", STAPRIVATE);

		/// Code generation entry point.
        public Expression Walk(SyntaxTree rootNode) {
			var childs = rootNode.GetChilds();
			var body = new Expression[childs.Length];
			for(int i = 0; i < childs.Length; ++i) {
				body[i] = childs[i].Accept(this);
			}

			Expression res = Block(body);

			return res.Type != typeof(object)
				? Convert(res, typeof(object))
				: res;
        }
		
        public Expression Walk(UnaryExpression expr) {
			var val = expr.Expr.Accept(this);
			if (expr.Op == Operator.Neg)
				return Negate(val);
			return val;
		}


        public Expression Walk(Const expr)
			=> Constant(expr.Val, expr.Type);

        public Expression Walk(BinExpression expr) {

			var op  = expr.Op;
			var lhs = expr.Lhs.Accept(this);
			var rhs = expr.Rhs.Accept(this);

			switch(op) {
				case Operator.Add:      return Add(lhs, rhs);
				case Operator.Sub:      return Subtract(lhs, rhs);
				case Operator.Multiply: return Multiply(lhs, rhs);
				case Operator.Divide:   return Divide(lhs, rhs);
				case Operator.Modulo:   return Modulo(lhs, rhs);
				case Operator.FloorDiv: return Floor(lhs, rhs);
				case Operator.Pow:      return Pow(lhs, rhs);
				default: break;
			}

			ReportUnkwnowBinaryOp(op);
			return null;
		}

		//RUNTIME HELPERS {{{
		static double __Pow(double num, double pow) 
			=> Math.Pow(num, pow);

		static double __Trunc(double num, double div)
			=> Math.Truncate(num/div);
		//}}}


		// COMPILE TIME HELPERS {{{
		//
		/// Converts an expression to double. If the
		/// expression's type is double already, returns
		/// the expression as is.
		Expression Dbl(Expression expr)
			=> (expr.Type != typeof(double))
			? Convert(expr, typeof(double))
			: expr;

		/// Helper method to emulate the pow operator.
		Expression Pow(Expression num, Expression pow) 
			=> Call(null, POW, Dbl(num), Dbl(pow));

		Expression Floor(Expression lhs, Expression rhs)
			=> Call(null, TRUNC, Dbl(lhs), Dbl(rhs)); 

		void ReportUnkwnowBinaryOp(Operator op) {
			Die($"Unknown binary operator {op}");
		}
	   //}}}
	}
}


