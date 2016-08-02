namespace Abacus {
	using System;
	using System.Collections;
	using System.Data;
	using static Interpreter;
	using static Utils;
	using static System.Convert;

	public static class AbacusTableExtensions {
		static Random Rnd = new Random();

		static bool ToBool(object val) {
			DbgPrint(val);

			if (val is bool)
				return (bool)val;

			if (val == null)
				return false;

			if (val is IConvertible)
				return ToBoolean(val);

			return false;
		}

		public static DataTable Reduce(this DataTable tbl, string formula) =>
			tbl.Reduce(formula, new string[0], new object[0]);

		public static DataTable Reduce(this DataTable tbl, 
				string formula, Func<Exception, object> onError) =>
			tbl.Reduce(formula, new string[0], new object[0], onError);

		public static DataTable Reduce(this DataTable tbl, 
				string formula, string[] locNames, object[] locals) =>
			tbl.Reduce(formula, locNames, locals, null);

		public static DataTable Reduce(this DataTable tbl, 
				string formula, string[] locNames, object[] locals, 
				Func<Exception, object> onError) {

			//TODO: Validate args.

			var sess  = new Session(Rnd.Next());
			var names = new string[tbl.Columns.Count + locNames.Length];
			var res = tbl.Clone();

			// ===========================================================
			// Merge local names.
			// ===========================================================
			var colslen = tbl.Columns.Count;
			for (int i = 0; i < colslen; ++i) 
				names[i] = tbl.Columns[i].ColumnName.ToLower();

			for (int i = 0; i < locNames.Length; ++i)
				names[colslen + i] = locNames[i];
			// ===========================================================

			var locs = new object[locals.Length + tbl.Rows[0].ItemArray.Length];
			foreach (DataRow row in tbl.Rows) {
				// ========================================================
				// Merge local values.
				// ========================================================
				int rowslen =  row.ItemArray.Length;
				for(int i = 0 ; i < rowslen; ++i)
					locs[i] = row.ItemArray[i];

				for (int i = 0; i < locals.Length; ++i)
					locs[rowslen + i] = locals[i];
				//==========================================================

				try {
					var r = Eval(formula, names, locs, ref sess, onError);
					if(ToBool(r))
						res.ImportRow(row);
				}
				catch(Exception ex) {
					if (onError != null) {
						onError(ex);
					}
					else {
						throw;
					}
				}
			}

			return res;
		}
	}
}

