using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ViewNew : EditorWindow
{
    string prePath = "/Resources/View/";
    string scrPath = "/Scripts/View/";

    //利用构造函数来设置窗口名称
    ViewNew()
    {
        this.titleContent = new GUIContent("生成view代码");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/生成view代码")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(ViewNew));
    }
    
    protected void OnEnable()
    {
        
    }

    void OnGUI()
    {

        if (GUILayout.Button("生成view代码"))
        {
            start();
        }
    }


    void start()
    {

        //获取指定路径下面的所有资源文件
        string path = "Assets" + prePath;
        //Debug.Log("路径：："+ path);
        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);

            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);
            int j = 0;
            
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                string str = files[i].Name.Replace(".prefab", "");
                string scrHave = Application.dataPath + scrPath + str + ".cs";
                GameObject gg = AssetDatabase.LoadAssetAtPath<GameObject>(path + files[i].Name);
                bool ishave= File.Exists(scrHave);
                
                if (ishave == true)
                {
                    string scrss = "Assets" + scrPath + str + ".cs";
                    Debug.Log("路径：：：" + scrss);
                    TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(scrss);
                    //Debug.Log("文件view路径：" + text + ",," + gg);
                    xie(text.text, str, gg);
                }
                else
                {
                    this.newsrc(str,gg);
                }
            }
        }
    }

    void newsrc(string viewName, GameObject gg)
    {
        Debug.Log("修改的view：："+ viewName);
        string ss = scrStr.Replace("###", viewName);
        xie(ss, viewName,gg);
    }

    void xie(string str,string viewName,GameObject gg)
    {
        str = getUiM(str,gg);
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
        using (FileStream fs = File.Create(Application.dataPath  + scrPath + viewName + ".cs"))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
    }

    /// <summary>
    /// 自动生成要获取的obj
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    string getUiM(string str,GameObject g)
    {
        string scrs = str;
        string[] strlist = str.Split('é');
        if (strlist.Length == 3)
        {
            List<GameObject> glist = this.GetObjAll(g);
            string strg = "";
            string strget = "";
            for (int i = 0; i < glist.Count; i++)
            {
                if (glist[i].name.StartsWith("m_") == true)
                {
                    strg += @"
    public GameObject "+ glist[i].name + " = null;";
                    
                    strget += @"
        "+ glist[i].name + @" = GetGameObjectByName(""" + glist[i].name + @""");";
                }
            }
            string strYes = "é获取m_前缀的物体"+ strg + @"
    private void initGetGameObj()
    {"+@""+strget+ @"
    }
    #endregion é";
            scrs = strlist[0] + strYes + strlist[2];   
        }
        return scrs;
    }

    /// <summary>
    /// 获取所有的obj
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    List<GameObject> GetObjAll(GameObject g)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            alllist.Add(g.transform.GetChild(i).gameObject);
            if (g.transform.GetChild(i).childCount > 0)
            {
                alllist.AddRange(GetObjAll(g.transform.GetChild(i).gameObject));
            }
        }
        return alllist;
    }

    #region 示例代码
    string scrStr = @"
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ### : UiBase
{
    #region $自动生成的代码
    #region $显示显示隐藏view
    public static ### self = null;
    public static string ViewName = ""###"";
    public static EventArg arg = null;
    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        ###.arg = arg;
        GameObject g = null;
        if (###.self != null)
        {
            g = ###.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            g = ViewManage.Instance.LoadView(###.ViewName, isTop, order);
        }
        ### scr = g.GetComponent<###>();
        if (scr == null)
        {
            scr = g.AddComponent<###>();
        }
        ###.self = scr;
        ###.self.show(arg);
    }
    private void Awake()
    {
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (###.self != null)
        {
            ###.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (###.self != null)
        {
            GameObject.Destroy(###.self.gameObject);
            ###.self = null;
        }
    }
    #endregion $显示显示隐藏view
    #region é获取m_前缀的物体
    private void initGetGameObj()
    {
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    ///加载调用一次
    private void load()
    {

    }
}
";

    #endregion
}
