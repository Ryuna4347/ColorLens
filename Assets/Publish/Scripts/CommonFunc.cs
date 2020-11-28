using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonFunc 
{
    public static void PrintArray<T>(this T[,] Go)
    {
        string result = "";
        for(int i=0; i<Go.GetLength(0); i++)
        {
            result += "[";
            for(int j=0; j<Go.GetLength(1); j++)
            {
                if(Go[i,j] == null)
                {
                    result += "null";
                }
                else
                {
                    result += Go[i,j].ToString().Split(' ')[0];
                }
                if (j != Go.GetLength(1))
                    result += ", ";
            }
            result += "]\n";
        }
        Debug.Log(result);
    }


    public static List<GameObject> GetAllChilds(this GameObject Go)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            list.Add(Go.transform.GetChild(i).gameObject);
        }
        return list;
    }
    /// <summary>
     /// Direction형을 통해 방위 벡터로 변환
     /// </summary>
     /// <param name="_dir"></param>
     /// <returns></returns>
    public static Vector3 GetVectorFromDirection(Direction _dir)
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
    public static Direction GetDirectionFromVector(Vector2 vec)
    {
        Direction dir;

        if (vec.x > 0)
        {
            dir = (Direction)(3 - vec.y);
        }
        else if (vec.x < 0)
        {
            dir = (Direction)(7 + vec.y);
        }
        else
        {
            dir = vec.y > 0 ? (Direction)1 : (Direction)5;
        }
        return dir;
    }
}
