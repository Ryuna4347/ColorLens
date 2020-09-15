using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedRatioButton : MonoBehaviour
{
    private bool buttonPressed = false;

    public void ChangeMoveSpeedRatio()
    {
        float speedRatio = buttonPressed ? 1.5f : 1.0f;
        buttonPressed = !buttonPressed;
        GameManager.instance.ChangeMoveRatio(speedRatio);
    }
}
