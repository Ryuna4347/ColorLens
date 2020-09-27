using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public static Action<float> moveRatioChanged;

    public List<GameObject> characters;

    public GameObject colorBox;//색상박스 부모변수
    [SerializeField] private int colorBox_Child_Count;//색상 박스 수
    [SerializeField] private int arrive_count;//도착한 캐릭터 수

    private int level; //현재 레벨
    public List<GameObject> movingChars; //현재 움직이고 있는 캐릭터 수
    public bool canMove=false;
    List<int> baseMoveCount; //각 맵마다 별을 획득할 수 있는 이동횟수 기준(2개)
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
        baseMoveCount = GameObject.Find("Map").GetComponent<Map>().GetBaseMoveCount();
        level = GameObject.Find("Map").GetComponent<Map>().GetLevel();
        moveCount = 0;
        moveRatio = 1.0f;
        LoadCharacters();
        if (moveRatioChanged != null)
        {
            moveRatioChanged(moveRatio);
        }

        StartCoroutine("CheckGameOver"); //캐릭터 로드가 완료된 이후 게임 종료여부를 체크한다.
        CheckTutorial();
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
                if (Input.GetKeyDown(KeyCode.R) && pausePanel.activeSelf==false) //R키 누르면 현재 씬 재시작
                {
                    SoundManager.instance.Play("Restart", 1, 1);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }

                if ((tutorialCanvas != null && pausePanel.activeSelf == false) && Input.GetKeyDown(KeyCode.H))
                {
                    canMove = false;
                    tutorialCanvas.SetActive(true);
                    return;
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
    }

    private void CheckTutorial()
    {
        if(tutorialCanvas == null) //튜토리얼이 없는 스테이지는 그냥 바로 이동가능
        {
            canMove = true;
        }
        else if (PlayerPrefs.GetInt("Tutorial" + level)>0)
        {
            tutorialCanvas.SetActive(false);
            canMove = true;
        }
    }

    public void CloseTutorial()
    {
        PlayerPrefs.SetInt("Tutorial" + level, 1);
        tutorialCanvas.SetActive(false);
        canMove = true;
    }

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
                ClearEffect.instance.Clear(star, level, moveCount);
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
            mergeChar.SetActive(true);
            mergeChar.transform.position = mergePos;
        }
        moveRatioChanged(moveRatio);
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
}
