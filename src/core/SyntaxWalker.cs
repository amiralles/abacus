
namespace Abacus {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BF = System.Reflection.BindingFlags;
    using static System.Linq.Expressions.Expression;
    using static Abacus.Error;
    using static System.String;
    using static System.Convert;

	//TODO: Explore different way to handle syntax errors. I think
	//      for this particular component throwing exceptions
	//      is not an option (performance wise).
    public class SyntaxWalker {
		static readonly Type WALKER_TYPE = typeof(SyntaxWalker);
		static readonly BF STAPRIVATE = 
			BF.Static | BF.NonPublic | BF.InvokeMethod | BF.DeclaredOnly;

		static readonly MethodInfo POW = WALKER_TYPE.GetMethod(
				"__Pow", STAPRIVATE);

		static readonly MethodInfo TRUNC = WALKER_TYPE.GetMethod(
				"__Trunc", STAPRIVATE);

		static readonly MethodInfo CMP = WALKER_TYPE.GetMethod(
				"__Cmp", STAPRIVATE);

		static readonly Expression 
			MINUSONE = Constant(-1),
			ZERO = Constant(0),
			ONE = Constant(1);
		
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
			var lhs = Dbl(expr.Lhs.Accept(this));
			var rhs = Dbl(expr.Rhs.Accept(this));

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

        public Expression Walk(EqualExpression expr)
			=> Equal(CallCmp(expr.Lhs, expr.Rhs), ZERO);

        public Expression Walk(NotEqualExpression expr)
			=> NotEqual(CallCmp(expr.Lhs, expr.Rhs), ZERO);

        public Expression Walk(LessThanExpression expr)
			=> Equal(CallCmp(expr.Lhs, expr.Rhs), MINUSONE);

        public Expression Walk(LessThanEqExpression expr)
			=> LessThan(CallCmp(expr.Lhs, expr.Rhs), ONE);

        public Expression Walk(GreaterThanExpression expr)
			=> Equal(CallCmp(expr.Lhs, expr.Rhs), ONE);
		
        public Expression Walk(GreaterThanEqExpression expr)
			=> GreaterThan(CallCmp(expr.Lhs, expr.Rhs), MINUSONE);


		//RUNTIME HELPERS {{{
		static double __Pow(double num, double pow) 
			=> Math.Pow(num, pow);

		static double __Trunc(double num, double div)
			=> Math.Truncate(num/div);

		static int __Cmp(object lhs, object rhs) {
			// As of now, strings are the only "special case" for comparisons,
			// all the other types are converted to number and then compared
			// to each other.
			// (This may have to change if we extend the abaucs type system.
			//  i.e. objects, arrays, etc....).
			if (lhs is string)
				return string.Compare((string)lhs, Intern(rhs?.ToString()));

			if (lhs is DateTime)
				lhs = ((DateTime)lhs).ToOADate();

			if (rhs is DateTime)
				rhs = ((DateTime)rhs).ToOADate();

			// Are we going to support multiple cultures?
			// If so, we must to take that into account when converting values.
			double lhd = ToDouble(lhs);
			double rhd = ToDouble(rhs);

			return lhd == rhd ? 0 : lhd < rhd ? -1 : 1;
		}
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

		Expression Obj(Expression expr)
			=> (expr.Type != typeof(object))
			? Convert(expr, typeof(object))
			: expr;


		Expression CallCmp(SyntaxNode lhs, SyntaxNode rhs) 
			=> Call(null, CMP, 
					Obj(lhs.Accept(this)), 
					Obj(rhs.Accept(this)));



		/// Helper method to emulate the pow operator.
		Expression Pow(Expression num, Expression pow) 
			=> Call(null, POW, Dbl(num), Dbl(pow));

		Expression Floor(Expression lhs, Expression rhs)
			=> Call(null, TRUNC, Dbl(lhs), Dbl(rhs)); 

		Expression Cmp(Expression lhs, Expression rhs)
			=> Call(null, CMP, lhs, rhs); 

		void ReportUnkwnowBinaryOp(Operator op) {
			Die($"Unknown binary operator {op}");
		}
	   //}}}
	}
}


