using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;


public class AdmobManager : MonoBehaviour
{
    public static AdmobManager instance { get; private set; }
    public bool isTestMode;
    public Text LogText;
    public Button FrontAdsBtn, RewardAdsBtn;
    [SerializeField]private int frontAdInterval; //전면 광고 등장 확률(4회 플레이부터 확률적으로 전면 광고 표기. 4회->50%, 5회->75%, 6회->100%)


    void Awake()
    {
        if(!instance)
        {
            var requestConfiguration = new RequestConfiguration
           .Builder()
           .SetTestDeviceIds(new List<string>() { "1DF7B7CC05014E8" }) // test Device ID
           .build();

            MobileAds.SetRequestConfiguration(requestConfiguration);

            LoadBannerAd();
            LoadFrontAd();
            LoadRewardAd();
            SceneManager.sceneLoaded += OnSceneLoaded;

            instance = this;
            frontAdInterval = PlayerPrefs.GetInt("admobInterval", 0);

            DontDestroyOnLoad(this.gameObject); 
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        //FrontAdsBtn.interactable = frontAd.IsLoaded();
        //RewardAdsBtn.interactable = rewardAd.IsLoaded();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ToggleBannerAd(true);
    }

    AdRequest GetAdRequest()
    {
        return new AdRequest.Builder().Build();
    }



    #region 배너 광고
    const string bannerTestID = "ca-app-pub-3940256099942544/6300978111";
    const string bannerID = "ca-app-pub-2683765173147911/2881219834";
    BannerView bannerAd;


    void LoadBannerAd()
    {
        bannerAd = new BannerView(isTestMode ? bannerTestID : bannerID,
            AdSize.SmartBanner, AdPosition.Bottom);
        bannerAd.LoadAd(GetAdRequest());
        ToggleBannerAd(false);
    }

    public void ToggleBannerAd(bool b)
    {
        if (b) bannerAd.Show();
        else bannerAd.Hide();
    }
    #endregion



    #region 전면 광고
    const string frontTestID = "ca-app-pub-3940256099942544/8691691433";
    const string frontID = "ca-app-pub-2683765173147911/6691923596";
    InterstitialAd frontAd;


    void LoadFrontAd()
    {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        frontAd.LoadAd(GetAdRequest());
        frontAd.OnAdClosed += (sender, e) =>
        {
            Debug.Log("전면광고 성공");
            //LogText.text = "전면광고 성공";
        };
    }

    public void ShowFrontAd()
    {
        if (++frontAdInterval >= 4)
        {
            int random = Random.Range(0, 100);
            Debug.Log(random+" "+ ((frontAdInterval - 4) * -25 + 50));
            if (random >= (frontAdInterval - 4) * -25 + 50)
            {
                frontAdInterval = 0;
                frontAd.Show();
                LoadFrontAd();
            }
        }
        PlayerPrefs.SetInt("admobInterval", frontAdInterval);
    }
    #endregion



    #region 리워드 광고
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-2683765173147911/1236610457";
    RewardedAd rewardAd;


    void LoadRewardAd()
    {
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);
        rewardAd.LoadAd(GetAdRequest());
        rewardAd.OnUserEarnedReward += (sender, e) =>
        {
            Debug.Log("리워드 광고 성공");
            //LogText.text = "리워드 광고 성공";
        };
    }

    public void ShowRewardAd()
    {
        rewardAd.Show();
        LoadRewardAd();
    }
    #endregion
}