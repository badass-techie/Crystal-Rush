using System;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class RewardedAdManager : MonoBehaviour{
    static RewardedAdManager instance = null;
    string adUnitId;
    public RewardedAd ad;
    AdRequest request;
    bool adWatched = false;
    [HideInInspector] public bool failedToLoad = false;
    [HideInInspector] public string errorMessage;

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this){
            Destroy(this.gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start(){
        MobileAds.Initialize(initStatus => { });
        #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3693486173534414/7944246116";
        #elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
                adUnitId = "unexpected_platform";
        #endif
        RequestConfiguration requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(new List<string>() { "7077D997C41C2D19C8ABE05CFDC1EA19", "FCB7C947606E60164A44C2DAF121B8C0" })
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
        CreateAdRequest();
    }
    void CreateAdRequest(){
        ad = new RewardedAd(adUnitId);
        // Called when an ad request has successfully loaded.
        //ad.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        ad.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        //ad.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        ad.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        ad.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        ad.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        ad.LoadAd(request);
    }
    public void ReloadAd(){
        failedToLoad = false;
        ad.LoadAd(request);
    }
    public void ShowAd(){
        Time.timeScale = 0f;
        ad.Show();
    }
    //ads
    //failed to load
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args){
        errorMessage = args.Message;
        failedToLoad = true;
    }
    //failed to play
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args){
        Time.timeScale = 1f;
    }
    //ad closed
    public void HandleRewardedAdClosed(object sender, EventArgs args){
        Time.timeScale = 1f;
        CreateAdRequest(); 
        if (adWatched){
            //reward earned
            Invoke("Revive", 1f);
            adWatched = false;
        } else {
            //reward lost

        }
    }
    //ad watched
    public void HandleUserEarnedReward(object sender, Reward args){
        adWatched = true;
    }
    //revive player
    void Revive(){
        FindObjectOfType<LevelManager>().Respawn();
    }
}