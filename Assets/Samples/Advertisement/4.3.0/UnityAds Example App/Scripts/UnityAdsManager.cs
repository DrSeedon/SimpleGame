using System;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine;
using UnityEngine.Serialization;

public class UnityAdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public string androidGameId;
    public string iOSGameId;
    public bool testMode = true;
    
    public string gameID;

    private string _bannerPlacement = "Banner_Android";
    private string _videoPlacement = "Interstitial_Android";
    private string _rewardedVideoPlacement = "Rewarded_Android";

    [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

    private bool _showBanner = false;

    //utility wrappers for debuglog
    public delegate void DebugEvent(string msg);
    public static event DebugEvent OnDebugLog;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameID = iOSGameId;
            _bannerPlacement = "Banner_iOS";
            _videoPlacement = "Interstitial_iOS";
            _rewardedVideoPlacement = "Rewarded_iOS";
        }
        else
        {
            gameID = androidGameId;
            _bannerPlacement = "Banner_Android";
            _videoPlacement = "Interstitial_Android";
            _rewardedVideoPlacement = "Rewarded_Android";
        }
        
        if (Advertisement.isSupported)
        {
            DebugLog(Application.platform + " supported by Advertisement");
        }
        Advertisement.Initialize(gameID, testMode, this);
    }

    public void ToggleBanner() 
    {
        _showBanner = !_showBanner;

        if (_showBanner)
        {
            Advertisement.Banner.SetPosition(bannerPosition);
            Advertisement.Banner.Show(_bannerPlacement);
        }
        else
        {
            Advertisement.Banner.Hide(false);
        }
    }

    public void LoadRewardedAd()
    {
        Advertisement.Load(_rewardedVideoPlacement, this);
    }

    public void ShowRewardedAd()
    {
        Advertisement.Show(_rewardedVideoPlacement, this);
    }

    public void LoadNonRewardedAd()
    {
        Advertisement.Load(_videoPlacement, this);
    }

    public void ShowNonRewardedAd()
    {
        Advertisement.Show(_videoPlacement, this);
    }
    
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
    
    #region Interface Implementations
    public void OnInitializationComplete()
    {
        DebugLog("Init Success");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        DebugLog($"Init Failed: [{error}]: {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        DebugLog($"Load Success: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        DebugLog($"Load Failed: [{error}:{placementId}] {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        DebugLog($"OnUnityAdsShowFailure: [{error}]: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        DebugLog($"OnUnityAdsShowStart: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        DebugLog($"OnUnityAdsShowClick: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        DebugLog($"OnUnityAdsShowComplete: [{showCompletionState}]: {placementId}");
    }
    #endregion

    public void OnGameIDFieldChanged(string newInput)
    {
        gameID = newInput;
    }
    
    //wrapper around debug.log to allow broadcasting log strings to the UI
    void DebugLog(string msg)
    {
        OnDebugLog?.Invoke(msg);
        Debug.Log(msg);
    }
}
