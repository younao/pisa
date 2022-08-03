using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ToolsMono : MonoBehaviour
{
    private void Awake()
    {
        Tools.Instance.mono = this;
    }
    private void Update()
    {
        List<FlyData> re = new List<FlyData>();
        for (int i = 0; i < flyList.Count; i++)
        {
            int state= flyList[i].update();
            if (state == 1)
            {
                re.Add(flyList[i]);
            }
        }
        for (int i = 0; i < re.Count; i++)
        {
            flyList.Remove(re[i]);
        }
    }

    public List<FlyData> flyList = new List<FlyData>();

    /// <summary>
    /// 飞过去的特效
    /// </summary>
    /// <param name="g"></param>
    /// <param name="endPos"></param>
    /// <param name="flyT"></param>
    /// <param name="fun"></param>
    /// <param name="isOkDel"></param>
    /// <param name="laterT"></param>
    public void flyEff(GameObject g, Vector3 endPos, float flyT=0.3f,  Action<GameObject, FlyEndData> fun = null, FlyEndData fdata =null, bool isOkDel = false, float laterT = 0)
    {
        FlyData info = new FlyData(g, endPos, flyT, isOkDel, fun, laterT,fdata);
        flyList.Add(info);
    }
}
public class FlyData
{
    public FlyData(GameObject _g,Vector3 _endPos,float _flyT, bool _isDel, Action<GameObject, FlyEndData> _fun, float _laterT,FlyEndData _fdata)
    {
        obj = _g;
        endPos.x = _endPos.x;
        endPos.y = _endPos.y;
        endPos.z = _endPos.z;
        flyT = _flyT;
        isDel = _isDel;
        okFun = _fun;
        laterT = _laterT;
        fdata = _fdata;

        float dic = Vector3.Distance(obj.transform.position, endPos);
        t = -laterT;
        goDir.x = (endPos.x - obj.transform.position.x)/ flyT;
        goDir.y = (endPos.y - obj.transform.position.y)/ flyT;
        goDir.z = (endPos.z - obj.transform.position.z)/ flyT;


        if (endPos.y > obj.transform.position.y)
        {
            goYmax = (endPos.y + offY);
        }
        else
        {
            goYmax = (obj.transform.position.y + offY);
        }
        goYspeed = (Mathf.Abs(endPos.y - obj.transform.position.y) + offY * 2) / flyT;
    }

    public float offY=0.5f;
    float goYmax = 0;
    float goYspeed = 0;

    public GameObject obj;
    public Vector3 endPos = new Vector3();

    public Vector3 goDir = new Vector3();

    public float flyT;
    public bool isDel;
    public Action<GameObject,FlyEndData> okFun = null;
    public FlyEndData fdata = null;
    public float laterT=0;
    public float t = 0;


    public int update()
    {
        int state = 0;
        t += 0.02f;

        if (obj != null)
        {
            if (t >= 0)
            {
                state = fly();
            }
        }
        else
        {

            
            state = 1;
        }
        return state;
    }

    Vector3 huan = new Vector3();
    int fly()
    {
        int state = 0;

        huan.x = obj.transform.position.x + goDir.x * 0.02f;

        float y = obj.transform.position.y + goYspeed * 0.02f;
        if (goYspeed > 0)
        {
            if (y >= goYmax)
            {
                goYspeed = -goYspeed;
            }
        }

        huan.y= y;
        huan.z = obj.transform.position.z + goDir.z * 0.02f;

        obj.transform.position = huan;
        if (t >= flyT)
        {
            obj.transform.position = endPos;
            if (okFun != null)
            {
                okFun(obj,fdata);
            }
            if (isDel == true)
            {
                GameObject.Destroy(obj);
            }
            state = 1;
        }
        return state;
    }
}

public class FlyEndData
{
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public GameObject g = null;
}