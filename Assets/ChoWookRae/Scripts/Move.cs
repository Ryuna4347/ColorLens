using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    RETURN,
    UP,
    UP_RIGHT,
    RIGHT,
    DOWN_RIGHT,
    DOWN,
    DOWN_LEFT,
    LEFT,
    UP_LEFT
}

public class Move : MonoBehaviour
{
    public string Color;
    private int Dir;

    public float speed;
    public float walkCount;
    public float currentWalkCount;
    private bool canMove = true;
    private bool MirrorMove = false;
    private Vector3 vector;
    private void Update()
    {
        if (canMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                if (Input.GetKey(KeyCode.W))
                    Dir = (int)Direction.UP;
                else if (Input.GetKey(KeyCode.A))
                    Dir = (int)Direction.LEFT;
                else if (Input.GetKey(KeyCode.S))
                    Dir = (int)Direction.DOWN;
                else if (Input.GetKey(KeyCode.D))
                    Dir = (int)Direction.RIGHT;
                Moving();
            }
        }

    }
    IEnumerator MoveCoroutine()//이동코루틴
    {
        vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);


        while (currentWalkCount < walkCount)
        {
            if (MirrorMove == false)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * speed, 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * speed, 0);
                }
            }
            else
            {
                switch (Dir)
                {
                    case (int)Direction.RIGHT:
                        transform.Translate(+ speed, 0, 0);
                        break;
                    case (int)Direction.UP:
                        transform.Translate(0,speed, 0);
                        break;
                    case (int)Direction.LEFT:
                        transform.Translate(-speed, 0, 0);
                        break;
                    case (int)Direction.DOWN:
                        transform.Translate(0, -speed, 0);
                        break;
                }
            }

           
            currentWalkCount++;
        }
        currentWalkCount = 0;

        yield return new WaitForSeconds(0.2f);
        canMove = true;
        MirrorMove = false;
    }

    void Moving()//색별 이동
    {
        switch (Color)
        {
            case "White":
                for (int i = 0; i < 20; i++)
                    StartCoroutine(MoveCoroutine());
                break;
            case "Red":
                StartCoroutine(MoveCoroutine());
                break;
            case "Green":
                for (int i = 0; i < 2; i++)
                    StartCoroutine(MoveCoroutine());
                break;
            case "Blue":
                for (int i = 0; i < 3; i++)
                    StartCoroutine(MoveCoroutine());
                break;

        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Concave_L")
        {
            if (collision.transform.rotation.eulerAngles.z == 90)
            {
                if (Dir == (int)Direction.DOWN)//수직 체크
                {
                    transform.Translate(0, speed, 0);
                    return;
                }
                switch(Dir)
                {
                    case (int)Direction.RIGHT:
                        transform.Translate(speed, speed, 0);
                        break;
                    case (int)Direction.LEFT:
                        transform.Translate(-speed, speed, 0);
                        break;

                }
                
                Moving();
            }
            else
            {
                if (Dir == (int)Direction.RIGHT)//수직 체크
                {
                    transform.Translate( -speed, 0, 0);
                    return;
                }
                switch (Dir)
                {
                    case (int)Direction.UP:
                        transform.Translate(-speed, speed, 0);
                        break;
                    case (int)Direction.DOWN:
                        transform.Translate(-speed, -speed, 0);
                        break;
                }
                
                Moving();
            }
          
        }
        if (collision.gameObject.tag == "Concave_R")
        {
            if (collision.transform.rotation.eulerAngles.z == 90)
            {
                if (Dir == (int)Direction.UP)//수직 체크
                {
                    transform.Translate(0,  -speed, 0);
                    return;
                }
                switch (Dir)
                {
                    case (int)Direction.RIGHT:
                        transform.Translate(speed, -speed, 0);
                        break;
                    case (int)Direction.LEFT:
                        transform.Translate(-speed, -speed, 0);
                        break;

                }
               
                Moving();
            }
            else
            {
                if (Dir == (int)Direction.LEFT)//수직 체크
                {
                    transform.Translate(speed, 0, 0);
                    return;
                }
                switch (Dir)
                {
                    case (int)Direction.UP:
                        transform.Translate(speed, speed, 0);
                        break;
                    case (int)Direction.DOWN:
                        transform.Translate(speed, -speed, 0);
                        break;
                }

                Moving();
            }

        }
        if(collision.gameObject.tag == "Convex_L")
        {
            if (collision.transform.rotation.eulerAngles.z == 90)
            {
                if (Dir == (int)Direction.DOWN)//수직 체크
                {
                    transform.Translate(0, speed, 0);
                    return;
                }
                switch(Dir)
                {
                    case (int)Direction.RIGHT:
                        transform.Translate(speed, -speed, 0);
                        break;
                    case (int)Direction.LEFT:
                        transform.Translate(-speed, -speed, 0);
                        break;
                }
                
                Moving();
            }
            else
            {
                if (Dir == (int)Direction.RIGHT)//수직 체크
                {
                    transform.Translate(-speed, 0, 0);
                    return;
                }
                switch (Dir)
                {
                    case (int)Direction.UP:
                        transform.Translate(speed, speed, 0);
                        break;
                    case (int)Direction.DOWN:
                        transform.Translate(speed, -speed, 0);
                        break;
                }

                Moving();
            }
        }
        if(collision.gameObject.tag == "Convex_R")
        {
            if (collision.transform.rotation.eulerAngles.z == 90)
            {
                if (Dir == (int)Direction.UP)//수직 체크
                {
                    transform.Translate(0,-speed, 0);
                    return;
                }
                switch (Dir)
                {
                    case (int)Direction.RIGHT:
                        transform.Translate(speed, speed, 0);
                        break;
                    case (int)Direction.LEFT:
                        transform.Translate(-speed, speed, 0);
                        break;
                }
            }
            else
            {
                if (Dir == (int)Direction.LEFT)//수직 체크
                {
                    transform.Translate(speed, 0, 0);
                    return;
                }
                switch (Dir)
                {
                    case (int)Direction.UP:
                        transform.Translate(-speed, speed, 0);
                        break;
                    case (int)Direction.DOWN:
                        transform.Translate(-speed, -speed, 0);
                        break;
                }
            }
            Moving();
        }
        if (collision.gameObject.tag == "Mirror")
        {
            MirrorMove = true;
            if (collision.transform.rotation.eulerAngles.z == 90)
            {
                switch (Dir)
                {
                    case (int)Direction.RIGHT:
                        Dir = (int)Direction.DOWN;
                        transform.Translate(0, -speed, 0);
                        break;
                    case (int)Direction.UP:
                        transform.Translate(- speed, 0, 0);
                        Dir = (int)Direction.LEFT;
                        break;
                    case (int)Direction.LEFT:
                        transform.Translate(0, speed, 0);
                        Dir = (int)Direction.UP;
                        break;
                    case (int)Direction.DOWN:
                        transform.Translate(speed, 0, 0);
                        Dir = (int)Direction.RIGHT;
                        break;
                }

            }
            else
            {
                switch (Dir)
                {
                    case (int)Direction.RIGHT:
                        transform.Translate(0, speed, 0);
                        Dir = (int)Direction.UP;
                        break;
                    case (int)Direction.UP:
                        transform.Translate(speed, 0, 0);
                        Dir = (int)Direction.RIGHT;
                        break;
                    case (int)Direction.LEFT:
                        transform.Translate(0, - speed, 0);
                        Dir = (int)Direction.DOWN;
                        break;
                    case (int)Direction.DOWN:
                        transform.Translate(-speed, 0, 0);
                        Dir = (int)Direction.LEFT;
                        break;
                }
            }
            Moving();
        }
        
    }

    

}
