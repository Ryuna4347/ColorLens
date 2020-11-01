using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [SerializeField] private float referenceWidth=1920f;

    // Start is called before the first frame update
    void Awake()
    {
        float widthNow = Screen.currentResolution.width;
        float distance = GetComponent<Camera>().orthographicSize;
        if(widthNow < 1920f)
        {
            float ratio = referenceWidth / widthNow;
            GetComponent<Camera>().orthographicSize = distance * ratio;
            Screen.SetResolution(1920, 1080, true);
        }
    }
}
