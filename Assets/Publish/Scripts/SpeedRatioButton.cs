using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedRatioButton : MonoBehaviour
{
    private bool buttonPressed = false;
    private Image buttonImg;
    public List<Sprite> ratioImages;

    private void Awake()
    {
        buttonImg = GetComponent<Image>();
    }

    public void ChangeMoveSpeedRatio()
    {
        buttonPressed = !buttonPressed;
        buttonImg.sprite = ratioImages[buttonPressed?1:0];
        float speedRatio = buttonPressed ? 1.5f : 1.0f;
        GameManager.instance.ChangeMoveRatio(speedRatio);
    }
}
