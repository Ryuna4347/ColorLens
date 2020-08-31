using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomColor : MonoBehaviour
{
    void Start()
    {
        int r = Random.Range(0, 3);
        if(r==0)
            GetComponent<Image>().color=Color.red;
        else if (r == 1)
            GetComponent<Image>().color = new Color(0,0.75f,1f);
        else
            GetComponent<Image>().color = Color.green;
    }

}
