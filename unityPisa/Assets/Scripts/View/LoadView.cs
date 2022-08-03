
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class LoadView : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static LoadView self = null;
    public static string ViewName = "LoadView";
    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        GameObject g = null;
        if (LoadView.self != null)
        {
            g = LoadView.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(LoadView.ViewName, isTop, order);
        }
        LoadView scr = g.GetComponent<LoadView>();
        if (scr == null)
        {
            scr = g.AddComponent<LoadView>();
        }
        LoadView.self = scr;
        LoadView.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (LoadView.self != null)
        {
            LoadView.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (LoadView.self != null)
        {
            GameObject.Destroy(LoadView.self.gameObject);
            LoadView.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    private void initGetGameObj()
    {
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    ///加载调用一次
    private void load()
    {

    }
}
