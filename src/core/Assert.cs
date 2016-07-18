
namespace Abacus {
	using System;
	using System.Diagnostics;
	using static System.String;
	using static Abacus.Error;


	public class Assert {
		[Conditional("DEBUG")]
		public static void DbgEnsureLhsRhs(SyntaxNode lhs, SyntaxNode rhs){
			DbgDieIf(lhs == null, "lhs can't be null.");
			DbgDieIf(rhs == null, "rhs can't be null.");
		}

		public static void Ensure(string name, object val) {
			DieIf(val == null, $"{name} can't be null.");
		}

		public static void EnsureStr(string name, string val) {
			DieIf(IsNullOrEmpty(val), $"{name} can't be null or empty.");
		}
	}
}
