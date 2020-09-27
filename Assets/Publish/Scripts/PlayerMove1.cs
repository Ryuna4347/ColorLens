using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMove1 : MonoBehaviour
{
    public int moveCount; //이 숫자만큼 이동
    public bool movePause; //렌즈 이동으로 인한 기존 이동 일시정지
    public Ease easeMode;
    //enum Color {red,orange,yellow,green,blue,}
    public float moveValue;
    public float delay;
    public float charMoveRatio;

    public bool collisionChecked; //충돌을 하면 a,b 모두에서 collisionStay가 발생해서 2번 함수를 실행하기 때문에 한쪽에서만 실행하도록 하기 위한 트리거
    [SerializeField] private GameObject collidingPrism=null; //움직임이 다 끝난 이후에 충돌처리를 하는 것이 자연스러울 것 같아서 추가
    [SerializeField]private Lens collidingLens=null;
    [SerializeField] private Mirror collidingMirror=null;
    [SerializeField] private Objective collidingObjective = null;
    [SerializeField] public List<GameObject> collisionList; //현재 충돌중인 색상들의 리스트(충돌한 모든 오브젝트에서 체크하고, 한 오브젝트만 게임매니저에 요청하는 걸로)

    private SpriteRenderer faceRenderer;
    public List<Sprite> faceList = new List<Sprite>();

    public CharacterMoveGuide moveGuide;
    public SpriteRenderer render;

    private void Awake()
    {
        faceRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        faceRenderer.sprite = faceList[0];

        if( moveGuide == null)
            moveGuide = GetComponentInChildren<CharacterMoveGuide>();

        render = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        collisionChecked = false;
        collisionList = new List<GameObject>();

        GameManager.moveRatioChanged += moveRatioChanged;

        if (moveGuide != null)
            StartCoroutine("ShowMoveGuide");
    }

    private void OnDisable()
    {
        GameManager.moveRatioChanged -= moveRatioChanged;

        if ( moveGuide != null )
            moveGuide.Rewind();

        Color color = render.color;
        color.a = 1;
        render.color = color;
        faceRenderer.color = color;
        StopAllCoroutines();
    }

    private void ChangeDirection(int dir)
    {
        Vector3 facePos = faceRenderer.transform.localPosition; //스프라이트 반전시 얼굴의 위치도 맞춰줘야한다.
        switch (dir)
        {
            case 2:
            case 3:
            case 4:
                render.flipX = false;
                faceRenderer.flipX = false;
                if(facePos.x<0)
                {
                    facePos.x *= -1;
                }
                break;
            case 6:
            case 7:
            case 8:
                render.flipX = true;
                faceRenderer.flipX = true;
                if (facePos.x > 0)
                {
                    facePos.x *= -1;
                }
                break;
            default:
                return;
        }
        faceRenderer.transform.localPosition = facePos;
    }

    public IEnumerator Move()
    {
        faceRenderer.sprite = faceList[1];
        bool isFlipped = faceRenderer.flipX;
        Vector3 facePos = faceRenderer.transform.localPosition;

        for (int i = 0; i < routeList.Count; i++)
        {
            Vector2 nextPos = GetVectorFromDirection(routeList[i]);

            if(!isFlipped)
            {
                if(nextPos.x<0)
                {
                    isFlipped = true; 
                    facePos.x *= -1;
                    faceRenderer.transform.localPosition = facePos;
                }
            }
            else
            {
                if (nextPos.x>0)
                {
                    isFlipped = false; 
                    facePos.x *= -1;
                    faceRenderer.transform.localPosition = facePos;
                }
            }
            render.flipX = isFlipped;
            faceRenderer.flipX = isFlipped;

            transform.DOMove(transform.position+(Vector3)nextPos*moveValue, delay/charMoveRatio).SetEase(easeMode);
            SoundManager.instance.Play("Move");
            yield return new WaitForSeconds(delay/ charMoveRatio * 1.5f);
        }

        faceRenderer.sprite = faceList[0];
        yield return null;
        
        if (!collisionChecked && collisionList.Count >= 1)
        {
            CheckCollideColors(); //색의 합병은 이동이 다 끝난 이후 체크
            yield break;
        }
        if (collidingPrism != null)
        {
            CheckCollidePrism();
            yield break;
        }
        if (collidingObjective != null)
        {
            CheckCollideObjective();
            yield break;
        }
        GameManager.instance.CheckMoveOver(gameObject);
    }

    public List<Direction> routeList = new List<Direction>(); //이동 시 지나게 될 경로(프리즘을 만난 경우 멈춤. 렌즈, 거울은 진행)

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="objEffect">분열에 의한 한 칸 이동인가?</param>
    public void CalculateRoute(Direction _dir)
    {
        Vector3 lastPos = transform.position;
        routeList = new List<Direction>();
        int layer = 1;
        Direction tempDir = Direction.RETURN;

        layer = ((1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("Obj"))); //캐릭터들은 이동이 완료하고 충돌 검사합니다!

        for (int i = 0; i < moveCount; i++)
        {
            Vector3 dir;

            if (tempDir == Direction.RETURN)
            {
                dir=GetVectorFromDirection(_dir);
            }
            else //렌즈에 의한 일회성 경로변경일 경우
            {
                dir = GetVectorFromDirection(tempDir);
                tempDir = Direction.RETURN;
            }
            
            List<Collider2D> hit = new List<Collider2D>(Physics2D.OverlapCircleAll(lastPos + dir * moveValue, 0.1f, layer));

            if (hit.Count>0)
            {
                GameObject hitObj;

                if(hit.Count>1) //장애물이 흰색타일 위에 있기 때문에 2개 이상 충돌이 된다. 이 경우 흰색타일은 무시하도록 한다.
                {
                    hitObj = hit.Find(x => x.gameObject.layer != LayerMask.NameToLayer("Tile")).gameObject;
                }
                else
                {
                    hitObj = hit[0].gameObject;
                }

                string collideObjTag = hitObj.tag; //레이캐스트에 충돌한 오브젝트의 태그

                Direction dirNow = GetDirectionFromVector(dir);
                lastPos= lastPos + dir * moveValue;

                routeList.Add(dirNow);

                if (collideObjTag.Equals("Wall"))
                {
                    break; //죽을 예정이므로 그 위치까지만 이동하면 됨
                }
                else if (collideObjTag.Equals("Objective"))
                {
                    break; //색상 상자에 도착해도 더 이상 이동할 필요 없음
                }
                else if (collideObjTag.Equals("Prism"))
                {
                    if (GameManager.instance.GetColorCombination(gameObject.name).Count == 1)
                    { //단색인 경우 프리즘을 통과하는게 좋다.
                        i--;
                        continue;
                    }
                    break; //프리즘에 도착한 혼합색일 경우 멈추고 분열 시도
                }
                else if (collideObjTag.Contains("Convex") || collideObjTag.Contains("Concave"))
                {
                    tempDir = hitObj.GetComponent<Lens>().GetConcaveRefractDirection((int)dirNow);
                    if (tempDir == 0) //튕겨 나올경우(거울/렌즈에 수직 진입 시도)
                    {
                        if((int)dirNow<=4)
                        {
                            routeList.Add(dirNow+4);
                        }
                        else
                        {
                            routeList.Add(dirNow - 4);
                        }
                        break;
                    }
                    i--; //렌즈와 거울은 한칸 이동으로 치지 않으므로
                }
                else if (collideObjTag.Equals("Mirror"))
                {
                    _dir = hitObj.GetComponent<Mirror>().GetMirrorReflectDirection(dirNow); //거울은 방향이 영구적으로 바뀌게 된다.
                    if (_dir == 0)
                    {
                        if ((int)dirNow <= 4)
                        {
                            routeList.Add(dirNow + 4);
                        }
                        else
                        {
                            routeList.Add(dirNow - 4);
                        }
                        break;
                    }
                    i--; //렌즈와 거울은 한칸 이동으로 치지 않으므로
                }
                else //흰색 타일
                {

                }
            }
            else
            {
                return;
            }
        }
        StartCoroutine("Move");
    }

    public void CalculateRoute(Direction _dir, Direction splitDir)
    {
        Vector3 lastPos = transform.position;
        routeList = new List<Direction>();
        int layer = 1;
        Direction tempDir = 0;

        layer = ((1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("Obj"))); //캐릭터들은 이동이 완료하고 충돌 검사합니다!

        for (int i = 0; i < moveCount+1; i++) //분열 이후 첫 한 칸은 이동횟수에 들어가지 않는다.
        {
            Vector3 dir;

            if (i == 0) //맨 처음은 분열된 이후 대각선 이동
            {
                dir = GetVectorFromDirection(splitDir);
            }
            else
            {
                if (tempDir != (Direction)0)
                {
                    dir = GetVectorFromDirection(tempDir);
                    tempDir = 0;
                }
                else
                {
                    dir = GetVectorFromDirection(_dir);
                }
            }

            List<Collider2D> hit = new List<Collider2D>(Physics2D.OverlapCircleAll(lastPos + dir * moveValue, 0.1f, layer));

            if (hit != null)
            {
                GameObject hitObj;

                if (hit.Count > 1) //장애물이 흰색타일 위에 있기 때문에 2개 이상 충돌이 된다. 이 경우 흰색타일은 무시하도록 한다.
                {
                    hitObj = hit.Find(x => x.gameObject.layer != LayerMask.NameToLayer("Tile")).gameObject;
                }
                else
                {
                    hitObj = hit[0].gameObject;
                }

                string collideObjTag = hitObj.tag; //레이캐스트에 충돌한 오브젝트의 태그

                Direction dirNow = GetDirectionFromVector(dir);
                lastPos = lastPos + dir * moveValue;

                routeList.Add(dirNow);

                if (collideObjTag.Equals("Wall"))
                {
                    break; //죽을 예정이므로 그 위치까지만 이동하면 됨
                }
                else if (collideObjTag.Equals("Objective"))
                {
                    break; //색상 상자에 도착해도 더 이상 이동할 필요 없음
                }
                else if (collideObjTag.Equals("Prism"))
                {
                    if (GameManager.instance.GetColorCombination(gameObject.name).Count == 1)
                    { //단색인 경우 프리즘을 통과하는게 좋다.
                        continue;
                    }
                    break; //프리즘에 도착한 혼합색일 경우 멈추고 분열 시도
                }
                else if (collideObjTag.Contains("Convex") || collideObjTag.Contains("Concave"))
                {
                    tempDir = hitObj.GetComponent<Lens>().GetConcaveRefractDirection((int)dirNow);
                    if (tempDir == 0)
                    {
                        if ((int)dirNow <= 4)
                        {
                            routeList.Add((Direction)((int)dirNow + 4));
                        }
                        else
                        {
                            routeList.Add((Direction)((int)dirNow - 4));
                        }
                        break;
                    }
                    i--; //렌즈와 거울은 한칸 이동으로 치지 않으므로
                }
                else if (collideObjTag.Equals("Mirror"))
                {
                    _dir = hitObj.GetComponent<Mirror>().GetMirrorReflectDirection(dirNow); //거울은 방향이 영구적으로 바뀌게 된다.
                    if (_dir == 0)
                    {
                        if ((int)dirNow <= 4)
                        {
                            routeList.Add(dirNow + 4);
                        }
                        else
                        {
                            routeList.Add(dirNow - 4);
                        }
                        break;
                    }
                    i--; //렌즈와 거울은 한칸 이동으로 치지 않으므로
                }
                else //흰색 타일
                {

                }
            }
            else
            {
                return;
            }
        }
        StartCoroutine("Move");
    }

    /// <summary>
    /// Direction형을 통해 방위 벡터로 변환
    /// </summary>
    /// <param name="_dir"></param>
    /// <returns></returns>
    private Vector3 GetVectorFromDirection(Direction _dir)
    {
        Vector3 dir = new Vector3();
        switch (_dir)
        {
            case (Direction)1: //위
                dir = new Vector3(0, 1);
                break;
            case (Direction)2: //오른쪽위
                dir = new Vector3(1, 1);
                break;
            case (Direction)3: //오른쪽
                dir = new Vector3(1, 0);
                break;
            case (Direction)4: //오른쪽아래
                dir = new Vector3(1, -1);
                break;
            case (Direction)5: //아래
                dir = new Vector3(0, -1);
                break;
            case (Direction)6: //왼쪽아래
                dir = new Vector3(-1, -1);
                break;
            case (Direction)7: //왼쪽
                dir = new Vector3(-1, 0);
                break;
            case (Direction)8: //왼쪽위
                dir = new Vector3(-1, 1);
                break;
        }
        return dir;
    }

    /// <summary>
    /// 방위를 통해 Direction형으로 변환
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    private Direction GetDirectionFromVector(Vector2 vec)
    {
        Direction dir;

        if(vec.x>0)
        {
            dir = (Direction)(3 - vec.y);
        }
        else if(vec.x<0)
        {
            dir = (Direction)(7 + vec.y);
        }
        else
        {
            dir = vec.y > 0 ? (Direction)1 : (Direction)5;
        }
        return dir;
    }

    /// <summary>
    /// 다른 색상과 충돌을 체크하여 색을 합친다.
    /// </summary>
    private void CheckCollideColors()
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

    /// <summary>
    /// 프리즘과의 충돌을 체크한다.
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="count"></param>
    private void CheckCollidePrism()
    {
        EffectManger.instance.PrismEffect(new Vector3(transform.position.x, transform.position.y, -3), "White");
        SoundManager.instance.Play("Division");
        GameManager.instance.CheckSplit(gameObject, routeList[routeList.Count - 1]);
    }

    /// <summary>
    /// 벽에 충돌해서 플레이어 사망
    /// </summary>
    private void CharacterDisappear()
    {
        SoundManager.instance.Play("DisAppear");
        movePause = true; //혹시 모를 이동에 대비해서 이동하지 못하게
        moveCount = -1; //위와 동일
        StartCoroutine("DisappearCharacter");
    }

    private void CheckCollideObjective()
    {
        movePause = true; //혹시 모를 이동에 대비해서 이동하지 못하게
        if (collidingObjective.CheckColor(gameObject.name))
        {
            collidingObjective.EraseColor(gameObject.name);
            CharacterDisappear();
        }
        else
            EffectDie();
    }

    public void EffectDie()
    {
        movePause = true; //혹시 모를 이동에 대비해서 이동하지 못하게
        moveCount = -1; //위와 동일
        SoundManager.instance.Play("Die");
        GameManager.instance.CheckMoveOver(gameObject);
        gameObject.SetActive(false);
        EffectManger.instance.circleEffect(transform.position, gameObject.name);
    }
    private IEnumerator DisappearCharacter()
    {
        float delay = 1.0f;

        Color color = render.color;
        for (float time = 0; time < delay; time+=0.05f)
        {
            color.a -= 0.05f;
            render.color = color;
            faceRenderer.color = color;

            yield return new WaitForSeconds(0.05f);
        }
        GameManager.instance.CheckMoveOver(gameObject);
        gameObject.SetActive(false);
    }

    private IEnumerator ShowMoveGuide()
    {
        while (GameManager.instance==null||!GameManager.instance.canMove)
        {
            yield return null;
        }
        moveGuide.Show(moveCount);
    }

    private void moveRatioChanged(float value)
    {
        charMoveRatio = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Prism"))
        {
            collidingPrism = collision.gameObject;
        }
        else if(collision.gameObject.tag.Equals("Wall"))
        {
            EffectDie();
        }
        else if (collision.gameObject.tag.Equals("Colors"))
        {
            collisionList.Add(collision.gameObject);
        }
        else if(collision.gameObject.tag.Equals("Objective"))
        {
            collidingObjective = collision.GetComponent<Objective>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Prism"))
        {
            collidingPrism = null;
        }
        else if (collision.gameObject.tag.Equals("Colors"))
        {
            collisionList.Remove(collision.gameObject);
        }
    }

}
