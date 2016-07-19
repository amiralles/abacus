
namespace Abacus {
	using System;
	using System.Linq.Expressions;
	using static System.Console;
	using static System.Linq.Expressions.Expression;

	public class Interpreter {
		public static readonly double Infinity = double.PositiveInfinity;
		public static readonly string 
			ERR     = "#ERROR!", 
			ERRNAN  = "#VALUE!", 
			ERRDIV0 = "#DIV!0";

		// TODO: Add session (for caching purposes).
		// TODO: Add OnSyntaxtErr handler.
		// TODO: Add OnError handler.
		public static object Eval(string src, string[] localNames = null) {
			//TODO: If same src and same locals,
			//      we can use a cached version of the compiled funcion.

			//TODO: If same session, same locals, same src, we don't
			//      have to do anything, just return the cached result.
			try {
				var fn   = Compile(src, localNames);
				var argv = new object[0];
				var res  = fn(argv);

				if (res is double) {
					var dbl = (double) res;
				   	if (double.IsPositiveInfinity(dbl))
						res = ERRDIV0;
					else if(double.IsNaN(dbl))
						res = ERRNAN;
				}

				return res;
			}
			catch(DivideByZeroException) {
				return ERRDIV0;
			}
#if DEBUG
			catch(Exception ex) {
				WriteLine(ex.Message);
#else
			catch(Exception) {
#endif
				return ERR;
			}
		}

		static Func<object[], object> Compile(string src, string[] localNames) {
			var tokenizer = new Tokenizer(src);
			var parser    = new Parser(tokenizer, localNames);
			var tree      = parser.Parse();
			var walker    = new SyntaxWalker();
			var program   = walker.Walk(tree);
			var argv      = Parameter(typeof(object[]), "argv");
			var lambda    = Lambda<Func<object[], object>>(program, argv);
			return lambda.Compile();
		}

		/// This method is responsable for dispaching any 
		/// function call than happens inside the script.
		public static object DispatchCall(
				object target, string lowerFnName, object[] argv) {

			return null;
		}
	}
}
