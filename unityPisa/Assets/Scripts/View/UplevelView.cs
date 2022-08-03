
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
public class UplevelView : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static UplevelView self = null;
    public static string ViewName = "UplevelView";
    public static EventArg arg = null;

    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        UplevelView.arg = arg;
        GameObject g = null;
        if (UplevelView.self != null)
        {
            g = UplevelView.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(UplevelView.ViewName, isTop, order);
        }
        UplevelView scr = g.GetComponent<UplevelView>();
        if (scr == null)
        {
            scr = g.AddComponent<UplevelView>();
        }
        UplevelView.self = scr;
        UplevelView.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (UplevelView.self != null)
        {
            UplevelView.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (UplevelView.self != null)
        {
            GameObject.Destroy(UplevelView.self.gameObject);
            UplevelView.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    public GameObject m_title = null;
    public GameObject m_close = null;
    public GameObject m_speed = null;
    public GameObject m_addHumun = null;
    public GameObject m_addObj = null;
    private void initGetGameObj()
    {
        m_title = GetGameObjectByName("m_title");
        m_close = GetGameObjectByName("m_close");
        m_speed = GetGameObjectByName("m_speed");
        m_addHumun = GetGameObjectByName("m_addHumun");
        m_addObj = GetGameObjectByName("m_addObj");
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    public static Action closeFun = null;

    public static bool ismy=false;

    string uipath = "Image/Atlas/ui";
    SpriteAtlas uiA = null;

    string ui1path = "Image/Atlas/uplevel";
    SpriteAtlas uiA1 = null;
    ///加载调用一次
    private void load()
    {
        isMy = UplevelView.ismy;
        uiA = Resources.Load<SpriteAtlas>(uipath);

        uiA1 = Resources.Load<SpriteAtlas>(ui1path);

        initMy();

        Tools.Instance.OnButton(m_close, () =>
        {
            if (closeFun != null)
            {
                closeFun();
                closeFun = null;
            }
            UplevelView.DestroyView();
        });
    }

    void uplevel(GameObject g, int goldnum, int level)
    {
        GameObject up = GetGameObjectByName("up", g);
        Text gold = GetComponentByName<Text>("gold", up);
        if (goldnum > 0)
        {
            gold.text = (goldnum).ToString();
        }
        else
        {
            gold.text = "max";
        }
        
        Transform pos = GetGameObjectByName("pos", g).transform;
        Sprite sp = uiA.GetSprite("dian_1");
        //Debug.Log("等级：：：：" + level);
        for (int i = 0; i < GameConfig.self.upLevelMax; i++)
        {
            if (i < level)
            {
                Image img = GetComponentByName<Image>("img_" + i, pos.gameObject);
                img.sprite = sp;
            }
        }
    }
    bool isMy = false;
    #region 更新我自己的等级
    void initMy()
    {
        if (isMy == true)
        {
            Image xr_1 =GetComponentByName<Image>("icon", m_addObj);
            Sprite xr_1_0= uiA1.GetSprite("xr_1_0");
            xr_1.sprite = xr_1_0;

            Image xr_2 = GetComponentByName<Image>("icon", m_speed);
            Sprite xr_2_0 = uiA1.GetSprite("xr_2_0");
            xr_2.sprite = xr_2_0;

            Image xr_3 = GetComponentByName<Image>("icon", m_addHumun);
            Sprite xr_3_0 = uiA1.GetSprite("xr_3_0");
            xr_3.sprite = xr_3_0;

            uplevel(m_speed, upSpeed(DataAll.Instance.speed), DataAll.Instance.speed);
            uplevel(m_addHumun, upAddHuman(DataAll.Instance.newGold), DataAll.Instance.newGold);
            uplevel(m_addObj, upAddObj(DataAll.Instance.addObj), DataAll.Instance.addObj);
        }
        else
        {
            uplevel(m_speed, upSpeed(DataAll.Instance.speedAi), DataAll.Instance.speedAi);
            uplevel(m_addHumun, upAddHuman(DataAll.Instance.addHuman), DataAll.Instance.addHuman);
            uplevel(m_addObj, upAddObj(DataAll.Instance.addObjAi), DataAll.Instance.addObjAi);
        }

        _initMy(m_speed);
        _initMy(m_addHumun);
        _initMy(m_addObj);
    }

    void _initMy(GameObject g)
    {
        GameObject up= GetGameObjectByName("up", g);
        GameObject free= GetGameObjectByName("free", g);
        Text gold = GetComponentByName<Text>("gold", up);
        //DataAll.Instance.gold = 200000;
        Tools.Instance.OnButton(up, () =>
        {
            int level = 0;
            int goldnum = 0;
            if (gold.text == "max")
            {
                return;
            }
            int needGold = int.Parse(gold.text);
            
            if (DataAll.Instance.gold > needGold)
            {
                //Debug.Log("升级：2222222：" + g.name + "  :" + DataAll.Instance.addHuman);
                if (g == m_speed)
                {

                    if (isMy == true)
                    {
                        if (DataAll.Instance.speed >= GameConfig.self.upLevelMax)
                        {
                            Tools.Instance.showTip("Already maximum grade");
                            return;
                        }
                        DataAll.Instance.speed++;
                        level = DataAll.Instance.speed + 1;
                    }
                    else
                    {
                        if (DataAll.Instance.speedAi >= GameConfig.self.upLevelMax)
                        {
                            Tools.Instance.showTip("Already maximum grade");
                            return;
                        }
                        DataAll.Instance.speedAi++;
                        level = DataAll.Instance.speedAi + 1;
                        
                    }
                    goldnum = upSpeed(level);
                }
                else if (g == m_addHumun)
                {
                    if (isMy == true)
                    {
                        if (DataAll.Instance.newGold >= GameConfig.self.upLevelMax)
                        {
                            Tools.Instance.showTip("Already maximum grade");
                            return;
                        }

                        DataAll.Instance.newGold++;
                        level = DataAll.Instance.newGold + 1;
                    }
                    else
                    {
                        if (DataAll.Instance.addHuman >= GameConfig.self.upLevelMax)
                        {
                            Tools.Instance.showTip("Already maximum grade");
                            return;
                        }

                        DataAll.Instance.addHuman++;
                        level = DataAll.Instance.addHuman + 1;
                        GameScene.self.mapControl.newNpc();
                    }



                    goldnum = upAddHuman(level);
                }
                else
                {
                    if (isMy == true)
                    {
                        if (DataAll.Instance.addObj >= GameConfig.self.upLevelMax)
                        {
                            Tools.Instance.showTip("Already maximum grade");
                            return;
                        }
                        DataAll.Instance.addObj++;
                        level = DataAll.Instance.addObj + 1;
                    }
                    else
                    {
                        if (DataAll.Instance.addObjAi >= GameConfig.self.upLevelMax)
                        {
                            Tools.Instance.showTip("Already maximum grade");
                            return;
                        }
                        DataAll.Instance.addObjAi++;
                        level = DataAll.Instance.addObjAi + 1;
                    }

                    goldnum = upAddObj(level);
                }
                //Debug.Log("升级：：" + g.name+"  :"+ level);
                DataAll.Instance.gold -= needGold;
                uplevel(g, goldnum, level-1);
            }
            else
            {
                Tools.Instance.showTip("Gold is not enough");
            }

        });
    }

    int upSpeed(int level)
    {
        if (level > 0)
        {
            level--;
        }
        if (ismy == true)
        {
            List<int> g = new List<int>() {1500,4000,7500,12000 ,0,0};
            return g[level];
        }
        else
        {
            List<int> g = new List<int>() { 1000, 3500, 7000, 10000, 0 };
            return g[level];
        }
        
    }

    int upAddHuman(int level)
    {
        if (level > 0)
        {
            level--;
        }
        if (ismy == true)
        {
            List<int> g = new List<int>() { 500, 1500, 3000, 6000, 0 };
            return g[level];
        }
        else
        {
            List<int> g = new List<int>() { 2000, 3000, 5000, 7000, 0 };
            return g[level];
        }
    }

    int upAddObj(int level)
    {
        if (level > 0)
        {
            level--;
        }
        if (ismy == true)
        {
            List<int> g = new List<int>() { 1000, 2500, 5000, 7500, 0 };
            return g[level];
        }
        else
        {
            List<int> g = new List<int>() { 2000, 4000, 6000, 8000, 0 };
            return g[level];
        }
    }
    #endregion

}
