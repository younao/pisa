using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class Tools
{
    private static Tools _instance;
    public static Tools Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Tools();

            }
            return _instance;
        }
    }
    public Tools()
    {
        
    }

    public ToolsMono mono = null;

    /// <summary>
    /// 延迟隐藏物体
    /// </summary>
    public void LaterActive(GameObject g,float t,bool isActive=false)
    {
        mono.StartCoroutine(IELaterActive(g, t, isActive));
    }
    IEnumerator IELaterActive(GameObject g, float t, bool isActive = false)
    {
        yield return new WaitForSeconds(t);
        g.SetActive(isActive);
    }

    /// <summary>
    /// 延迟执行方法
    /// </summary>
    public void LaterFun(Action fun, float t)
    {
        mono.StartCoroutine(IELaterFun(fun, t));
    }
    IEnumerator IELaterFun(Action fun, float t)
    {
        yield return new WaitForSeconds(t);
        fun();
    }

    //加载图片
    public void LoadTex<T>(string url,Action<Texture2D,T> fun,T t,bool isHttp=false)
    {
        if (AllTex.ContainsKey(url) == true)
        {
            Texture2D tex = Texture2D.Instantiate(AllTex[url]);
            fun(tex,t);
        }
        else
        {
            if (isHttp)
            {
                mono.StartCoroutine(loadtex(url, fun, t));
            }
            else
            {
                Texture2D tex = Resources.Load<Texture2D>(url);
                fun(tex, t);
            }
        }
    }
    public Dictionary<string, Texture2D> AllTex = new Dictionary<string, Texture2D>();
    IEnumerator loadtex<T>(string url, Action<Texture2D, T> fun,T t) // 协程
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();
        int width = 720;
        int high = 1080;
        if (!wr.isNetworkError)
        {
            Texture2D tex = new Texture2D(width, high);
            tex = texDl.texture;
            if (AllTex.ContainsKey(url) == false)
            {
                AllTex.Add(url, tex);
            }
            Texture2D tex1 = Texture2D.Instantiate(AllTex[url]);
            fun(tex1,t);
            //Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.Log("加载图片出错：" + wr.error);
        }

    }

    /**清空子物体 */
    public void ClearObj(GameObject CarParent)
    {
       
        List<GameObject> ClearCarList = new List<GameObject>();
        for (int index = 0; index < CarParent.transform.childCount; index++)
        {
            //Debug.Log("清空车2:" + index);
            GameObject g = CarParent.transform.GetChild(index).gameObject;
            
            g.SetActive(false);
            ClearCarList.Add(g);
        }
        for (int index = 0; index < ClearCarList.Count; index++)
        {
            GameObject.Destroy(ClearCarList[index]);
        }
        Debug.Log("清空车:" + CarParent.transform.childCount);
    }

    #region 处理方法

    /// <summary>
    /// 克隆列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public List<T> cloneList<T>(List<T> list)
    {
        List<T> l = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            l.Add(list[i]);
        }
        return l;
    }

    /// <summary>
    /// 通过名字获取对象
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="g"></param>
    /// <returns></returns>
    public GameObject GetGameObjectByName(string objName, GameObject g)
    {
        if (g == null)
        {
            return null;
        }
        GameObject obj = null;
        if (g.name == objName)
        {
            return g;
        }
        else
        {
            Transform tran = g.transform;
            obj = GetObjAll(tran, objName);
        }

        if (obj != null)
        {
            return obj;
        }
        else
        {
            Debug.LogError("在" + g.name + "找不到物体:" + objName);
            return null;
        }
    }

    private GameObject GetObjAll(Transform t, string tname)
    {
        GameObject g = null;
        for (int i = 0; i < t.childCount; i++)
        {
            //Debug.Log("all:" + t.GetChild(i).gameObject.name);
            if (tname == t.GetChild(i).gameObject.name)
            {
                g = t.GetChild(i).gameObject;
                break;
            }
            if (g == null)
            {
                if (t.GetChild(i).childCount > 0)
                {
                    g = GetObjAll(t.GetChild(i), tname);
                    if (g != null)
                    {
                        break;
                    }
                }
            }
        }
        return g;
    }
    #endregion

    #region 事件
    public void AddListenerMessage(string actionName, Action<EventArg> fun)
    {
        if (ScriptManage.Instance.messageDic.ContainsKey(actionName) == true)
        {
            ScriptManage.Instance.messageDic[actionName].funlist.Add(fun);
        }
        else
        {
            EventObj a = new EventObj();
            a.funlist.Add(fun);
            ScriptManage.Instance.messageDic.Add(actionName, a);
        }

        
    }
    public void RemoveListenerMessage(string actionName, Action<EventArg> fun = null)
    {
        if (ScriptManage.Instance.messageDic.ContainsKey(actionName))
        {
            if (fun == null)
            {
                ScriptManage.Instance.messageDic.Remove(actionName);
            }
            else
            {
                ScriptManage.Instance.messageDic[actionName].funlist.Remove(fun);
                if (ScriptManage.Instance.messageDic[actionName].funlist.Count <= 0)
                {
                    ScriptManage.Instance.messageDic.Remove(actionName);
                }
            }
        }
    }
    public void sendMessage(string actionName, EventArg arg = null)
    {

        if (ScriptManage.Instance.messageDic.ContainsKey(actionName))
        {
            EventObj a = ScriptManage.Instance.messageDic[actionName];
            for (int i = 0; i < a.funlist.Count; i++)
            {
                a.funlist[i](arg);
            }
        }
        else
        {
            Debug.LogWarning("没有监听：" + actionName);
        }
    }
    #endregion

    #region 3d向量角度方向计算

    public Vector3 zore = new Vector3();

    /// <summary>
    /// 角度转弧度 默认角度转弧度
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="isAngle"></param>
    /// <returns></returns>
    public float radian(float angle,bool isAngle = true)
    {

        if (isAngle == true)
        {
            return angle * (Mathf.PI / 180); //计算出弧度
        }
        else
        {
            return angle * (180 / Mathf.PI); //计算出角度
        }
    }

    /// <summary>
    /// 方向转角度 通过2个轴的坐标算出另外的一个轴的欧拉角，如x，z轴，计算出y轴的欧拉角
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float dirToAngle(float x,float z)
    {
        float angle = 0;
        if (x == 0 || z==0)
        {
            if (x == 0)
            {
                if (z < 0)
                {
                    angle = 180;
                }
                else
                {
                    angle = 0;
                }
            }
            if (z == 0)
            {
                if (x < 0)
                {
                    angle = 270;
                }
                else
                {
                    angle = 90;
                }
            }
            if(x==0 && z == 0)
            {
                angle = 0;
            }
        }
        else
        {
            angle = Mathf.Atan(Mathf.Abs(z / x));
            angle = this.radian(angle, false);

            if (x > 0 )
            {
                if(z > 0)
                {
                    angle = 90 - angle;
                }
                else
                {
                    angle = 90 + angle;
                }
            }
            else
            {
                if (z > 0)
                {
                    angle = 270 + angle;
                }
                else
                {
                    angle = 180 + (90 - angle);
                }
            }
        }
        return angle;

    }

    /// <summary>
    /// 通过角度算出向量
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public float[] AngleToDir(float angle)
    {
        float aR = radian(angle);

        float x =(float)Math.Sin(aR);
        float y = (float)Math.Cos(aR);
        float[] r = { x, y };
        return r;
    }

    /// <summary>
    /// 返回两个点的距离的平方
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <returns></returns>
    public float DistanceSquare(Vector3 pos,Vector3 pos1)
    {
        float dic = Mathf.Pow(pos.x-pos1.x,2) + Mathf.Pow(pos.y - pos1.y, 2) + Mathf.Pow(pos.z - pos1.z, 2);
        return dic;
    }

    /// <summary>
    /// 判断两个点是否在dic距离内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public bool isDic(Vector3 pos, Vector3 pos1,float dic)
    {
        float s= DistanceSquare(pos, pos1);
        if (s <= Mathf.Pow(dic, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 一个点绕着center，以axis为轴旋转angle度
    /// </summary>
    /// <param name="position"></param>
    /// <param name="center"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }
    #endregion

    #region 2d向量计算
    
    /// <summary>
    /// 一点绕着另外的点旋转
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="ori"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public float[] rotatePos(Vector2 pos, Vector2 ori,float angle)
    {
        float ra = this.radian(-angle);
        float x = (pos.x - ori.x) * Mathf.Cos(ra) - (pos.y - ori.y) * Mathf.Sin(ra) + ori.x;
        float y = (pos.x - ori.x) * Mathf.Sin(ra) + (pos.y - ori.y) * Mathf.Cos(ra) + ori.y;
        float[] r = { x, y };
        return r;
    }
    private Vector2 huan1 = new Vector2();

    /// <summary>
    /// 计算方向夹角 以two为基准的角度，one在two右边的是0~180 ，one在two的左边的是0~-180
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    public float DirAngle(Vector2 one, Vector2 two)
    {
        //let right = new Laya.Vector2(-one.y, one.x);

        huan1.x = -one.y;
        huan1.y = one.x;

        float angle = Vector2.Angle(one, two);
        float angleDir =Vector2.Dot(huan1, two);
        if (0 < angleDir)
        {
            angle = 360 - angle;
        }
        return angle;
    }

    /// <summary>
    /// 判断两个点是否在dic距离内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public bool isDic2d(Vector2 pos, Vector2 pos1, float dic)
    {
        float s = Mathf.Pow(pos.x - pos1.x, 2) + Mathf.Pow(pos.y - pos1.y, 2);
        if (s <= Mathf.Pow(dic, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 判断两个点是否在dic距离内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public bool isDic2d(float[] pos, float[] pos1, float dic)
    {
        float s = Mathf.Pow(pos[0] - pos1[0], 2) + Mathf.Pow(pos[1] - pos1[1], 2);
        if (s <= Mathf.Pow(dic, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region ui监听事件

    public float ModuleY = 810f; //810f;

    public float ModuleX = 720f;

    /// <summary>
    /// 坐标自适应
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="IsZhen">true为鼠标坐标变ui坐标</param>
    /// <returns></returns>
    public Vector3 AdaptPos(Vector3 pos, bool IsZhen = true)
    {

        float bili = 720f / Screen.width;
        float U = (float)Screen.width / (float)Screen.height - 720f / 1080f;
        if (U > 0)
        {
            bili = 1080f / Screen.height;
        }

        if (IsZhen == true)
        {
            return new Vector3(pos.x * bili, pos.y * bili, 0);
        }
        else
        {

            return new Vector3(pos.x / bili, pos.y / bili, 0);
        }

    }

    /// <summary>
    /// 开始拖动
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool DragStartObj(GameObject go, Action<GameObject> callback, double dateT = 0.02f)
    {
        bool isOk = true;
        if (go != null)
        {
            EvnetBase evn;
            if (go.GetComponent<EvnetBase>() == null)
            {
                evn = go.AddComponent<EvnetBase>();
            }
            else
            {
                evn = go.GetComponent<EvnetBase>();
            }
            evn.CallFunStart = callback;
            evn.dateD = dateT;
        }
        else
        {
            Debug.LogError("拖动的obj为Null");
            isOk = false;
        }
        return isOk;
    }
    /// <summary>
    /// 拖动中
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool DragObj(GameObject go, Action<PointerEventData> callback, bool isFollowDrag = true, double dateT = 0.02f)
    {
        bool isOk = true;
        if (go != null)
        {
            EvnetBase evn;
            if (go.GetComponent<EvnetBase>() == null)
            {
                evn = go.AddComponent<EvnetBase>();
            }
            else
            {
                evn = go.GetComponent<EvnetBase>();
            }
            evn.CallFunDrag = callback;
            evn._IsFollowDrag = isFollowDrag;
            evn.dateD = dateT;
        }
        else
        {
            Debug.LogError("拖动的obj为Null");
            isOk = false;
        }
        return isOk;
    }
    /// <summary>
    /// 拖动
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool DragEndObj(GameObject go, Action<PointerEventData> callback, bool isFollowDrag = true, double dateT = 0.02f, bool isX = true, float xRightOffset = 0f)
    {
        bool isOk = true;


        if (go != null)
        {
            EvnetBase evn;
            if (go.GetComponent<EvnetBase>() == null)
            {
                evn = go.AddComponent<EvnetBase>();
            }
            else
            {
                evn = go.GetComponent<EvnetBase>();
            }
            evn.Initi(isFollowDrag, callback);
            evn.dateD = dateT;
            evn.IsDragX = isX;
            evn._XRightOffset = xRightOffset;
        }
        else
        {
            Debug.LogError("拖动的obj为Null");
            isOk = false;
        }
        return isOk;
    }
    /// <summary>
    /// 点击抬起
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool OnPressObj(GameObject go, Action<GameObject> tr, bool IsMove = false, double dateT = 0.02f)
    {
        bool isOk = true;
        if (go != null)
        {
            EvnetBase evn;
            if (go.GetComponent<EvnetBase>() == null)
            {
                evn = go.AddComponent<EvnetBase>();
            }
            else
            {
                evn = go.GetComponent<EvnetBase>();
            }
            evn.dateD = dateT;
            evn.IsPressMove = IsMove;
            evn.CallPressFun = tr;
        }
        else
        {
            isOk = false;
        }
        return isOk;
    }

    /// <summary>
    /// 抬起
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool OnPressObjUp(GameObject go, Action<PointerEventData> tr)
    {
        bool isOk = true;
        if (go != null)
        {
            EvnetBase evn;
            if (go.GetComponent<EvnetBase>() == null)
            {
                evn = go.AddComponent<EvnetBase>();
            }
            else
            {
                evn = go.GetComponent<EvnetBase>();
            }

            evn.RaiseTheCallback = tr;
        }
        else
        {
            isOk = false;
        }
        return isOk;
    }

    /// <summary>
    /// 离开
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool OnPressExitObj(GameObject go, Action<GameObject> tr)
    {
        bool isOk = true;
        if (go != null)
        {
            EvnetBase evn;
            if (go.GetComponent<EvnetBase>() == null)
            {
                evn = go.AddComponent<EvnetBase>();
            }
            else
            {
                evn = go.GetComponent<EvnetBase>();
            }

            evn.ExitFun = tr;
        }
        else
        {
            isOk = false;
        }
        return isOk;
    }

    /// <summary>
    /// 按下
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool OnStartPressObj(GameObject go, Action<PointerEventData> tr, double dateT = 0.02f)
    {
        bool isOk = true;
        if (go != null)
        {
            EvnetBase evn;
            if (go.GetComponent<EvnetBase>() == null)
            {
                evn = go.AddComponent<EvnetBase>();
            }
            else
            {
                evn = go.GetComponent<EvnetBase>();
            }
            evn.dateD = dateT;
            evn.CallStartPressFun = tr;
        }
        else
        {
            isOk = false;
        }
        return isOk;
    }

    /// <summary>
    /// 添加按钮事件
    /// </summary>
    /// <param name="g"></param>
    /// <param name="fun"></param>
    public void OnButton(GameObject g,UnityAction fun)
    {
        Button btn = g.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(fun);
        }
        else
        {
            Debug.Log("物体：(" + g.name + ")没有Button组件");
        }
    }

    /// <summary>
    /// 添加按钮事件
    /// </summary>
    /// <param name="g"></param>
    /// <param name="fun"></param>
    public void OnButton(GameObject g, UnityAction<GameObject> fun)
    {
        Button btn = g.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(()=> {
                fun(g);
            });
        }
        else
        {
            Debug.Log("物体：(" + g.name + ")没有Button组件");
        }
    }

    public List<TouchData> TouchInfo = new List<TouchData>();
    #endregion

    #region 游戏工具
    /// <summary>
    /// 提示
    /// </summary>
    /// <param name="info">内容</param>
    /// <param name="t">自动关闭时间</param>
    public void showTip(string info,float t=2)
    {
        TipView.t = t;
        TipView.tip = info;
        TipView.ShowView(true, 10);
    }

    public string getGoldK(int num)
    {
        string str = num.ToString();
        if (num > 1000)
        {
            int q = (num / 1000);

            int y = num % 1000;
            if (y <= 0)
            {
                str = q + "k";
            }
            else
            {
                string qianstr = "";
                bool ishave = false;
                for (int i = 2; i <= 3; i++)
                {
                    
                    int mm=(int)(num% Mathf.Pow(10, i));
                    mm = mm - (int)(mm % Mathf.Pow(10, i - 1));
                    mm =int.Parse(mm.ToString().Substring(0,1));
                    //Debug.Log("大学:::::" + mm+"   "+ num);
                    if (mm !=0)
                    {
                        qianstr = mm + qianstr;
                        ishave = true;
                    }
                    else
                    {
                        if (ishave == true)
                        {
                            qianstr = mm + qianstr;
                        }
                    }
                    
                }
                if (qianstr != "")
                {
                    qianstr = "." + qianstr;
                }
                str = q + qianstr + "k";
            }
        }
        return str;
    }

    public int getTime()
    {
        TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
        return (int)(ts.TotalMilliseconds / 1000);
    }
    #endregion


}