using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    public static void Copy(this DeepCopyTransform copy, Transform origin)
    {
        copy.localPosition = origin.localPosition;
        copy.localRotation = origin.localRotation;
        copy.localScale = origin.localScale;
    }

    public static void Copy<T>(this T copy, T origin)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, origin);
            stream.Position = 0;
            copy = (T)formatter.Deserialize(stream);
        }
    }


    #region 색상관련
    public static List<string> GetColorCombination(string color)
    {
        List<string> returnList = new List<string>();

        switch (color)
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
    public static string GetColorName(List<string> combination)
    {
        string colorName = "";

        if (combination.Count == 3)
        {
            return "White";
        }
        else
        {
            if (combination.Contains("Red"))
            {
                if (colorName.Equals(""))
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
                else if (colorName.Equals("Red"))
                {
                    colorName = "Yellow";
                }
            }
            if (combination.Contains("Blue"))
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
