
namespace Abacus {
	using System;
	using System.Reflection;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using static Abacus.Assert;
	using static Abacus.Utils;
	using static Abacus.Error;
	using static System.Console;
	using static System.Linq.Expressions.Expression;
	using BF = System.Reflection.BindingFlags;
	using FN = System.Func<string[], object[], object>;

	public struct Session {
		struct CacheEntry {
			public readonly string Src;
			public readonly object[] Locals;
			public readonly object Res;

			public CacheEntry(string src, object[] locals, object res) {
				Src    = src;
				Locals = locals;
				Res    = res;
			}
		}

		//Do not use this field, use GetCompilationCache instead.
		Dictionary<string, FN> _compilationCache;
		Dictionary<int, CacheEntry> _resCache;

		public Session(int id) {
			CacheMisses    = 0;
			CacheHits      = 0;
			ResCacheHits   = 0;
			ResCacheMisses = 0;
			Id = id;
			
			_compilationCache = new Dictionary<string, FN>();
			_resCache = new Dictionary<int, CacheEntry>();
		}

		Dictionary<string, FN> GetCompilationCache() => _compilationCache ?? 
			(_compilationCache = new Dictionary<string, FN>());


		public int CacheMisses, CacheHits, ResCacheMisses, ResCacheHits, Id;

		/// Returns a cached version of the compiled function
		/// if it has one, null otherwise.
		public Func<string[], object[], object> GetCompiled(string src) {
			var res = GetCompilationCache().ContainsKey(src) 
				? GetCompilationCache()[src]
				: null;

			if (res == null)
				++ CacheMisses;
			else
				++ CacheHits;

			return res;
		}

		/// Caches a compilation unit.
		public void CacheCompilation (string src, FN fn) {
			Ensure("src", src);
			Ensure("fn", fn);

			_compilationCache[src] = fn;
		}

		/// Creates a hash to represent the combination of src + locals.
		static int CreateHash(string src, object[]locals) {
			var hash = GetHashCode<object>(locals) * 23 + src.GetHashCode();
			WriteLine("create hash");
			WriteLine(src);
			WriteLine(ArrToStr(locals));
			WriteLine(hash);
			return hash;
		}

		/// Caches the result of an execution.
		public void Cache(string src, object[] locals, object res) {
			if (_resCache == null)
				_resCache = new Dictionary<int, CacheEntry>();

			var hash = CreateHash(src, locals);
			_resCache[hash] = new CacheEntry(src, locals, res);

		}

		/// It tries to get a cached result based on src and locals.
		public bool TryGetRes(string src, object[] locals, ref object res) {
			var hash = CreateHash(src, locals);

			if (_resCache != null && _resCache.ContainsKey(hash)) {
				var e =_resCache[hash];
				if (e.Src == src && CmpArr(locals, e.Locals)) {
					++ ResCacheHits;
					res = e.Res;
					return true;
				}
			}
			++ ResCacheMisses;
			res = null;
			return false;
		}
	}
}

