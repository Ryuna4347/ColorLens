using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepCopyTransform
{
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
}

public class DeepCopyObject<T>
{
    private DeepCopyTransform objTransform;
    private T objScript;

    public DeepCopyObject(Transform originTransform, T originScript)
    {
        objTransform.Copy(originTransform);
        objScript.Copy<T>(originScript);
    }
}

public class MapInfoPerTurn
{
    public DeepCopyObject<TileBase>[,] tileArr;
    public DeepCopyObject<ObjectBase>[,] objectArr; //objective+obstacle
    public DeepCopyObject<PlayerMove1>[,] aliveCharacterArr;

    public MapInfoPerTurn(GameObject[,] tiles, GameObject[,] objects, List<GameObject>[,] aliveCharacters)
    {
        tileArr = CopyArray<TileBase>(tiles);
        objectArr = CopyArray<ObjectBase>(objects);
        
        //aliveCharacterList = aliveCharacters;
    }

    private DeepCopyObject<T>[,] CopyArray<T>(GameObject[,] originArr)
    {
        DeepCopyObject<T>[,] copyArr = new DeepCopyObject<T>[originArr.GetLength(0), originArr.GetLength(1)];

        for(int i=0; i<originArr.GetLength(0); i++)
        {
            for(int j = 0; j<originArr.GetLength(1); j++)
            {
                copyArr[i,j] = new DeepCopyObject<T>(originArr[i, j].transform, originArr[i, j].GetComponent<T>());
            }
        }
        return copyArr;
    }

    private List<DeepCopyObject<T>>[,] CopyArray<T>(List<GameObject>[,] originArr)
    {
        List<DeepCopyObject<T>>[,] copyArr = new List<DeepCopyObject<T>>[originArr.GetLength(0), originArr.GetLength(1)];

        for (int i = 0; i < originArr.GetLength(0); i++)
        {
            for (int j = 0; j < originArr.GetLength(1); j++)
            {
                if(originArr[i,j].Count == 0)
                {
                    copyArr[i, j] = null;
                    continue;
                }
                foreach(GameObject item in originArr[i,j])
                {
                    DeepCopyObject<T> copyItem = new DeepCopyObject<T>(item.transform, item.GetComponent<T>());
                    copyArr[i, j].Add(copyItem);
                }
            }
        }
        return copyArr;
    }
}

public class UtilForMapTest : MonoBehaviour
{
    private List<MapInfoPerTurn> mapInfoList;
    private int nowTurn; //되감기 이후 오브젝트를 움직일 경우 겹치는 부분을 제거하기 위함

    // Start is called before the first frame update
    void Awake()
    {
        if (SceneManager.GetActiveScene().name.NotEqual("MapTest"))
            Destroy(gameObject);
        GameManager.moveTurnEnded += SaveMapInfo;
        nowTurn = 0;
    }

    private void OnDestroy()
    {
        GameManager.moveTurnEnded -= SaveMapInfo;
    }

    private void SaveMapInfo()
    {
        MapInfoPerTurn newMapInfo = new MapInfoPerTurn(GameManager.instance.tileArr, GameManager.instance.objectArr, GameManager.instance.characterArr);
        if(nowTurn < mapInfoList.Count - 1)
        {
            mapInfoList.RemoveRange(nowTurn + 1, mapInfoList.Count - nowTurn - 1);
        }
        mapInfoList.Add(newMapInfo);
    }

    public MapInfoPerTurn LoadPrevMapInfo()
    {
        if(nowTurn == 0)
        {
            return null;
        }
        nowTurn--;
        return mapInfoList[nowTurn];
    }

    public MapInfoPerTurn LoadNextMapInfo()
    {
        if(nowTurn == mapInfoList.Count - 1)
        {
            return null;
        }
        nowTurn++;
        return mapInfoList[nowTurn];
    }
}
