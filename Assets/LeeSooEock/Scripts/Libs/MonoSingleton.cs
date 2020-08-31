using UnityEngine;

public class MonoSingleton<TSelfType> : MonoBehaviour where TSelfType : MonoBehaviour
{
    private void Awake()
    {
        if ( instance == null ) {
            _CreateInstance();
        }
    }

    private static TSelfType instance = null;
    public static TSelfType In {
        get {
            if ( _applicationIsQuitting ) {
                return null;
            }

            if ( instance == null ) {
                _CreateInstance();
            }
            return instance;
        }
    }

    private static void _CreateInstance()
    {
        instance = (TSelfType)FindObjectOfType( typeof( TSelfType ) );

        if ( instance == null ) {
            instance = (new GameObject( typeof( TSelfType ).Name )).AddComponent<TSelfType>();
            DontDestroyOnLoad( instance.gameObject );
        }
    }

    protected static bool _applicationIsQuitting = false;
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}
