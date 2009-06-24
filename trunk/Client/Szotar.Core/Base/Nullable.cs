using System.Linq;
using System;

namespace Szotar {
	public static class Maybe {
		public static U? SelectMany<T, U>(this T? x, Func<T, U?> k)
			where U : struct
			where T : struct
		{
			return x.FromMaybe(null, k);
		}

		public static U FromMaybe<T, U>(this T? x, U def, Func<T, U> f)
			where T : struct
		{
			if (x.HasValue)
				return f(x.Value);
			else
				return def;
		}
	}
}