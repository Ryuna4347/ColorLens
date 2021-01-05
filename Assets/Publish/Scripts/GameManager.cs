using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public static Action<float> moveRatioChanged;
    public static Action moveTurnEnded; //색상캐릭터의 전체 움직임 한번이 끝남에 따라 실행되는 이벤트들

    public List<GameObject> characters;
    public List<GameObject> movableWalls; //기존 맵 이외의 움직일 수 있는 벽(흰색 타일 파괴 메꾸기용)

    public GameObject colorBox;//색상박스 부모변수
    [SerializeField] private int colorBox_Child_Count;//색상 박스 수
    [SerializeField] private int arrive_count;//도착한 캐릭터 수

    private int width, height;
    public GameObject[,] tileArr;
    public GameObject[,] objectArr; //objective+obstacle
    public List<GameObject>[,] characterArr;
    public List<GameObject> unusedCharacters = new List<GameObject>();

    private int level; //현재 레벨
    public List<GameObject> movingChars; //현재 움직이고 있는 캐릭터 수
    public bool canMove=false;
    public List<int> baseMoveCount; //각 맵마다 별을 획득할 수 있는 이동횟수 기준(2개)

    //public Animator[] StarAnims;//인게임 별이동
    //public Animator[] Star_bar_Anims;//인게임 별이동 바
    //public Text[] Star_standard_Text;//인게임 별이동 기준 텍스트

    //public Text MoveText;//인게임 텍스트UI

    public int moveCount; //현재 맵에서 이동횟수
    private bool isGameOver;
    public float moveRatio { get; private set; } //배속

    public GameObject tutorialCanvas;
    public GameObject pausePanel;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        colorBox_Child_Count = colorBox.transform.childCount;
        moveCount = 0;
        moveRatio = 1.0f;
        ReadMapData();
        LoadCharacters();
        if (moveRatioChanged != null)
        {
            moveRatioChanged(moveRatio);
        }

        StartCoroutine("CheckGameOver"); //캐릭터 로드가 완료된 이후 게임 종료여부를 체크한다.

        if (!SceneManager.GetActiveScene().name.Equals("1-1")) //현재 1-1스테이지만 컷신이 존재하므로 컷신이 CheckTutorial을 호출한다.
        {
            CheckTutorial();
        }
    }

    public void MoveBtn(int _dir)
    {
    //    if (!isGameOver)
    //    {
    //        if (canMove)
    //        {
    //            int dir = _dir;
    //            canMove = false;

    //            List<GameObject> activeObj = characters.FindAll(x => x.activeSelf == true);
    //            foreach (GameObject obj in activeObj)
    //            {
    //                StartCoroutine(obj.GetComponent<PlayerMove1>().Move(dir));
    //            }
    //            movingChars = activeObj.Count;
    //            StartCoroutine("CheckCharactersMove");
    //            moveCount++;
    //        }
    //    }
    }
    private void Update()
    {
        if (!isGameOver)
        {
            if (canMove)
            {
                if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && (tutorialCanvas == null || tutorialCanvas.activeSelf==false))
                {
                    CheckPause();
                }

                int dir = 0;
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                    dir = 7;
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    dir = 3;
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    dir = 5;
                else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    dir = 1;
                if (dir != 0)
                {
                    canMove = false;

                    List<GameObject> activeChars = characters.FindAll(x => x.activeSelf == true);
                    foreach (GameObject obj in activeChars)
                    {
                        obj.GetComponent<PlayerMove1>().CalculateRoute((Direction)dir); //여기 확인(테스트 씬)
                        AddMovingCharacter(obj);
                    }
                    StartCoroutine("CheckCharactersMove");
                    moveCount++;
                }
            }
        }
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.R) && pausePanel.activeSelf == false) //R키 누르면 현재 씬 재시작
            {
                SoundManager.instance.Play("Restart", 1, 1);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }

    }

    public void CheckTutorial()
    {
        if(tutorialCanvas == null) //튜토리얼이 없는 스테이지는 그냥 바로 이동가능
        {
            canMove = true;
            return;
        }
        else if (PlayerPrefs.GetInt("Tut_" + level,0) > 0)
        {
            canMove = true;
            return;
        }
        else
        {
            canMove = false;
            tutorialCanvas.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        PlayerPrefs.SetInt("Tut_" + level, 1);
        SoundManager.instance.Play("Back");
        tutorialCanvas.SetActive(false);
        canMove = true;
    }

    public void CheckPause()
    {
        if (Time.timeScale == 0) //일시정지상태일때 
        {
            pausePanel.SetActive(false); //일시정지 품   
            Time.timeScale = 1;
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    #region 게임관리(타일 배열 관련)
    private void ReadMapData()
    {
        int i, j;

        Map mapdata = GameObject.Find("Map").GetComponent<Map>();

        baseMoveCount = mapdata.GetBaseMoveCount();
        level = mapdata.Lev;
        width = mapdata.Width;
        height = mapdata.Height;

        tileArr = new GameObject[height, width];
        objectArr = new GameObject[height, width];
        characterArr = new List<GameObject>[height, width];
        unusedCharacters = new List<GameObject>();

        List<GameObject> tileMapList = GameObject.Find("Tilemap").GetAllChilds();
        tileMapList = tileMapList.OrderBy(c => c.transform.position.y).ThenBy(n => n.transform.position.x).ToList<GameObject>();
        for (i = 0; i < height; i++)
        {
            for (j = 0; j < width; j++)
            {
                tileArr[i, j] = tileMapList[i * width + j];
            }
        }

        List<GameObject> obstacleMap = GameObject.Find("Obstacles").GetAllChilds();
        for (i = 0; i < obstacleMap.Count; i++)
        {
            if (obstacleMap[i].transform.childCount == 2) //렌즈의 경우 2칸을 차지하고 있으므로 별도 처리를 한다.
            {
                Vector2 objPos = obstacleMap[i].transform.GetChild(0).position;
                objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
                objPos.y = objPos.y + (height / 2);
                objectArr[(int)objPos.y, (int)(objPos.x)] = obstacleMap[i].transform.GetChild(0).gameObject;

                objPos = obstacleMap[i].transform.GetChild(1).position;
                objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
                objPos.y = objPos.y + (height / 2);
                objectArr[(int)objPos.y, (int)(objPos.x)] = obstacleMap[i].transform.GetChild(1).gameObject;
            }
            else
            {
                Vector2 objPos = obstacleMap[i].transform.position;
                objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
                objPos.y = objPos.y + (height / 2);
                objectArr[(int)objPos.y, (int)(objPos.x)] = obstacleMap[i];
            }
        }

        List<GameObject> objectiveMap = GameObject.Find("Objectives").GetAllChilds();
        objectiveMap = objectiveMap.OrderBy(c => c.transform.position.y).ThenBy(n => n.transform.position.x).ToList<GameObject>();
        for (i = 0; i < objectiveMap.Count; i++)
        {
            Vector2 objPos = objectiveMap[i].transform.position;
            objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
            objPos.y = objPos.y + (height / 2);
            objectArr[(int)objPos.y, (int)(objPos.x)] = objectiveMap[i];
        }

        for(i=0; i<height; i++)
        {
            for(j=0; j<width; j++)
            {
                characterArr[i, j] = new List<GameObject>();
            }
        }

        List<GameObject> characterMap = GameObject.Find("Characters").GetAllChilds();
        for (i = 0; i < characterMap.Count; i++)
        {
            characterMap[i].name = characterMap[i].name.Split(' ')[0]+ "_" + characterMap[i].transform.GetSiblingIndex();
            if (characterMap[i].transform.position.x < - width / 2)
            {
                unusedCharacters.Add(characterMap[i]);
            }
            else
            {
                Vector2 objPos = characterMap[i].transform.position;
                objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
                objPos.y = objPos.y + (height / 2);
                characterArr[(int)objPos.y, (int)(objPos.x)].Add(characterMap[i]);
            }
        }
    }

    /// <summary>
    /// 다음 이동할 위치에 충돌할 물체가 있는지 체크
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public List<GameObject> GetObjsNextPosition(string characterName, Vector2 characterPos, Direction dir)
    {
        List<GameObject> result = new List<GameObject>();

        int[] arrayIndex = ConvertPosToTwoDimentionIdx(characterPos);
        if (!CheckCharacterAlive(characterName)) //게임 정보가 일치하지 않음.
        {
            Debug.LogError("게임 정보가 일치하지 않습니다.(잘못된 캐릭터)");
            return null;
        }
        Vector2 nextDirection = CommonFunc.GetVectorFromDirection(dir);
        int h = (int)(arrayIndex[1]+nextDirection.y), w = (int)(arrayIndex[0]+nextDirection.x);

        result.Add(tileArr[h, w]); //타일맵은 기본으로 존재한다.

        if(objectArr[h,w] != null) //색 상자 or 아이템이 존재하는 경우 추가
        {
            result.Add(objectArr[h, w]);
        }

        return result;
    }

    /// <summary>
    /// 오브젝트(상자+아이템)의 activeSelf 상황을 갱신
    /// </summary>
    /// <param name="position"></param>
    /// <param name="active"></param>
    /// <returns></returns>
    public bool UpdateObjectActive(GameObject objectItem, Vector2 position, bool active = false)
    {
        int[] arrIdx = ConvertPosToTwoDimentionIdx(position);

        if (active)
        {
            objectArr[arrIdx[1], arrIdx[0]] = objectItem;
        }
        else
        {
#if UNITY_EDITOR
            if (objectArr[arrIdx[1], arrIdx[0]] == null)
                Debug.LogWarning("아이템/색 상자가 없는데 삭제가 요청됨. 인덱스 : " + arrIdx);
#endif
            if (objectArr[arrIdx[1], arrIdx[0]] != objectItem)
                return false;

            if (!objectArr[arrIdx[1], arrIdx[0]].CompareTag("Objective"))
            {
                GameObject newWall = CreateWall(position, GameObject.Find("Tilemap").transform);
                tileArr[arrIdx[1], arrIdx[0]] = newWall;
            }
            objectArr[arrIdx[1], arrIdx[0]] = null;
        }
        return true;
    }

    public bool UpdateCharacterActive(GameObject character, Vector2 position, bool active)
    {
        int[] arrIdx = ConvertPosToTwoDimentionIdx(position);

        if (active)
            characterArr[arrIdx[1], arrIdx[0]].Add(character);
        else
        {
            if (!characterArr[arrIdx[1], arrIdx[0]].Contains(character))
                return false;
            characterArr[arrIdx[1], arrIdx[0]].Remove(character);
            unusedCharacters.Add(character);
        }
        return true;
    }

    public bool UpdateCharacterPos(GameObject character, Vector2 oldPosition, Vector2 newPosition)
    {
        int[] oldIdx, newIdx;

        oldIdx = ConvertPosToTwoDimentionIdx(oldPosition);
        newIdx = ConvertPosToTwoDimentionIdx(newPosition);

        if (!characterArr[oldIdx[1], oldIdx[0]].Contains(character))
            return false;
        characterArr[oldIdx[1], oldIdx[0]].Remove(character);
        characterArr[newIdx[1], newIdx[0]].Add(character);
        return true;
    }
    

    /// <summary>
    /// characterArr에서 캐릭터가 위치하고 있는 인덱스를 반환한다.
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public Vector2 GetArrayIndexOfCharacter(string characterName)
    {
        for(int i=0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                if (characterArr[i, j].Count < 1) continue;
                if(characterArr[i,j].Find(x=>x.name.Equals(characterName)))
                {
                    return new Vector2(i,j);
                }
            }
        }
        return new Vector2(-100,-100);
    }

    private int[] ConvertPosToTwoDimentionIdx(Vector2 pos)
    {
        int[] index = new int[2];

        index[0] = (int)(Mathf.Ceil(pos.x) + (width / 2) - 1);
        index[1] = (int)(pos.y + (height / 2));

        return index;
    }

    private bool CheckCharacterAlive(string characterName)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (characterArr[i, j].Count < 1) continue;
                if (characterArr[i, j].Find(x => x.name.Equals(characterName)))
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region 게임관리(성공/실패)

    /// <summary>
    ///캐릭터 생존 확인 함수
    /// </summary>
    /// <returns></returns> 캐릭터가 하나라도 살아있으면 true반환
    private IEnumerator CheckGameOver()
    {
        while (!isGameOver)
        {
            if (colorBox_Child_Count == arrive_count)
            { //게임 클리어
                int star = 1;
                if (moveCount <= baseMoveCount[1])
                {
                    star = 3;
                }
                else if (moveCount <= baseMoveCount[0])
                {
                    star = 2;
                }
                ClearEffect.instance.Clear(star, moveCount);
                isGameOver = true;
            }
            else
            {
                List<GameObject> aliveChars = characters.FindAll(x => x.activeSelf == true);

                if (aliveChars.Count == 0)
                {
                    GameOverEffect.instance.GameOver();
                    isGameOver = true;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 목표 상자의 갯수를 조절
    /// </summary>
    /// <param name="amount"></param>
    public void ControlColorBoxCount(int amount=1)
    {
        if(arrive_count<colorBox_Child_Count)
        {
            arrive_count += amount;
        }
    }

    #endregion

    #region 게임 로직관련(한턴의 움직임)
    /// <summary>
    /// 모든 캐릭터의 움직임이 종료되었는지 확인한다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckCharactersMove()
    {
        while(movingChars.Count>0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f/moveRatio); // 모든 캐릭터가 멈춰있어도 일정 시간 다음 이동에 딜레이를 준다.
        if (moveTurnEnded != null) //한 턴 종료에 대응되는 이벤트가 있을 시 실행
            moveTurnEnded();
        CheckMerge();

        //CheckCharacterPosition();
        //CheckObjectPosition();
        canMove = true;
    }

    /// <summary>
    /// 각 캐릭터의 움직임이 끝나면 호출하는 함수(CheckCharactersMove에 사용됨)
    /// </summary>
    public void CheckMoveOver(GameObject character)
    {
        if(movingChars.Count > 0&&movingChars.Contains(character))
        {
            movingChars.Remove(character);
        }
    }
    public void AddMovingCharacter(GameObject character)
    {
        if (!movingChars.Contains(character))
        {
            movingChars.Add(character);
        }
        else
        {
            Debug.Log("중복 추가 시도");
        }
    }

    /// <summary>
    /// 게임에 사용될 색상 캐릭터들을 미리 로드
    /// </summary>
    private void LoadCharacters()
    {
        characters = new List<GameObject>();

        Transform characterParent = GameObject.Find("Characters").transform;

        foreach(Transform character in characterParent)
        {
            characters.Add(character.gameObject);
        }
    }

    public void ChangeMoveRatio(float value)
    {
        moveRatio = value;
        if(moveRatioChanged !=null)
        {
            moveRatioChanged(moveRatio);
        }
    }

    /// <summary>
    /// 현재 캐릭터들의 위치를 확인해서 합성가능한 캐릭터들을 합성시킨다.
    /// </summary>
    private void CheckMerge()
    {
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if (characterArr[i, j].Count > 1)
                    CharacterMerge(characterArr[i, j]);
            }
        }
    }

    /// <summary>
    /// 색상 캐릭터를 합성하여 해당 위치에 소환한다.
    /// </summary>
    /// <param name="colors">충돌한 색상들의 리스트</param>
    /// <returns>합병 성공 여부</returns>
    public bool CharacterMerge(List<GameObject> colors)
    {
        GameObject mergeChar = null; //병합되어 생성되는 오브젝트
        
        if (colors.Count==3) //흰색이 나올 수 밖에 없는 조합(3개가 충돌하는 경우는 빨/초/파 밖에 없기 때문입니다.)
        {
            Vector3 mergePos = colors[0].transform.position;
            List<GameObject> copy = new List<GameObject>(colors);
            foreach (GameObject color in copy)
            {
                color.GetComponent<PlayerMove1>().CharacterDisappear();
            }
            mergeChar = unusedCharacters.Find(x => x.name.Contains("White")); //흰색 캐릭터를 켜고 충돌한 위치에 가져다 둔다.
            unusedCharacters.Remove(mergeChar);
            mergeChar.SetActive(true);
            mergeChar.transform.position = mergePos;
            UpdateCharacterActive(mergeChar, mergePos, true);
        }
        else //2가지 색상의 충돌일 경우 하드코딩으로 맞는 색을 찾는다.
        {
            if(colors.Find(x=>x.name.Contains("Red")))
            {
                if(colors.Find(x=>x.name.Contains("Green")))
                {
                    mergeChar= unusedCharacters.Find(x => x.name.Contains("Yellow"));
                }
                else if (colors.Find(x => x.name.Contains("Blue")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("Magenta"));
                }
                else if (colors.Find(x => x.name.Contains("Cyan")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("White"));
                }
            }
            else if(colors.Find(x => x.name.Contains("Green")))
            {
                if (colors.Find(x => x.name.Contains("Red")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("Yellow"));
                }
                else if (colors.Find(x => x.name.Contains("Blue")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("Cyan"));
                }
                else if (colors.Find(x => x.name.Contains("Magenta")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("White"));
                }
            }
            else if (colors.Find(x => x.name.Contains("Blue")))
            {
                if (colors.Find(x => x.name.Contains("Red")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("Magenta"));
                }
                else if (colors.Find(x => x.name.Contains("Green")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("Cyan"));
                }
                else if (colors.Find(x => x.name.Contains("Yellow")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("White"));
                }
            }
            else if (colors.Find(x => x.name.Contains("Yellow")))
            {
                if (colors.Find(x => x.name.Contains("Green")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("White"));
                }
            }
            else if (colors.Find(x => x.name.Contains("Cyan")))
            {
                if (colors.Find(x => x.name.Contains("Red")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("White"));
                }
            }
            else if (colors.Find(x => x.name.Contains("Magenta")))
            {
                if (colors.Find(x => x.name.Contains("Green")))
                {
                    mergeChar = unusedCharacters.Find(x => x.name.Contains("White"));
                }
            }
            Vector3 mergePos = colors[0].transform.position; 
            List<GameObject> copy= new List<GameObject>(colors);
            for (int i=0; i<copy.Count; i++)
            {
                copy[i].GetComponent<PlayerMove1>().CharacterDisappear();
            }
            if (mergeChar != null)
            {
                mergeChar.SetActive(true);
                mergeChar.transform.position = mergePos;
                UpdateCharacterActive(mergeChar, mergePos, true);
            }

        }
        if(moveRatioChanged != null)
            moveRatioChanged(moveRatio);

        return true;
    }
    
    public bool IsCharacterMoving(GameObject character)
    {
        if (movingChars.Contains(character))
        {
            return true;
        }
        else
            return false;
    }

    public bool CharacterSplit(PlayerMove1 character)
    {
        List<PlayerMove1> splitedChars = new List<PlayerMove1>(); //분리될 색상들의 리스트
        Direction dir;

        Vector3 splitPos = character.transform.position;
        string colorName = character.name.Split('_')[0];

        if(colorName.Equals("Red")|| colorName.Equals("Green")||colorName.Equals("Blue"))
        { //3원색은 불리가 불가능
            return false;
        }
        dir = character.routeList[character.routeList.Count - 1];
        character.CharacterDisappear();

        switch (colorName)
        {
            case "Yellow":
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Red") && x.activeSelf==false).GetComponent<PlayerMove1>());
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Green") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            case "Cyan":
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Green") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Blue") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            case "Magenta":
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Red") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Blue") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            case "White":
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Red") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Green") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(unusedCharacters.Find(x => x.name.Contains("Blue") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            default:
#if UNITY_EDITOR
                Debug.LogError("잘못된 분리 요청. 분리 색상 : " + colorName);
#endif
                break;
        }

        for (int i = 0; i<splitedChars.Count; i++)
        {
            splitedChars[i].gameObject.SetActive(true);
            splitedChars[i].transform.position = splitPos;
            UpdateCharacterActive(splitedChars[i].gameObject, splitedChars[i].transform.position, true);
        }

        moveRatioChanged(moveRatio);
       
        List<Direction> diagonals = GetRefractDirections(dir);
        if (splitedChars.Count == 3)
        {
            AddMovingCharacter(splitedChars[0].gameObject);
            AddMovingCharacter(splitedChars[1].gameObject);
            AddMovingCharacter(splitedChars[2].gameObject);
            unusedCharacters.Remove(splitedChars[0].gameObject);
            unusedCharacters.Remove(splitedChars[1].gameObject);
            unusedCharacters.Remove(splitedChars[2].gameObject);
            splitedChars[0].CalculateRoute(dir, diagonals[0]);
            splitedChars[1].CalculateRoute(dir, dir);
            splitedChars[2].CalculateRoute(dir, diagonals[1]);
        }
        else
        {
            AddMovingCharacter(splitedChars[0].gameObject);
            AddMovingCharacter(splitedChars[1].gameObject);
            unusedCharacters.Remove(splitedChars[0].gameObject);
            unusedCharacters.Remove(splitedChars[1].gameObject);
            splitedChars[0].CalculateRoute(dir, diagonals[0]);
            splitedChars[1].CalculateRoute(dir, diagonals[1]);
        }
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="character"></param>
    /// <param name="position"></param>
    /// <returns>이후에도 캐릭터가 유지되면 true, 사라지면 false</returns>
    public bool CheckCollisionWithObjects(PlayerMove1 character, Vector3 position)
    {
        int[] arrIdx = ConvertPosToTwoDimentionIdx(position);
        GameObject selectedObj;

        if ((selectedObj = tileArr[arrIdx[1], arrIdx[0]]) != null)
        {
            if (selectedObj.name.Contains("Wall"))
            {
                character.EffectDie();
            }
        }
        if ((selectedObj = objectArr[arrIdx[1], arrIdx[0]]) != null)
        {
            if(selectedObj.name.Contains("Objective"))
            {
                Objective colorbox = selectedObj.GetComponent<Objective>();
                if (colorbox.CheckColor(character.name.Split('_')[0]))
                {
                    colorbox.EraseColor(character.name.Split('_')[0]);
                    character.CharacterDisappear();
                }
                else
                    character.EffectDie();
                return false;
            }
            else if(selectedObj.name.Contains("Portal"))
            {
                selectedObj.GetComponent<Portal>().TeleportCharacter(character);
                return true;
            }
            else if(selectedObj.name.Contains("Prism"))
            {
                return !CharacterSplit(character);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("예상과 다른 아이템과의 충돌 발생. 해당 오브젝트 : " + character.name + " " + character.transform.position);
#endif
                return true;
            }
        }
        
        return true;
    }

    /// <summary>
    /// 특정 방향으로 들어오는 벡터가 프리즘에 의해 분열될 경우의 벡터들의 리스트를 구한다.
    /// </summary>
    /// <returns>진행방향을 제외한 좌/우측 대각 방향의 리스트</returns>
    private List<Direction> GetRefractDirections(Direction direction)
    {
        List<Direction> diagonalVectors=new List<Direction>();

        switch (direction)
        {
            case (Direction)1: //위
                diagonalVectors.Add((Direction)8);
                diagonalVectors.Add((Direction)2);
                break;
            case (Direction)2: //오른쪽위
                diagonalVectors.Add((Direction)3);
                diagonalVectors.Add((Direction)3);
                break;
            case (Direction)3: //오른쪽
                diagonalVectors.Add((Direction)2);
                diagonalVectors.Add((Direction)4);
                break;
            case (Direction)4: //오른쪽아래
                diagonalVectors.Add((Direction)3);
                diagonalVectors.Add((Direction)3);
                break;
            case (Direction)5: //아래
                diagonalVectors.Add((Direction)4);
                diagonalVectors.Add((Direction)6);
                break;
            case (Direction)6: //왼쪽아래
                diagonalVectors.Add((Direction)7);
                diagonalVectors.Add((Direction)7);
                break;
            case (Direction)7: //왼쪽
                diagonalVectors.Add((Direction)6);
                diagonalVectors.Add((Direction)8);
                break;
            case (Direction)8: //왼쪽위
                diagonalVectors.Add((Direction)7);
                diagonalVectors.Add((Direction)7);
                break;
        }
        return diagonalVectors;
    }
    #endregion

    #region 색상관련
    public List<string> GetColorCombination(string color)
    {
        List<string> returnList=new List<string>();

        switch(color)
        {
            case "Red":
                returnList.Add("Red");
                break;
            case "Green":
                returnList.Add("Green");
                break;
            case "Blue":
                returnList.Add("Blue");
                break;
            case "Yellow":
                returnList.Add("Red");
                returnList.Add("Green");
                break;
            case "Cyan":
                returnList.Add("Green");
                returnList.Add("Blue");
                break;
            case "Magenta":
                returnList.Add("Red");
                returnList.Add("Blue");
                break;
            case "White":
                returnList.Add("Red");
                returnList.Add("Green");
                returnList.Add("Blue");
                break;
        }

        return returnList;
    }

    /// <summary>
    /// 3원색 색상 리스트를 통해 현재 색상을 반환한다.
    /// </summary>
    /// <returns></returns>
    public string GetColorName(List<string> combination)
    {
        string colorName = "";

        if(combination.Count==3)
        {
            return "White";
        }
        else
        {
            if(combination.Contains("Red"))
            {
                if(colorName.Equals(""))
                {
                    colorName = "Red";
                }
            }
            if (combination.Contains("Green"))
            {
                if (colorName.Equals(""))
                {
                    colorName = "Green";
                }
                else if(colorName.Equals("Red"))
                {
                    colorName = "Yellow";
                }
            }
            if(combination.Contains("Blue"))
            {
                if (colorName.Equals(""))
                {
                    colorName = "Blue";
                }
                else if (colorName.Equals("Red"))
                {
                    colorName = "Magenta";
                }
                else if (colorName.Equals("Green"))
                {
                    colorName = "Cyan";
                }
            }
            return colorName;
        }
    }
    #endregion

    #region 타일 관련(TileBase에 속한 오브젝트)

    /// <summary>
    /// 벽 오브젝트를 생성
    /// </summary>
    /// <param name="position">오브젝트의 로컬 포지션</param>
    /// <param name="parentTransform">오브젝트의 부모 오브젝트</param>
    public GameObject CreateWall(Vector3 position, Transform parentTransform)
    {
        GameObject instWall = movableWalls.Find(x => x.gameObject.activeSelf == false);
        if(!instWall)
        {
            instWall = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Objs/Wall"), parentTransform);
            movableWalls.Add(instWall);
        }
        position.y = position.y - 0.5f;
        position.z = (-3.5f + position.y) * 0.01f;
        instWall.transform.localPosition = position;
        return instWall;
    }
    #endregion


    #region TEST

    private void CheckCharacterPosition()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Test"))
        {
            Destroy(obj);
        }

        for(int i = 0; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                if(characterArr[i,j].Count >= 1)
                {
                    foreach (GameObject obj in characterArr[i, j])
                    {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.tag = "Test";
                        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        sphere.transform.position = new Vector3(j - (width / 2) + 0.5f, i - height / 2, -2f);
                    }
                }
            }
        }
    }
    private void CheckObjectPosition()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Test_Obj"))
        {
            Destroy(obj);
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (objectArr[i, j] != null)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.GetComponent<MeshRenderer>().material.color = Color.red;
                    sphere.tag = "Test_Obj";
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    sphere.transform.position = new Vector3(j - (width / 2) + 0.5f, i - height / 2, -2f);

                }
            }
        }
    }
    #endregion

    #region 테스트 씬 전용

#if UNITY_EDITOR
    public UtilForMapTest utilTestManager;

    private bool RefreshTilesPosition(GameObject[,] tiles)
    {
        List<GameObject> tileMapList = GameObject.Find("Tilemap").GetAllChilds();
        foreach(GameObject tile in tileMapList)
        {
            tile.SetActive(false);
        }

        tileArr = tiles;
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j<width; j++)
            {
                tileArr[i,j].SetActive(true);
                tileArr[i,j].transform.position = new Vector2((j - width/2 + 1) - 0.5f, i - height / 2);
            }
        }
        return true;
    }
    private bool RefreshObjectsPosition(GameObject[,] objects)
    {
        return true;
    }
    private bool RefreshCharactersPosition()
    {
        return true;
    }


    /// <summary>
    /// 이전 턴의 맵 상태를 불러온다.
    /// </summary>
    public void LoadPrevMap()
    {
        if (utilTestManager == null)
            return;
        MapInfoPerTurn prevTurn = utilTestManager.LoadPrevMapInfo();

        //if (RefreshTilesPosition())
        //{

        //}
    }

    /// <summary>
    /// 다음 턴의 맵 상태를 불러온다.
    /// </summary>
    public void LoadNextMap()
    {
        if (utilTestManager == null)
            return;
        MapInfoPerTurn nextTurn = utilTestManager.LoadNextMapInfo();
    }

#endif
    #endregion
}
