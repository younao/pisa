using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapControl : BaseScript
{
    // Start is called before the first frame update

    public GameObject deskPos = null;//桌子坐标
    public GameObject waimaiPos = null;//外卖
    public GameObject men = null;//门
    public GameObject getPos = null;//获取披萨坐标
    public GameObject up = null;//显示升级的房间
    public GameObject goPos = null;//ai移动的坐标
    public GameObject newWorkerPos = null;//员工生成点
    public GameObject obj = null;//场景的物体
    public GameObject speedEat = null;//狂吃
    public GameObject enterPos = null;

    public GameObject onePlay = null;

    private void Awake()
    {
        deskPos = GetOneByName("deskPos");
        waimaiPos = GetOneByName("waimaiPos");
        men = GetOneByName("men");
        getPos = GetOneByName("getPos");
        up = GetOneByName("up");
        goPos = GetOneByName("goPos");
        obj = GetOneByName("obj");
        enterPos = GetOneByName("enterPos", goPos);
        newWorkerPos = GetOneByName("newWorkerPos");
        speedEat = GetOneByName("speedEat");
        speedEatIndex = int.Parse(speedEat.transform.GetChild(0).name.Split('_')[1]);
        speedEat.SetActive(false);

        if (DataAll.Instance.getFood.Contains(speedEatIndex) == true)
        {
            setSpeedEat();
        }


    }

    void Start()
    {
        setDesk();
        setFood();
        setMen();

        setWaiMai();
        for (int i = 0; i < DataAll.Instance.addHuman; i++)
        {
            newNpc();
        }

        if (DataAll.Instance.level == 0)
        {
            onePlayer();
        }

    }

    private void Update()
    {
        updateAi();
        updateDesk();
        updateMen();
    }

    #region 员工处理

    /// <summary>
    /// 工作员工
    /// </summary>
    public List<Npc> workerList = new List<Npc>();

    /// <summary>
    /// 增加新员工
    /// </summary>
    public void newNpc()
    {
        //tttt
        //return;
        GameObject pre = GameScene.self.pre.transform.Find("npc").gameObject;
        GameObject g = GameObject.Instantiate(pre);
        g.transform.SetParent(newWorkerPos.transform);
        g.transform.position = new Vector3(newWorkerPos.transform.position.x, GameScene.self.addPlayerY, newWorkerPos.transform.position.z);
        workerList.Add(g.GetComponent<Npc>());
        addYanEff(g.transform.position);
    }

    #endregion

    #region 处理桌子
    /// <summary>
    /// 所有要处理的物体
    /// </summary>
    public List<CheckPos> checkPosList = new List<CheckPos>();

    /// <summary>
    /// 锁住的桌子
    /// </summary>
    public List<DeskLock> DeskLockList = new List<DeskLock>();

    /// <summary>
    /// 购买了桌子
    /// </summary>
    public List<DeskOpen> DeskOpenList = new List<DeskOpen>();

    void setDesk()
    {
        for (int i = 0; i < deskPos.transform.childCount; i++)
        {
            GameObject g = deskPos.transform.GetChild(i).gameObject;
            if (DataAll.Instance.isOnePlay > 0)
            {
                if (i <= DataAll.Instance.deskIndex)
                {
                    setLock(g, false, i);
                }
                else
                {
                    setLock(g, true, i);
                }
            }
            else
            {
                setLock(g, true, i);
            }

        }
        openDesk();
        openWaimai();
        openFood();
    }
    void setLock(GameObject g, bool isLock, int index)
    {
        string deskty = g.name.Split('_')[0];
        if (isLock == true)
        {
            g.SetActive(false);

            int opgold =  GameConfig.self.openDesk + index * 200;

            if (DataAll.Instance.isOnePlay < 20)
            {
                if (index <= 1)
                {
                    opgold = 100;
                }
            }
            if (deskty != "zhuo")
            {
                opgold = GameConfig.self.openDesk4;
            }
            else
            {
                if (index >= 2)
                {
                    opgold = 2000;
                }
            }
            DeskLock lockobj = new DeskLock(g, obj, opgold, index);

            DeskLockList.Add(lockobj);
            if (index == 0)
            {
                onePlay3obj = lockobj.lockobj;
            }
            

            CheckPos p = new CheckPos("deck", lockobj.lockobj, 100, 1, (CheckPos cp) =>
            {
                DeskLockList.Remove(lockobj);
                addYanEff(lockobj.lockobj.transform.position);
                Tools.Instance.LaterFun(() =>
                {
                    if (DataAll.Instance.isOnePlay < 20)
                    {
                        if (index == 1)
                        {
                            onePlay5();
                        }
                        if (index == 2)
                        {
                            //onePlay9();
                        }
                    }

                    setLock(g, false, index);
                }, 0.5f);
            });
            p.foodMax = opgold;
            p.updateFun = (CheckPos cp, bool isdic) =>  //检测解锁桌子
              {
                  int state = 0;
                  Material mat = cp.obj.GetComponent<MeshRenderer>().material;

                  if (isdic == true)
                  {
                      if (DataAll.Instance.gold >= p.foodMax / 10)
                      {

                          cp.enterT += 1;
                          float t = 100 - cp.enterT * 10;
                          p.foodNum += (p.foodMax / 10);
                          DataAll.Instance.gold -= (p.foodMax / 10);
                          GameObject ui = lockobj.lockobj.transform.GetChild(0).gameObject;
                          Text num = ui.transform.Find("num").GetComponent<Text>();
                          num.text = "$" + (cp.foodMax - p.foodNum);

                          for (int k = 0; k < 4; k++)
                          {
                              GameObject pre = GameScene.self.pre.transform.Find("gold").gameObject;
                              GameObject goldobj = GameObject.Instantiate(pre);
                              goldobj.transform.SetParent(obj.transform);
                              huan4.x = GameScene.self.player.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f);
                              huan4.y = GameScene.self.player.transform.position.y;
                              huan4.z = GameScene.self.player.transform.position.z + UnityEngine.Random.Range(-0.2f, 0.2f);
                              goldobj.transform.position = huan4;
                              Tools.Instance.mono.flyEff(goldobj, cp.obj.transform.position, 0.2f, null, null, true, 0.03f * k);
                          }

                          if (t <= 0)
                          {
                              t = 0;
                          }
                          mat.SetFloat("_Rate", t);
                          if (cp.enterT >= 10)
                          {
                              state = 1;
                              if (DataAll.Instance.isOnePlay == 2)
                              {
                                  onePlay3();
                              }

                          }
                      }
                      if (DataAll.Instance.isOnePlay < 20)
                      {
                          onePlayStartTip(lockobj.lockobj, false);
                      }

                  }
                  else
                  {
                      if (DataAll.Instance.isOnePlay < 20)
                      {
                          onePlayStartTip(lockobj.lockobj, true);
                      }
                  }
                  return state;
              };
            checkPosList.Add(p);
        }
        else
        {
            DeskOpen info = new DeskOpen(g);
            if (index == 0)
            {
                Debug.Log("解锁：：：：：：" + DataAll.Instance.isOnePlay);
                if (DataAll.Instance.isOnePlay == 3)
                {
                    info.onePlay = true;
                }
            }
            DeskOpenList.Add(info);
            g.SetActive(true);
            CheckPos p = new CheckPos("deskFood", info.foodPos, 100, 1.5f, (CheckPos cp) =>
            {

            });
            p.foodData = info;
            p.updateFun = checkDeskFood;


            if (deskty == "zhuo")
            {
                p.foodTime = GameConfig.self.aiEatTime * 2;
            }
            else
            {
                p.foodTime = GameConfig.self.aiEatTime;
            }


            p.updateYesTime = GameConfig.self.giveFoodTime;
            checkPosList.Add(p);

            CheckPos goldp = new CheckPos("gold", info.goldPos, 100, 0.8f, (CheckPos cp) =>
            {

            });

            if (index > DataAll.Instance.deskIndex)
            {
                DataAll.Instance.deskIndex = index;
                if (DataAll.Instance.isOnePlay > 20)
                {
                    openDesk();
                    openWaimai();
                    openFood();
                }
            }

            goldp.foodData = info;
            goldp.updateFun = getGold;
            checkPosList.Add(goldp);
        }
    }

    void openDesk()
    {

        for (int i = 0; i < DeskLockList.Count; i++)
        {
            if (DataAll.Instance.isOnePlay > 1)
            {
                if (DeskLockList[i].index <= DataAll.Instance.deskIndex + 1)
                {
                    DeskLockList[i].lockobj.SetActive(true);
                }
                else
                {
                    DeskLockList[i].lockobj.SetActive(false);
                }
            }
            else
            {
                DeskLockList[i].lockobj.SetActive(false);
            }
        }


    }

    Vector3 huan4 = new Vector3();
    /// <summary>
    /// 客人吃食物
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="isdic"></param>
    /// <returns></returns>
    int checkDeskFood(CheckPos cp, bool isdic)
    {
        int state = 0;
        if (isdic == true)
        {
            cp.enterT += 1;
            if (cp.enterT >= 2)
            {
                GameScene.self.player.giveFood(cp);
            }
        }
        else
        {
            cp.enterT = 0;
        }
        cp.foodT += cp.updateT;
        if (cp.foodT > cp.foodTime)
        {
            cp.foodT = 0;
            bool isHaveAi = false;
            for (int i = 0; i < cp.foodData.chairList.Count; i++)
            {
                ChairInfo cinfo = cp.foodData.chairList[i];
                if (cinfo.Ai != null)
                {
                    if (cp.foodData.foodPos.transform.childCount > 0)
                    {
                        cinfo.Ai.GetComponent<Ai>().isEat = true;
                    }
                    else
                    {
                        cinfo.Ai.GetComponent<Ai>().isEat = false;
                    }
                    isHaveAi = true;
                }
            }
            if (isHaveAi == true)
            {
                if (cp.foodData.foodPos.transform.childCount > 0)
                {
                    cp.eatFoodNum++;
                    int indexf = cp.foodData.foodPos.transform.childCount - 1;
                    GameObject.Destroy(cp.foodData.foodPos.transform.GetChild(indexf).gameObject);
                    addGold(cp.foodData.goldPos, newGold);

                    if (cp.eatFoodNum > 45)
                    {
                        cp.eatFoodNum = 0;
                        for (int i = 0; i < cp.foodData.chairList.Count; i++)
                        {
                            ChairInfo cinfo = cp.foodData.chairList[i];
                            if (cinfo.Ai != null)
                            {
                                cinfo.Ai.GetComponent<Ai>().exitGo(cp.foodData, i);
                            }
                        }
                    }
                }
            }
        }
        checkDeskFoodWorder(cp);
        return state;
    }
    //工人桌子事件
    void checkDeskFoodWorder(CheckPos cp)
    {
        for (int i = 0; i < workerList.Count; i++)
        {
            Transform tran = workerList[i].transform;
            float[] a = { cp.obj.transform.position.x, cp.obj.transform.position.z };
            float[] b = { tran.position.x, tran.position.z };
            bool isdic = Tools.Instance.isDic2d(a, b, cp.dic);
            if (isdic == true)
            {
                workerList[i].giveFood(cp);
            }
        }
    }

    //金币排布
    float goldOffx = 0.25f;
    float goldOffz = 0.13f;
    float goldOffy = 0.08f;
    int goldXmax = 3;
    int goldZmax = 5;

    public int newGold
    {
        get
        {
            return GameConfig.self.newGold + DataAll.Instance.newGold * 5;
        }
    }

    /// <summary>
    /// 添加钱
    /// </summary>
    /// <param name="foodpar"></param>
    /// <param name="gold"></param>
    void addGold(GameObject foodpar, float gold)
    {
        int length = foodpar.transform.childCount;

        int Py = length / (goldXmax * goldZmax);
        int PP = length % (goldXmax * goldZmax);
        int Px = PP / goldZmax;
        int Pz = PP % goldZmax;

        GameObject pre = GameScene.self.pre.transform.Find("gold").gameObject;
        GameObject g = GameObject.Instantiate(pre);
        g.transform.SetParent(foodpar.transform);
        huan4.x = foodpar.transform.position.x + goldOffx * Px - 0.25f;
        huan4.y = foodpar.transform.position.y + goldOffy * Py;
        huan4.z = foodpar.transform.position.z + goldOffz * Pz - 0.08f;
        g.transform.position = huan4;

        float ranMax = 5;

        huan4.x = g.transform.eulerAngles.x + UnityEngine.Random.Range(-ranMax, ranMax);
        huan4.y = g.transform.eulerAngles.y + UnityEngine.Random.Range(-ranMax, ranMax);
        huan4.z = g.transform.eulerAngles.z + UnityEngine.Random.Range(-ranMax, ranMax);

        g.transform.eulerAngles = huan4;

        g.name = gold.ToString();

        //g.transform.localPosition
    }

    /// <summary>
    /// 自己获取到金币
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="isdic"></param>
    /// <returns></returns>
    int getGold(CheckPos cp, bool isdic)
    {
        int state = 0;
        //if(cp.foodData.desk.name== "zhuo_0 (2)")
        //{
        //    Debug.Log("金币计较：：：：" + isdic);
        //}

        if (isdic == true)
        {
            cp.enterT += 1;
            if (cp.enterT >= 2)
            {
                GameScene.self.player.getGold(cp);
            }
        }
        else
        {
            cp.enterT = 0;
        }
        return state;
    }

    float checkDeskT = 0;
    float checkDeskTime = 0.15f;

    void checkDesk()
    {
        checkDeskT += Time.deltaTime;
        if (checkDeskT > checkDeskTime)
        {
            checkDeskT = 0;
            for (int i = 0; i < DeskOpenList.Count; i++)
            {
                DeskOpen info = DeskOpenList[i];
                info.updateT += checkDeskTime;
                info.update();
                for (int j = 0; j < info.chairList.Count; j++)
                {
                    ChairInfo chair = info.chairList[j];

                    if (chair.isOpen == true)//
                    {
                        if (waitYesAi != null)
                        {
                            //Debug.Log("增加人物进：" + DeskOpenList.Count);
                            chair.isOpen = false;
                            waitAiList.Remove(waitYesAi);
                            waitYesAi.setGoEat(info, j);

                            waitYesAi = null;
                            reSetAiGroup();
                        }
                    }
                }
            }
        }
    }

    void updateDesk()
    {
        checkDesk();
    }

    #endregion

    #region 处理外卖

    /// <summary>
    /// 摩托车
    /// </summary>
    Ai motoAi = null;

    void setWaiMai()
    {
        setMoto();
        List<int> waimailist = new List<int>();
        waimailist = DataAll.Instance.waimai;
        for (int i = 0; i < waimaiPos.transform.childCount; i++)
        {
            if (waimaiPos.transform.GetChild(i).name.Split('_')[0] == "waimai")
            {
                GameObject g = waimaiPos.transform.GetChild(i).gameObject;
                if (waimailist.Contains(i) == true)
                {
                    setLockWaimai(g, false, i);
                }
                else
                {
                    setLockWaimai(g, true, i);
                }
            }
        }
        openWaimai();
    }

    void setMoto()
    {
        Transform car = waimaiPos.transform.Find("car");
        GameObject pre = GameScene.self.pre.transform.Find("moto").gameObject;
        GameObject moto = GameObject.Instantiate(pre);
        moto.transform.SetParent(car);
        Vector3 startPos = goPos.transform.Find("motoPos").Find("startPos").position;
        moto.transform.position = startPos;
        motoAi = moto.GetComponent<Ai>();
        motoAi.isMoto = true;

        DeskOpen info1 = new DeskOpen(car.gameObject);
        CheckPos phe = new CheckPos("car", info1.foodPos, 100, 1.6f, (CheckPos cp) =>
        {

        });
        phe.foodData = info1;
        phe.updateFun = waimaiCarFood;
        phe.updateYesT = GameConfig.self.giveFoodTime;
        checkPosList.Add(phe);

        CheckPos goldp = new CheckPos("gold", info1.goldPos, 100, 0.8f, (CheckPos cp) =>
        {

        });
        goldp.foodData = info1;
        goldp.updateFun = getGold;
        checkPosList.Add(goldp);
    }

    List<DeskLock> waimaiLockList = new List<DeskLock>();

    void openWaimai()
    {

        for (int i = 0; i < waimaiLockList.Count; i++)
        {

            if (DataAll.Instance.deskIndex >= GameConfig.self.setWaimai)
            {
                waimaiLockList[i].lockobj.SetActive(true);
            }
            else
            {
                waimaiLockList[i].lockobj.SetActive(false);
            }
        }
    }

    void setLockWaimai(GameObject g, bool isLock, int index)
    {
        if (isLock == true)
        {
            g.SetActive(false);

            int gggg = GameConfig.self.openWaimai * (index + 1);

            if (index == 0)
            {
                gggg = 1000;
            }
            else
            {
                gggg = 3500;
            }

            DeskLock lockobj = new DeskLock(g, obj, gggg, 0);
            waimaiLockList.Add(lockobj);
            CheckPos p = new CheckPos("waimaiLock", lockobj.lockobj, 100, 1, (CheckPos cp) =>
            {
                waimaiLockList.Remove(lockobj);
                addYanEff(lockobj.lockobj.transform.position);
                Tools.Instance.LaterFun(() =>
                {
                    setLockWaimai(g, false, index);
                }, 0.5f);
                if (index == 0)
                {
                    if (DataAll.Instance.isOnePlay < 20)
                    {
                        onePlay8();
                    }
                }
            });
            p.foodMax = gggg;
            p.updateFun = (CheckPos cp, bool isdic) =>  //检测解锁桌子
            {
                int state = 0;
                Material mat = cp.obj.GetComponent<MeshRenderer>().material;

                if (isdic == true)
                {
                    if (DataAll.Instance.gold >= p.foodMax / 10)
                    {

                        cp.enterT += 1;
                        float t = 100 - cp.enterT * 10;
                        p.foodNum += (p.foodMax / 10);
                        DataAll.Instance.gold -= (p.foodMax / 10);
                        GameObject ui = lockobj.lockobj.transform.GetChild(0).gameObject;
                        Text num = ui.transform.Find("num").GetComponent<Text>();
                        num.text = "$" + (cp.foodMax - p.foodNum);

                        for (int k = 0; k < 4; k++)
                        {
                            GameObject pre = GameScene.self.pre.transform.Find("gold").gameObject;
                            GameObject goldobj = GameObject.Instantiate(pre);
                            goldobj.transform.SetParent(obj.transform);
                            huan4.x = GameScene.self.player.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f);
                            huan4.y = GameScene.self.player.transform.position.y;
                            huan4.z = GameScene.self.player.transform.position.z + UnityEngine.Random.Range(-0.2f, 0.2f);
                            goldobj.transform.position = huan4;
                            Tools.Instance.mono.flyEff(goldobj, cp.obj.transform.position, 0.2f, null, null, true, 0.03f * k);
                        }

                        if (t <= 0)
                        {
                            t = 0;
                        }
                        mat.SetFloat("_Rate", t);
                        if (cp.enterT >= 10)
                        {
                            state = 1;
                        }
                    }
                    if (DataAll.Instance.isOnePlay < 20)
                    {
                        onePlayStartTip(lockobj.lockobj, false);
                    }
                }
                else
                {
                    if (DataAll.Instance.isOnePlay < 20)
                    {
                        onePlayStartTip(lockobj.lockobj, true);
                    }
                }

                return state;
            };
            checkPosList.Add(p);
        }
        else
        {
            DeskOpen info = new DeskOpen(g);
            DeskOpenList.Add(info);
            g.SetActive(true);
            CheckPos p = new CheckPos("waimai", info.foodPos, 100, 1f, (CheckPos cp) =>
            {

            }, g);
            p.foodData = info;
            p.updateFun = checkWaimaiFood;
            p.updateYesTime = GameConfig.self.giveFoodTime;
            p.foodTime = GameConfig.self.waimaiNewTime;
            checkPosList.Add(p);

            DeskOpen info1 = new DeskOpen(g);
            GameObject gg = g.transform.Find("goldPos").gameObject;
            CheckPos phe = new CheckPos("waimaihe", gg, 100, 1f, (CheckPos cp) =>
            {

            }, g);

            if (DataAll.Instance.waimai.Contains(index) == false)
            {
                List<int> l = Tools.Instance.cloneList(DataAll.Instance.waimai);
                l.Add(index);
                DataAll.Instance.waimai = l;
            }

            phe.foodData = info1;
            phe.foodData.foodPos = gg;
            phe.updateFun = waimaiFood;
            checkPosList.Add(phe);
            if (isOneMotoGo == false)
            {
                isOneMotoGo = true;
                MotoGo();
            }
        }
    }
    bool isOneMotoGo = false;
    void MotoGo()
    {
        List<Vector3> go = new List<Vector3>();
        Vector3 yesPos = goPos.transform.Find("motoPos").Find("yesPos").position;
        go.Add(yesPos);
        motoAi.setGopos(go);
        motoAi.goOkFun = null;
    }

    public void motoExit()
    {
        GameObject car = waimaiPos.transform.Find("car").gameObject;
        Transform foodPos = GetGameObjectByName("foodPos", car).transform;
        Transform goldPos = GetGameObjectByName("goldPos", car).transform;

        List<Vector3> go = new List<Vector3>();
        Vector3 yesPos = goPos.transform.Find("motoPos").Find("exitPos").position;
        go.Add(yesPos);
        motoAi.setGopos(go);
        motoAi.goOkFun = (Ai my) =>
        {
            for (int i = 0; i < foodPos.childCount; i++)
            {
                GameObject.Destroy(foodPos.transform.GetChild(i).gameObject);
            }

            Vector3 startPos = goPos.transform.Find("motoPos").Find("startPos").position;
            motoAi.transform.position = startPos;
            Tools.Instance.LaterFun(() =>
            {
                MotoGo();
            }, 0.1f);

        };


        for (int i = 0; i < foodPos.childCount; i++)
        {
            Tools.Instance.LaterFun(() =>
            {
                addGold(goldPos.gameObject, newGold);
            }, 0.1f * i);
        }

    }

    int waimaiFood(CheckPos cp, bool isdic)
    {
        int state = 0;
        if (isdic == true)
        {
            cp.enterT += 1;
            if (cp.enterT >= 2)
            {
                GameScene.self.player.getFood(cp, 1);
            }
        }
        else
        {
            cp.enterT = 0;
        }
        return state;
    }

    int waimaiCarFood(CheckPos cp, bool isdic)
    {
        int state = 0;
        if (isdic == true)
        {
            cp.enterT += 1;
            if (cp.enterT >= 2)
            {
                GameScene.self.player.giveFood(cp, 1);
            }
        }
        else
        {
            cp.enterT = 0;
        }

        checkDeskFoodWorder(cp);
        return state;
    }

    Vector3 huan6 = new Vector3();
    /// <summary>
    /// 生成外卖
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="isdic"></param>
    /// <returns></returns>
    int checkWaimaiFood(CheckPos cp, bool isdic)
    {
        int state = 0;
        if (isdic == true)
        {
            cp.enterT += 1;
            if (cp.enterT >= 2)
            {
                GameScene.self.player.giveFood(cp, 0);
            }
        }
        else
        {
            cp.enterT = 0;
        }
        cp.foodT += cp.updateYesTime;
        if (cp.foodT > cp.foodTime)
        {
            cp.foodT = 0;
            Transform hefoodPos = cp.data.transform.Find("foodPos");
            Transform par = cp.data.transform.Find("newPos");
            Transform hePar = cp.data.transform.Find("goldPos");
            Transform tip = cp.data.transform.Find("tip");
            Transform maxTip = tip.GetChild(0);
            if (hePar.childCount < GameConfig.self.waimaiHeMax)
            {
                tip.gameObject.SetActive(false);
                if (hefoodPos.transform.childCount > 0)
                {
                    GameObject pre = GameScene.self.pre.transform.Find("heObj").gameObject;
                    GameObject he = GameObject.Instantiate(pre);
                    he.SetActive(par);
                    he.transform.position = par.position;
                    he.GetComponent<Animator>().SetBool("open", true);

                    GameObject pisi = hefoodPos.GetChild(hefoodPos.childCount - 1).gameObject;
                    pisi.transform.SetParent(obj.transform);
                    Tools.Instance.LaterFun(() =>
                    {
                        Tools.Instance.mono.flyEff(pisi, par.transform.position, 0.2f, null, null, true);
                    }, 0.2f);

                    Tools.Instance.LaterFun(() =>
                    {
                        he.transform.SetParent(hePar);

                        huan6.x = hePar.transform.position.x;
                        huan6.y = hePar.transform.position.y + hePar.transform.childCount * 0.08f;
                        huan6.z = hePar.transform.position.z;
                        Tools.Instance.mono.flyEff(he, huan6, 0.25f);
                    }, 0.5f);
                }

            }
            else
            {
                tip.gameObject.SetActive(true);
                maxTip.LookAt(CameraFollow.self.transform);
            }

        }
        checkDeskFoodWorder(cp);
        return state;
    }

    void updateWaiMai()
    {

    }

    #endregion

    #region 处理客人

    /// <summary>
    /// 正在吃饭的人
    /// </summary>
    public List<Ai> eatAiList = new List<Ai>();
    /// <summary>
    /// 等候吃饭的人
    /// </summary>
    public List<Ai> waitAiList = new List<Ai>();

    public Ai waitYesAi = null;

    float maxAi = 6;
    float newAiT = 0;
    float newAiTime = 1;
    void updateAi()
    {
        newAiT += Time.deltaTime;
        if (newAiT > newAiTime)
        {
            newAiT = 0;
            newAi();
            checkAi();
        }
    }

    void newAi()
    {
        if (waitAiList.Count < maxAi)
        {
            GameObject pre = GameScene.self.pre.transform.Find("ai").gameObject;
            GameObject g = GameObject.Instantiate(pre);
            g.transform.SetParent(obj.transform);
            GameObject pos = GetOneByName("npcCome", goPos);
            GameObject oriobj = pos.transform.GetChild(0).gameObject;
            Vector3 ori = new Vector3(oriobj.transform.position.x, GameScene.self.player.transform.position.y, oriobj.transform.position.z);
            g.transform.position = ori;
            Ai ai = g.GetComponent<Ai>();

            List<Vector3> poslist = new List<Vector3>();
            poslist.Add(pos.transform.GetChild(0).transform.position);
            Vector3 endPos = pos.transform.GetChild(1).transform.position;
            Vector3 everPos = new Vector3(endPos.x - waitAiList.Count * 0.5f, endPos.y, endPos.z);
            poslist.Add(everPos);
            ai.setGopos(poslist);
            waitAiList.Add(ai);

            Vector3 yesWaitPos = new Vector3(endPos.x, endPos.y, endPos.z);
            ai.goOkFun = (Ai my) =>
            {
                float[] a = { my.transform.position.x, my.transform.position.z };
                float[] b = { yesWaitPos.x, yesWaitPos.z };
                bool isdic = Tools.Instance.isDic2d(a, b, 0.1f);
                if (isdic == true)
                {
                    waitYesAi = my;
                }

            };
        }
    }

    void checkAi()
    {
        if (waitYesAi == null)
        {
            waitYesAi = waitAiList[0];
        }
    }

    /// <summary>
    /// 重新设置排队
    /// </summary>
    public void reSetAiGroup()
    {
        GameObject pos = GetOneByName("npcCome", goPos);
        Vector3 ovrpos = pos.transform.GetChild(1).transform.position;
        waitAiList.Sort((Ai a, Ai b) =>
        {
            return -a.transform.position.x.CompareTo(b.transform.position.x);
        });
        for (int i = 0; i < waitAiList.Count; i++)
        {
            Ai ai = waitAiList[i];
            GameObject g = ai.gameObject;

            List<Vector3> poslist = new List<Vector3>();

            Vector3 everPos = new Vector3(ovrpos.x - i * 0.5f, ovrpos.y, ovrpos.z);
            //Debug.Log("ai坐标：：：" + g.transform.position.x+"::"+i+"::"+ everPos);
            poslist.Add(everPos);
            ai.setGopos(poslist);
        }
    }

    #endregion

    #region 处理食物
    void setFood()
    {
        List<int> flist = new List<int>();
        flist = DataAll.Instance.getFood;
        for (int i = 0; i < getPos.transform.childCount; i++)
        {
            GameObject g = getPos.transform.GetChild(i).gameObject;
            if (DataAll.Instance.isOnePlay > 0)
            {
                if (flist.Contains(i) == true)
                {
                    setFoodLock(g, false, i);
                }
                else
                {
                    setFoodLock(g, true, i);
                }
            }
            else
            {
                setFoodLock(g, true, i);
            }

        }
        openFood();
        if (DataAll.Instance.isOnePlay < 2)
        {
            FoodLockList[0].lockobj.SetActive(true);
        }

    }

    void openFood()
    {
        for (int i = 0; i < FoodLockList.Count; i++)
        {

            if (DataAll.Instance.deskIndex >= GameConfig.self.setFood)
            {
                FoodLockList[i].lockobj.SetActive(true);
            }
            else
            {
                FoodLockList[i].lockobj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 锁住的食物机
    /// </summary>
    public List<DeskLock> FoodLockList = new List<DeskLock>();

    /// <summary>
    /// 食物机
    /// </summary>
    public List<DeskOpen> FoodOpenList = new List<DeskOpen>();

    Vector3 huan3 = new Vector3();
    void setFoodLock(GameObject g, bool isLock, int index)
    {
        if (isLock == true)
        {
            g.SetActive(false);
            int gggg = GameConfig.self.openFood;
            if (index == 0 || index == 1)
            {
                gggg = 100+ index*100;
            }
            if (index ==3)
            {
                gggg = 1500;    
            }
            DeskLock lockobj = new DeskLock(g, obj, gggg, index);
            GameObject ui = lockobj.lockobj.transform.GetChild(0).gameObject;
            Text num = ui.transform.Find("num").GetComponent<Text>();
            if (index == 2)
            {
                num.text = "free";
            }
            FoodLockList.Add(lockobj);
            CheckPos p = new CheckPos("food", lockobj.lockobj, 100, 1, (CheckPos cp) =>
            {
                ///lockobj.desk.SetActive(true);
                FoodLockList.Remove(lockobj);
                if (DataAll.Instance.isOnePlay < 9)
                {
                    if (index == 2)
                    {
                        onePlay85();
                    }
                }

            });
            p.foodMax = gggg;
            p.updateFun = (CheckPos cp, bool isdic) => //检测解锁食物
            {
                int state = 0;
                Material mat = cp.obj.GetComponent<MeshRenderer>().material;
                bool isOk = false;
                if (isdic == true)
                {
                    if (DataAll.Instance.gold >= p.foodMax / 10)
                    {

                        cp.enterT += 1;

                        float t = 100 - cp.enterT * 10;
                        p.foodNum += (p.foodMax / 10);

                        if (index !=2)
                        {
                            num.text = "$" + (cp.foodMax - p.foodNum);
                            DataAll.Instance.gold -= (p.foodMax / 10);
                            for (int k = 0; k < 4; k++)
                            {
                                GameObject pre = GameScene.self.pre.transform.Find("gold").gameObject;
                                GameObject goldobj = GameObject.Instantiate(pre);
                                goldobj.transform.SetParent(obj.transform);
                                huan4.x = GameScene.self.player.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f);
                                huan4.y = GameScene.self.player.transform.position.y;
                                huan4.z = GameScene.self.player.transform.position.z + UnityEngine.Random.Range(-0.2f, 0.2f);
                                goldobj.transform.position = huan4;
                                Tools.Instance.mono.flyEff(goldobj, cp.obj.transform.position, 0.2f, null, null, true, 0.03f * k);
                            }
                        }

                        mat.SetFloat("_Rate", t);
                        if (cp.enterT >= 10)
                        {
                            cp.enterT = 0;
                            if (DataAll.Instance.isOnePlay < 20)
                            {
                                if (index == 0)
                                {
                                    onePlay2();
                                }
                                if (index == 1)
                                {
                                    onePlay7();
                                }
                            }
                            
                            if (index == 2)
                            {
                                
                                GoogleAd.self.video.show((data) =>
                                {
                                    if (data.isOk == true)
                                    {
                                        state = 1;
                                        isOk = true;
                                        addYanEff(lockobj.lockobj.transform.position);
                                        FoodLockList.Remove(lockobj);
                                        GameObject.Destroy(lockobj.lockobj);
                                        Debug.Log("时间：：：：：：" + index);
                                        Tools.Instance.LaterFun(() =>
                                        {
                                            lockobj.desk.SetActive(true);
                                            
                                            setFoodLock(g, false, index);
                                            
                                        }, 0.5f);
                                    }
                                    else
                                    {
                                        
                                        mat.SetFloat("_Rate", 100);
                                    }
                                });
                            }
                            else
                            {
                                isOk = true;
                                addYanEff(lockobj.lockobj.transform.position);
                                lockobj.lockobj.SetActive(false);
                                Tools.Instance.LaterFun(() =>
                                {
                                    lockobj.desk.SetActive(true);
                                    setFoodLock(g, false, index);
                                    
                                }, 0.5f);
                            }

                            if (isOk == true)
                            {
                                state = 1;
                            }
                        }
                    }
                    if (DataAll.Instance.isOnePlay < 20)
                    {
                        onePlayStartTip(lockobj.lockobj, false);
                    }

                }
                else
                {
                    if (DataAll.Instance.isOnePlay < 20)
                    {
                        onePlayStartTip(lockobj.lockobj, true);
                    }
                }
                return state;
            };
            checkPosList.Add(p);
        }
        else
        {
            DeskOpen info = new DeskOpen(g);
            g.SetActive(true);
            CheckPos p = new CheckPos("food", info.goldPos, 100, 0.85f, (CheckPos cp) =>
            {

            });

            if (DataAll.Instance.getFood.Contains(index) == false)
            {
                List<int> l = Tools.Instance.cloneList(DataAll.Instance.getFood);
                l.Add(index);
                DataAll.Instance.getFood = l;
                openFood();
                if (index == speedEatIndex)
                {
                    setSpeedEat();
                }
            }

            p.foodData = info;
            p.updateFun = checkNewFood;

            Dictionary<int, int> foodl = DataAll.Instance.getFoolLevel();
            int vv = 0;
            if (foodl.ContainsKey(index) == true)
            {
                vv = foodl[index];
            }

            int max = GameConfig.self.newFoodMax + vv * GameConfig.self.upFoodMax;
            float nnt = vv * GameConfig.self.upFoodGetSpeed;
            if (nnt > 0.6f)
            {
                nnt = 0.6f;
            }
            float nt = 1 - nnt;
            p.foodMax = max;
            p.foodTime = nt;
            checkPosList.Add(p);
            FoodOpenList.Add(info);

            GameObject upJi = g.transform.Find("up").gameObject;
            if (DataAll.Instance.isOnePlay < 4)
            {
                upJi.SetActive(false);
            }
            else
            {
                upJi.SetActive(true);
            }
            int upG = GameConfig.self.upFoodLeve;
            if (vv > 0)
            {
                upG = 2000;
            }

            GameObject ui = upJi.transform.GetChild(0).gameObject;
            Text num = ui.transform.Find("num").GetComponent<Text>();
            num.text = "$" + upG;

            CheckPos upP = new CheckPos("upfood", upJi, 100, 0.8f, (CheckPos cp) =>
            {

            });


            Dictionary<int, int> intifood = DataAll.Instance.getFoolLevel();
            bool isfMax = false;
            if (intifood.ContainsKey(index) == true)
            {
                if (intifood[index] > GameConfig.self.maxUpFood)
                {
                    upJi.SetActive(false);
                    isfMax = true;
                }
            }
            if (isfMax == false)
            {
                upP.updateFun = (CheckPos cp, bool isdic) =>
                {
                    int state = 0;
                    Material mat = cp.obj.GetComponent<MeshRenderer>().material;
                    if (isdic == true)
                    {
                        if (DataAll.Instance.gold < (upG / 10))
                        {
                            return 0;
                        }
                        cp.enterT += 1;
                        if (cp.enterT < 0)
                        {
                            return 0;
                        }


                        float t = 100 - cp.enterT * 10;
                        p.foodNum += (upG / 10);

                        DataAll.Instance.gold -= (upG / 10);
                        num.text = "$" + (upG - p.foodNum);

                        for (int k = 0; k < 4; k++)
                        {
                            GameObject pre = GameScene.self.pre.transform.Find("gold").gameObject;
                            GameObject goldobj = GameObject.Instantiate(pre);
                            goldobj.transform.SetParent(obj.transform);
                            huan4.x = GameScene.self.player.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f);
                            huan4.y = GameScene.self.player.transform.position.y;
                            huan4.z = GameScene.self.player.transform.position.z + UnityEngine.Random.Range(-0.2f, 0.2f);
                            goldobj.transform.position = huan4;
                            Tools.Instance.mono.flyEff(goldobj, cp.obj.transform.position, 0.2f, null, null, true, 0.03f * k);
                        }

                        if (t <= 0)
                        {
                            t = 0;
                        }
                        if (t > 100)
                        {
                            t = 100;
                        }
                        mat.SetFloat("_Rate", t);
                        Dictionary<int, int> foodlevel = DataAll.Instance.getFoolLevel();

                        if (cp.enterT >= 10)
                        {
                            p.foodNum = 0;

                            int value = 1;
                            if (foodlevel.ContainsKey(index) == true)
                            {
                                value = foodlevel[index] + 1;
                            }
                            //Debug.Log("ssssssssssssssssssssssssssssss"+index+"  aaa"+value);
                            DataAll.Instance.setFoodLevel(index, value);

                            addYanEff(upP.obj.transform.position);

                            int aamax = GameConfig.self.newFoodMax + value * GameConfig.self.upFoodMax;
                            float aannt = value * GameConfig.self.upFoodGetSpeed;
                            if (aannt > 0.6f)
                            {
                                aannt = 0.6f;
                            }
                            float aant = 1 - aannt;
                            p.foodMax = aamax;
                            p.foodTime = aant;
                            mat.SetFloat("_Rate", 100);
                            cp.enterT = -30;
                            upG = 2000;
                            num.text = "$" + upG;

                            if (foodlevel.ContainsKey(index) == true)
                            {
                                if (foodlevel[index] > GameConfig.self.maxUpFood)
                                {
                                    upJi.SetActive(false);
                                    Tools.Instance.showTip("max");
                                    return 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        cp.enterT = -10;
                        if (cp.enterT != -10)
                        {
                            mat.SetFloat("_Rate", 100);
                        }
                    }
                    return state;
                };
            }
            upP.outFun = (CheckPos cp) =>
            {
                Material mat = cp.obj.GetComponent<MeshRenderer>().material;
                mat.SetFloat("_Rate", 100);
            };
            checkPosList.Add(upP);
        }
    }
    /// <summary>
    /// 检测生成食物拿走食物
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="isdic"></param>
    /// <returns></returns>
    int checkNewFood(CheckPos cp, bool isdic)
    {
        int state = 0;
        if (isdic == true)
        {
            cp.enterT += 1;
            if (cp.enterT >= 2)
            {
                GameScene.self.player.getFood(cp);
            }

        }
        else
        {
            cp.enterT = 0;
        }
        cp.foodT += cp.updateT;
        if (cp.foodT > cp.foodTime)
        {
            cp.foodT = 0;

            bool isokFood = false;
            bool ismax = true;
            for (int i = 0; i < cp.foodData.foodPos.transform.childCount; i++)
            {
                Transform f = cp.foodData.foodPos.transform.GetChild(i);
                if (f.childCount < cp.foodMax)
                {
                    ismax = false;
                    isokFood = true;
                    GameObject pre = GameScene.self.pre.transform.Find("pisi_" + DataAll.Instance.level).gameObject;
                    GameObject pisi = GameObject.Instantiate(pre);
                    pisi.transform.SetParent(f);
                    huan3.x = f.transform.position.x;
                    huan3.y = f.transform.position.y + f.childCount * GameConfig.self.pisiY;
                    huan3.z = f.transform.position.z;
                    pisi.transform.position = huan3;
                    break;
                }
            }

            if (ismax == true)
            {
                Transform par = cp.foodData.foodPos.transform.parent;
                Transform maxTip = par.Find("tip").GetChild(0);
                maxTip.gameObject.SetActive(true);
                maxTip.LookAt(CameraFollow.self.transform);
            }
            else
            {
                Transform par = cp.foodData.foodPos.transform.parent;
                Transform maxTip = par.Find("tip").GetChild(0);
                maxTip.gameObject.SetActive(false);
            }

            if (isokFood == true)
            {
                cp.foodNum++;
            }

        }
        checkNewFoodWorker(cp);
        return state;
    }

    void checkNewFoodWorker(CheckPos cp)
    {
        for (int i = 0; i < workerList.Count; i++)
        {
            Transform tran = workerList[i].transform;
            float[] a = { cp.obj.transform.position.x, cp.obj.transform.position.z };
            float[] b = { tran.position.x, tran.position.z };
            bool isdic = Tools.Instance.isDic2d(a, b, cp.dic);
            if (isdic == true)
            {
                workerList[i].getFood(cp);
            }
        }


    }

    #endregion

    #region 处理门

    List<CheckPos> menPos = new List<CheckPos>();

    void setMen()
    {
        for (int i = 0; i < men.transform.childCount; i++)
        {
            GameObject g = men.transform.GetChild(i).gameObject;
            setMen1(g, i);
            if (DataAll.Instance.isOnePlay <= 4)
            {
                GameObject menobj = GetOneByName("menobj", g);
                menobj.SetActive(false);
            }
            else
            {
                openMenIndex.Add(i);
            }
        }
        if (DataAll.Instance.level == 0)
        {
            if (DataAll.Instance.openMen == 0)
            {
                onePlayMen(men.transform.GetChild(1).gameObject, 1);
            }
        }
    }

    void setMen1(GameObject g, int i)
    {

        GameObject menObj = g.transform.Find("men_Pre").gameObject;
        CheckPos p = new CheckPos("menOpen", menObj, 100, 1.2f, (CheckPos cp) =>
        {
        });
        p.foodMax = i;
        p.updateFun = (CheckPos cp, bool isdic) =>  //
        {
            int state = 0;

            if (isdic == true)
            {
                menObj.GetComponent<Animator>().SetBool("open", true);
                cp.foodNum = 1;
            }
            else
            {
                menObj.GetComponent<Animator>().SetBool("open", false);
                cp.foodNum = 0;
            }
            return state;
        };
        p.onUpdate = (CheckPos cp, bool isdic) =>  //
        {
            int state = 0;

            return state;
        };
        checkPosList.Add(p);
        menPos.Add(p);

        GameObject show = g.transform.Find("show").gameObject;
        CheckPos showP = new CheckPos("menShow", show, 100, 0.8f, (CheckPos cp) =>
        {
        });
        showP.foodMax = i;
        Debug.Log("门：：：1111：：：" + openMenIndex.Count);
        showP.updateFun = (CheckPos cp, bool isdic) =>  //
        {
            int state = 0;
            cp.updateT++;
            if (isdic == true)
            {
                Debug.Log("门：：：：：：" + openMenIndex.Count);
                if (cp.updateT >= 2)
                {
                    cp.updateT = 0;
                    if (cp.foodNum == 0)
                    {
                        cp.foodNum = 1;

                        if (openMenIndex.Contains(cp.foodMax) == true)
                        {
                            if (cp.foodMax == 0)
                            {
                                UplevelView.ismy = false;
                                UplevelView.ShowView();
                            }
                            else
                            {
                                if (DataAll.Instance.openMen != 0)
                                {
                                    UplevelView.ismy = true;
                                    UplevelView.ShowView();
                                }

                            }
                        }

                    }
                }
            }
            else
            {
                if (cp.foodNum == 1)
                {
                    cp.foodNum = 0;

                }
            }
            return state;
        };
        checkPosList.Add(showP);
    }

    void updateMen()
    {
        for (int i = 0; i < menPos.Count; i++)
        {
            if (menPos[i] != null)
            {
                menPos[i].onUpdate(menPos[i], true);
            }
        }
    }
    #endregion

    #region 狂吃

    bool isSpeedEat = false;
    int speedEatIndex = 2;
    void setSpeedEat()
    {
        if (isSpeedEat == true)
        {
            return;
        }
        isSpeedEat = true;
        GameObject g = speedEat.transform.GetChild(0).gameObject;
        speedEat.SetActive(true);
        Transform ori = getPos.transform.GetChild(speedEatIndex);

        DeskLock lockobj = new DeskLock(g, obj, 0, 0);
        GameObject ui = lockobj.lockobj.transform.GetChild(0).gameObject;
        Text num = ui.transform.Find("num").GetComponent<Text>();
        num.text = "free";
        DeskOpen info = new DeskOpen(g);

        GameObject foodPos = GetGameObjectByName("foodPos", g);
        GameObject goldPos = GetGameObjectByName("goldPos", g);
        Animator eatAi = GetGameObjectByName("eatAi", g).transform.Find("anim").GetComponent<Animator>();

        GameObject preEff = GameScene.self.pre.transform.Find("eff_eat").gameObject;
        GameObject eff = GameObject.Instantiate(preEff);
        eff.transform.SetParent(obj.transform);

        Vector3 effpos = ori.transform.position;
        effpos.y += 1;

        eff.transform.position = effpos;
        eff.gameObject.SetActive(false);

        GameObject pre = GameScene.self.pre.transform.Find("pisi_" + DataAll.Instance.level).gameObject;

        CheckPos p = new CheckPos("deck", lockobj.lockobj, 100, 1, (CheckPos cp) =>
        {

        });
        Vector3 pp = lockobj.lockobj.transform.position;
        pp.z -= 0.8f;
        lockobj.lockobj.transform.position = pp;
        p.foodData = info;
        p.updateFun = (CheckPos cp, bool isdic) =>  //检测解锁桌子
        {
            int state = 0;
            if (isdic == true)
            {
                if (cp.foodT == 0)
                {
                    cp.enterT += 1;
                    float t = 100 - cp.enterT * 10;
                    p.foodNum += (p.foodMax / 10);
                    if (t <= 0)
                    {
                        t = 0;
                    }
                    Material mat = cp.obj.GetComponent<MeshRenderer>().material;
                    mat.SetFloat("_Rate", t);

                }

                if (DataAll.Instance.isOnePlay < 20)
                {
                    onePlayStartTip(lockobj.lockobj, false);
                }
            }
            else
            {
                if (DataAll.Instance.isOnePlay < 20)
                {
                    onePlayStartTip(lockobj.lockobj, true);
                }
            }
            if (cp.enterT >= 10)
            {
                cp.foodT = 1;
                Material mat = cp.obj.GetComponent<MeshRenderer>().material;
                cp.enterT = 0;
                GoogleAd.self.video.show((data)=>
                {
                    if (data.isOk == true)
                    {
                        cp.data1 = 1;
                        mat.SetFloat("_Rate", 100);
                    }
                    else
                    {
                        mat.SetFloat("_Rate", 100);
                        cp.data1 = 0;
                        cp.eatFoodNum = 0;
                        p.enterT = 0;
                        cp.foodT = 0;
                        eff.SetActive(false);
                        Debug.Log("拒绝：：：：：：：：：");
                    }
                });
            }

            if (cp.data1 == 1)
            {
                eff.SetActive(true);

                GameObject pisi = GameObject.Instantiate(pre);
                pisi.transform.SetParent(obj.transform);
                huan3.x = ori.transform.position.x;
                huan3.y = ori.transform.position.y;
                huan3.z = ori.transform.position.z;
                pisi.transform.position = huan3;
                huan3.x = foodPos.transform.position.x;
                huan3.y = foodPos.transform.position.y + foodPos.transform.childCount * GameConfig.self.pisiY;
                huan3.z = foodPos.transform.position.z;
                Tools.Instance.mono.flyEff(pisi, huan3, 0.2f, (GameObject flyobj, FlyEndData fdata) =>
                {
                    flyobj.transform.SetParent(foodPos.transform);
                }, null);
                cp.eatFoodNum++;
                if (cp.eatFoodNum > 50)
                {
                    cp.data1 = 0;
                    cp.eatFoodNum = 0;
                    p.enterT = 0;
                    cp.foodT = 0;
                    eff.SetActive(false);
                }
            }

            cp.updateT += cp.updateYesTime;
            if (cp.updateT > 0.2f)
            {
                cp.updateT = 0;
                if (cp.foodData.foodPos.transform.childCount > 0)
                {

                    int indexf = cp.foodData.foodPos.transform.childCount - 1;
                    GameObject.Destroy(cp.foodData.foodPos.transform.GetChild(indexf).gameObject);
                    addGold(cp.foodData.goldPos, newGold);
                    eatAi.SetBool("eat", true);
                }
                else
                {
                    eatAi.SetBool("eat", false);
                }
            }

            return state;
        };
        p.outFun = (CheckPos cp) =>
        {
            Material mat = cp.obj.GetComponent<MeshRenderer>().material;
            mat.SetFloat("_Rate", 100);
            if (cp.foodT == 0)
            {
                cp.enterT = 0;
            }
        };

        checkPosList.Add(p);

        CheckPos goldp = new CheckPos("gold", goldPos, 100, 0.8f, (CheckPos cp) =>
        {


        });
        goldp.foodData = info;
        goldp.updateFun = getGold;
        checkPosList.Add(goldp);
    }

    #endregion

    #region 新手引导

    GameObject jian = null;

    void onePlayer()
    {
        onePlay = GetOneByName("onePlay");
        jian = GameScene.self.pre.transform.Find("jian").gameObject;
        jian = GameObject.Instantiate(jian);
        jian.SetActive(true);
        jian.transform.SetParent(transform);
        Debug.Log("新手：：：：：：：" + DataAll.Instance.isOnePlay);
        if (jian != null)
        {
            if (DataAll.Instance.isOnePlay > 20)
            {
                jian.SetActive(false);
            }
        }
        if (DataAll.Instance.isOnePlay == 0)
        {
            if (onePlay != null)
            {
                onePlay.SetActive(true);
            }
            GameObject playerPos = GetOneByName("playerPos", onePlay);
            Vector3 ori = playerPos.transform.position;
            ori.y = GameScene.self.addPlayerY;
            GameScene.self.player.transform.position = ori;

            GameObject goldPos = GetOneByName("goldPos", onePlay);
            jian.transform.position = new Vector3(goldPos.transform.position.x, -0.8f, goldPos.transform.position.z);

            for (int i = 0; i < 30; i++)
            {
                addGold(goldPos, 10);
            }
            CheckPos goldp = new CheckPos("gold", goldPos, 100, 0.8f, (CheckPos cp) =>
            {

            });
            goldp.updateFun = (CheckPos cp, bool isdic) =>
            {
                int state = 0;
                if (isdic == true)
                {
                    cp.enterT += 1;
                    if (cp.enterT >= 2)
                    {
                        GameScene.self.player.getGold(cp);
                        state = 1;
                        DataAll.Instance.isOnePlay = 1;
                    }
                }
                else
                {
                    cp.enterT = 0;
                }
                return state;
            };
            DeskOpen info = new DeskOpen(onePlay);
            goldp.foodData = info;
            checkPosList.Add(goldp);

        }

        if (onePlay != null)
        {
            if (DataAll.Instance.isOnePlay > 2)
            {
                onePlay.SetActive(false);
            }


        }
    }

    void onePlay2()
    {
        DataAll.Instance.isOnePlay = 2;
        onePlay3obj.SetActive(true);
        jian.transform.position = new Vector3(onePlay3obj.transform.position.x, -0.8f, onePlay3obj.transform.position.z);
    }

    GameObject onePlay3obj = null;
    void onePlay3()
    {
        DataAll.Instance.isOnePlay = 3;
    }

    public void onePlay4()
    {
        CameraFollow.self.isMoveGo = false;
        DataAll.Instance.isOnePlay = 4;
        if (onePlay != null)
        {
            onePlay.transform.Find("noGo").gameObject.SetActive(false);
        }
        DeskLockList[0].lockobj.SetActive(true);
        jian.transform.position = new Vector3(DeskLockList[0].lockobj.transform.position.x, -0.8f, DeskLockList[0].lockobj.transform.position.z);

    }

    public void onePlay5()
    {
        DataAll.Instance.isOnePlay = 5;

        onePlayMen(men.transform.GetChild(0).gameObject, 0);
        GameObject menobj = GetOneByName("menobj", men.transform.GetChild(0).gameObject);
        jian.transform.position = new Vector3(menobj.transform.position.x, -0.8f, menobj.transform.position.z);
    }

    public void onePlay6()
    {
        DataAll.Instance.isOnePlay = 6;
        FoodLockList[0].lockobj.SetActive(true);
        jian.transform.position = new Vector3(FoodLockList[0].lockobj.transform.position.x, -0.8f, FoodLockList[0].lockobj.transform.position.z);
    }

    public void onePlay7()
    {
        DataAll.Instance.isOnePlay = 7;
        GameObject lock_0 = onePlay.transform.Find("lock_0").gameObject;
        lock_0.SetActive(false);
        Transform yanpos = lock_0.transform.Find("lock");
        addYanEff(yanpos.transform.position);
        waimaiLockList[0].lockobj.SetActive(true);
        jian.transform.position = new Vector3(waimaiLockList[0].lockobj.transform.position.x, -0.8f, waimaiLockList[0].lockobj.transform.position.z);
    }

    //外卖机激活
    public void onePlay8()
    {
        //DataAll.Instance.isOnePlay = 8;
        FoodLockList[0].lockobj.SetActive(true);
        //jian.transform.position = new Vector3(FoodLockList[0].lockobj.transform.position.x, -0.8f, FoodLockList[0].lockobj.transform.position.z);

        DataAll.Instance.isOnePlay = 9;
        DeskLockList[0].lockobj.SetActive(true);
        jian.transform.position = new Vector3(DeskLockList[0].lockobj.transform.position.x, -0.8f, DeskLockList[0].lockobj.transform.position.z);
        onePlay9();
    }

    public void onePlay85()
    {

    }

    public void onePlay9()
    {
        jian.SetActive(false);
        DataAll.Instance.isOnePlay = 100;
        if (onePlay != null)
        {
            
            GameObject lock_0 = onePlay.transform.Find("lock_1").gameObject;
            Transform yanpos = lock_0.transform.Find("lock");
            CameraFollow.self.moveTarget(yanpos.gameObject);
            Tools.Instance.LaterFun(() =>
            {
                onePlay.SetActive(false);
                addYanEff(yanpos.transform.position);
            }, 2);
            Tools.Instance.LaterFun(() =>
            {
                CameraFollow.self.isMoveGo = false;

            }, 3);

        }
        openDesk();
        openWaimai();
        //openFood();
        onePlayMen(men.transform.GetChild(1).gameObject, 1);
        
        for (int i = 0; i < getPos.transform.childCount; i++)
        {
            GameObject ggg = getPos.transform.GetChild(i).gameObject;
            GameObject upJi = ggg.transform.Find("up").gameObject;
            upJi.SetActive(true);
        }
    }

    List<int> openMenIndex = new List<int>();
    void onePlayMen(GameObject g, int index)
    {
        GameObject ng = new GameObject("foodPos");
        GameObject menobj = GetOneByName("menobj", g);
        ng.transform.SetParent(g.transform);
        ng.transform.position = menobj.transform.position;
        menobj.SetActive(false);
        DeskLock lockobj = new DeskLock(g, obj, 500, index);
        CheckPos p = new CheckPos("deck", lockobj.lockobj, 100, 1, (CheckPos cp) =>
        {
            addYanEff(lockobj.lockobj.transform.position);
            openMenIndex.Add(index);
            lockobj.lockobj.SetActive(false);
            Tools.Instance.LaterFun(() =>
            {
                menobj.SetActive(true);

                if (index == 0)
                {
                    UplevelView.ismy = false;
                    UplevelView.closeFun = () =>
                    {
                        DataAll.Instance.addHuman++;
                        GameScene.self.mapControl.newNpc();
                    };
                    //Debug.Log("初始：：：：：：：：：："+ DataAll.Instance.isOnePlay)
                    if (DataAll.Instance.isOnePlay < 6)
                    {
                        onePlay6();
                    }

                    UplevelView.ShowView();
                }
                else
                {
                    UplevelView.ismy = true;
                    UplevelView.ShowView();
                }
            }, 0.5f);

        });
        p.foodMax = 500;

        p.updateFun = (CheckPos cp, bool isdic) =>  //检测解锁桌子
        {
            int state = 0;
            Material mat = cp.obj.GetComponent<MeshRenderer>().material;

            if (isdic == true)
            {
                if (DataAll.Instance.gold >= p.foodMax / 10)
                {

                    cp.enterT += 1;
                    float t = 100 - cp.enterT * 10;
                    p.foodNum += (p.foodMax / 10);
                    DataAll.Instance.gold -= (p.foodMax / 10);
                    GameObject ui = lockobj.lockobj.transform.GetChild(0).gameObject;
                    Text num = ui.transform.Find("num").GetComponent<Text>();
                    num.text = "$" + (cp.foodMax - p.foodNum);

                    for (int k = 0; k < 4; k++)
                    {
                        GameObject pre = GameScene.self.pre.transform.Find("gold").gameObject;
                        GameObject goldobj = GameObject.Instantiate(pre);
                        goldobj.transform.SetParent(obj.transform);
                        huan4.x = GameScene.self.player.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f);
                        huan4.y = GameScene.self.player.transform.position.y;
                        huan4.z = GameScene.self.player.transform.position.z + UnityEngine.Random.Range(-0.2f, 0.2f);
                        goldobj.transform.position = huan4;
                        Tools.Instance.mono.flyEff(goldobj, cp.obj.transform.position, 0.2f, null, null, true, 0.03f * k);
                    }

                    if (t <= 0)
                    {
                        t = 0;
                    }
                    mat.SetFloat("_Rate", t);
                    if (cp.enterT >= 10)
                    {
                        state = 1;
                        if (index == 1)
                        {
                            DataAll.Instance.openMen = 1;
                        }

                    }
                }
                onePlayStartTip(lockobj.lockobj, false);

            }
            else
            {
                onePlayStartTip(lockobj.lockobj, true);
            }
            return state;
        };
        checkPosList.Add(p);
    }

    void onePlayStartTip(GameObject g, bool isp)
    {
        TweenScale ts = g.GetComponent<TweenScale>();
        //Debug.Log("播放：：：：：" + isp + "    " + ts.isPlay);
        if (ts != null)
        {
            if (isp == true)
            {
                if (ts.isPlay == false)
                {
                    ts.RePlay();
                }
            }
            else
            {
                if (ts.isPlay == true)
                {
                    ts.stopPlay();
                }
            }

        }
    }

    #endregion

    void addYanEff(Vector3 pos)
    {
        GameObject effPre = GameScene.self.pre.transform.Find("eff_open").gameObject;
        GameObject eff = GameObject.Instantiate(effPre);
        eff.transform.SetParent(obj.transform);
        eff.transform.position = pos;
        Tools.Instance.LaterFun(() =>
        {
            GameObject.Destroy(eff);
        }, 1);

    }

    public void clear()
    {

    }

    /// <summary>
    /// 寻找ai路径 （地图拼npcGo的时候，子物体按顺序从大到小）
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public List<Vector3> findWay(Vector3 startPos, Vector3 endPos, Vector3 oneAddpos)
    {
        GameObject npcGo = goPos.transform.Find("npcGo").gameObject;
        List<Vector3> goList = new List<Vector3>();
        if (oneAddpos != null && oneAddpos.x > -1000)
        {
            goList.Add(oneAddpos);
        }
        else
        {

        }
        float minDic = 10000;
        GameObject mixpos = null;
        for (int i = 0; i < npcGo.transform.childCount; i++)
        {
            GameObject pos = npcGo.transform.GetChild(i).gameObject;
            //Debug.Log("移动11111" + pos.transform.position.x+" 索引："+i);
            float dic = Mathf.Abs(startPos.x - pos.transform.position.x);
            if (dic < minDic)
            {
                minDic = dic;
                mixpos = pos;
            }
        }
        goList.Add(_findWayDic(mixpos, endPos));

        if (endPos.x < startPos.x)
        {
            for (int i = 0; i < npcGo.transform.childCount; i++)
            {
                GameObject pos = npcGo.transform.GetChild(i).gameObject;
                //Debug.Log("移动11111" + pos.transform.position.x+" 索引："+i);

                if (pos.transform.position.x >= endPos.x && pos.transform.position.x <= startPos.x)
                {
                    //Debug.Log("移动2222222" + pos.transform.position.x + " 索引：" + i+" 开始："+startPos.x);
                    goList.Add(_findWayDic(pos, endPos));
                }
            }
        }
        else
        {
            for (int i = npcGo.transform.childCount - 1; i >= 0; i--)
            {
                GameObject pos = npcGo.transform.GetChild(i).gameObject;
                if (pos.transform.position.x >= startPos.x && pos.transform.position.x <= endPos.x)
                {
                    goList.Add(_findWayDic(pos, endPos));
                }
            }
        }
        goList.Add(endPos);
        return goList;


    }
    Vector3 _findWayDic(GameObject pos, Vector3 endPos)
    {
        float dic = 100000;
        Vector3 p = new Vector3();
        for (int j = 0; j < pos.transform.childCount; j++)
        {
            Transform tran = pos.transform.GetChild(j);
            float nowDic = Mathf.Abs(tran.position.z - endPos.z);
            if (nowDic < dic)
            {
                dic = nowDic;
                p.x = tran.position.x;
                p.y = tran.position.y;
                p.z = tran.position.z;
            }
        }
        p.x = p.x + UnityEngine.Random.Range(-0.1f, 0.1f);
        p.z = p.z + UnityEngine.Random.Range(-0.8f, 0.8f);
        return p;
    }
}
/// <summary>
/// 锁住的桌子
/// </summary>
public class DeskLock
{
    public DeskLock(GameObject d, GameObject par, float n, int _index, bool isFree = false)
    {
        desk = d;
        index = _index;
        GameObject pre = GameScene.self.pre.transform.Find("deskQuan").gameObject;
        GameObject l = GameObject.Instantiate(pre);
        GameObject ui = l.transform.GetChild(0).gameObject;
        Text num = ui.transform.Find("num").GetComponent<Text>();
        //Debug.Log("ssssssssssssssssssssss" + n+"   sss"+ Tools.Instance.getGoldK((int)n)+" aa:"+d.name);
        num.text = "$" + Tools.Instance.getGoldK((int)n);
        l.transform.SetParent(par.transform);
        lockobj = l;
        GameObject foodPos = Tools.Instance.GetGameObjectByName("foodPos", d);
        lockobj.transform.position = new Vector3(foodPos.transform.position.x, -1, foodPos.transform.position.z);
        GameObject free = Tools.Instance.GetGameObjectByName("free", l);
        free.SetActive(isFree);

    }


    public int index = 0;

    public GameObject desk;
    public GameObject lockobj;
    /// <summary>
    /// 需要的金币
    /// </summary>
    public float needGold = 400;
    /// <summary>
    /// 已投入的金币
    /// </summary>
    public float giveGold = 0;
}
/// <summary>
/// 购买的桌子
/// </summary>
public class DeskOpen
{
    public bool onePlay = false;

    public DeskOpen(GameObject _desk)
    {
        desk = _desk;
        foodPos = Tools.Instance.GetGameObjectByName("foodPos", desk);
        goldPos = Tools.Instance.GetGameObjectByName("goldPos", desk);
        for (int i = 0; i < desk.transform.childCount; i++)
        {
            Transform g = desk.transform.GetChild(i);
            if (g.name.Split('_')[0] == "yizi")
            {
                chairList.Add(new ChairInfo(g.gameObject));
            }
        }
    }

    public GameObject foodPos = null;
    public GameObject goldPos = null;

    /// <summary>
    /// 食物数量
    /// </summary>
    public List<GameObject> foolList = new List<GameObject>();

    public GameObject desk;
    public List<ChairInfo> chairList = new List<ChairInfo>();
    public float updateT = 0;
    public void update()
    {

    }
}
public class ChairInfo
{
    public ChairInfo(GameObject g)
    {
        chair = g;
        string[] str = g.name.Split('_');
        if (str.Length >= 2)
        {
            chairId = int.Parse(str[1]);
        }
    }
    public int chairId = 0;
    public GameObject chair;
    public bool isOpen = true;
    public GameObject Ai;
}
