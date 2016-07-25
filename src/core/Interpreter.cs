
namespace Abacus {
	using System;
	using System.Reflection;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using static Abacus.Assert;
	using static Abacus.Utils;
	using static Abacus.Error;
	using static System.Console;
	using static System.Linq.Expressions.Expression;
	using BF = System.Reflection.BindingFlags;
	using FN = System.Func<string[], object[], object>;

	public class Interpreter {
		public static readonly double Infinity = double.PositiveInfinity;
		public static readonly string 
			/// Generic error.
			ERR     = "#ERROR!", 
			/// Value is not a number.
			ERRNAN  = "#VALUE!", 
			/// Trying to access an undefined variable.
			ERRNAME = "#NAME!", 
			/// Error division by zero.
			ERRDIV0 = "#DIV!0";


		static readonly Type STDLIB = typeof(StdLib);

		static BF STAPUBCI = BF.Static | BF.DeclaredOnly | 
						 	 BF.InvokeMethod | BF.IgnoreCase | BF.Public;

		public static object Eval(string src) {
			var localNames = new string[0];
			var locals     = new string[0];
			var session    = new Session();
			return Eval(src, localNames, locals, ref session);
		}

		// TODO: Add OnSyntaxtErr handler.
		// TODO: Add OnError handler.
		public static object Eval(
				string src, string[] localNames, object[] locals, 
				ref Session session) {

			locals = SanitizeNumLocals(locals);
			EnsureEvalArgs(src, localNames, locals);

			// Since state is inmutable and function calls 
			// are side effects free, 
			// same session + same src + same locals == same res.
			object res = null;
			//TODO: Sanitize localNames make sure there is no 
			//      keyword in there.
			localNames = Merge<string>(BuiltinLocals.NAMES, localNames);
			locals     = Merge<object>(BuiltinLocals.VALUES, locals);
			DbgEnsureMerge(localNames, locals);
			if (session.TryGetRes(src, locals, ref res))
				return res;

			try {
				var fn  = session.GetCompiled(src) ?? 
					      Compile(ref session, src, localNames);

				res = fn(localNames, locals);

				if (res is double) {
					var dbl = (double) res;
				   	if (double.IsPositiveInfinity(dbl))
						res = ERRDIV0;
					else if(double.IsNaN(dbl))
						res = ERRNAN;
				}

			}
			catch(DivideByZeroException) {
				res = ERRDIV0;
			}
#if DEBUG
			catch(Exception ex) {
				WriteLine(ex.Message);
				throw;
#else
			catch(Exception) {
				res = ERR;
#endif
			}

			session.Cache(src, locals, res);
			return res;
		}

		static Func<string[], object[], object> Compile(
				ref Session session, string src, string[] localNames) {

			var tokenizer = new Tokenizer(src);
			var parser    = new Parser(tokenizer, localNames);
			var tree      = parser.Parse();

			var walker  = new SyntaxWalker();
			var fn      =  walker.Compile(tree);
			session.CacheCompilation(src, fn);
			return fn;
		}


		/// This allows client code to provide their own implementation
		/// on how to dispatch method calls. (This is for integration
		/// purposes).
		public static Func<object, string, object[], MethodResult> 
			OnDispatchCall = (reciever, lowerName, argv) => 
				new MethodResult(handled: false, result: null);

		/// This method is responsable for dispaching any 
		/// function call than happens inside the script.
		/// Client code must provide its own implementation to resolve
		/// calls to non std functions. (i.e. Integrate their own api).
		/// See: OnDispatchCall.
		static object __Dispatch(
				object target, string lowerName, object[] argv) {

			if (target != null)
				Die("Non Static Calls are not implemented yet...");

			var mi = __GetMethod(target, lowerName, StdLib.GetTypes(argv));

			if (mi == null) {
				var call = OnDispatchCall(target, lowerName, argv);
				if (!call.Handled)
					Die($"No method error ({lowerName}).");
				return call.Result;
			}

			//TODO: cache method info.
			return mi.Invoke(null, argv);
		}

		static MethodInfo __GetMethod(object target, string name, Type[]argv) {
			if (target != null)
				Die("Non std call are not implemented yet...");

			return STDLIB.GetMethod(name, STAPUBCI, null, argv, null);
		}

		[Conditional("DEBUG")]
		static void DbgEnsureMerge(string[] localNames, object[]locals) {
				DbgDieIf(localNames.Length == 0, 
						"Internal Error. Failed to merge localNames.");
				DbgDieIf(locals.Length == 0,     
						"Internal Error. Failed to merge locals.");
				DbgDieIf(locals.Length != localNames.Length, 
						"Internal Error. Failed to merge locals.");

		}

		static void EnsureEvalArgs(
				string src, string[] localNames, object[]locals) {

			Ensure("src", src);
			Ensure("localNames", localNames);
			Ensure("locals", locals);
		}
	}
}
