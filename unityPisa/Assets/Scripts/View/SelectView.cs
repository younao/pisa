
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class SelectView : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static SelectView self = null;
    public static string ViewName = "SelectView";
    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        GameObject g = null;
        if (SelectView.self != null)
        {
            g = SelectView.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(SelectView.ViewName, isTop, order);
        }
        SelectView scr = g.GetComponent<SelectView>();
        if (scr == null)
        {
            scr = g.AddComponent<SelectView>();
        }
        SelectView.self = scr;
        SelectView.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (SelectView.self != null)
        {
            SelectView.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (SelectView.self != null)
        {
            GameObject.Destroy(SelectView.self.gameObject);
            SelectView.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    public GameObject m_close = null;
    public GameObject m_info = null;
    public GameObject m_yes = null;
    public GameObject m_no = null;
    public GameObject m_title = null;
    private void initGetGameObj()
    {
        m_close = GetGameObjectByName("m_close");
        m_info = GetGameObjectByName("m_info");
        m_yes = GetGameObjectByName("m_yes");
        m_no = GetGameObjectByName("m_no");
        m_title = GetGameObjectByName("m_title");
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码

    public static int getGold = 0;

    ///加载调用一次
    private void load()
    {
        m_info.GetComponent<Text>().text= "$" + Tools.Instance.getGoldK(SelectView.getGold);
        Tools.Instance.OnButton(m_yes, () =>
        {
            DataAll.Instance.gold += SelectView.getGold;
            DataAll.Instance.timeLi= Tools.Instance.getTime().ToString();
            SelectView.DestroyView();
        });

        Tools.Instance.OnButton(m_close, () =>
        {
            SelectView.DestroyView();
        });
    }
}
