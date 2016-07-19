
namespace Abacus {
	using System;
	using System.Linq.Expressions;
	using static System.Linq.Expressions.Expression;

	public class Interpreter {

		// TODO: Add session (for caching purposes).
		public static object Eval(string src, string[] localNames = null) {
			//TODO: If same src and same locals,
			//      we can use a cached version of the compiled funcion.

			//TODO: If same session, same locals, same src, we don't
			//      have to do anything, just return the cached result.

			var fn   = Compile(src, localNames);
			var argv = new object[0];
			return fn(argv);
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
