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

    public bool holdByPrison = false;

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
        GameManager.moveRatioChanged += moveRatioChanged;

        if (moveGuide != null)
            StartCoroutine("ShowMoveGuide");
    }

    private void OnDisable()
    {
        GameManager.moveRatioChanged -= moveRatioChanged;

        if (moveGuide != null )
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
        GameObject collideKey;

        Vector3 oldPosition = transform.position;

        Vector3 facePos = faceRenderer.transform.localPosition;
        faceRenderer.sprite = faceList[1];
        bool isFlipped = faceRenderer.flipX;

        for (int i = 0; i < routeList.Count; i++)
        {
            Vector2 nextPos = CommonFunc.GetVectorFromDirection(routeList[i]);

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
            if ((collideKey = GameManager.instance.CheckCollisionWithKey(transform.position)) != null)
            {
                collideKey.GetComponent<DoorKey>().GetKey();
            }
        }
        faceRenderer.sprite = faceList[0];

        GameManager.instance.UpdateCharacterPos(gameObject, oldPosition, transform.position);
        if (GameManager.instance.CheckCollisionWithObjects(this, transform.position)) //게임매니저에서 어떤 종류의 충돌인지 확인하고 넘겨주도록 한다.
        {
            GameManager.instance.CheckMoveOver(gameObject);
        }
    }

    public List<Direction> routeList = new List<Direction>(); //이동 시 지나게 될 경로(프리즘을 만난 경우 멈춤. 렌즈, 거울은 진행)

    /// <summary>
    /// 이동할 경로 예측
    /// </summary>
    /// <param name="_dir">초기 이동 방향</param>
    /// <param name="remainMoveCount">남은 이동 횟수(포탈 아이템 이용시)</param>
    public void CalculateRoute(Direction _dir, int alreadyMoveCount = -1)
    {
        Vector3 lastPos = transform.position;
        Direction tempDir = Direction.RETURN; //이동 방향이 일시적으로 변하는 경우를 위한 변수
        int layer = 1;
        int characterMoveCount;

        routeList = new List<Direction>();

        layer = ((1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("Obj")));

        if (holdByPrison)
            return;

        if (alreadyMoveCount >= 0)
        {
            characterMoveCount = moveCount - alreadyMoveCount;
            routeList.Add(_dir); //처음 포탈에서 나가는 건 횟수 X
            lastPos += CommonFunc.GetVectorFromDirection(_dir);
        }
        else
        {
            characterMoveCount = moveCount;
        }

        try
        {
            _dir = (Direction)CheckFirstPosition(lastPos,_dir); //현재 캐릭터가 위치한 타일로 인해 초기 방향이 변할 수 있다.
        }
        catch(System.Exception e)
        {
            Debug.LogError("이동 궤도 예측에 문제 발생 : " + e.Message +" "+ _dir);
            return;
        }

        for (int i = 0; i < characterMoveCount; i++)
        {
            List<GameObject> hitObjList = GameManager.instance.GetObjsNextPosition(gameObject.name,lastPos, tempDir==Direction.RETURN?_dir:tempDir);

            if (hitObjList != null && hitObjList.Count > 0)
            {
                GameObject hitObj;

                if (hitObjList.Count > 1) //장애물이 흰색타일 위에 있기 때문에 2개 이상 충돌이 된다. 이 경우 흰색타일은 무시하도록 한다.
                {
                    hitObj = hitObjList.Find(x => x.gameObject.layer != LayerMask.NameToLayer("Tile")).gameObject;
                }
                else
                {
                    hitObj = hitObjList[0].gameObject;
                }

                string collideObjTag = hitObj.tag; //레이캐스트에 충돌한 오브젝트의 태그

                Direction dirNow;
                if (tempDir == Direction.RETURN)
                {
                    dirNow = _dir;
                }
                else //렌즈에 의한 일회성 경로변경일 경우
                {
                    dirNow = tempDir;
                    tempDir = Direction.RETURN;
                }
                lastPos = lastPos + CommonFunc.GetVectorFromDirection(dirNow) * moveValue;

                routeList.Add(dirNow);

                if (collideObjTag.Equals("Wall") || collideObjTag.Equals("Objective") || collideObjTag.Equals("Portal"))
                {
                    break; //이 블럭 이상으로 이동이 불가하다.
                }
                else if (collideObjTag.Equals("Prism"))
                {
                    if (CommonFunc.GetColorCombination(gameObject.name.Split('_')[0]).Count == 1)
                    { //단색인 경우 프리즘을 통과한다.
                        i--;
                        continue;
                    }
                    break; //프리즘까지만 이동하고 분열 시도
                }
                else if (collideObjTag.Equals("Filter"))
                {
                    if (hitObj.GetComponent<ColorFilter>().CanCharacterPenetrate(gameObject.name.Split('_')[0]))
                    {
                        i--;
                        continue;
                    }
                    break;
                }
                else if (collideObjTag.Contains("Convex") || collideObjTag.Contains("Concave"))
                {
                    tempDir = hitObj.GetComponent<Lens>().GetConcaveRefractDirection((int)dirNow, ref _dir);
                    if (tempDir == 0) //거울/렌즈에 수직 진입 시도
                    {
                        routeList.Add((Direction)((int)dirNow <= 4 ? (int)dirNow + 4 : (int)dirNow - 4));

                        break;
                    }
                    Debug.Log(tempDir);
                    i--;
                }
                else if (collideObjTag.Equals("Mirror"))
                {
                    _dir = hitObj.GetComponent<Mirror>().GetMirrorReflectDirection(dirNow); //거울은 방향이 영구적으로 바뀌게 된다.
                    if (_dir == 0)
                    {
                        routeList.Add((Direction)((int)dirNow <= 4 ? (int)dirNow + 4 : (int)dirNow - 4));

                        break;
                    }
                    i--; //아이템은 한칸 이동으로 치지 않으므로
                }
                else if (collideObjTag.Equals("Key"))
                {
                    Debug.Log("key " + gameObject.name);
                }
                else if (collideObjTag.Equals("Door"))
                {
                    routeList.Add((Direction)((int)dirNow <= 4 ? (int)dirNow + 4 : (int)dirNow - 4));
                    break;
                }
                else //타일류(흰색 타일, 깨진 타일, 방향 지정 타일)
                {
                    TileBase tileBase = hitObj.GetComponent<TileBase>();

                    if (tileBase.GetTileType != TileType.NONE && tileBase.GetTileType != TileType.REVERSE) //일반 타일과 반전타일 이외 타일들은 아이템처럼 취급한다.
                    {
                        if (tileBase.GetTileType.Equals(TileType.ROTATE)) //회전하는 타일은 영구적으로 이동방향을 바꾼다.
                        {
                            try
                            {
                                _dir = (Direction)tileBase.GetNextDirection(dirNow, i + 1);
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError("이동 경로 예측 오류\n" + gameObject.name + ", " + e.Message);
                            }
                        }
                        i--; //아이템은 한칸 이동으로 치지 않으므로
                    }
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

        if (holdByPrison)
            return;

        try
        {
            splitDir = (Direction)CheckFirstPosition(lastPos,splitDir); //현재 캐릭터가 위치한 타일로 인해 초기 방향이 변할 수 있다.
        }
        catch (System.Exception e)
        {
            Debug.LogError("이동 궤도 예측에 문제 발생 : " + e.Message);
            return;
        }

        for (int i = 0; i < moveCount+1; i++) //분열 이후 첫 한 칸은 이동횟수에 들어가지 않는다.
        {
            Vector3 dir;

            if (i == 0) //맨 처음은 분열된 이후 대각선 이동
            {
                dir = CommonFunc.GetVectorFromDirection(splitDir);
            }
            else
            {
                if (tempDir != (Direction)0)
                {
                    dir = CommonFunc.GetVectorFromDirection(tempDir);
                    tempDir = 0;
                }
                else
                {
                    dir = CommonFunc.GetVectorFromDirection(_dir);
                }
            }

            List<GameObject> hitObjList = GameManager.instance.GetObjsNextPosition(gameObject.name, lastPos, CommonFunc.GetDirectionFromVector(dir));

            if (hitObjList != null && hitObjList.Count > 0)
            {
                GameObject hitObj;

                if (hitObjList.Count > 1) //장애물이 흰색타일 위에 있기 때문에 2개 이상 충돌이 된다. 이 경우 흰색타일은 무시하도록 한다.
                {
                    hitObj = hitObjList.Find(x => x.gameObject.layer != LayerMask.NameToLayer("Tile")).gameObject;
                }
                else
                {
                    hitObj = hitObjList[0].gameObject;
                }

                string collideObjTag = hitObj.tag; //레이캐스트에 충돌한 오브젝트의 태그

                Direction dirNow;
                if (tempDir == Direction.RETURN)
                {
                    dirNow = (i == 0) ? splitDir : _dir; //첫 이동은 대각선으로 분리
                }
                else //렌즈에 의한 일회성 경로변경일 경우
                {
                    dirNow = tempDir;
                    tempDir = Direction.RETURN;
                }
                lastPos = lastPos + CommonFunc.GetVectorFromDirection(dirNow) * moveValue;

                routeList.Add(dirNow);

                if (collideObjTag.Equals("Wall") || collideObjTag.Equals("Objective") || collideObjTag.Equals("Portal"))
                {
                    break; //이 블럭 이상으로 이동이 불가하다.
                }
                else if (collideObjTag.Equals("Prism"))
                {
                    if (CommonFunc.GetColorCombination(gameObject.name).Count == 1)
                    { //단색인 경우
                        i--;
                        continue;
                    }
                    break; //혼합색일 경우 분열 시도
                }
                else if(collideObjTag.Equals("Filter"))
                {
                    if(hitObj.GetComponent<ColorFilter>().CanCharacterPenetrate(gameObject.name.Split('_')[0]))
                    {
                        i--;
                        continue;
                    }
                    break;
                }
                else if (collideObjTag.Contains("Convex") || collideObjTag.Contains("Concave"))
                {
                    tempDir = hitObj.GetComponent<Lens>().GetConcaveRefractDirection((int)dirNow, ref _dir);
                    if (tempDir == 0)
                    {
                        routeList.Add((Direction)((int)dirNow<=4? (int)dirNow + 4: (int)dirNow - 4));
                        
                        break;
                    }
                    i--; //렌즈와 거울은 한칸 이동으로 치지 않으므로
                }
                else if (collideObjTag.Equals("Mirror"))
                {
                    _dir = hitObj.GetComponent<Mirror>().GetMirrorReflectDirection(dirNow); //거울은 방향이 영구적으로 바뀌게 된다.
                    if (_dir == 0)
                    {
                        routeList.Add((Direction)((int)dirNow <= 4 ? (int)dirNow + 4 : (int)dirNow - 4));

                        break;
                    }
                    i--; //렌즈와 거울은 한칸 이동으로 치지 않으므로
                }
                else if (collideObjTag.Equals("Key"))
                {

                }
                else if (collideObjTag.Equals("Door"))
                {
                    routeList.Add((Direction)((int)dirNow <= 4 ? (int)dirNow + 4 : (int)dirNow - 4));
                    break;
                }
                else //타일류(흰색 타일, 깨진 타일, 방향 지정 타일)
                {
                    TileBase tileBase = hitObj.GetComponent<TileBase>();

                    if (tileBase.GetTileType != TileType.NONE && tileBase.GetTileType != TileType.REVERSE) //일반 타일과 반전타일 이외 타일들은 아이템처럼 취급한다.
                    {
                        Direction nextDirection;

                        try
                        {
                            nextDirection = (Direction)tileBase.GetNextDirection(dirNow, i);
                            routeList.Add(nextDirection); //TileBase로 형변환은 했지만 GetNextDirection()은 원래 오버라이딩한 함수가 실행된다.

                            if (tileBase.GetTileType.Equals(TileType.ROTATE)) //회전하는 타일은 영구적으로 이동방향을 바꾼다.
                            {
                                _dir = nextDirection;
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("이동 궤도 예측에 문제 발생 : " + e.Message);
                        }
                    }
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
    /// 현재 색상 캐릭터가 밟고 있는 타일로 인한 초기 진행방향의 전환 여부를 확인
    /// </summary>
    /// <param name="direction">진행하려고 하는 초기 방향</param>
    /// <returns></returns>
    private Direction? CheckFirstPosition(Vector2 pos, Direction direction)
    {
        List<Collider2D> hit = new List<Collider2D>(Physics2D.OverlapCircleAll(pos, 0.1f,(1 << LayerMask.NameToLayer("Tile")) | (1 << LayerMask.NameToLayer("Obj"))));

        if (hit != null)
        {
            GameObject hitObj;

            if (hit.Count > 1) //장애물이 있는 타일은 처음부터 밟고 있을 수 없다.
            {
                hitObj = hit.Find(x => x.GetComponent<TileBase>() != null).gameObject;
            }
            else
            {
                hitObj = hit[0].gameObject;
            }

            TileType tileType = hitObj.GetComponent<TileBase>().GetTileType;

            if (tileType.Equals(TileType.NONE))
            {
                return direction;
            }
            else if (tileType.Equals(TileType.REVERSE))
            {
                return hitObj.GetComponent<TileBase>().GetNextDirection(direction, 0);
            }
            else if (tileType.Equals(TileType.BREAKABLE))
            {
                return direction;
            }
            else if (tileType.Equals(TileType.ROTATE))
            {
                return hitObj.GetComponent<TileBase>().GetNextDirection(direction, 0);
            }
            else
            {
                return null;
            }
        }
        return null;
    }


    /// <summary>
    /// 벽에 충돌해서 플레이어 사망
    /// </summary>
    public void CharacterDisappear()
    {
        SoundManager.instance.Play("DisAppear");
        movePause = true; //혹시 모를 이동에 대비해서 이동하지 못하게
        moveCount = -1; //위와 동일
        StartCoroutine("DisappearCharacter");
    }
    

    public void EffectDie()
    {
        movePause = true; //혹시 모를 이동에 대비해서 이동하지 못하게
        moveCount = -1; //위와 동일
        SoundManager.instance.Play("Die");
        GameManager.instance.UpdateCharacterActive(gameObject, transform.position, false);
        GameManager.instance.CheckMoveOver(gameObject);
        gameObject.SetActive(false);
        EffectManager.instance.circleEffect(transform.position, gameObject.name);
    }
    private IEnumerator DisappearCharacter()
    {
        float delay = 1.0f;

        Color color = render.color;
        for (float time = 0; time < delay; time += 0.05f * charMoveRatio)
        {
            color.a -= 0.05f* charMoveRatio;
            render.color = color;
            faceRenderer.color = color;

            yield return new WaitForSeconds(0.05f);
        }

        GameManager.instance.UpdateCharacterActive(gameObject, transform.position, false);
        GameManager.instance.CheckMoveOver(gameObject);

        transform.position = new Vector3(-180, 0, transform.position.z);

        gameObject.SetActive(false);
    }

    private IEnumerator ShowMoveGuide()
    {
        while (GameManager.instance==null||!GameManager.instance.CanMove)
        {
            yield return null;
        }
        moveGuide.Show(moveCount);
    }
    private void moveRatioChanged(float value)
    {
        charMoveRatio = value;
    }

    #region 테스트용

    public PlayerMove1(PlayerMove1 script)
    {
        charMoveRatio = script.charMoveRatio;
    }

    #endregion
}
