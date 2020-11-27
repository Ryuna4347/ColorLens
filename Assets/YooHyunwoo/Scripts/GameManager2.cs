using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 instance { get; private set; }

    public List<GameObject> characters;

    public GameObject colorBox;//색상박스 부모변수
    [SerializeField] private int colorBox_Child_Count;//색상 박스 수
    [SerializeField] private int arrive_count;//도착한 캐릭터 수
    [SerializeField] private int character_Count;//현재 씬 내에 존재하는 캐릭터 수

    private int level; //현재 레벨
    public int movingChars; //현재 움직이고 있는 캐릭터 수
    public bool canMove;
    List<int> baseMoveCount; //각 맵마다 별을 획득할 수 있는 이동횟수 기준(2개)
    int moveCount; //현재 맵에서 이동횟수
    private bool isGameOver;

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
        Character_CountCheck();
        colorBox = GameObject.Find("ColorBox_Parent");
        colorBox_Child_Count = colorBox.transform.childCount;
        baseMoveCount = GameObject.Find("Map").GetComponent<Map>().GetBaseMoveCount();
        level = GameObject.Find("Map").GetComponent<Map>().Lev;
        moveCount = 0;
        //StartCoroutine("CheckGameOver");
        canMove = true;
    }

    private void Update()
    {
        if (!isGameOver)
        {
            if (canMove)
            {
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

                    List<GameObject> activeObj = characters.FindAll(x => x.activeSelf == true);
                    foreach (GameObject obj in activeObj)
                    {
                    }
                    movingChars = activeObj.Count;
                    //StartCoroutine("CheckCharactersMove");
                    moveCount++;
                }
            }
        }
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
            if (character_Count <= 0)
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
                    //ClearEffect.instance.Clear(star,level,moveCount);
                }
                else
                {
                    GameOverEffect.instance.GameOver();
                }
                isGameOver = true;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 캐릭터 수 확인 함수 
    /// </summary>
    void Character_CountCheck()
    {
        List<GameObject> activeChars = characters.FindAll(x => x.activeSelf == true);
        character_Count = activeChars.Count;
    }

    /// <summary>
    /// 현재 살아 있는 색상 캐릭터의 숫자를 조절(기본적으로 검은칸에 들어가 죽거나 도착칸에서 사라지는 것)
    /// </summary>
    /// <param name="amount"></param>
    public void ControlCharacterCount(int amount=-1)
    {
        if (character_Count > 0)
        {
            character_Count += amount;
        }
    }

    /// <summary>
    /// 목표 상자의 갯수를 조절
    /// </summary>
    /// <param name="amount"></param>
    public void ControlColorBoxCount(int amount=-1)
    {
        if(colorBox_Child_Count>0)
        {
            colorBox_Child_Count += amount;
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
        while(movingChars>0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f); // 모든 캐릭터가 멈춰있어도 일정 시간 다음 이동에 딜레이를 준다.
        canMove = true;
    }

    /// <summary>
    /// 각 캐릭터의 움직임이 끝나면 호출하는 함수(CheckCharactersMove에 사용됨)
    /// </summary>
    public void CheckMoveOver()
    {
        if(movingChars > 0)
            movingChars--;
    }

    /// <summary>
    /// 게임에 사용될 색상 캐릭터들을 미리 로드
    /// </summary>
    private void LoadCharacters()
    {
        
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
                color.SetActive(false);
            }
            movingChars-=2;
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

            movingChars -= 1;
            character_Count -= 1;
            Vector3 mergePos = colors[0].transform.position; 
            List<GameObject> copy= new List<GameObject>(colors);
            for (int i=0; i<copy.Count; i++)
            {
                copy[i].SetActive(false);
            }
            mergeChar.SetActive(true);
            mergeChar.transform.position = mergePos;
            mergeChar.GetComponent<Collider2D>().enabled=true; //합쳐진 색상은 연속으로 합쳐질 수 있다.
        }
    }

    public void CheckSplit(GameObject color, int _dir, int count)
    {
        List<PlayerMove1> splitedChars = new List<PlayerMove1>(); //분리될 색상들의 리스트
        Vector3 splitPos = color.transform.position;
        string colorName = color.name;

        if(colorName.Equals("Red")|| colorName.Equals("Green")||colorName.Equals("Blue"))
        { //3원색은 불리가 불가능
            color.GetComponent<PlayerMove1>().movePause = true;
            ////StartCoroutine(color.GetComponent<PlayerMove1>().Move(_dir, 0, true));
            return;
        }

        color.transform.position = new Vector3(-180, 0, color.transform.position.z);
        color.SetActive(false);

        switch (colorName)
        {
            case "Yellow":
                splitedChars.Add(characters.Find(x => x.name.Equals("Red")).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Green")).GetComponent<PlayerMove1>());
                break;
            case "Cyan":
                splitedChars.Add(characters.Find(x => x.name.Equals("Green")).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Blue")).GetComponent<PlayerMove1>());
                break;
            case "Magenta":
                splitedChars.Add(characters.Find(x => x.name.Equals("Red")).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Blue")).GetComponent<PlayerMove1>());
                break;
            case "White":
                splitedChars.Add(characters.Find(x => x.name.Equals("Red")).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Green")).GetComponent<PlayerMove1>());
                splitedChars.Add(characters.Find(x => x.name.Equals("Blue")).GetComponent<PlayerMove1>());
                break;
            default:
                return;
        }

        for (int i = 0; i<splitedChars.Count; i++)
        {
            splitedChars[i].gameObject.SetActive(true);
            splitedChars[i].transform.position = splitPos;
        }
        List<int> diagonals = GetRefractDirections(_dir);
        if (splitedChars.Count == 3)
        {
            //StartCoroutine(splitedChars[0].Move(diagonals[0], _dir));
            //StartCoroutine(splitedChars[1].Move(_dir, _dir));
            //StartCoroutine(splitedChars[2].Move(diagonals[1], _dir));
            movingChars += 2;
            character_Count += 2;
        }
        else
        {
            //StartCoroutine(splitedChars[0].Move(diagonals[0], _dir));
            //StartCoroutine(splitedChars[1].Move(diagonals[1],  _dir));
            movingChars += 1;
            character_Count += 1;
        }
    }

    /// <summary>
    /// 특정 방향으로 들어오는 벡터가 프리즘에 의해 분열될 경우의 벡터들의 리스트를 구한다.
    /// </summary>
    /// <returns>진행방향을 제외한 좌/우측 대각 벡터의 리스트</returns>
    private List<int> GetRefractDirections(int direction)
    {
        List<int> diagonalVectors=new List<int>();

        switch (direction)
        {
            case 1: //위
                diagonalVectors.Add(8);
                diagonalVectors.Add(2);
                break;
            case 2: //오른쪽위
                diagonalVectors.Add(1);
                diagonalVectors.Add(3);
                break;
            case 3: //오른쪽
                diagonalVectors.Add(2);
                diagonalVectors.Add(4);
                break;
            case 4: //오른쪽아래
                diagonalVectors.Add(3);
                diagonalVectors.Add(5);
                break;
            case 5: //아래
                diagonalVectors.Add(4);
                diagonalVectors.Add(6);
                break;
            case 6: //왼쪽아래
                diagonalVectors.Add(5);
                diagonalVectors.Add(7);
                break;
            case 7: //왼쪽
                diagonalVectors.Add(6);
                diagonalVectors.Add(8);
                break;
            case 8: //왼쪽위
                diagonalVectors.Add(7);
                diagonalVectors.Add(1);
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
            else if(combination.Contains("Blue"))
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
