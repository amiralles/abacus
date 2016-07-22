
namespace Abacus {
	using System;
	using static System.Convert;
	using static Abacus.Error;
	using static Abacus.Assert;

	public static class StdLib {

		public static Type[] GetTypes (object[] values) {
			if (values == null)
				return null;

			if (values.Length == 0)
				return new Type[0];

			var res = new Type[values.Length];
			for (int i = 0; i < values.Length; ++i)
				res[i] = values[i]?.GetType();

			return res;
		}

		public static bool CanConvert(object obj, Type to) {
			Ensure("obj", obj);
			Ensure("to", to);

			if (obj is IConvertible)
				return true;

			//TODO: Check for custom convertions.
			return false;
		}
		
		//TODO: Implement this functions.
		public static bool Bln(object val) {
			if (val == null)
				return false;

			if (val is bool)
				return (bool)val;

			if (CanConvert(val, typeof(bool)))
				return ToBoolean(val);
			
			Die($"Can't convert {val} ({val.GetType()}) to bool");
			return false;
		}

		public static string Str(object val) {
			return "";
		}

		public static int Int(object val) {
			return 0;
		}

		public static double Dbl(object val) {
			return 0d;
		}

		public static Decimal Dec(object val) {
			return 0m;
		}

		public static object Obj(object val) {
			return null;
		}

		public static DateTime Dat(object val) {
			return DateTime.MinValue;
		}
	}
}
