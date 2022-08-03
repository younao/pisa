
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class TipView : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static TipView self = null;
    public static string ViewName = "TipView";
    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        GameObject g = null;
        if (TipView.self != null)
        {
            g = TipView.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(TipView.ViewName, isTop, order);
        }
        TipView scr = g.GetComponent<TipView>();
        if (scr == null)
        {
            scr = g.AddComponent<TipView>();
        }
        TipView.self = scr;
        TipView.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (TipView.self != null)
        {
            TipView.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (TipView.self != null)
        {
            GameObject.Destroy(TipView.self.gameObject);
            TipView.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    public GameObject m_tip = null;
    private void initGetGameObj()
    {
        m_tip = GetGameObjectByName("m_tip");
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    ///加载调用一次
    private void load()
    {

    }
    private void OnEnable()
    {
        this.showtip(TipView.tip, TipView.t);
    }

    void show(EventArg arg)
    {
        if (m_tip != null)
        {
            this.showtip(TipView.tip, TipView.t);
        }
    }
    public static string tip = "";
    public static float t = 0;
    void showtip(string info, float t)
    {
        if (IEtip != null)
        {
            StopCoroutine(IEtip);
        }
        m_tip.GetComponent<Text>().text = info;
        IEtip = tiptime(t);
        StartCoroutine(IEtip);
    }

    IEnumerator IEtip = null;

    IEnumerator tiptime(float t)
    {
        yield return new WaitForSeconds(t);
        TipView.HideView();
    }
}
