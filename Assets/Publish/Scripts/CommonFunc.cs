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
}
