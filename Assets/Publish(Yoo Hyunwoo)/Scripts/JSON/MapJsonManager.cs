using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameObjectData
{
    private string objectName;
    private Vector2 objectPos;
    private float objectRotationZ; //z회전값(회전타일/렌즈/거울 등에 사용)

    public GameObjectData(string objName, Vector2 objPos, float objRotation = 0)
    {
        objectName = objName;
        objectPos = objPos;
        objectRotationZ = objRotation;
    }

    public string ObjectName
    {
        get
        {
            return objectName;
        }
    }
    public Vector2 ObjectPos
    {
        get
        {
            return objectPos == null? new Vector2(0,0) : objectPos;
        }
    }
    public float ObjectRotationZ
    {
        get
        {
            return objectRotationZ;
        }
    }
}

public class MapData {
    private List<List<GameObjectData>> tilemap;
    private List<GameObjectData> objectives;
    private List<GameObjectData> obstacles;
    private List<GameObjectData> characters;


    /// <param name="stringTileMap">맵 전체 타일 리스트</param>
    /// <param name="stringObjMap">맵에 존재하는 색상 상자 리스트(이름_x위치_y위치)</param>
    /// <param name="stringObsMap">맵에 존재하는 아이템 리스트(이름_x위치_y위치_회전각)</param>
    public MapData(string[][] stringTileMap, string[] stringObjMap, string[] stringObsMap)
    {
        ParseMapData(stringTileMap);
        ParseObjectives(stringObjMap);
        ParseObstacles(stringObsMap);
    }

    private void ParseMapData(string[][] stringifyMap)
    {
        int height = stringifyMap.Length;
        int width = stringifyMap[0].Length;

        for(int i=0; i<height; i++)
        {
            tilemap[i]=new List<GameObjectData>();
            for (int j=0; j<width; j++)
            {
                string[] tileData = stringifyMap[i][j].Split('_');

                GameObjectData newTile = new GameObjectData(tileData[0], new Vector3(float.Parse(tileData[1]), float.Parse(tileData[2])), tileData.Length<=3 ? 0 : float.Parse(tileData[3])); //회전 타일은 마지막에 초기 방향이 주어진다.

                tilemap[i].Add(newTile);
            }
        }
    }
    private void ParseObjectives(string[] stringObjMap)
    {
        int count = stringObjMap.Length;

        for (int i = 0; i < count; i++)
        {
            string[] obstacleData = stringObjMap[i].Split('_');
            GameObjectData newTile = new GameObjectData(obstacleData[0], new Vector3(float.Parse(obstacleData[1]), float.Parse(obstacleData[2])));

            objectives.Add(newTile);
        }
    }
    private void ParseObstacles(string[] stringObsMap)
    {
        int count = stringObsMap.Length;

        for (int i = 0; i < count; i++)
        {
            string[] obstacleData = stringObsMap[i].Split('_');

            GameObjectData newTile = new GameObjectData(obstacleData[0], new Vector3(float.Parse(stringObsMap[1]), float.Parse(stringObsMap[2])), float.Parse(obstacleData[3]));

            obstacles.Add(newTile);
        }
    }
    private void ParseCharacters(string[] stringChaMap)
    {
        int count = stringChaMap.Length;

        for (int i = 0; i < count; i++)
        {
            string[] obstacleData = stringChaMap[i].Split('_');

            GameObjectData newTile = new GameObjectData(obstacleData[0], new Vector3(float.Parse(stringChaMap[1]), float.Parse(stringChaMap[2])));

            characters.Add(newTile);
        }
    }
}
[System.Serializable]
public class MapDataJSON
{
    public string[][] mapData;
    public string[] objectiveData;
    public string[] obstacleData;
    public string[] characterData;

    public MapDataJSON(List<List<GameObject>> tileDatas, List<GameObject> objData, List<GameObject> obsData, List<GameObject> chaData)
    {
        mapData = ParseTileToString(tileDatas);
        objectiveData = ParseToString(objData);
        obstacleData = ParseToString(obsData, true);
        obstacleData = ParseToString(chaData);
    }

    private string[][] ParseTileToString(List<List<GameObject>> tileDatas)
    {
        List<string[]> result = new List<string[]>();
        for (int i=0; i<tileDatas.Count; i++)
        {
            List<string> line = new List<string>();
            for (int j = 0; j < tileDatas[0].Count; j++)
            {
                string data = tileDatas[i][j].name.Substring(0, 4) + "_" + tileDatas[i][j].transform.position.x + "_" + tileDatas[i][j].transform.position.y;
                if(tileDatas[i][j].GetComponent<RotateTile>() != null)
                {
                    data += "_" + tileDatas[i][j].GetComponent<RotateTile>().NextDirection.ToString().Substring(0,1);
                }
                line.Add(data);
            }
            result.Add(line.ToArray());
        }
        return result.ToArray();
    }

    private string[] ParseToString(List<GameObject> objectData, bool needRotation = false)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < objectData.Count; i++)
        {
            Transform targetTransform = objectData[i].transform;

            string data = objectData[i].name.Substring(0, 4) + "_" + targetTransform.position.x + "_" + targetTransform.position.y;
            if (needRotation)
                data += "_" + targetTransform.rotation.z.ToString();
            result.Add(data);
        }
        return result.ToArray();
    }
}

public class MapJsonManager : MonoBehaviour
{
    //저장에 필요한 맵 정보
    [SerializeField] private Text stageNumText;
    [SerializeField] private Text heightNumText;
    [SerializeField] private Text widthNumText;
    [SerializeField] private GameObject mapPalleteParent;
    [SerializeField] private GameObject objPalleteParent;
    [SerializeField] private GameObject obsPalleteParent;
    [SerializeField] private GameObject chaPalleteParent;


    private MapData mapData;

    private string jsonDir = "Assets/Resources/GameJSONData/";

    private MapData LoadMapData(string stageNum)
    {
        MapDataJSON tempMapData = JsonUtility.FromJson<MapDataJSON>(Resources.Load<TextAsset>(jsonDir).ToString());
        if(tempMapData == null)
        {
            Debug.Log("맵이 존재하지 않습니다.");
            return null;
        }
        mapData = new MapData(tempMapData.mapData, tempMapData.objectiveData, tempMapData.obstacleData);
        return mapData;
    }
    
    private void SaveMapData(string stageNum,List<List<GameObject>> tileDatas, List<GameObject> objectiveData, List<GameObject> obstacleData, List<GameObject> characterData)
    {
#if UNITY_EDITOR
        MapDataJSON newMapJSON = new MapDataJSON(tileDatas, objectiveData, obstacleData, characterData);

        using (FileStream fs = new FileStream(jsonDir+stageNum+".json", FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(JsonUtility.ToJson(newMapJSON));
                writer.Close();
                writer.Dispose();
            }
            fs.Close();
            fs.Dispose();
        }
#endif
    }

    public void Save()
    {
        int height = int.Parse(heightNumText.text), width = int.Parse(widthNumText.text);

        List<GameObject> tileMap = mapPalleteParent.GetAllChilds();
        tileMap.Sort(SortByPosition);

        List<List<GameObject>> twoDimentionalMap = new List<List<GameObject>>();
        for(int i=0; i<height; i++)
        {
            twoDimentionalMap.Add(new List<GameObject>());
            for(int j = 0; j<width; j++)
            {
                twoDimentionalMap[i].Add(tileMap[i * height + j]);
            }
        }

        List<GameObject> objectiveMap = objPalleteParent.GetAllChilds();
        objectiveMap.Sort(SortByPosition);

        List<GameObject> obstacleMap = obsPalleteParent.GetAllChilds();
        obstacleMap.Sort(SortByPosition);

        List<GameObject> characterMap = chaPalleteParent.GetAllChilds();
        obstacleMap.Sort(SortByPosition);

        SaveMapData(stageNumText.text, twoDimentionalMap, objectiveMap, obstacleMap, characterMap);
    }

    public MapData Load()
    {
        MapData resultMap;

        if((resultMap = LoadMapData(stageNumText.text)) == null)
        {
            return null;
        }
        return resultMap;
    }

    public int SortByPosition(GameObject first, GameObject second)
    {
        if (second.transform.position.y < first.transform.position.y)
        {
            return 1;
        }
        else if (second.transform.position.y > first.transform.position.x)
        {
            return 0;
        }
        else
        {
            return second.transform.position.x < first.transform.position.x ? 1 : 0;
        }
    }
}
