using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Prism : MonoBehaviour
{
    public List<GameObject> PrismObjs= new List<GameObject>();
    
    public void StartPrismEffect(int i)
    {
        PrismObjs[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
 
    }
}
