using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableTile : TileBase
{
    private int willBreak;

    public int WillBreak
    {
        get { return willBreak; }
    }

    private void Awake()
    {
        willBreak=-1; 
        GetComponent<Collider2D>().enabled = true;
    }

    /// <summary>
    /// 다음에 진행할 방향을 알려준다. 동시에 들어오는 경우는 통과하며, 캐릭터가 지나가는 순간 붕괴되어 낭떠러지가 생긴다.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="time">얼마나 움직여서 이 블럭에 도착하는가?(동시 도착 체크)</param>
    /// <returns></returns>
    public override Direction? GetNextDirection(Direction direction, int time)
    {
        if (willBreak > 0 && willBreak < time) //부서진 이후에 접근하는 색상 캐릭터는 튕기지 않고 소멸되므로 방향을 줄 수 없다.
        {
            return null; 
        }
        willBreak = time;
        return direction; //별다른 문제가 없다면 그대로 이동이 가능하다.
    }

    private IEnumerator Disappear()
    {
        float time = 1f;
        float passedTime = 0;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;

        GetComponent<Collider2D>().enabled = false; //혹시 모를 충돌에 방지

        yield return new WaitForSeconds(0.05f/GameManager.instance.moveRatio);
        
        GameManager.instance.CreateWall(transform.position, transform.parent);

        while (passedTime < time)
        {
            color.a = 1 - passedTime / time;
            spriteRenderer.color = color;
            passedTime += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //만약 타일 위에서 캐릭터가 만나서 합성하면 이게 실행될텐데...?
        StartCoroutine("Disappear");
    }
}
