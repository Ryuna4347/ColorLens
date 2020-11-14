using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prison : LockBase
{
    public float disappearTime;

    public override void UnlockObj(GameObject obj)
    {
        if (obj.name.Contains(keyObjName))
        {
            StartCoroutine("DisappearPrison");
        }
    }

    private IEnumerator DisappearPrison()
    {
        float time = 0f;
        Color color = gameObject.GetComponent<SpriteRenderer>().color;

        while (time < disappearTime)
        {
            color.a -= 0.05f;
            time += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.SetActive(false);
    }
}
