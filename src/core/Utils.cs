namespace Abacus {
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Collections;
	using System.Collections.Generic;
	using static System.Console;

	public static class Utils {

		public static string ArrToStr(object[] arr) {
			return arr == null 
				? "null" : arr.Length == 0 
				? "empty" : string.Join(", ", 
						(from a in arr
						 select a?.ToString() ?? "null"));
		}

		[Conditional("DEBUG")]
		public static void DbgPrint(object msg) {
			WriteLine(msg);
		}

		[Conditional("DEBUG")]
		public static void PrintLinqTree(Expression tree) {

			const BindingFlags flags =
				BindingFlags.NonPublic
				| BindingFlags.Instance
				| BindingFlags.GetProperty
				| BindingFlags.DeclaredOnly;

			var pi = typeof(Expression).GetProperties(flags).First(
					m => m.Name == "DebugView");
			if (pi == null) {
				CantPrintLinqTree();
				return;
			}

			var dbgView = (string)pi.GetValue(tree, null);
			WriteLine("\nLinq Tree\n{0}\n", dbgView);
		}

		[Conditional("DEBUG")]
		static void CantPrintLinqTree() {
				Debug.Print("Can't print the linq tree.\n " + 
						" (Maybe Linq's internal API has changed).");
		}

		public static T[] Merge<T>(T[] a1, T[]a2) {
			var merged = new T[a1.Length + a2.Length];
			Array.Copy(a1, merged, a1.Length);
			Array.Copy(a2, 0, merged, a1.Length, a2.Length);
			return merged;
		}
	}
}

