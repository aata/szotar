namespace Szotar {
	// These are defined in System.Core, which we don't use.

	public delegate void Action();
	public delegate void Action<T1>(T1 arg);
	public delegate void Action<T1, T2>(T1 arg, T2 arg2);
	public delegate void Action<T1, T2, T3>(T1 arg, T2 arg2, T3 arg3);
	
	public delegate R Func<R>();
	public delegate R Func<R, T1>(T1 arg);
	public delegate R Func<R, T1, T2>(T1 arg, T2 arg2);
	public delegate R Func<R, T1, T2, T3>(T1 arg, T2 arg2, T3 arg3);
}