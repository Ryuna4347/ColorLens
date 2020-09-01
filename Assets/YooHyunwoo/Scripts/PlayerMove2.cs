using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMove2 : MonoBehaviour
{
    public int moveCount; //이 숫자만큼 이동
    public bool movePause; //렌즈 이동으로 인한 기존 이동 일시정지
    public Ease easeMode;
    //enum Color {red,orange,yellow,green,blue,}
    public float moveValue;
    public float delay;

    public int moveDir;

    public bool collisionChecked; //충돌을 하면 a,b 모두에서 collisionStay가 발생해서 2번 함수를 실행하기 때문에 한쪽에서만 실행하도록 하기 위한 트리거
    public GameObject collidingPrism=null; //움직임이 다 끝난 이후에 충돌처리를 하는 것이 자연스러울 것 같아서 추가
    public Lens collidingLens=null; 
    public Mirror collidingMirror=null;
    public List<GameObject> collisionList; //현재 충돌중인 색상들의 리스트(충돌한 모든 오브젝트에서 체크하고, 한 오브젝트만 게임매니저에 요청하는 걸로)

    private void OnEnable()
    {
        collisionChecked = false;
        collisionList = new List<GameObject>();
        GetComponent<Collider2D>().enabled=false; //분열하는 경우 생성되면서 다른 색의 오브젝트와 충돌해서 첫 이동을 마치고 충돌체크하는 걸로
    }

    private void OnDisable()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color color = sprite.color;
        color.a = 1;
        sprite.color = color;
        StopAllCoroutines();
    }

    public IEnumerator Move(int _dir)
    {
        int tempDir=_dir;

        moveDir = _dir;
        for (int i = 0; i < moveCount; i++)
        {
            Vector2 dir=new Vector2();
            switch (moveDir)
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
            yield return new WaitForSeconds(delay);
            transform.position = goal;
            if (GetComponent<Collider2D>().enabled == false) //첫 이동 이후 충돌체크 진행
                GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            while (collidingPrism != false || collidingMirror != null || collidingLens != null)
            {
                CheckCollidePrism(moveDir, i + 1);
                if (CheckCollideLens(moveDir) == 0) //렌즈 측면으로 접근할 경우
                {
                    break; //이동 중지
                }

                int reflectDir = CheckCollideMirror(moveDir); //거울의 경우 반사된 방향으로 나머지 이동경로를 바꾼다.
                if (reflectDir == 0)
                {
                    break; //이동 중지
                }
                else if (reflectDir > 0)
                {
                    moveDir = reflectDir;
                    tempDir = moveDir;
                }
                yield return new WaitForSeconds(delay + 0.2f);
            }
            while (movePause) //이동 중 렌즈를 통과하는 경우 기존 이동을 일시 중지시키기 위해서 사용
            {
                yield return null;
            }
            moveDir = tempDir;
        }
        GameManager.instance.CheckMoveOver();
        while (GameManager.instance.movingChars > 0)
        {
            yield return null;
        }
        CheckCollide(); //색의 합병은 이동이 다 끝난 이후 체크
    }

    /// <summary>
    /// 분열/굴절 이후 이동할 때 사용
    /// </summary>
    /// <param name="_dir">분열시 이동하는 방향</param>
    /// <param name="nextDir">굴절되어 1칸이동한 다음 이동할 방향</param>
    /// <param name="noPrismEffect">프리즘 효과를 받은 오브젝트인가?(합성색인 경우 true)</param>
    /// <returns></returns>
    public IEnumerator Move(int _dir, int nextDir=0, bool noPrismEffect = false)
    {
        moveDir = _dir;
        for (int j = 0; j < 1; j++)
        {
            Vector2 dir = new Vector2();
            switch (_dir)
            {
                case 1: //위
                    dir = new Vector2(0, 1);
                    break;
                case 2: //오른쪽위
                    dir = new Vector2(1, 1);
                    break;
                case 3: //오른쪽
                    dir = new Vector2(1, 0);
                    break;
                case 4: //오른쪽아래
                    dir = new Vector2(1, -1);
                    break;
                case 5: //아래
                    dir = new Vector2(0, -1);
                    break;
                case 6: //왼쪽아래
                    dir = new Vector2(-1, -1);
                    break;
                case 7: //왼쪽
                    dir = new Vector2(-1, 0);
                    break;
                case 8: //왼쪽위
                    dir = new Vector2(-1, 1);
                    break;
            }

            Vector3 goal = transform.position + new Vector3(dir.x * moveValue, dir.y * moveValue, 0);
            transform.DOMove(goal, delay).SetEase(easeMode);
            yield return new WaitForSeconds(delay);
            transform.position = goal;
            if (GetComponent<Collider2D>().enabled == false) //첫 이동 이후 충돌체크 진행
                GetComponent<Collider2D>().enabled = true;
            yield return new WaitForSeconds(0.5f);

            while (collidingPrism != false || collidingMirror != null || collidingLens != null)
            {
                CheckCollidePrism(moveDir, j + 1);
                if (CheckCollideLens(moveDir) == 0) //렌즈 측면으로 접근할 경우
                {
                    break; //이동 중지
                }

                int reflectDir = CheckCollideMirror(moveDir); //거울의 경우 반사된 방향으로 나머지 이동경로를 바꾼다.
                if (reflectDir == 0)
                {
                    break; //이동 중지
                }
                else if (reflectDir > 0)
                {
                    moveDir = reflectDir;
                }
                yield return new WaitForSeconds(delay + 0.2f);
            }
        }
        if (!noPrismEffect) //프리즘에 의해 분열된 색상들은 새로 이동명령을 내려야한다.
        {
            StartCoroutine(Move(nextDir));
        }
        else //3원색은 그냥 통과하므로 기존의 이동을 속행시킨다.
        {
            movePause = false;
        }
    }

    /// <summary>
    /// 다른 색상과 충돌을 체크하여 색을 합친다.
    /// </summary>
    private void CheckCollide()
    {
        if (!collisionChecked)
        {
            if (collisionList.Count >= 1)
            {
                collisionChecked = true;
                foreach (GameObject collide in collisionList) //중복처리 방지하도록 제일 빠르게 도달한 오브젝트가 다른 오브젝트의 이 함수 진행을 막는다.
                {
                    collide.GetComponent<PlayerMove1>().collisionChecked = true;
                }
                if (!collisionList.Contains(gameObject))
                {
                    collisionList.Add(this.gameObject);
                }
                GameManager.instance.CheckMerge(collisionList);
            }
        }
        
    }

    /// <summary>
    /// 프리즘과의 충돌을 체크한다.
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="count"></param>
    private void CheckCollidePrism(int _dir, int count)
    {
        if (collidingPrism!=null)
        {
            GameManager.instance.CheckSplit(gameObject, _dir);
        }
    }

    /// <summary>
    /// 현재 접촉해있는 렌즈가 있는지 체크
    /// </summary>
    /// <param name="_dir"></param>
    /// <returns>이동 기능 중지(렌즈에 수직으로 접근하여 튕겨나옴)</returns>
    private int CheckCollideLens(int _dir)
    {
        if(collidingLens != null)
        {
            movePause = true;
            Direction refract = collidingLens.GetConcaveRefractDirection(_dir);
            if(refract == Direction.RETURN)
            {
                int reverseDir = _dir > 4 ? _dir - 4 : _dir + 4; //역방향
                StartCoroutine(Move(reverseDir,0,true)); //직전에 모든 코루틴을 종료했기 때문에 한칸만 이동하고 이동이 종료된다.
                return 0;
            }
            else
            {
                StartCoroutine(Move((int)refract,0,true));
                return 1;
            }
        }
        return 1; //충돌한 렌즈가 없는 경우 정상 진행
    }

    /// <summary>
    /// 현재 접촉해있는 대각 거울이 있는지 체크
    /// </summary>
    /// <param name="_dir"></param>
    /// <returns>이후 움직일 방향</returns>
    private int CheckCollideMirror(int _dir)
    {
        if (collidingMirror != null)
        {
            movePause = true;
            Direction reflect = collidingMirror.GetMirrorReflectDirection(_dir);
            if (reflect == Direction.RETURN)
            {
                int reverseDir = _dir > 4 ? _dir - 4 : _dir + 4; //역방향
                StartCoroutine(Move(reverseDir, 0, true)); //직전에 모든 코루틴을 종료했기 때문에 한칸만 이동하고 이동이 종료된다.
                return 0;
            }
            else
            {
                StartCoroutine(Move((int)reflect, 0, true));
                return (int)reflect;
            }
        }
        return -1; //충돌한 렌즈가 없는 경우 정상 진행
    }

    /// <summary>
    /// 벽에 충돌해서 플레이어 사망
    /// </summary>
    private void PlayerDead()
    {
        movePause = true; //혹시 모를 이동에 대비해서 이동하지 못하게
        moveCount = -1; //위와 동일
        StartCoroutine("DisappearPlayer");
    }
    private void CheckCollideObjective(Objective objective)
    {
        movePause = true; //혹시 모를 이동에 대비해서 이동하지 못하게
        if (objective.CheckColor(gameObject.name))
        {
            objective.EraseColor(gameObject.name);
        }
        PlayerDead();
    }

    private IEnumerator DisappearPlayer()
    {
        float delay = 1.0f;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color color = sprite.color;
        for (float time = 0; time < delay; time+=0.05f)
        {
            color.a -= 0.05f;
            sprite.color = color;

            yield return new WaitForSeconds(0.05f);
        }
        GameManager.instance.ControlCharacterCount(); //캐릭터 사망에 따른 전체 캐릭터 갯수 감소
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        {
            PlayerDead();
        }
        else if (collision.gameObject.tag.Equals("Objective"))
        {
            CheckCollideObjective(collision.GetComponent<Objective>());
        }
        else if (collision.gameObject.tag.Equals("Colors"))
        {
            collisionList.Add(collision.gameObject);
        }
        else if (collision.gameObject.tag.Equals("Prism")) //프리즘과 충돌한 경우
        {
            collidingPrism = collision.gameObject;
        }
        else if (collision.gameObject.tag.Contains("Concave") || collision.gameObject.tag.Contains("Convex")) //렌즈와 충돌한 경우
        {
            collidingLens = collision.gameObject.GetComponent<Lens>();
        }
        else if (collision.gameObject.tag.Equals("Mirror"))
        {
            collidingMirror = collision.gameObject.GetComponent<Mirror>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Colors"))
        {
            collisionList.Remove(collision.gameObject);
        }
        if (collision.gameObject.tag.Equals("Prism") &&collidingPrism == collision.gameObject)
        {
            collidingPrism = null;
        }
        if ((collision.gameObject.tag.Contains("Concave") || collision.gameObject.tag.Contains("Convex"))&&collidingLens.gameObject==collision.gameObject) //연속 렌즈 접근시 새로운 렌즈로 갱신된 이후 이전 렌즈의 exit이 실행되서 null로 되버리는 경우 발생 
        {
            collidingLens = null;
        }
        if (collision.gameObject.tag.Equals("Mirror") && collidingMirror.gameObject == collision.gameObject)
        {
            collidingMirror = null;
        }
    }
}
