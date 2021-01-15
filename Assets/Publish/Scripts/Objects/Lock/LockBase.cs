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
    public LockType GetLockType { get { return lockType; } }

    protected virtual void Awake()
    {
    }

    /// <summary>
    /// 오브젝트 해제
    /// </summary>
    public virtual void UnlockObj(GameObject obj)
    {
        return;
    }
}
