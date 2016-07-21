
namespace Abacus {
	using static System.Double;

	public class BuiltinLocals {

		// =========================================================
		// NAMES and VALUES *MUST* be synchronized.
		public static readonly string[] NAMES = new [] {
			"null", "nan", "false", "true" 
		};

		public static readonly object[] VALUES = new object [] {
			null, NaN, false, true
		};
		// =========================================================
	}
}

