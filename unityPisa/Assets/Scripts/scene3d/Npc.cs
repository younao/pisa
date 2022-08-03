using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Npc : BaseScript
{
    // Start is called before the first frame update
    public Animator Anim = null;
    void Start()
    {
        standRom = UnityEngine.Random.Range(2, 4f);
        Anim = transform.Find("anim").GetComponent<Animator>();
        holdPos = GetOneByName("holdPos");
        goWorkT = UnityEngine.Random.Range(1f, 3f);
    }

    #region 移动
    public List<Vector3> movePos = new List<Vector3>();

    Vector3 goPos = new Vector3();
    Vector2 goDir = new Vector2();
    int goIndex = 0;
    public Action<Npc> goOkFun = null;
    public string goOk = "";

    public void setGopos(List<Vector3> list)
    {
        movePos = list;
        int index = 0;
        float dic = 10000;
        for (int i = 0; i < list.Count; i++)
        {

            float nowdic = Vector3.Distance(transform.position, list[i]);
            if (nowdic < dic)
            {
                dic = nowdic;
                index = i;
            }

        }
        goIndex = index;
        goPos = movePos[index];
        setGoPosOk(goPos);

        int overIndex = movePos.Count - 1;
        float[] a = { transform.position.x, transform.position.z };
        float[] b = { movePos[overIndex].x, movePos[overIndex].z };
        bool isdic = Tools.Instance.isDic2d(a, b, Time.deltaTime * moveSpeed + 0.02f);
        if (isdic == true)
        {
            if (goOkFun != null)
            {
                goOkFun(this);
            }
            movePos = new List<Vector3>();
        }
    }

    void setGoPosOk(Vector3 dir)
    {
        goDir.x = dir.x - transform.position.x;
        goDir.y = dir.z - transform.position.z;
        setDir(goDir.x, goDir.y);
    }
    void setDir(float x, float z)
    {
        float angleY = Tools.Instance.dirToAngle(x, z);
        huan2.y = angleY;
        transform.localEulerAngles = huan2;
    }

    //float moveSpeed = GameConfig.self.playerSpeed;
    public float moveSpeed
    {
        get
        {
            return GameConfig.self.playerSpeed + DataAll.Instance.speedAi * 0.2f;
        }
    }
    private float isMoveT = 0;
    bool _isMove = true;
    public bool isMove
    {
        get
        {
            return _isMove;
        }
        set
        {
            _isMove = value;
            if (_isMove == false)
            {
                isMoveT = 0.2f;
            }

        }
    }
    private Vector3 huan1 = new Vector3(0, 0, 0);
    private Vector3 huan2 = new Vector3(0, 0, 0);
    void move()
    {
        isMoveT -= Time.deltaTime;
        if (isMoveT <= 0)
        {
            _isMove = true;
        }
        if (movePos.Count > 0)
        {
            if (isHold == false)
            {
                setAnim(PlayerAnimTy.run);
            }
            else
            {
                setAnim(PlayerAnimTy.runHold);
            }
            setGoPosOk(goPos);
            if (_isMove == true)
            {
                huan1.x = transform.position.x + this.goDir.normalized.x * Time.deltaTime * moveSpeed;
                huan1.z = transform.position.z + this.goDir.normalized.y * Time.deltaTime * moveSpeed;
                huan1.y = transform.position.y;
                transform.position = huan1;
            }


            float[] a = { transform.position.x, transform.position.z };
            float[] b = { goPos.x, goPos.z };
            bool isdic = Tools.Instance.isDic2d(a, b, Time.deltaTime * moveSpeed + 0.02f);

            if (isdic == true)
            {
                goIndex++;
                if (goIndex >= movePos.Count)
                {
                    if (goOkFun != null)
                    {
                        goOkFun(this);
                    }
                    movePos = new List<Vector3>();
                }
                else
                {
                    goPos = movePos[goIndex];
                }
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

    #endregion

    private void Update()
    {
        if (gameObject.activeInHierarchy == true)
        {
            move();
            updateWork();
        }

    }

    #region 工作事件

    /// <summary>
    /// 工作阶段 0空闲，1去拿食物，2送食物
    /// </summary>
    public int workState = 0;

    public float goWorkT = 0;

    public DeskOpen goChairDesk = null;

    //去某个椅子中
    void goChair(DeskOpen info)
    {
        if (workState == 1)
        {
            goChairDesk = info;
            GameObject npcGo = GameScene.self.mapControl.goPos.transform.Find("npcGo").gameObject;
            List<Vector3> goList = GameScene.self.mapControl.findWay(transform.position, info.foodPos.transform.position, new Vector3(-2000f, 0, 0));
            //Debug.Log("移动的桌子：：：" + goList.Count+"桌子：："+ info.desk.name);
            setGopos(goList);
            workState = 2;
        }

    }

    //获取食物
    void goGetFood(DeskOpen info)
    {
        if (workState == 0)
        {
            GameObject npcGo = GameScene.self.mapControl.goPos.transform.Find("npcGo").gameObject;
            List<Vector3> goList = GameScene.self.mapControl.findWay(transform.position, info.goldPos.transform.position, new Vector3(-2000f, 0, 0));
            setGopos(goList);
            workState = 1;
        }

    }

    void updateWork()
    {
        if (goWorkT > 0)
        {
            goWorkT -= Time.deltaTime;
            if (goWorkT <= 0)
            {
                goGetFood(GameScene.self.mapControl.FoodOpenList[getFoodIndex()]);
            }
        }
    }

    int getFoodIndex()
    {
        int index = UnityEngine.Random.Range(0, GameScene.self.mapControl.FoodOpenList.Count);
        for (int i = 0; i < GameScene.self.mapControl.FoodOpenList.Count; i++)
        {
            DeskOpen d = GameScene.self.mapControl.FoodOpenList[i];
            if (d.foodPos.transform.childCount > 10)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    #endregion

    #region 食物

    public int getFoodMax
    {
        get
        {
            return GameConfig.self.PlayerGetNum + DataAll.Instance.addObjAi * 2-2;
        }
    }

    GameObject holdPos = null;
    Vector3 zore = new Vector3(0, 0, 0);
    Vector3 huan3 = new Vector3(0, 0, 0);
    /// <summary>
    /// 拿上食物
    /// </summary>
    /// <param name="food"></param>
    public void getFood(CheckPos food)
    {
        if (holdPos.transform.childCount > getFoodMax)
        {
            return;
        }

        Transform foodPar = food.foodData.foodPos.transform;
        GameObject foodobj = null;
        for (int i = 0; i < foodPar.childCount; i++)
        {
            Transform f = foodPar.GetChild(i);
            for (int j = f.childCount - 1; j >= 0; j++)
            {
                foodobj = f.GetChild(j).gameObject;
                break;
            }
        }
        if (foodobj != null)
        {
            food.foodNum--;
            foodobj.transform.SetParent(holdPos.transform);

            huan3.y = holdPos.transform.position.y + holdPos.transform.childCount * 0.04f;
            huan3.x = holdPos.transform.position.x;
            huan3.z = holdPos.transform.position.z;

            FlyEndData ff = new FlyEndData();
            ff.y = holdPos.transform.childCount * GameConfig.self.pisiY;
            ff.g = holdPos;

            Tools.Instance.mono.flyEff(foodobj, huan3, 0.3f, (GameObject flyObj, FlyEndData fdata) =>
            {
                Vector3 end = new Vector3();

                end.y = fdata.g.transform.position.y + fdata.y;
                end.x = fdata.g.transform.position.x;
                end.z = fdata.g.transform.position.z;
                flyObj.transform.position = end;
            }, ff);

            //foodobj.transform.localPosition = huan3;
            //foodobj.transform.localEulerAngles = zore;
            isHold = true;
        }

        int foodnn = 0;
        for (int i = 0; i < food.foodData.foodPos.transform.childCount; i++)
        {
            foodnn+= food.foodData.foodPos.transform.GetChild(i).childCount;
        }

        if(holdPos.transform.childCount >= 2)
        {
            if (holdPos.transform.childCount >= getFoodMax - 2 || foodnn <= 0)
            {

                goChair(GameScene.self.mapControl.DeskOpenList[goDeskIndex()]);
            }
        }
    }

    int goDeskIndex()
    {
        int index = UnityEngine.Random.Range(0, GameScene.self.mapControl.DeskOpenList.Count);
        List<int> noD = new List<int>();
        int ll = 100;
        int indexii = 0;
        if (UnityEngine.Random.Range(0, 1f) < 0.9f)
        {
            for (int i = 0; i < GameScene.self.mapControl.DeskOpenList.Count; i++)
            {
                DeskOpen d = GameScene.self.mapControl.DeskOpenList[i];
                if (d.foodPos.transform.childCount < 3)
                {
                    index = i;

                    noD.Add(i);
                }
                if(d.foodPos.transform.childCount < ll)
                {
                    ll = d.foodPos.transform.childCount;
                    indexii = i;
                }
            }
        }
        if (noD.Count > 0)
        {
            int indexx = UnityEngine.Random.Range(0, noD.Count);
            index = noD[indexx];
        }
        else
        {
            index = indexii;
        }

        return index;
    }

    Vector3 huan4 = new Vector3();
    /// <summary>
    /// 送食物
    /// </summary>
    /// <param name="food"></param>
    public void giveFood(CheckPos food)
    {
        if (food.foodData == goChairDesk)
        {
            if (holdPos.transform.childCount > 0)
            {
                if (workState != 0)
                {
                    workState = 0;
                    movePos = new List<Vector3>();
                }

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
                Tools.Instance.mono.flyEff(foodobj.gameObject, huan4, 0.3f, (GameObject flyobj, FlyEndData fdata) =>
                {
                    if (flyobj != null)
                    {
                        Vector3 end = new Vector3();
                        flyobj.transform.SetParent(par);
                        end.x = par.position.x;
                        end.y = par.position.y + par.childCount * GameConfig.self.pisiY;
                        end.z = par.position.z;
                        flyobj.transform.position = end;
                    }
                }, ff);
            }
            else
            {
                if (goWorkT < 0)
                {
                    goWorkT = UnityEngine.Random.Range(3f, 6f);
                }
            }
        }
    }

    #endregion

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
}
