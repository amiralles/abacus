

namespace Abacus {
	using System;
	using static System.Console;
	using static Abacus.Interpreter;

	class Program {
		static void Prompt() => Write("> ");

		static int Main(string[] argv) {

			string expr;
			var sess   = new Session();
			var names  = new string[0];
			var locals = new object[0];
			Func<Exception, object> handleErr = ex => ex.Message;

			Prompt();
			while((expr = ReadLine()) != "exit" && expr != "quit") {
				try {
					WriteLine(Eval(expr, names, locals, ref sess, handleErr));
					Prompt();
				}
				catch (Exception ex) {
					WriteLine(ex.Message);
					Prompt();
				}
			}

			return 0;
		}
	}
}
