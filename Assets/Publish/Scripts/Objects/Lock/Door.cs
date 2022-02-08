using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : LockBase
{
    [SerializeField] private List<GameObject> remainKeyList;


    protected override void Awake()
    {
        base.Awake();
        remainKeyList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Key"));
    }

    /// <summary>
    /// 키 획득하여 문 조건 확인
    /// </summary>
    /// <param name="keyObj"></param>
    public void CheckGainedKey(GameObject keyObj)
    {
        if(!remainKeyList.Contains(keyObj))
        {
#if UNITY_EDITOR
            Debug.Log("키 열림 실패");
#endif
            return;
        }
        remainKeyList.Remove(keyObj);
        if(remainKeyList.Count == 0)
        {
            StartCoroutine("Disappear");
        }
    }

    private IEnumerator Disappear()
    {
        float disappearTime = 1.0f;
        float time = 0;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color spriteColor = spriteRenderer.color;

        GameManager.instance.UpdateObjectActive(this.gameObject, transform.position, false);
        while (time < disappearTime)
        {
            spriteColor.a -= 1 / disappearTime * Time.deltaTime;
            spriteRenderer.color = spriteColor;
            time += Time.deltaTime;
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
}
