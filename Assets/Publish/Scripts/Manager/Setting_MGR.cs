using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting_MGR : MonoBehaviour
{
    private float bgmvalue, sevalue;
    string bgmkey = "bgmkey";
    private string sekey = "sekey";
    public Slider BGM;
    public Slider SE;
    private bool isSoundbgm = true;
    private string soundKeybgm = "soundkeybgm";
    private bool isSoundbgs = true;
    private string soundKeybgs = "soundkeybgs";

    [SerializeField] private GameObject settingUI;
    private int touchType = -1; //터치(안드로이드/아이폰) 사용시 캐릭터 움직임 제어 방법(0=>버튼, 1=>슬라이드)
    private GameObject touchTypeUI; //터치 사용시 터치방식 옵션 설정 UI
    [SerializeField]private List<Transform> touchTypeUI_btn = new List<Transform>();

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        settingUI = transform.Find("Setting_Phone").gameObject;
#elif UNITY_EDITOR || UNITY_STANDALONE
        settingUI = transform.Find("Setting_PC").gameObject;
#endif
        BGM = settingUI.transform.Find("Panel").Find("BGM").Find("Slider").GetComponent<Slider>();
        SE = settingUI.transform.Find("Panel").Find("SE").Find("Slider").GetComponent<Slider>();

        bgmvalue = PlayerPrefs.GetFloat(bgmkey, 1f);
        sevalue = PlayerPrefs.GetFloat(sekey, 1f);
        BGM.value = bgmvalue;
        SE.value = sevalue;
        SetBGM();
        SetSE();

#if UNITY_ANDROID || UNITY_IOS
        Transform toggleGroup = transform.Find("Setting_Phone").Find("Panel").Find("TouchType");
        touchTypeUI_btn.Add(toggleGroup.GetChild(0));
        touchTypeUI_btn.Add(toggleGroup.GetChild(1));
        touchType = PlayerPrefs.GetInt("TouchType", 1);
#endif
    }

    public void SetBGM()
    {
        SoundManager.instance.savedBGM = BGM.value;
        PlayerPrefs.SetFloat(bgmkey, BGM.value);
    }

    public void SetSE()
    {
        SoundManager.instance.savedSE = SE.value;
        PlayerPrefs.SetFloat(sekey, SE.value);
    }

    public void Open()
    {
        if (settingUI.activeSelf)
        {
            settingUI.SetActive(false);
        }
        else
        {
            settingUI?.SetActive(true);
#if UNITY_ANDROID || UNITY_IOS
            touchTypeUI_btn[touchType].GetComponent<ToggleButton>().SetIsOn(true);
#endif
        }
    }

    public void SetBgmSound()
    {
        if (isSoundbgm)
        {
            BGM.value = PlayerPrefs.GetFloat(bgmkey + "btn", 1);
        }
        else
        {
            PlayerPrefs.SetFloat(bgmkey+"btn", BGM.value);
            BGM.value = 0;
        }
    }

    public void SetBgsSound()
    {
        if (isSoundbgs)
        {
            SE.value = PlayerPrefs.GetFloat(sekey + "btn", 1);
        }
        else
        {
            PlayerPrefs.SetFloat(sekey + "btn", SE.value);
            SE.value = 0;
        }
    }
#if UNITY_ANDROID || UNITY_IOS
    public void SetTouchType(int type)
    {
        touchType = type;
        PlayerPrefs.SetInt("TouchType", type);
    }
#endif

}
