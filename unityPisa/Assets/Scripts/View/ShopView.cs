
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
public class ShopView : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static ShopView self = null;
    public static string ViewName = "ShopView";
    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        GameObject g = null;
        if (ShopView.self != null)
        {
            g = ShopView.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(ShopView.ViewName, isTop, order);
        }
        ShopView scr = g.GetComponent<ShopView>();
        if (scr == null)
        {
            scr = g.AddComponent<ShopView>();
        }
        ShopView.self = scr;
        ShopView.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (ShopView.self != null)
        {
            ShopView.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (ShopView.self != null)
        {
            GameObject.Destroy(ShopView.self.gameObject);
            ShopView.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    public GameObject m_close = null;
    public GameObject m_shop_0 = null;
    public GameObject m_shopParent = null;
    private void initGetGameObj()
    {
        m_close = GetGameObjectByName("m_close");
        m_shop_0 = GetGameObjectByName("m_shop_0");
        m_shopParent = GetGameObjectByName("m_shopParent");
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    ///加载调用一次
    private void load()
    {
        uiA = Resources.Load<SpriteAtlas>(uipath);
        Tools.Instance.OnButton(m_close, () =>
        {
            ShopView.DestroyView();
        });
        m_shop_0.SetActive(false);
        initShop();
        //DataAll.Instance.gold = 20000;
    }

    string uipath= "Image/Atlas/ui";
    SpriteAtlas uiA = null;

    void initShop()
    {

        List<string> levelList = DataAll.Instance.getOpenLevel();
        for (int i = 0; i < GameConfig.self.totleLevel; i++)
        {
            GameObject g = GameObject.Instantiate(m_shop_0);
            GameObject icon= g.transform.Find("icon").gameObject;
            GameObject say= g.transform.Find("say").gameObject;
            GameObject bg = g.transform.Find("bg").gameObject;
            Text say_1 = GetComponentByName<Text>("say_1", g);
            GameObject open= g.transform.Find("open").gameObject;
            say_1.text = GameConfig.self.levelSay[i];
            say.GetComponent<Text>().text= GameConfig.self.levelSay[i];
            g.transform.SetParent(m_shopParent.transform,false);
            g.SetActive(true);
            Text ind = GetComponentByName<Text>("index", g);
            ind.text = i.ToString();
            if (levelList.Contains(i.ToString()) == true)
            {
                if (DataAll.Instance.level == i)
                {
                    bg.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
                }
                else
                {
                    Tools.Instance.OnPressObjUp(g, (PointerEventData p) =>
                    {

                        Text index = GetComponentByName<Text>("index", p.pointerPress);
                        int mapid = int.Parse(index.text);
                        if (mapid == DataAll.Instance.level)
                        {
                            Tools.Instance.showTip("Is here");
                            return;
                        }
                        ShopView.DestroyView();
                        GameScene.self.loadMap(mapid);

                    });
                }

                icon.SetActive(true);
                say.SetActive(true);
                Sprite sp= uiA.GetSprite("food_"+i);
                icon.GetComponent<Image>().sprite = sp;
                open.SetActive(false);
            }
            else
            {
                icon.SetActive(false);
                say.SetActive(false);
                open.SetActive(true);

                Text goldnum = GetComponentByName<Text>("gold",open);
                goldnum.text = GameConfig.self.levelGold[i].ToString();

                Tools.Instance.OnButton(open, (GameObject btn) => {
                    Tools.Instance.showTip("purchase succeeds");
                    Text gg= GetComponentByName<Text>("gold", btn);
                    Text index = GetComponentByName<Text>("index", btn.transform.parent.gameObject);
                    int mapid = int.Parse(index.text);

                    if (DataAll.Instance.gold >= int.Parse(gg.text))
                    {
                        DataAll.Instance.gold -= int.Parse(gg.text);
                        string str = DataAll.Instance.openLevel + "," + mapid;
                        //Debug.Log("博爱从：：" + str);
                        DataAll.Instance.openLevel = str;
                        ShopView.DestroyView();
                        GameScene.self.loadMap(mapid);
                    }
                    else
                    {
                        Tools.Instance.showTip("Gold is not enough");
                    }
                });
            }


        }
    }
}
