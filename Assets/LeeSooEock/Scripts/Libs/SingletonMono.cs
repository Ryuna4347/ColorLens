using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
	protected static T _instance;
	public static T In { get { return _instance; } }

	void Awake()
	{
		if ( _instance == null ) {
			_instance = (T)this;
			DontDestroyOnLoad( gameObject );
		} else if ( _instance != this ) {
			GameObject.Destroy( gameObject );
			return;
		}
		OnAwake();
	}
	protected virtual void OnAwake() { }
}

/// <summary>
/// 씬전환시 Destory 되는 싱글톤.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMonoD<T> : MonoBehaviour where T : SingletonMonoD<T>
{
	protected static T _instance;
	public static T In { get { return _instance; } }

	void Awake()
	{
		if ( _instance == null ) {
			_instance = (T)this;
		}
		//else if ( _instance != this ) {
		//	GameObject.Destroy( gameObject );
		//	return;
		//}
		OnAwake();
	}
	protected virtual void OnAwake() { }

	protected virtual void OnDestroy()
	{
		_instance = null;
	}
}