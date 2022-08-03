using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidSdk : MonoBehaviour
{
    // Start is called before the first frame update

    public static AndroidSdk self = null;

    private AndroidJavaClass jarclass = null;
    private void Awake()
    {
        self = this;
        //Debug.Log("初始化安卓》》》》》》》》》》");
        //jarclass = new AndroidJavaClass("com.xiangjiao.usetest.MainActivity"); //new AndroidJavaClass
        //jarclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); //new AndroidJavaClass
        Debug.Log("初始化安卓》》》》1》》》》》》"+ jarclass);
    }

    public void zhen()
    {
        if (DataAll.Instance.isZhen == 1)
        {
            return;
        }

#if UNITY_IOS

#elif UNITY_ANDROID
        //jo.Call("SetVibrator", 2);
#endif


    }
}



