using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Prism : MonoBehaviour
{
    public List<GameObject> PrismObjs= new List<GameObject>();
    
    public void StartPrismEffect()
    {
        foreach (GameObject prism in PrismObjs)
        {
            prism.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
        }
            
    }
}
