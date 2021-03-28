using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedRatioButton : MonoBehaviour
{
    private bool buttonPressed;
    private Image buttonImg;
    public List<Sprite> ratioImages;

    private void Awake()
    {
        buttonImg = GetComponent<Image>();
    }

    private void Start()
    {
        buttonPressed = PlayerPrefs.GetInt("SpeedRatio", 0) == 0 ? false : true;
        float speedRatio = buttonPressed ? 1.5f : 1.0f;
        buttonImg.sprite = ratioImages[buttonPressed ? 1 : 0];
        GameManager.instance.ChangeMoveRatio(speedRatio);
    }

    public void ChangeMoveSpeedRatio()
    {
        buttonPressed = !buttonPressed;
        buttonImg.sprite = ratioImages[buttonPressed?1:0];
        float speedRatio = buttonPressed ? 1.5f : 1.0f;
        PlayerPrefs.SetInt("SpeedRatio", buttonPressed ? 1 : 0);
        GameManager.instance.ChangeMoveRatio(speedRatio);
    }
}
