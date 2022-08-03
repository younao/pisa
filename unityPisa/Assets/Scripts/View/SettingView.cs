
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SettingView : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static SettingView self = null;
    public static string ViewName = "SettingView";
    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        GameObject g = null;
        if (SettingView.self != null)
        {
            g = SettingView.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(SettingView.ViewName, isTop, order);
        }
        SettingView scr = g.GetComponent<SettingView>();
        if (scr == null)
        {
            scr = g.AddComponent<SettingView>();
        }
        SettingView.self = scr;
        SettingView.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (SettingView.self != null)
        {
            SettingView.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (SettingView.self != null)
        {
            GameObject.Destroy(SettingView.self.gameObject);
            SettingView.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    public GameObject m_close = null;
    public GameObject m_audio = null;
    public GameObject m_zhen = null;
    private void initGetGameObj()
    {
        m_close = GetGameObjectByName("m_close");
        m_audio = GetGameObjectByName("m_audio");
        m_zhen = GetGameObjectByName("m_zhen");
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    ///加载调用一次
    private void load()
    {
        Tools.Instance.OnButton(m_close, () =>
        {
            SettingView.DestroyView();
        });

        Tools.Instance.OnPressObjUp(m_audio, (PointerEventData evn) =>
        {
            DataAll.Instance.isMusic = setis(m_audio, DataAll.Instance.isMusic);
        });

        Tools.Instance.OnPressObjUp(m_zhen, (PointerEventData evn) =>
        {
            DataAll.Instance.isZhen= setis(m_zhen, DataAll.Instance.isZhen);
        });
        this.init();
    }

    void init()
    {
        if (DataAll.Instance.isMusic == 0)
        {
            setis(m_audio, 1);
        }
        else
        {
            setis(m_audio, 0);
        }
        if (DataAll.Instance.isZhen == 0)
        {
            setis(m_zhen, 1);
        }
        else
        {
            setis(m_zhen, 0);
        }
    }

    int setis(GameObject g,int value)
    {
        RectTransform open = GetComponentByName<RectTransform>("open", g);
        Text no = GetComponentByName<Text>("no", g);
        Text off = GetComponentByName<Text>("off", g);
        if (value == 0)//关
        {
            open.localPosition = new Vector3(57, open.localPosition.y, 0);
            off.color = hong;
            no.color = bai;
            return 1;
        }
        else
        {
            open.localPosition = new Vector3(-57, open.localPosition.y, 0);
            off.color = bai;
            no.color = hong;
            return 0;
        }
    }

    Color hong = new Color(0.5843138f, 0.3058824f, 0.1411765f);
    Color  bai  = new Color(0.9960785f, 1, 0.9019608f);
}
