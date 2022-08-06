using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour, IGameAnalyticsATTListener
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        DataAll.Instance.init();
        gameObject.AddComponent<ToolsMono>();
        new ViewManage();
        //Debug.logger.logEnabled = false;
        Invoke("laterFun", 2);
    }

    void laterFun()
    {
        GameObject.Destroy(ViewManage.Instance.open_anim);
        GameView.ShowView();
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            GameAnalytics.Initialize();
        }
        
        GameAnalytics.SetCustomId(SystemInfo.deviceUniqueIdentifier);
    }

    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }
}
