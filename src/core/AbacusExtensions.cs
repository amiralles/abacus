namespace Abacus {
	using System;
	using System.Collections;
	using System.Data;
	using static Interpreter;
	using static Utils;
	using static System.Convert;

	public static class AbacusTableExtensions {

		static bool ToBool(object val) {
			DbgPrint(val);
			if (val is bool)
				return (bool)val;

			if (val is IConvertible)
				return ToBoolean(val);

			return false;
		}


		public static DataTable Reduce(this DataTable tbl, string formula) {
			var sess   = new Session(1);
			var names = new string[tbl.Columns.Count];
			var res = tbl.Clone();
			for (int i = 0; i < tbl.Columns.Count; ++i) 
				names[i] = tbl.Columns[i].ColumnName.ToLower();

			foreach (DataRow row in tbl.Rows) {
				var r = Eval(formula, names, row.ItemArray, ref sess);
				if(ToBool(r))
					res.ImportRow(row);
			}

			return res;
		}
	}
}

