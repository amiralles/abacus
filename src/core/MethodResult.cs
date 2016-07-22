namespace Abacus {
	public struct MethodResult {
		public readonly bool Handled;
		public readonly object Result;

		public MethodResult(bool handled, object result) {
			Handled = handled;
			Result  = result;
		}
	}
}
