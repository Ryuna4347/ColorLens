using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject CircleParticle;
    public GameObject PrismParticle;

    public void circleEffect(Vector3 pos, string color)
    {
        GameObject c= Instantiate(CircleParticle, pos, quaternion.identity);
        Color colors=Color.white;
        if(color=="Red")
            colors=Color.red;
        else if(color=="Blue")
            colors=Color.blue;
        else if(color=="Green")
            colors=Color.green;
        else if (color == "White")
            colors = Color.white;
            c.GetComponent<ParticleSystem>().startColor = colors;
    }
    public void PrismEffect(Vector3 pos, string color)
    {
        GameObject c= Instantiate(PrismParticle, pos, quaternion.identity);
        Color colors=Color.white;
        if(color=="Red")
            colors=Color.red;
        else if(color=="Blue")
            colors=Color.blue;
        else if(color=="Green")
            colors=Color.green;
        else if (color == "White")
            colors = Color.white;
        c.GetComponent<ParticleSystem>().startColor = colors;
    }
}
