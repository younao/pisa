using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Ai : BaseScript
{
    // Start is called before the first frame update

    public CharacterController con = null;
    GameObject tip = null;
    Text say;
    Transform maxTip = null;
    public Animator Anim = null;
    void Start()
    {
        if (isMoto == false)
        {
            moveSpeed = GameConfig.self.playerSpeed + UnityEngine.Random.Range(-0.3f, 0.3f);
            tip = GetGameObjectByName("tip");
            say = GetComponentByName<Text>("say");
            maxTip = GetOneByName("maxTip", tip).transform;
        }
        else
        {
            moveSpeed = GameConfig.self.waimaiMotoSpeed;
        }
        con = gameObject.GetComponent<CharacterController>();
        standRom = UnityEngine.Random.Range(2, 4f);
        Anim = transform.Find("anim").GetComponent<Animator>();
        isMove = true;
    }

    #region 移动
    public bool isMoto = false;

    public List<Vector3> movePos = new List<Vector3>();

    Vector3 goPos = new Vector3();
    Vector2 goDir = new Vector2();
    int goIndex = 0;
    public Action<Ai> goOkFun = null;
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

    public float moveSpeed = 1;

    private Vector3 huan1 = new Vector3(0, 0, 0);
    private Vector3 huan2 = new Vector3(0, 0, 0);

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

            //huan1.x =  this.goDir.normalized.x * Time.deltaTime * moveSpeed;
            //huan1.z =  this.goDir.normalized.y * Time.deltaTime * moveSpeed;
            //huan1.y = 0;
            //con.Move(this.huan1 * 0.02f);
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
            if (isSit == false)
            {
                move();
            }
            else
            {
                if (isEat == true)
                {
                    setAnim(PlayerAnimTy.sitEat);
                }
                else
                {
                    setAnim(PlayerAnimTy.sit);
                }

            }
            if (isMoto == false)
            {
                checkNoFood();
            }
        }

    }

    #region 设置去吃东西

    public DeskOpen eatDesk;
    public int chairI = 0;

    /// <summary>
    /// 去吃东西
    /// </summary>
    /// <param name="info"></param>
    public void setGoEat(DeskOpen info, int chairIndex)
    {
        eatDesk = info;
        chairI = chairIndex;
        info.chairList[chairIndex].isOpen = false;

        setGoChair(info, chairIndex);
        goOkFun = (Ai my) =>
        {
            info.chairList[chairIndex].Ai = gameObject;
            setAnim(PlayerAnimTy.sit);
            isSit = true;

            noFoodT = noFoodTime;

            Vector3 sitpos = new Vector3();

            sitpos.x = info.chairList[chairIndex].chair.transform.position.x;
            sitpos.y = GameScene.self.addPlayerY;
            sitpos.z = info.chairList[chairIndex].chair.transform.position.z;

            transform.position = sitpos;

            float x = info.foodPos.transform.position.x - info.chairList[chairIndex].chair.transform.position.x;
            float z = info.foodPos.transform.position.z - info.chairList[chairIndex].chair.transform.position.z;
            setDir(x, z);
            if (info.onePlay == true)
            {
                if (DataAll.Instance.isOnePlay == 3)
                {
                    GameScene.self.mapControl.onePlay4();
                }

            }
        };

        //Debug.Log("跟随ai：：：：" + info.onePlay);
        if (DataAll.Instance.isOnePlay == 3)
        {
            if (info.onePlay == true)
            {
                //Debug.Log("跟随ai：：：：");
                CameraFollow.self.moveTarget(gameObject);
            }
        }
    }

    void setGoChair(DeskOpen info, int chairIndex)
    {
        GameObject npcGo = GameScene.self.mapControl.goPos.transform.Find("npcGo").gameObject;
        Vector3 endPos = new Vector3();
        endPos.x = info.foodPos.transform.position.x + 1f;
        endPos.y = info.foodPos.transform.position.y;
        endPos.z = info.foodPos.transform.position.z;
        List<Vector3> goList = GameScene.self.mapControl.findWay(transform.position, endPos, GameScene.self.mapControl.enterPos.transform.position);
        setGopos(goList);
    }

    int eatNum = 0;
    int eatMax = 20;

    /// <summary>
    /// 吃完走人
    /// </summary>
    public void exitGo(DeskOpen info, int chairIndex)
    {
        //tttt
        //return;
        ChairInfo cinfo = info.chairList[chairIndex];
        cinfo.isOpen = true;
        cinfo.Ai = null;
        isSit = false;
        setAnim(PlayerAnimTy.run);
        Vector3 sitpos = new Vector3();

        sitpos.x = info.foodPos.transform.position.x + 1f;
        sitpos.y = GameScene.self.addPlayerY;
        sitpos.z = info.foodPos.transform.position.z;

        transform.position = sitpos;

        Vector3 endPos = GameScene.self.mapControl.goPos.transform.Find("enterPos").position;

        List<Vector3> goList = GameScene.self.mapControl.findWay(transform.position, endPos, GameScene.self.mapControl.enterPos.transform.position);
        setGopos(goList);

        goOkFun = (Ai my) =>
        {
            GameObject.Destroy(gameObject);
        };
    }

    float noFoodT = 0;
    float noFoodTime = GameConfig.self.aiSayGoTime;
    string[] sayStr = new string[] { "?", "*", "%", "$", "#", "~" };
    float noFoodSayT = 0;
    public void checkNoFood()
    {
        if (noFoodT > 0)
        {
            noFoodT -= Time.deltaTime;
            if (eatDesk.foodPos.transform.childCount > 0)
            {
                noFoodT = noFoodTime;
            }
            if (noFoodT < 8)
            {
                noFoodSayT -= Time.deltaTime;
                if (tip.activeInHierarchy == false)
                {
                    tip.SetActive(true);
                }
                maxTip.LookAt(CameraFollow.self.transform);

                if (noFoodSayT < 0)
                {
                    noFoodSayT = 2;
                    say.text = "";
                    for (int i = 0; i < 5; i++)
                    {
                        int index = UnityEngine.Random.Range(0, sayStr.Length);
                        string str = sayStr[index];
                        say.text += str;
                    }
                }
            }
            else
            {
                if (tip.activeInHierarchy == true)
                {
                    tip.SetActive(false);
                }
            }
            if (noFoodT <= 0)
            {
                tip.SetActive(false);
                exitGo(eatDesk, chairI);
            }
        }
    }
    #endregion

    #region 动画

    float standT = 0;
    float standRom = 3;

    bool isHold = false;

    bool isSit = false;

    public bool isEat = false;

    /// <summary>
    /// 动画状态
    /// </summary>
    string state = "";
    void setAnim(string str)
    {
        if (Anim != null)
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

    }

    #endregion
}