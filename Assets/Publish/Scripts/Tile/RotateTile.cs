using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTile : TileBase
{
    [SerializeField]private Direction nextDirection;
    [SerializeField]private bool isClockwise;
    [SerializeField]private float rotateTime;

    //배속에 따른 회전속도 변경 추가 필요

    private void Awake()
    {
        GameManager.moveTurnEnded += Rotate;
    }

    private void OnDestroy()
    {
        GameManager.moveTurnEnded -= Rotate;
    }

    private IEnumerator RotateTransform()
    {
        float degree = isClockwise ? -90 : 90;
        float time = 0;

        while(time < rotateTime)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Rotate(transform.forward, degree / rotateTime * 0.01f);
            time += 0.01f;
        }
    }

    public void Rotate()
    {
        if (!tileType.Equals(TileType.Rotate))
            return;
        if(isClockwise)
        {
            nextDirection += 2;
            if(nextDirection > (Direction)8)
            {
                nextDirection -= 8;
            }
        }
        else
        {
            nextDirection -= 2;
            if (nextDirection < (Direction)0)
            {
                nextDirection += 8;
            }
        }
        StartCoroutine("RotateTransform");
    }
}
