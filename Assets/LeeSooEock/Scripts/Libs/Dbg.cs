using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

public enum Log
{
	to,                 // 태그 없는 일반 로그용
	Table,
	Svr,                // for Server/Network
	Client,
	Time,
	Build,
	UI,
	App,
	Sound,
	Destroy,            //객채 삭제시키는경우의 로그.
	Check,              //임시테스트용
	Asset,              //자원에대한 생산소모 등에대한 검증 로그
	Resources,          //리소스 불러오기 관련 로그.
	Seq,                //게임 시퀀스 체크 용도.
	Stage,              //스테이지 시퀀스 체크
	Cheat,
}

public interface ILoggable
{
	string LogPrefix();
}

public static class LogEx
{
	public static string Prefix( string prefix, string addStr )
	{
		return Dbg.Prefix( prefix ) + " " + addStr.GetSafe();
	}
	public static string Prefix( this Log kind, string addStr )
	{
		return kind == Log.to ? addStr.GetSafe() : Dbg.Prefix( kind.ToString() ) + " " + addStr.GetSafe();
	}
	public static string Prefix( this ILoggable loggable, string addStr )
	{
		return loggable.LogPrefix() + " " + addStr.GetSafe();
	}

	#region L : Log. 릴리즈 빌드에서도 로깅
	/// <summary>
	/// 로그를 무조건 출력한다. 릴리즈 빌드에도 로깅이 되므로 가급적 사용을 자제할 것.
	/// </summary>
	public static void L( this Log kind, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( kind.Prefix( str ), args );
		else UnityEngine.Debug.Log( kind.Prefix( str ) );
	}
	/// <summary>
	/// 로그를 무조건 출력한다. 릴리즈 빌드에도 로깅이 되므로 가급적 사용을 자제할 것.
	/// </summary>
	public static void L( this ILoggable loggable, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( loggable.Prefix( str ), str, args );
		else UnityEngine.Debug.Log( loggable.Prefix( str ) );
	}

	/// <summary>
	/// 로그를 무조건 출력한다. 릴리즈 빌드에도 로깅이 되므로 가급적 사용을 자제할 것.
	/// </summary>
	public static void logL( this UnityEngine.Object context, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( context, Prefix( context.name, str ), args );
		else UnityEngine.Debug.Log( Prefix( context.name, str ), context );
	}
	#endregion

	#region I : Info. 유니티나 DEBUG 빌드일때만 로깅
	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void I( this Log kind, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( kind.Prefix( str ), args );
		else UnityEngine.Debug.Log( kind.Prefix( str ) );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void I( this ILoggable loggable, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( loggable.Prefix( str ), args );
		else UnityEngine.Debug.Log( loggable.Prefix( str ) );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void I( this ILoggable loggable, Func<string> makeStr )
	{
		UnityEngine.Debug.Log( loggable.Prefix( makeStr() ) );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void logI( this UnityEngine.Object context, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( context, Prefix( context.name, str ), args );
		else UnityEngine.Debug.Log( Prefix( context.name, str ), context );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void logI( this UnityEngine.Object context, Func<string> makeStr )
	{
		UnityEngine.Debug.Log( Prefix( context.name, makeStr() ), context );
	}
	#endregion

	#region W
	public static void W( this Log kind, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogWarningFormat( kind.Prefix( str ), args );
		else UnityEngine.Debug.LogWarning( kind.Prefix( str ) );
	}

	public static void W( this ILoggable loggable, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogWarningFormat( loggable.Prefix( str ), args );
		else UnityEngine.Debug.LogWarning( loggable.Prefix( str ) );
	}

	public static void logW( this UnityEngine.Object context, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogWarningFormat( context, Prefix( context.name, str ), args );
		else UnityEngine.Debug.LogWarning( Prefix( context.name, str ), context );
	}
	#endregion

	#region E
	public static void E( this Log kind, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogErrorFormat( kind.Prefix( str ), args );
		else UnityEngine.Debug.LogError( kind.Prefix( str ) );
	}

	public static void E( this ILoggable loggable, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogErrorFormat( loggable.Prefix( str ), args );
		else UnityEngine.Debug.LogError( loggable.Prefix( str ) );
	}

	public static void logE( this UnityEngine.Object context, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogErrorFormat( context, Prefix( context.name, str ), args );
		else UnityEngine.Debug.LogError( Prefix( context.name, str ), context );
	}
	#endregion

	#region A : Assert. 유니티나 DEBUG 빌드일때만 로깅
	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void A( this Log kind, bool condition, string str, params object[] args )
	{
		UnityEngine.Debug.AssertFormat( condition, kind.Prefix( str ), args );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void A( this ILoggable loggable, bool condition, string str, params object[] args )
	{
		UnityEngine.Debug.AssertFormat( condition, loggable.Prefix( str ), args );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void logA( this UnityEngine.Object context, bool condition, string str, params object[] args )
	{
		UnityEngine.Debug.AssertFormat( condition, context, str, args );
	}
	#endregion
}

public static class Dbg
{

	public static string Prefix( string header, params string[] args )
	{
		if ( args.Length == 0 ) {
			return "[" + header + "]";
		} else {
			header = Prefix( header );
			for ( int i = 0; i < args.Length; i++ )
				header += Prefix( args[i] );
			return header;
		}
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void I( string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( str, args );
		else UnityEngine.Debug.Log( str );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void I( Func<string> makeStr )
	{
		UnityEngine.Debug.Log( makeStr() );
	}

	public static void L( string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogFormat( str, args );
		else UnityEngine.Debug.Log( str );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void E( string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogErrorFormat( str, args );
		else UnityEngine.Debug.LogError( str );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void W( string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.LogWarningFormat( str, args );
		else UnityEngine.Debug.LogWarning( str );
	}

	[Conditional( "UNITY_EDITOR" ), Conditional( "DEBUG" )]
	public static void A( bool val, string str, params object[] args )
	{
		if ( args.Length > 0 ) UnityEngine.Debug.AssertFormat( val, str, args );
		else UnityEngine.Debug.Assert( val, str );
	}

	public static string GetCallStackInfo( int frame, int count = 0 )
	{
		var sb = new StringBuilder( 512 );
		var st = new StackTrace( true );

		int frameCount = count + frame;
		if ( count == 0 || frameCount > st.FrameCount )
			frameCount = st.FrameCount;

		for ( int i = frame + 1; i < frameCount; i++ ) {
			StackFrame sf = st.GetFrame( i );
			sb.AppendFormat( "{0} (at {1}:{2})\n", sf.GetMethod().ToString(), sf.GetFileName(), sf.GetFileLineNumber() );
		}
		return sb.ToString();
	}
}