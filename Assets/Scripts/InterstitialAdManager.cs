using System;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class InterstitialAdManager : MonoBehaviour {
    static InterstitialAdManager instance = null;
    string adUnitId;
    public InterstitialAd ad;
    AdRequest request;
    public string errorMessage;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) {
            Destroy(this.gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start() {
        MobileAds.Initialize(initStatus => { });
        #if UNITY_ANDROID
                adUnitId = "ca-app-pub-3693486173534414/8716904757";
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

    void CreateAdRequest() {
        ad = new InterstitialAd(adUnitId);
        // Called when an ad request has successfully loaded.
        ad.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        ad.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        //ad.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        ad.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        //ad.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        ad.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args) {
        //loaded
    }

    public void ShowAd() {
        if (ad.IsLoaded()) {
            if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
                FindObjectOfType<AudioManager>().Pause("ThemeSong");
            ad.Show();
        }
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        //failed
        errorMessage = args.Message;
        Invoke(nameof(ReloadAd), 10f);
    }

    void ReloadAd(){
        ad.LoadAd(request);
    }

    public void HandleOnAdClosed(object sender, EventArgs args) {
        //closed
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Resume("ThemeSong");
        ad.Destroy();
        CreateAdRequest();
    }
}
