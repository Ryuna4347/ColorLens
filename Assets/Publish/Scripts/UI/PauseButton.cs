using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
#if UNITY_ANDROID || UNITY_IOS
    public void PauseUIOn()
    {
        if (!GameManager.instance.IsGameOver && GameManager.instance.CanMove)
        {
            GameManager.instance.CheckPause();
            //gameObject.SetActive(false);
        }
    }
#endif
}
