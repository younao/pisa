
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameView : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static GameView self = null;
    public static string ViewName = "GameView";
    public static void ShowView(bool isTop = true, int order = 0, EventArg arg = null)
    {
        GameObject g = null;
        if (GameView.self != null)
        {
            g = GameView.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(GameView.ViewName, isTop, order);
        }
        GameView scr = g.GetComponent<GameView>();
        if (scr == null)
        {
            scr = g.AddComponent<GameView>();
        }
        GameView.self = scr;
        GameView.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (GameView.self != null)
        {
            GameView.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (GameView.self != null)
        {
            GameObject.Destroy(GameView.self.gameObject);
            GameView.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    public GameObject m_move = null;
    public GameObject m_gold = null;
    public GameObject m_setting = null;
    public GameObject m_ad = null;
    public GameObject m_upHuman = null;
    public GameObject m_icon = null;
    public GameObject m_test = null;
    private void initGetGameObj()
    {
        m_move = GetGameObjectByName("m_move");
        m_gold = GetGameObjectByName("m_gold");
        m_setting = GetGameObjectByName("m_setting");
        m_ad = GetGameObjectByName("m_ad");
        m_upHuman = GetGameObjectByName("m_upHuman");
        m_icon = GetGameObjectByName("m_icon");
        m_test = GetGameObjectByName("m_test");
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    ///加载调用一次
    private void load()
    {
        uiA1 = Resources.Load<SpriteAtlas>(ui1path);
        //Debug.Log("加载一次的方法：");
        Tools.Instance.OnStartPressObj(m_move, (PointerEventData evn) =>
        {
            this.moveOri.x = evn.position.x;
            this.moveOri.y = evn.position.y;
            //Debug.Log("点击    坐标：" + evn.position);
        });
        Tools.Instance.DragObj(m_move, (PointerEventData evn) =>
        {
            this.dir.x = evn.position.x - this.moveOri.x;
            this.dir.y = evn.position.y - this.moveOri.y;
            float[] yesDir = Tools.Instance.rotatePos(dir, huan, CameraFollow.self.transform.eulerAngles.y);
            dir.x = yesDir[0];
            dir.y = yesDir[1];
            //Debug.Log("拖动坐标：" + evn.position+ this.dir+ this.moveOri);
        }, false);
        Tools.Instance.OnPressObjUp(m_move, (PointerEventData evn) =>
        {
            this.dir.x = 0;
            this.dir.y = 0;
        });
        Tools.Instance.OnPressExitObj(m_move, (GameObject evn) =>
        {
            this.dir.x = 0;
            this.dir.y = 0;
        });

        Tools.Instance.OnButton(m_setting, () =>
        {
            //tttt
            //if (CameraFollow.self.isView1 == true)
            //{
            //    CameraFollow.self.isView1 = false;
            //}
            //else
            //{
            //    CameraFollow.self.isView1 = true;
            //}
            SettingView.ShowView();
        });
        m_test.SetActive(false);
        Tools.Instance.OnButton(m_test, () =>
        {
            MaxSdkAndroid.ShowMediationDebugger();
        });

        Tools.Instance.OnButton(m_ad, () =>
        {
            ShopView.ShowView();
        });
        //m_setting.GetComponent<Button>().onClick.AddListener(() =>
        //{

        //});
        m_upHuman.SetActive(false);
        m_gold.GetComponent<Text>().text = "$" + Tools.Instance.getGoldK(DataAll.Instance.gold);

        AddListenerMessage("updateGold", (EventArg arg) =>
        {
            m_gold.GetComponent<Text>().text = "$" + Tools.Instance.getGoldK(DataAll.Instance.gold);
        });

        initAd();
        initLi();

        
        //DataAll.Instance.gold = 20000;
    }
    Vector2 huan = new Vector2(0, 0);
    private Vector2 moveOri = new Vector2();
    private Vector2 dir = new Vector2();

    public float newAdT = 0;

    string ui1path = "Image/Atlas/uplevel";
    SpriteAtlas uiA1 = null;


    bool isMaxad = false;
    int yesIndex = 0;
    List<string> adstr = new List<string>() { "xr_3", "xr_2_0", "xr_1_0" };

    void initAd()
    {
        randowAd();

        Tools.Instance.OnButton(m_upHuman, (GameObject g) =>
        {
            GoogleAd.self.video.show((data) =>
            {
                if (data.isOk == true)
                {
                    if (yesIndex == 0)
                    {
                        DataAll.Instance.addHuman++;
                        GameScene.self.mapControl.newNpc();

                    }
                    else if (yesIndex == 1)
                    {
                        DataAll.Instance.speed++;
                    }
                    else
                    {
                        DataAll.Instance.addObj++;
                    }
                    m_upHuman.SetActive(false);
                    Tools.Instance.showTip("Succeed");
                }
            });
 
        });
    }

    void randowAd()
    {
        isMaxad = true;
        if (DataAll.Instance.addHuman < GameConfig.self.upLevelMax)
        {
            yesIndex = 0;
            isMaxad = false;
        }
        else if (DataAll.Instance.speed < GameConfig.self.upLevelMax)
        {
            yesIndex = 1;
            isMaxad = false;
        }
        else if (DataAll.Instance.addObj < GameConfig.self.upLevelMax)
        {
            yesIndex = 2;
            isMaxad = false;
        }
        m_icon.GetComponent<Image>().sprite = uiA1.GetSprite(adstr[yesIndex]);
    }


    private void Update()
    {
        if (GameScene.self.player != null)
        {
            GameScene.self.player.go(this.dir);

        }

        if (DataAll.Instance.isOnePlay > 20)
        {
            newAdT += Time.deltaTime;
            if (newAdT > GameConfig.self.adNew)
            {
                newAdT = 0;
                //tttt
                //m_upHuman.SetActive(false);
                m_upHuman.SetActive(true);
                randowAd();
            }
            if (newAdT > 20)
            {
                m_upHuman.SetActive(false);
            }
            insertT -= Time.deltaTime;
            if (insertT < 0)
            {
                insertT = GameConfig.self.inseAd;
                GoogleAd.self.insertAd.Show();
            }
        }

        //Debug.Log("gasgasafsgas:" + Tools.Instance.getTime());
    }

    float insertT = GameConfig.self.inseAd;

    void initLi()
    {
        int nowT = int.Parse(DataAll.Instance.timeLi);
        int gold = (Tools.Instance.getTime() - nowT);

        //Debug.LogError("当前时间金币：：：" + nowT + "     " + Tools.Instance.getTime());

        gold = gold / 6;
        int max = 2000;
        if (DataAll.Instance.openLevel.Split(',').Length >= 2)
        {
            gold *= 2;
            max *= 2;
        }

        if (gold > max)
        {
            gold = max;
        }

        SelectView.getGold = gold;
        SelectView.ShowView(true, 1);

        
    }


}
