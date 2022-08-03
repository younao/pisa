using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : BaseScript
{

    // Start is called before the first frame update
    public Animator Anim = null;
    public GameObject getGoldGG = null;
    public CharacterController con = null;

    public GameObject tip = null;
    public Transform maxTip = null;
    void Start()
    {
        con = gameObject.GetComponent<CharacterController>();
        standRom = UnityEngine.Random.Range(2, 4f);
        Anim = transform.Find("anim").GetComponent<Animator>();
        holdPos = GetOneByName("holdPos");
        tip = GetOneByName("tip");
        maxTip = tip.transform.GetChild(0);
        tip.SetActive(false);
        getGoldGG = GetGameObjectByName("getGold");
    }

    private Vector3 huan1 = new Vector3(0, 0, 0);
    private Vector3 huan2 = new Vector3(0, 0, 0);

    public float speed
    {
        get
        {
            return GameConfig.self.playerSpeed + DataAll.Instance.speed * 0.4f - 0.8f;
        }
    }

    Vector2 godir = new Vector2();
    public void go(Vector2 dir)
    {
        godir = dir;
        if (dir.x != 0 && dir.y != 0)
        {
            float angleY = Tools.Instance.dirToAngle(dir.x, dir.y);
            huan2.y = angleY;
            transform.localEulerAngles = huan2;
            this.huan1.x = dir.normalized.x * speed;
            this.huan1.z = dir.normalized.y * speed;
            float y = 0;
            if (Math.Abs(GameScene.self.addPlayerY - transform.position.y) > 0.05)
            {
                y = GameScene.self.addPlayerY - transform.position.y;
            }
            this.huan1.y = y;
            this.con.Move(this.huan1 * 0.02f);
            if (isHold == false)
            {
                setAnim(PlayerAnimTy.run);
            }
            else
            {
                setAnim(PlayerAnimTy.runHold);
            }
            standT = 0;
        }
        else
        {
            if (isHold == true)
            {
                setAnim(PlayerAnimTy.standHold);
            }
            else
            {
                if (standT == 0)
                {
                    setAnim(PlayerAnimTy.stand);
                }
                standT += Time.deltaTime;
                if (standT > standRom)
                {
                    standT = 0.1f;
                    if (UnityEngine.Random.Range(0, 1f) > 0.5)
                    {
                        setAnim(PlayerAnimTy.stand);
                    }
                    else
                    {
                        setAnim(PlayerAnimTy.standPlay);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (GameScene.self.mapControl != null)
        {
            updateCheck();
            if (tip.activeInHierarchy == true)
            {
                maxTip.LookAt(CameraFollow.self.gameObject.transform);
            }
        }
        List<GetGoldNum> re = new List<GetGoldNum>();
        for (int i = 0; i < getGoldList.Count; i++)
        {
            int s = getGoldList[i].update();
            if (s == 1)
            {
                re.Add(getGoldList[i]);
            }
        }
        for (int i = 0; i < re.Count; i++)
        {
            GameObject.Destroy(re[i].g);
            getGoldList.Remove(re[i]);
        }
    }

    #region 动画

    float standT = 0;
    float standRom = 3;

    bool isHold = false;

    /// <summary>
    /// 动画状态
    /// </summary>
    string state = "";
    void setAnim(string str)
    {
        if (str != state)
        {
            if (state != "")
            {
                Anim.SetBool(state, false);
            }
            Anim.SetBool(str, true);
            state = str;
        }
    }

    #endregion

    #region 检测人物行为

    float upT = 0;
    void updateCheck()
    {
        List<CheckPos> re = new List<CheckPos>();
        for (int i = 0; i < GameScene.self.mapControl.checkPosList.Count; i++)
        {
            CheckPos c = GameScene.self.mapControl.checkPosList[i];
            c.updateYesT += Time.deltaTime;
            if (c.obj != null)
            {
                if (c.updateYesT > c.updateYesTime)
                {
                    c.updateYesT = 0;
                    int r = c.update();
                    if (r == 1)
                    {
                        re.Add(c);
                    }
                }
            }
            else
            {
                re.Add(c);
            }

        }
        for (int i = 0; i < re.Count; i++)
        {
            CheckPos c = re[i];
            if (re != null)
            {
                GameScene.self.mapControl.checkPosList.Remove(re[i]);
                if (c.obj != null)
                {
                    GameObject.Destroy(c.obj);
                }
   
                if (c.data != null)
                {
                    GameObject.Destroy(c.data);
                }
            }

        }
    }

    void noUpdate()
    {
        upT += Time.deltaTime;
        if (upT >= GameConfig.self.checkPosTime)
        {
            List<CheckPos> re = new List<CheckPos>();
            for (int i = 0; i < GameScene.self.mapControl.checkPosList.Count; i++)
            {
                CheckPos c = GameScene.self.mapControl.checkPosList[i];
                c.updateYesT += Time.deltaTime;
                if (c.updateYesT > c.updateYesTime)
                {
                    c.updateYesT = 0;
                    int r = c.update();
                    if (r == 1)
                    {
                        re.Add(c);
                    }
                }
            }
            for (int i = 0; i < re.Count; i++)
            {
                CheckPos c = re[i];
                if (re != null)
                {
                    GameScene.self.mapControl.checkPosList.Remove(re[i]);
                    GameObject.Destroy(c.obj);
                    if (c.data != null)
                    {
                        GameObject.Destroy(c.data);
                    }
                }

            }
            upT = 0;
        }
    }
    #endregion

    #region 食物

    int holdFoodTy = 0;

    public int getFoodMax
    {
        get
        {
            return GameConfig.self.PlayerGetNum + DataAll.Instance.addObj * 2;
        }
    }

    public GameObject holdPos = null;
    Vector3 zore = new Vector3(0, 0, 0);
    Vector3 huan3 = new Vector3(0, 0, 0);
    /// <summary>
    /// 拿上食物
    /// </summary>
    /// <param name="food"></param>
    public void getFood(CheckPos food, int _holdFoodTy = 0)
    {
        if (holdPos.transform.childCount > 0)
        {
            if (_holdFoodTy != holdFoodTy)
            {
                return;
            }
        }

        if (holdPos.transform.childCount > getFoodMax)
        {
            tip.SetActive(true);
            return;
        }
        else
        {
            tip.SetActive(false);
        }

        holdFoodTy = _holdFoodTy;

        Transform foodPar = food.foodData.foodPos.transform;
        GameObject foodobj = null;
        if (_holdFoodTy == 0)
        {
            for (int i = 0; i < foodPar.childCount; i++)
            {
                Transform f = foodPar.GetChild(i);
                if (f.childCount > 0)
                {
                    foodobj = f.GetChild(f.childCount - 1).gameObject;
                }

            }
        }
        else
        {
            if (foodPar.childCount > 0)
            {
                foodobj = foodPar.GetChild(foodPar.childCount - 1).gameObject;
            }

        }

        if (foodobj != null)
        {
            food.foodNum--;
            foodobj.transform.SetParent(holdPos.transform);
            float off = 0.04f;
            if (_holdFoodTy == 1)
            {
                off = 0.08f;
            }
            huan3.y = holdPos.transform.position.y + holdPos.transform.childCount * off;
            huan3.x = holdPos.transform.position.x;
            huan3.z = holdPos.transform.position.z;

            FlyEndData ff = new FlyEndData();
            ff.y = holdPos.transform.childCount * GameConfig.self.pisiY;
            ff.g = holdPos;

            Tools.Instance.mono.flyEff(foodobj, huan3, 0.3f, (GameObject flyObj, FlyEndData fdata) =>
              {
                  Vector3 end = new Vector3();

                  if (holdFoodTy == 1)
                  {
                      flyObj.transform.localEulerAngles = Tools.Instance.zore;
                  }


                  end.y = fdata.g.transform.position.y + fdata.y;
                  end.x = fdata.g.transform.position.x;
                  end.z = fdata.g.transform.position.z;
                  flyObj.transform.position = end;
              }, ff);

            //foodobj.transform.localPosition = huan3;
            //foodobj.transform.localEulerAngles = zore;
            isHold = true;
            MusicManage.Instance.play(MusicType.audio, "get");
            AndroidSdk.self.zhen();
        }
    }

    Vector3 huan4 = new Vector3();
    /// <summary>
    /// 送出食物
    /// </summary>
    /// <param name="food"></param>
    /// <param name="foodty"></param>
    public void giveFood(CheckPos food, int foodty = 0)
    {

        if (GameScene.self.mapControl == null)
        {
            return;
        }
        if (godir.x != 0 && godir.y != 0)
        {
            return;
        }

        if (foodty != holdFoodTy)
        {
            return;
        }
        tip.SetActive(false);
        if (holdPos.transform.childCount > 0)
        {
            if (holdPos.transform.childCount == 1)
            {
                isHold = false;
            }
            Transform foodobj = holdPos.transform.GetChild(holdPos.transform.childCount - 1);
            Transform par = food.foodData.foodPos.transform;
            foodobj.SetParent(GameScene.self.mapControl.obj.transform);

            huan4.x = par.position.x;
            huan4.y = par.position.y + par.childCount * GameConfig.self.pisiY;
            huan4.z = par.position.z;
            FlyEndData ff = new FlyEndData();
            ff.x = huan4.x;
            ff.y = huan4.y;
            ff.z = huan4.z;
            Tools.Instance.mono.flyEff(foodobj.gameObject, huan4, 0.2f, (GameObject flyobj, FlyEndData fdata) =>
              {
                  if (flyobj != null)
                  {
                      Vector3 end = new Vector3();
                      if (holdFoodTy == 1)
                      {
                          flyobj.transform.localEulerAngles = Tools.Instance.zore;
                      }
                      foodobj.SetParent(par);
                      huan4.x = par.position.x;
                      huan4.y = par.position.y + par.childCount * GameConfig.self.pisiY;
                      huan4.z = par.position.z;
                      flyobj.transform.position = huan4;
                  }
                  if (holdPos.transform.childCount == 0)
                  {
                      if (GameScene.self.mapControl != null)
                      {
                          if (holdFoodTy == 1)
                          {
                              Tools.Instance.LaterFun(() =>
                              {
                                  GameScene.self.mapControl.motoExit();
                              }, 0.5f);
                          }

                      }
                  }
              }, ff);
            AndroidSdk.self.zhen();
            MusicManage.Instance.play(MusicType.audio, "get");
            //huan4.x = par.position.x;
            //huan4.y = par.position.y+ par.childCount * 0.04f;
            //huan4.z = par.position.z;
            //foodobj.position = huan4;
        }
    }

    Vector3 huan5 = new Vector3();
    public void getGold(CheckPos food)
    {
        //Debug.Log("食物：：：：：：：：：：：" + food.foodData.goldPos.transform.childCount);
        Transform par = food.foodData.goldPos.transform;
        List<GameObject> goldlist = new List<GameObject>();
        for (int i = par.childCount - 1; i >= 0; i--)
        {
            GameObject g = par.GetChild(i).gameObject;
            goldlist.Add(par.GetChild(i).gameObject);

        }
        huan5.x = transform.position.x;
        huan5.y = transform.position.y - 0.12f;
        huan5.z = transform.position.z;

        int musicMax = 10;
        int getgoldNum = 0;

        int maxGold = 50;

        for (int i = 0; i < goldlist.Count; i++)
        {
            int addGold = 10;
            addGold = int.Parse(goldlist[i].name);
            DataAll.Instance.gold += addGold;
            getgoldNum += addGold;
            if (i < maxGold)
            {
                goldlist[i].transform.SetParent(GameScene.self.mapControl.obj.transform);
                Tools.Instance.mono.flyEff(goldlist[i], transform.position, 0.2f, (GameObject g, FlyEndData f) =>
                {
                    AndroidSdk.self.zhen();
                }, null, true, 0);
            }
            else
            {
                GameObject.Destroy(goldlist[i]);
            }

            if (i <= musicMax)
            {
                Tools.Instance.LaterFun(() =>
                {
                    MusicManage.Instance.play(MusicType.audio, "gold");
                },0.015f*i);
                
            }
        }
        if (getgoldNum > 0)
        {
            GameObject preg = GameScene.self.pre.transform.Find("eff_getGold").gameObject;
            GameObject gg = GameObject.Instantiate(preg);
            gg.transform.SetParent(getGoldGG.transform);
            GetGoldNum gn = new GetGoldNum(gg, getgoldNum);
            getGoldList.Add(gn);
        }

    }

    List<GetGoldNum> getGoldList = new List<GetGoldNum>();
    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("碰撞检查到：：：：："+ hit);
        Ai ai = hit.gameObject.GetComponent<Ai>();
        if ( ai!= null)
        {
            ai.isMove = false;
        }
        Npc npc = hit.gameObject.GetComponent<Npc>();
        if (npc != null)
        {
            npc.isMove = false;
        }
    }
}

public class GetGoldNum
{
    Vector3 v = new Vector3(0, 0, 0);
    public GetGoldNum(GameObject _g, int gold)
    {
        g = _g;
        g.transform.localPosition = v;
        GameObject num = Tools.Instance.GetGameObjectByName("num", g);
        num.GetComponent<Text>().text = "+" + gold;
    }
    public float t = 0;
    public GameObject g = null;
    public int update()
    {
        int state = 0;
        g.transform.LookAt(CameraFollow.self.transform);
        v.y += 0.01f;
        g.transform.localPosition = v;
        t += Time.deltaTime;
        if (t > 1)
        {
            state = 1;
        }
        return state;
    }
}

/// <summary>
/// 动作类型
/// </summary>
public static class PlayerAnimTy
{
    /// <summary>
    /// 空
    /// </summary>
    public static string none = "";
    /// <summary>
    /// 站着
    /// </summary>
    public static string stand = "stand";
    /// <summary>
    /// 站着玩手机
    /// </summary>
    public static string standPlay = "standPlay";
    /// <summary>
    /// 端着东西站着
    /// </summary>
    public static string standHold = "standHold";
    /// <summary>
    /// 坐下待机
    /// </summary>
    public static string sit = "sit";
    /// <summary>
    /// 坐下吃东西
    /// </summary>
    public static string sitEat = "sitEat";
    /// <summary>
    /// 走路
    /// </summary>
    public static string walk = "walk";
    /// <summary>
    /// 奔跑
    /// </summary>
    public static string run = "run";
    /// <summary>
    /// 端着东西奔跑
    /// </summary>
    public static string runHold = "runHold";
}

/// <summary>
/// 检测人物碰撞
/// </summary>
public class CheckPos
{
    public CheckPos(string _ty, GameObject _obj, float _yesT, float _dic, Action<CheckPos> _okfun, GameObject _data = null)
    {
        ty = _ty;
        obj = _obj;
        yesT = _yesT;
        dic = _dic;
        okfun = _okfun;
        updateT = GameConfig.self.checkPosTime;
        data = _data;
    }
    public string ty = "";
    public GameObject obj = null;
    public float enterT = 0;
    public float yesT = 1;
    public float dic = 1;
    public float updateT = 0.02f;
    public funC updateFun = null;
    public delegate int funC(CheckPos p, bool isdic);
    public Action<CheckPos> okfun = null;

    public Action<CheckPos> outFun = null;
    public bool isOut = true;

    public GameObject data = null;
    public int data1 = 0;

    public funC onUpdate;

    public float foodT = 0;
    public float foodTime = 1;
    public int foodMax = 10;
    public int foodNum = 0;
    public int eatFoodNum = 0;
    public DeskOpen foodData = null;

    public float updateYesT = 0;
    public float updateYesTime = GameConfig.self.checkPosTime;
    public int update()
    {
        int state = 0;
        if (obj.activeInHierarchy == true)
        {
            if (GameScene.self.player != null)
            {
                float[] a = { obj.transform.position.x, obj.transform.position.z };
                float[] b = { GameScene.self.player.transform.position.x, GameScene.self.player.transform.position.z };
                bool isdic = Tools.Instance.isDic2d(a, b, dic);
                if (updateFun != null)
                {
                    state = updateFun(this, isdic);
                    if (state == 1)
                    {
                        if (okfun != null)
                        {
                            okfun(this);
                        }
                    }
                }
                if (isdic == true)
                {
                    isOut = true;
                }
                else
                {
                    if (isOut == true)
                    {
                        isOut = false;
                        if (outFun != null)
                        {
                            outFun(this);
                        }
                    }
                }
            }
        }

        return state;
    }

}
