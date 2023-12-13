using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

#if UNITY_ANDROID
    private const string APP_KEY = "1caefcd45";
#elif UNITY_IPHONE
    private const string APP_KEY = "";
#else
    private const string APP_KEY = "unexpected_platform";
#endif

    private bool isBannerLoadingFailed = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(APP_KEY);
    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SDKInitialized;

        //Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }

    private void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }

    private void SDKInitialized()
    {
        print("SDK Init");
        StartCoroutine(LoadAds());
    }

    private IEnumerator LoadAds()
    {
        YieldInstruction yieldInstruction = new WaitForSeconds(5);

        while (true)
        {
            if (IronSource.Agent.isInterstitialReady() == false)
            {
                IronSource.Agent.loadInterstitial();
            }

            if (IronSource.Agent.isRewardedVideoAvailable() == false)
            {
                IronSource.Agent.loadRewardedVideo();
            }

            if (isBannerLoadingFailed)
            {
                LoadBanner();
            }

            yield return yieldInstruction;
        }
    }

    #region banner

    public void LoadBanner()
    {
        isBannerLoadingFailed = false;

        print("LoadBanner");
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void DestroyBanner()
    {
        isBannerLoadingFailed = false;

        print("DestroyBanner");
        IronSource.Agent.destroyBanner();
    }
    public void HideBanner()
    {
        print("HideBanner");
        IronSource.Agent.hideBanner();
    }
    public void DisplayBanner()
    {
        print("DisplayBanner");
        IronSource.Agent.displayBanner();
    }

    /************* Banner AdInfo Delegates *************/
    //Invoked once the banner has loaded
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        print("Banner Loaded");
        isBannerLoadingFailed = false;
    }
    //Invoked when the banner loading process has failed.
    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        print("Banner Failed");
        //Invoke("LoadBanner", 5);
        isBannerLoadingFailed = true;
    }
    // Invoked when end user clicks on the banner ad
    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        print("BannerOnAdClickedEvent");
    }
    //Notifies the presentation of a full screen content following user click
    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        print("BannerOnAdScreenPresentedEvent");
    }
    //Notifies the presented screen has been dismissed
    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        print("BannerOnAdScreenDismissedEvent");
    }
    //Invoked when the user leaves the app
    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        print("BannerOnAdLeftApplicationEvent");
    }

    #endregion


    #region Interstitial

    private void LoadInterstitial()
    {
        print("LoadInterstitial");
        IronSource.Agent.loadInterstitial();
    }

    public bool CanShowInterstitial()
    {
        return IronSource.Agent.isInterstitialReady();
    }

    public void ShowInterstitial()
    {
        print("ShowInterstitial");
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            print("Interstitial not ready");
        }
    }

    /************* Interstitial AdInfo Delegates *************/
    // Invoked when the interstitial ad was loaded succesfully.
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        print("Interstitial Ready");
    }
    // Invoked when the initialization process has failed.
    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        print("Interstitial Failed");
    }
    // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        print("InterstitialOnAdOpenedEvent");
    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        print("InterstitialOnAdClickedEvent");
    }
    // Invoked when the ad failed to show.
    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        print("InterstitialOnAdShowFailedEvent");
    }
    // Invoked when the interstitial ad closed and the user went back to the application screen.
    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        print("InterstitialOnAdClosedEvent");
    }
    // Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
    // This callback is not supported by all networks, and we recommend using it only if  
    // it's supported by all networks you included in your build. 
    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
        print("InterstitialOnAdShowSucceededEvent");
    }
    #endregion


    #region Rewarded

    private void LoadRewarded()
    {
        print("LoadRewarded");
        IronSource.Agent.loadRewardedVideo();
    }

    public bool CanShowRewarded()
    {
        return IronSource.Agent.isRewardedVideoAvailable();
    }

    public void ShowRewarded()
    {
        print("ShowRewarded");
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            print("Rewarded not ready");
        }
    }

    /************* RewardedVideo AdInfo Delegates *************/
    // Indicates that there’s an available ad.
    // The adInfo object includes information about the ad that was loaded successfully
    // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        print("Rewarded Available");
    }
    // Indicates that no ads are available to be displayed
    // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    void RewardedVideoOnAdUnavailable()
    {
        print("Rewarded Unavailable");
    }
    // The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        print("RewardedVideoOnAdOpenedEvent");
    }
    // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        print("RewardedVideoOnAdClosedEvent");
    }
    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        int score = PlayerPrefs.GetInt("score");
        PlayerPrefs.SetInt("score", score + 1);
        print("Give reward: " + score);
    }
    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        print("RewardedVideoOnAdShowFailedEvent");
    }
    // Invoked when the video ad was clicked.
    // This callback is not supported by all networks, and we recommend using it only if
    // it’s supported by all networks you included in your build.
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        print("RewardedVideoOnAdClickedEvent");
    }

    #endregion
}
