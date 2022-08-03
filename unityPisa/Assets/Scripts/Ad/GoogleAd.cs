using UnityEditor;
using UnityEngine;

public class GoogleAd:MonoBehaviour
{
    public static GoogleAd self = null;

    private void Awake()
    {
        init();
        self = this;
    }

    public InterstitialAd insertAd = null;
    public VideoAd video = null;
    public BannerAd banner = null;

    public string insertId = "f7bc84afa3a6513b";
    public string videoId = "a7eef1c19077cb45";
    public string bannerId = "267fe2a6cf4955ca";
    public string sdkKey = "4iqTSX4iLW4Tbuo_D5gUJggmCGqDPK4STEZGO3eet2GAJZcWGhOoH5D9DyIHxKOC_FkRF8rsgRddso_TnzXVku";
    public string usrid = "";

    //初始化google广告
    void init()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            
            // AppLovin SDK is initialized, start loading ads
            Debug.Log("初始化完广告>>加载广告>>>>>"+ sdkConfiguration);
            banner = new BannerAd(bannerId);
            insertAd = new InterstitialAd(insertId);
            video = new VideoAd(videoId);
            
        };

        MaxSdk.SetSdkKey(sdkKey);
        //MaxSdk.SetUserId(usrid);
        MaxSdk.InitializeSdk();

        
    }
}