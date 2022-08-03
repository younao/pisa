using System;
using UnityEditor;
using UnityEngine;

public class VideoData
{
    public bool isOk = false;
    public MaxSdk.Reward reward;
}

public class VideoAd
{
    public VideoAd(string adid="")
    {
        if (adid != "")
        {
            adUnitId = adid;
        }

        InitializeRewardedAds();
    }
    string adUnitId = "YOUR_AD_UNIT_ID";
    int retryAttempt;

    private void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }
    //加载广告
    public void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(adUnitId);
    }

    //观看完成
    public Action<VideoData> okFun = null;

    public void show(Action<VideoData> fun=null)
    {
        if (fun != null)
        {
            okFun = fun;

            //tttt
            //VideoData d = new VideoData();
            //d.isOk = true;
            //d.reward = new MaxSdkBase.Reward();
            //fun(d);
            //return;
        }
        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
            MaxSdk.ShowRewardedAd(adUnitId);
        }
        else
        {
            if (okFun != null)
            {
                VideoData d = new VideoData();
                d.isOk = false;
                d.reward = new MaxSdk.Reward();
                Tools.Instance.showTip("No ads");
                okFun(d);
            }
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        Debug.Log("视频：：：：：：：：OnRewardedAdLoadedEvent：" + adInfo);
        // Reset retry attempt
        retryAttempt = 0;
    }
    bool isLook = false;
    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Debug.Log("视频：：：：：：：：OnRewardedAdLoadFailedEvent：" + errorInfo);
        Tools.Instance.LaterFun(() =>
        {
            LoadRewardedAd();
        }, (float)retryDelay);
    }

    //调起视频调用一次
    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        Debug.Log("视频：：：：：：：：OnRewardedAdDisplayedEvent：" + adUnitId);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("视频：：：：：：：：OnRewardedAdFailedToDisplayEvent：" + adUnitId);
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        Debug.Log("视频：：：：：：：：OnRewardedAdClickedEvent：" + adUnitId);
    }

    //关闭视频
    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
         Debug.Log("视频：：：：：：：：OnRewardedAdHiddenEvent：" + adUnitId);
        // Rewarded ad is hidden. Pre-load the next ad
        if (isLook == false)
        {
            if (okFun != null)
            {
                VideoData d = new VideoData();
                d.isOk = false;
                d.reward = new MaxSdk.Reward();
                
                okFun(d);
            }
        }
        isLook = false;
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("视频：：：：：：：：OnRewardedAdReceivedRewardEvent：" + adUnitId);
        isLook = true;
        if (okFun != null)
        {
            VideoData d = new VideoData();
            d.isOk = true;
            d.reward = reward;
            okFun(d);
        }
        // The rewarded ad displayed and the user should receive the reward.
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("视频：：：：：：：：OnRewardedAdRevenuePaidEvent：" + adUnitId);
        // Ad revenue paid. Use this callback to track user revenue.
    }
}