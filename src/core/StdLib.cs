
namespace Abacus {
	using System;
	using static System.Convert;
	using static Abacus.Error;
	using static Abacus.Assert;
	using static Abacus.Utils;

	public static class StdLib {
		public static string 
			// Client code can override this format to support different
			// cultures.
			DateFmt     = "dd/MM/yyyy",
			DateTimeFmt = "dd/MM/yyyy H:mm:ss";

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
			
			Die($"Can't convert {val} ({val.GetType()}) to bool.");
			return false;
		}

		public static string Str(object val) => 
			val?.ToString();

		public static int Int(object val) {
			if (val == null)
				return 0;

			if (val is int)
				return ((int) val);

			if (CanConvert(val, typeof(int)))
				return ToInt32(val);
			
			Die($"Can't convert {val} ({val.GetType()}) to Int32.");
			return 0;
		}

		public static double Dbl(object val) {
			if (val == null)
				return 0d;

			if (val is double)
				return ((double) val);

			if (CanConvert(val, typeof(double)))
				return ToDouble(val);

			Die($"Can't convert {val} ({val.GetType()}) to Double.");
			return 0d;
		}

		public static Decimal Dec(object val) {
			if (val == null)
				return 0m;

			if (val is decimal)
				return ((decimal) val);

			if (CanConvert(val, typeof(decimal)))
				return ToDecimal(val);

			Die($"Can't convert {val} ({val.GetType()}) to Decimal.");
			return 0m;
		}

		public static object Obj(object val) {
			object res = val;
			return res;
		}

		public static DateTime Dat(object val) =>
			DateTime(val, DateFmt).Date;

		public static DateTime DateTime(object val) =>
			DateTime(val, DateTimeFmt);

		static DateTime DateTime(object val, string fmt) {
			if (val == null)
				return System.DateTime.MinValue;

			if (val is DateTime)
				return ((DateTime) val);

			if (CanConvert(val, typeof(DateTime)))
				return DateFromStr(val.ToString(), fmt);
			
			Die($"Can't convert {val} ({val.GetType()}) to DateTime.");
			return System.DateTime.MinValue;
		}

		static DateTime DateFromStr(string dat, string fmt) {
			DbgPrint($"DateFromStr => {dat}");
			return System.DateTime.ParseExact(dat, fmt, null);
		}
	}
}
