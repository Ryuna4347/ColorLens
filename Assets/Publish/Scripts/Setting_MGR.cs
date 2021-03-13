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
    public GameObject bgm1, bgm2, bgs1, bgs2;

    [SerializeField] private GameObject settingUI;
    private int touchType = -1; //터치(안드로이드/아이폰) 사용시 캐릭터 움직임 제어 방법(0=>버튼, 1=>슬라이드)
    private GameObject touchTypeUI; //터치 사용시 터치방식 옵션 설정 UI
    private List<Transform> touchTypeUI_btn = new List<Transform>();

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        settingUI = transform.Find("Setting_Phone").gameObject;
#elif UNITY_EDITOR || UNITY_STANDALONE
        settingUI = transform.Find("Setting_PC").gameObject;
#endif
        bgmvalue = PlayerPrefs.GetFloat(bgmkey, 1f);
        sevalue = PlayerPrefs.GetFloat(sekey, 1f);
        BGM.value = bgmvalue;
        SE.value = sevalue;

        if (PlayerPrefs.GetInt(soundKeybgs, 0) == 0)
        {
            bgs1.SetActive(true);
            bgs2.SetActive(false);
            isSoundbgs = true;
        }
        else
        {
            bgs2.SetActive(true);
            bgs1.SetActive(false);
            isSoundbgs = false;
        }

        if (PlayerPrefs.GetInt(soundKeybgm, 0) == 0)
        {
            bgm1.SetActive(true);
            bgm2.SetActive(false);
            isSoundbgm = true;
        }
        else
        {
            bgm2.SetActive(true);
            bgm1.SetActive(false);
            isSoundbgm = false;
        }

        SoundManager.instance.isSoundbgs = isSoundbgs;
        SoundManager.instance.isSoundbgm = isSoundbgm;

#if UNITY_ANDROID || UNITY_IOS
        Transform toggleGroup = transform.Find("Setting_Phone").Find("Panel").Find("TouchType");
        touchTypeUI_btn.Add(toggleGroup.GetChild(0));
        touchTypeUI_btn.Add(toggleGroup.GetChild(1));
        touchType = PlayerPrefs.GetInt("TouchType", 0);
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
            touchTypeUI_btn[touchType].GetComponent<Toggle>().Select();
#endif
        }
    }

    public void SetBgmSound()
    {
        isSoundbgm = !isSoundbgm;
        SoundManager.instance.isSoundbgm = isSoundbgm;
        if (isSoundbgm)
            PlayerPrefs.SetInt(soundKeybgm, 0);
        else
            PlayerPrefs.SetInt(soundKeybgm, 1);
        if (isSoundbgm)
        {
            bgm1.SetActive(true);
            bgm2.SetActive(false);
        }
        else
        {
            bgm2.SetActive(true);
            bgm1.SetActive(false);
        }
    }

    public void SetBgsSound()
    {
        isSoundbgs = !isSoundbgs;
        SoundManager.instance.isSoundbgs = isSoundbgs;
        if (isSoundbgs)
            PlayerPrefs.SetInt(soundKeybgs, 0);
        else
            PlayerPrefs.SetInt(soundKeybgs, 1);
        if (isSoundbgs)
        {
            bgs1.SetActive(true);
            bgs2.SetActive(false);
        }
        else
        {
            bgs2.SetActive(true);
            bgs1.SetActive(false);
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
