using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedRatioButton : MonoBehaviour
{
    private bool buttonPressed = false;

    public void ChangeMoveSpeedRatio()
    {
        buttonPressed = !buttonPressed;
        float speedRatio = buttonPressed ? 1.5f : 1.0f;
        GameManager.instance.ChangeMoveRatio(speedRatio);
    }
}
