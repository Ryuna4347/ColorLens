using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LockType
{
    PRISON, DOOR 
}

/// <summary>
/// 오브젝트를 제거하기 위해서 열쇠가 필요한 장애물들
/// </summary>
public class LockBase : MonoBehaviour
{
    [SerializeField]protected LockType lockType;
    [SerializeField] protected string keyObjName;
    public LockType GetLockType { get { return lockType; } }

    protected virtual void Awake()
    {
#if UNITY_EDITOR
        if (keyObjName == null)
            Debug.LogError(gameObject.name + " : 해제 오브젝트가 등록되어있지 않습니다");
#endif
    }

    /// <summary>
    /// 오브젝트 해제
    /// </summary>
    public virtual void UnlockObj(GameObject obj)
    {
        return;
    }
}
