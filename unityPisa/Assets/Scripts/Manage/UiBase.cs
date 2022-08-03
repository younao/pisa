using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UiBase : MonoBehaviour
{

    #region 内置方法
    protected virtual T GetComponentByName<T>(string objName, GameObject g = null)
    {
        GameObject FindObj = GetGameObjectByName(objName, g);

        if (FindObj != null)
        {
            return FindObj.GetComponent<T>();
        }
        else
        {
            Debug.LogError("在" + FindObj.name + "找不到该组件:");
            return default(T);
        }
    }

    protected virtual GameObject GetGameObjectByName(string objName, GameObject g = null)
    {
        if (g == null)
        {
            g = gameObject;
        }
        GameObject obj = null;
        if (g.name == objName)
        {
            return g;
        }
        else
        {
            //foreach (Transform t in g.GetComponentsInChildren<Transform>())
            //{
            //    if (t.name == objName)
            //    {
            //        obj = t.gameObject;
            //    }
            //}
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
    protected virtual void AddListenerMessage(string actionName, Action<EventArg> fun)
    {
        eventList.Add(new eventBase(actionName, fun));
        if (ScriptManage.Instance.messageDic.ContainsKey(actionName) == true)
        {
            ScriptManage.Instance.messageDic[actionName].funlist.Add(fun);
        }
        else
        {
            EventObj a = new EventObj();
            a.funlist.Add(fun);
            ScriptManage.Instance.messageDic.Add(actionName, a);
            //Debug.Log("添加监听：：：：：" + ScriptManage.Instance.messageDic.Count);
        }
    }
    protected virtual void RemoveListenerMessage(string actionName, Action<EventArg> fun = null)
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
    protected virtual void sendMessage(string actionName, EventArg arg = null)
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
            Debug.Log("没有监听：" + actionName);
        }
    }
    void destroyEvent()
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            eventBase evn = eventList[i];
            this.RemoveListenerMessage(evn.funstr, evn.fun);
        }
    }
    private List<eventBase> eventList = new List<eventBase>();
    #endregion
    /// <summary>
    /// 显示时候会执行一次
    /// </summary>
    /// <param name="arg"></param>
    public void show(EventArg arg) { }

    private void OnDestroy()
    {
        this.destroyEvent();
    }
}
