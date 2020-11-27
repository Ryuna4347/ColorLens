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
    public GameObject[,] objectiveArr;
    public GameObject[,] obstacleArr;
    public GameObject[,] characterArr;
    public List<GameObject> unusedCharacters = new List<GameObject>();

    private int level; //현재 레벨
    public List<GameObject> movingChars; //현재 움직이고 있는 캐릭터 수
    public bool canMove=false;
    List<int> baseMoveCount; //각 맵마다 별을 획득할 수 있는 이동횟수 기준(2개)
    public Animator[] StarAnims;//인게임 별이동
    public Animator[] Star_bar_Anims;//인게임 별이동 바
    public Text[] Star_standard_Text;//인게임 별이동 기준 텍스트

    public Text MoveText;//인게임 텍스트UI
    int moveCount; //현재 맵에서 이동횟수
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
        InGame_standard_Text_Set();//별기준 세팅
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
                        obj.GetComponent<PlayerMove1>().CalculateRoute((Direction)dir);
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
        InGameStates();

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

    #region 인게임 별표시
    void InGameStates()
    {
        if (moveCount > baseMoveCount[0])
        {
            StarAnims[3].Play("Star_Delete");//세번째 별
            StarAnims[4].Play("Star_Delete");//세번째 별
            Star_bar_Anims[1].Play("2Star_bar_Delete");
            Invoke("Star1_Appear", 0.4f);
        }
        else if (moveCount > baseMoveCount[1])
        {
            StarAnims[0].Play("Star_Delete");//세번째 별
            StarAnims[1].Play("Star_Delete");//세번째 별
            StarAnims[2].Play("Star_Delete");//세번째 별
            Star_bar_Anims[0].Play("3Star_bar_Delete");
            Invoke("Star2_Appear", 0.4f);
            
        }

        MoveText.text = "이동횟수 : " + moveCount;

    }
    void Star2_Appear()
    {
        StarAnims[3].Play("Star_Appear");
        StarAnims[4].Play("Star_Appear");
        Star_bar_Anims[1].Play("2Star_bar_APpearanim");
    }
    void Star1_Appear()

    {
        StarAnims[5].Play("Star_Appear");
        Star_bar_Anims[2].Play("1Star_bar_APpearanim");

    }
    void InGame_standard_Text_Set()
    {
        Star_standard_Text[0].text = baseMoveCount[1].ToString();
        Star_standard_Text[1].text = baseMoveCount[0].ToString();
    }

    #endregion

    #region 게임관리(타일 배열 관련)
    private void ReadMapData()
    {
        Map mapdata = GameObject.Find("Map").GetComponent<Map>();

        baseMoveCount = mapdata.GetBaseMoveCount();
        level = mapdata.Lev;
        width = mapdata.Width;
        height = mapdata.Height;

        tileArr = new GameObject[height, width];
        obstacleArr = new GameObject[height, width];
        objectiveArr = new GameObject[height, width];
        characterArr = new GameObject[height, width];
        unusedCharacters = new List<GameObject>();

        List<GameObject> tileMapList = GameObject.Find("Tilemap").GetAllChilds();
        tileMapList = tileMapList.OrderBy(c => c.transform.position.y).ThenBy(n => n.transform.position.x).ToList<GameObject>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tileArr[i, j] = tileMapList[i * width + j];
            }
        }

        List<GameObject> obstacleMap = GameObject.Find("Obstacles").GetAllChilds();
        for (int i = 0; i < obstacleMap.Count; i++)
        {
            if (obstacleMap[i].transform.childCount == 2) //렌즈의 경우 2칸을 차지하고 있으므로 별도 처리를 한다.
            {

                Vector2 objPos = obstacleMap[i].transform.GetChild(0).position;
                objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
                objPos.y = objPos.y + (height / 2);
                obstacleArr[(int)objPos.y, (int)(objPos.x)] = obstacleMap[i].transform.GetChild(0).gameObject;

                objPos = obstacleMap[i].transform.GetChild(1).position;
                objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
                objPos.y = objPos.y + (height / 2);
                obstacleArr[(int)objPos.y, (int)(objPos.x)] = obstacleMap[i].transform.GetChild(1).gameObject;
            }
            else
            {
                Vector2 objPos = obstacleMap[i].transform.position;
                objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
                objPos.y = objPos.y + (height / 2);
                obstacleArr[(int)objPos.y, (int)(objPos.x)] = obstacleMap[i];
            }
        }

        List<GameObject> objectiveMap = GameObject.Find("Objectives").GetAllChilds();
        objectiveMap = objectiveMap.OrderBy(c => c.transform.position.y).ThenBy(n => n.transform.position.x).ToList<GameObject>();
        for (int i = 0; i < objectiveMap.Count; i++)
        {
            Vector2 objPos = objectiveMap[i].transform.position;
            objPos.x = Mathf.Ceil(objPos.x) + (width / 2) - 1;
            objPos.y = objPos.y + (height / 2);
            objectiveArr[(int)objPos.y, (int)(objPos.x)] = objectiveMap[i];
        }
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
    /// 충돌한 색상을 체크하고 알맞는 캐릭터를 해당 위치에 소환한다.
    /// </summary>
    /// <param name="colors">충돌한 색상들의 리스트</param>
    public void CheckMerge(List<GameObject> colors)
    {
        GameObject mergeChar = null; //병합되어 생성되는 오브젝트

        if (colors.Count==3) //흰색이 나올 수 밖에 없는 조합(3개가 충돌하는 경우는 빨/초/파 밖에 없기 때문입니다.)
        {
            Vector3 mergePos = colors[0].transform.position;
            List<GameObject> copy = new List<GameObject>(colors);
            foreach (GameObject color in copy)
            {
                color.transform.position = new Vector3(-180, 0, color.transform.position.z);
                CheckMoveOver(color);
                color.SetActive(false);
            }
            mergeChar = characters.Find(x => x.name.Equals("White")); //흰색 캐릭터를 켜고 충돌한 위치에 가져다 둔다.
            mergeChar.SetActive(true);
            mergeChar.transform.position = mergePos;
        }
        else //2가지 색상의 충돌일 경우 하드코딩으로 맞는 색을 찾는다.
        {
            if(colors.Find(x=>x.name.Equals("Red")))
            {
                if(colors.Find(x=>x.name.Equals("Green")))
                {
                    mergeChar= characters.Find(x => x.name.Equals("Yellow"));
                }
                else if (colors.Find(x => x.name.Equals("Blue")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("Magenta"));
                }
                else if (colors.Find(x => x.name.Equals("Cyan")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("White"));
                }
            }
            else if(colors.Find(x => x.name.Equals("Green")))
            {
                if (colors.Find(x => x.name.Equals("Red")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("Yellow"));
                }
                else if (colors.Find(x => x.name.Equals("Blue")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("Cyan"));
                }
                else if (colors.Find(x => x.name.Equals("Magenta")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("White"));
                }
            }
            else if (colors.Find(x => x.name.Equals("Blue")))
            {
                if (colors.Find(x => x.name.Equals("Red")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("Magenta"));
                }
                else if (colors.Find(x => x.name.Equals("Green")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("Cyan"));
                }
                else if (colors.Find(x => x.name.Equals("Yellow")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("White"));
                }
            }
            else if (colors.Find(x => x.name.Equals("Yellow")))
            {
                if (colors.Find(x => x.name.Equals("Green")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("White"));
                }
            }
            else if (colors.Find(x => x.name.Equals("Cyan")))
            {
                if (colors.Find(x => x.name.Equals("Red")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("White"));
                }
            }
            else if (colors.Find(x => x.name.Equals("Magenta")))
            {
                if (colors.Find(x => x.name.Equals("Green")))
                {
                    mergeChar = characters.Find(x => x.name.Equals("White"));
                }
            }
            Vector3 mergePos = colors[0].transform.position; 
            List<GameObject> copy= new List<GameObject>(colors);
            for (int i=0; i<copy.Count; i++)
            {
                copy[i].SetActive(false); 
                CheckMoveOver(copy[i]);
            }
            if (mergeChar != null)
            {
                mergeChar.SetActive(true);
                mergeChar.transform.position = mergePos;
            }
        }
        if(moveRatioChanged != null)
            moveRatioChanged(moveRatio);
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

    public void CheckSplit(GameObject color, Direction _dir)
    {
        List<PlayerMove1> splitedChars = new List<PlayerMove1>(); //분리될 색상들의 리스트
        Vector3 splitPos = color.transform.position;
        string colorName = color.name;

        if(colorName.Equals("Red")|| colorName.Equals("Green")||colorName.Equals("Blue"))
        { //3원색은 불리가 불가능
            return;
        }

        color.transform.position = new Vector3(-180, 0, color.transform.position.z);
        CheckMoveOver(color);
        color.SetActive(false);

        switch (colorName)
        {
            case "Yellow":
                splitedChars.Add(characters.Find(x => x.name.Equals("Red") && x.activeSelf==false).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Green") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            case "Cyan":
                splitedChars.Add(characters.Find(x => x.name.Equals("Green") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Blue") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            case "Magenta":
                splitedChars.Add(characters.Find(x => x.name.Equals("Red") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Blue") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            case "White":
                splitedChars.Add(characters.Find(x => x.name.Equals("Red") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Green") && x.activeSelf == false).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Blue") && x.activeSelf == false).GetComponent<PlayerMove1>());
                break;
            default:
                return;
        }

        for (int i = 0; i<splitedChars.Count; i++)
        {
            splitedChars[i].gameObject.SetActive(true);
            splitedChars[i].transform.position = splitPos;
        }

        moveRatioChanged(moveRatio);
       
        List<Direction> diagonals = GetRefractDirections(_dir);
        if (splitedChars.Count == 3)
        {
            AddMovingCharacter(splitedChars[0].gameObject);
            AddMovingCharacter(splitedChars[1].gameObject);
            AddMovingCharacter(splitedChars[2].gameObject);
            splitedChars[0].CalculateRoute(_dir, diagonals[0]);
            splitedChars[1].CalculateRoute(_dir, _dir);
            splitedChars[2].CalculateRoute(_dir, diagonals[1]);
        }
        else
        {
            AddMovingCharacter(splitedChars[0].gameObject);
            AddMovingCharacter(splitedChars[1].gameObject);
            splitedChars[0].CalculateRoute(_dir, diagonals[0]);
            splitedChars[1].CalculateRoute(_dir, diagonals[1]);
        }
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
    public void CreateWall(Vector3 position, Transform parentTransform)
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
    }
    #endregion


}
