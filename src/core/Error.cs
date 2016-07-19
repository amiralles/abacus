namespace Abacus {
	using System;
	using System.Diagnostics;

	public class Error {
		public static void Die(object msg) => Die(msg?.ToString() ?? "null");

		public static void Die(string msg) {
			throw new Exception(msg);
		}
	
		public static void DieIf(bool cnd, string msg){
			if (cnd)
				Die(msg);
		}

		[Conditional("DEBUG")]
		public static void DbgDieIf(bool cnd, string msg){ 
			DieIf(cnd, msg);
		}
	}
}
