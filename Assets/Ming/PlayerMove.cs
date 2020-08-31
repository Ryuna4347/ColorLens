using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMove : MonoBehaviour
{
    public int moveCount; //이 숫자만큼 이동
    public Ease easeMode;
    //enum Color {red,orange,yellow,green,blue,}
    public float moveValue;
    public float delay;
    private void Update()
    {
        if (MoveCtrl.instance.canMove)
        {
            int dir = 0;
            if (Input.GetAxisRaw("Horizontal") == -1) 
                dir = 7;
            else if (Input.GetAxisRaw("Horizontal") == 1)
                dir = 3;
            else if (Input.GetAxisRaw("Vertical") == -1)
                dir = 5;
            else if (Input.GetAxisRaw("Vertical") == 1)
                dir = 1;
            if(dir!=0)
                StartCoroutine(Move(dir,moveCount));
        }
    }

    public void Move(int _dir)
    {
        StartCoroutine(Move(_dir, moveCount));
    }
    IEnumerator Move(int _dir,int _moveCount)
    {
        for (int i = 0; i < _moveCount; i++)
        {
            Vector2 dir=new Vector2();
            switch (_dir)
            {
                case 1: //위
                    dir = new Vector2(0,1);
                    break;
                case 2: //오른쪽위
                    dir = new Vector2(1,1);
                    break;
                case 3: //오른쪽
                    dir = new Vector2(1,0);
                    break;
                case 4: //오른쪽아래
                    dir = new Vector2(1,-1);
                    break;
                case 5: //아래
                    dir = new Vector2(0,-1);
                    break;
                case 6: //왼쪽아래
                    dir = new Vector2(-1,-1);
                    break;
                case 7: //왼쪽
                    dir = new Vector2(-1,0);
                    break;
                case 8: //왼쪽위
                    dir = new Vector2(-1,1);
                    break;
            }
        
            Vector3 goal = transform.position + new Vector3(dir.x * moveValue, dir.y * moveValue, 0);
            transform.DOMove(goal, delay).SetEase(easeMode);
            yield return new WaitForSeconds(0.01f);
            MoveCtrl.instance.canMove = false;
            yield return new WaitForSeconds(delay-0.01f);
            transform.position = goal;
        }
        MoveCtrl.instance.canMove = true;
    }
}
