
namespace Abacus {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BF = System.Reflection.BindingFlags;
    using static System.Convert;
    using static System.Console;
    using static System.Linq.Expressions.Expression;
    using static System.Double;
    using static System.String;
    using static Abacus.Error;
    using static Abacus.Assert;
    using static Abacus.Utils;

	//TODO: Explore different way to handle syntax errors. I think
	//      for this particular component throwing exceptions
	//      is not an option (performance wise).
    public class SyntaxWalker {
		static readonly Type WALKER_TYPE = typeof(SyntaxWalker);
		static readonly Type INTER_TYPE  = typeof(Interpreter);

		static readonly BF STAPRIVATE = 
			BF.Static | BF.NonPublic | BF.InvokeMethod | BF.DeclaredOnly;

		static readonly BF INSTAPUB =
			BF.Instance | BF.Public | BF.InvokeMethod | BF.IgnoreCase;

		static readonly BF INSTAPRIVATE =
			BF.Instance | BF.NonPublic | BF.InvokeMethod | BF.IgnoreCase;

		// static readonly BF PRIVATE = 
		// 	BF.Instance | BF.NonPublic | BF.InvokeMethod | BF.DeclaredOnly;

		static readonly MethodInfo IN = WALKER_TYPE.GetMethod(
				"__In", STAPRIVATE);

		static readonly MethodInfo POW = WALKER_TYPE.GetMethod(
				"__Pow", STAPRIVATE);

		static readonly MethodInfo TRUNC = WALKER_TYPE.GetMethod(
				"__Trunc", STAPRIVATE);

		static readonly MethodInfo CMP = WALKER_TYPE.GetMethod(
				"__Cmp", STAPRIVATE);

		static readonly MethodInfo ADDDAT = WALKER_TYPE.GetMethod(
				"__AddDat", STAPRIVATE);

		static readonly MethodInfo SUBDAT = WALKER_TYPE.GetMethod(
				"__SubDat", STAPRIVATE);

		static readonly MethodInfo TOB = WALKER_TYPE.GetMethod(
				"__ToBln", STAPRIVATE);

		static readonly MethodInfo GET = WALKER_TYPE.GetMethod(
				"__GetLocal", STAPRIVATE);

		static readonly MethodInfo DISP = INTER_TYPE.GetMethod(
				"__Dispatch", STAPRIVATE, 
				null,
				new [] {
					typeof(object),
					typeof(string),
					typeof(object[])
				},
				null);

		static readonly Expression 
			MINUSONE = Constant(-1),
			ZERO     = Constant(0),
			ONE      = Constant(1);

		// Compiled function's arguments.
		ParameterExpression _localNames, _locals;

		/// Code generation entry point.
		public Func<string[], object[], object> Compile(SyntaxTree tree) {
			_localNames = Parameter(typeof(string[]), "localNames");
			_locals     = Parameter(typeof(object[]), "locals");

			var program = Walk(tree);
			var lambda  = Lambda<Func<string[], object[], object>>(
					program, 
					new [] {
						_localNames,
						_locals
					});
					
			PrintLinqTree(lambda);
			return lambda.Compile();
		}
		
		// Version simplificada del walk, accedo al parametro directamente.
        public Expression Walk(SyntaxTree rootNode) {
			var childs = rootNode.GetChilds();
			var body = new Expression[childs.Length];
			for(int i = 0; i < childs.Length; ++i) {
				body[i] = childs[i].Accept(this);
			}

			int last = body.Length-1;
			if (body[last].Type != typeof(object))
				body[last] = Convert(body[last], typeof(object));

			return Block(body);
        }

        public Expression Walk(GetLocal expr) {
			return Call(null, GET, Constant(expr.Name), _localNames, _locals);
		}


		public Expression Walk(FuncCall fn) {

			Expression reciever = Constant(null);
			if (fn.Target != null)
				reciever = fn.Target.Accept(this);

			Expression[] args = new Expression[0];
			Type[] argsTypes   = new Type[0];

			if (fn.Argv != null && fn.Argv.Length > 0) {
				args = new Expression[fn.Argv.Length];
				argsTypes = new Type[args.Length];
				Expression a;
				for(int i = 0; i < fn.Argv.Length; ++i) {
					a = fn.Argv[i].Accept(this);
					argsTypes[i] = a.Type;
					args[i] = a;
				}
			}

			DbgPrint(ArrToStr(args));

			// Can we get the method's meta from target at compile time?
			// If so, use a std linq instance method call.
			if (reciever != null) {
				MethodInfo mi;
			   	if (TryGetMethod(reciever.Type, fn.FuncName, argsTypes, out mi))
					return Call(reciever, mi, args);
			}

			// If we can't get method's meta at compile time, we just use 
			// the generic dispatch mecanism. (Which is slower, but has access
			// to actual runtime types).
			for (int i= 0; i < args.Length; ++i) {
				if (args[i].Type != typeof(object))
					args[i] = Convert(args[i], typeof(object));
			}

			var argv = NewArrayInit(typeof(object), args);

			//Global function.
			var call = Call(null, DISP, reciever, Constant(fn.FuncName), argv);

			// Special case to support expressions like:
			// today + 1
			// today - 1
			if (fn.FuncName == "dat")
				return Convert(call, typeof(DateTime));
			return call;
		}

		public static bool TryGetMethod(
				Type recieverType, string fnName, Type[] argstypes, 
				out MethodInfo mi) {

			DbgPrint($"Looking for {fnName} ({ArrToStr(argstypes)})" + 
					 $" on {recieverType}");

			mi = recieverType.GetMethod(fnName, INSTAPUB, null, argstypes, null);

			if (mi == null) {
				// maybe is a property getter, setter.
				if (fnName.StartsWith("get_") && argstypes.Length == 0) {
					mi = recieverType.GetMethod(
							fnName, INSTAPRIVATE, null, argstypes, null);
				}
				else if (fnName.StartsWith("set_") && argstypes.Length == 1) {
					mi = recieverType.GetMethod(
							fnName, INSTAPRIVATE, null, argstypes, null);
				}
			}
			return mi != null;
		}

		public Expression Walk(InExpression expr) {
			var item = expr.Item.Accept(this);
			var opts = expr.Opts.Accept(this);

			if (item.Type != typeof(object))
				item = Convert(item, typeof(object));

			if (opts.Type != typeof(object))
				opts = Convert(opts, typeof(object));

			return Call(null, IN, item, opts);
		}


		public Expression Walk(ArrayExpression arr) {
			var items = new Expression[arr.Items.Length];
			for(int i=0; i < arr.Items.Length; ++i) {
				items[i] = arr.Items[i].Accept(this);
				if (items[i].Type != typeof(object))
					items[i] = Convert(items[i], typeof(object));
			}
			return Expression.NewArrayInit(typeof(object), items);
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
			rhs = Dbl(rhs);


			/// Special cases to suppor expressions
			/// like this: 
			/// now + 1 => tomorow.
			/// now - 1 => yesterday.
			if (lhs.Type == typeof(DateTime)) {
				if (op == Operator.Add) return Call(null, ADDDAT, lhs, rhs);
				if (op == Operator.Sub) return Call(null, SUBDAT, lhs, rhs);
			}

			lhs = Dbl(lhs);

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

        public Expression Walk(AndExpression expr)
			=> And(Bln(expr.Lhs.Accept(this)), Bln(expr.Rhs.Accept(this)));

        public Expression Walk(OrExpression expr)
			=> Or(Bln(expr.Lhs.Accept(this)), Bln(expr.Rhs.Accept(this)));

        public Expression Walk(NotExpression expr)
			=> Not(Bln(expr.Expr.Accept(this)));

		//RUNTIME HELPERS {{{
		static double __Pow(double num, double pow) 
			=> Math.Pow(num, pow);

		static double __Trunc(double num, double div)
			=> Math.Truncate(num/div);

		static bool __In(object item, object opts) {
			var @enum = opts as IEnumerable;
			DieIf(@enum == null, "opts must be enumerable");

			var e = @enum.GetEnumerator();
			while(e.MoveNext()) {
				if (item == null && e.Current == null)
					return true;

				if (item != null && item.Equals(e.Current))
					return true;
			}
			// opts must be enumerable
			return false;
		}


		static object __GetLocal(
				string name, string[] localNames,  object[] locals) {
			// Be careful, _localNames and GetLocals must be in sync with
			// eachother.

			 DbgPrint($"==> get { name }");
			 DbgPrint($"==> localNames { ArrToStr(localNames) }");
			 DbgPrint($"==> locals     { ArrToStr(locals) }");

			var idx = Array.IndexOf(localNames, name);
			if (idx > -1) {
#if DEBUG
				if (idx >= locals.Length)
					Die("_localNames and _locals out of sync.");
				return locals[idx];
#else
				return locals[idx];
#endif
			}

#if DEBUG
			Die($"walker => undefined local {name}");
#endif
			// Undefined variable.
			return Interpreter.ERRNAME;
		}

		static bool __ToBln(object obj) {
			if (obj == null)
				return false;

			if (obj is bool)
				return (bool)obj;

			if (obj is string)
				return !IsNullOrEmpty((string)obj);

			if (obj is DateTime)
				obj = ((DateTime)obj).ToOADate();

			return ToBoolean(obj);
		}

		static DateTime __AddDat(DateTime date, double days) => 
			date.AddDays(days).Date;

		static DateTime __SubDat(DateTime date, double days) => 
			date.AddDays(days * -1).Date;

		//TODO: Add strict comparison (something like JS).
		static int __Cmp(object lhs, object rhs) {
			DbgPrintCmp(lhs, rhs);

			// As of now, strings are the only "special case" for comparisons,
			// all the other types are converted to number and then compared
			// to each other.
			// (This may have to change if we extend the abaucs type system.
			//  i.e. objects, arrays, etc....).
			if (lhs is string) {
				if (
					(rhs == null) ||
					(rhs is bool   && !((bool)rhs)) ||
					(rhs is double && (((double)rhs) ==0 || IsNaN((double)rhs)))
					) {
					rhs = "";
				}

				var lstr = (string)lhs;
				if (IsNullOrEmpty(lstr))
					lstr = "";

				return string.Compare(lstr, rhs?.ToString());
			}
			// ===============================================================

			if (lhs is DateTime)
				lhs = ((DateTime)lhs).ToOADate();

			if (rhs is DateTime)
				rhs = ((DateTime)rhs).ToOADate();

			if (rhs is string && IsNullOrEmpty((string)rhs))
				rhs = 0d;


			// Are we going to support multiple cultures?
			// If so, we must to take that into account when converting values.
			double lhd = ToDouble(lhs);
			double rhd = ToDouble(rhs);

			//NaN are also special cases. For comparison purposses
			//zero and NaN are the same thing.
			if (IsNaN(lhd) && rhd == 0d)
				return 0;

			if (IsNaN(rhd) && lhd == 0d)
				return 0;
			// ======================================
			return lhd == rhd ? 0 : lhd < rhd ? -1 : 1;
		}

		[Conditional("DEBUG")]
		static void DbgPrintCmp(object lhs, object rhs) {
			WriteLine("========= cmp ========");
			WriteLine($"lhs: {lhs}({lhs?.GetType()})");
			WriteLine($"rhs: {rhs}({rhs?.GetType()})");
			WriteLine("======================");
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

		Expression Bln(Expression expr)
			=> (expr.Type != typeof(bool))
			? Call(null, TOB, Obj(expr))
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


