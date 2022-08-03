using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        DataAll.Instance.init();
        gameObject.AddComponent<ToolsMono>();
        new ViewManage();
        Invoke("laterFun", 1);
    }

    void laterFun()
    {
        GameObject.Destroy(ViewManage.Instance.open_anim);
        GameView.ShowView();
    }
}
