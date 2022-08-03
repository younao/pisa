using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExportJson : EditorWindow
{
    // Start is called before the first frame update
    [SerializeField]//必须要加
    protected List<GameObject> objlist = new List<GameObject>();
    //public GameObject objlist;
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;

    //是否寻找第一层
    public bool isFindAll = true;

    //是否有激活的active
    public bool isHaveActive = false;


    //格式：类型_数据

    //利用构造函数来设置窗口名称
    ExportJson()
    {
        this.titleContent = new GUIContent("导出json文件");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/导出json文件")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(ExportJson));
    }

    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性
        _assetLstProperty = _serializedObject.FindProperty("objlist");
    }

    void OnGUI()
    {

        Rect fileRect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isFindAll = EditorGUI.Toggle(fileRect, "是否寻找第一层", isFindAll);

        Rect fileRect1 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isHaveActive = EditorGUI.Toggle(fileRect1, "是否有激活的active", isHaveActive);

        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        //绘制对象
        //GUILayout.Space(10);
        //objlist = (GameObject)EditorGUILayout.ObjectField("Buggy Game Object", objlist, typeof(GameObject), true);
        EditorGUILayout.PropertyField(_assetLstProperty, true);



        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {//提交修改
            _serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("开始生成json"))
        {
            start();
        }
    }


    void start()
    {
        ConfigMap map = new ConfigMap();
        for (int i = 0; i < objlist.Count; i++)
        {

            GameObject par = objlist[i];

            for (int k = 0; k < par.transform.childCount; k++)
            {
                GameObject g = par.transform.GetChild(k).gameObject;
                List<GameObject> listg = GetByName(g);

                List<objinfo> info = new List<objinfo>();
                for (int j = 0; j < listg.Count; j++)
                {
                    info.Add(ToInfo(listg[j]));
                }


                objParent parinfo = new objParent();
                parinfo.name = g.name;
                parinfo.objinfo = info;
                map.mapinfo.Add(parinfo);
            }

            
        }
        string str= JsonUtility.ToJson(map);

        byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);

        //FileInfo f4 = new FileInfo(Application.dataPath+ "/ExportJson/"+ "ConfigMap.js");
        using (FileStream fs = File.Create(Application.dataPath + "LayaTools/Editor/json/" + "ConfigMap.json"))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
        Debug.Log("得到的json：" + str+","+ Application.dataPath);
    }

    List<GameObject> GetByName(GameObject g)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            if (isHaveActive == false)
            {
                if (g.transform.GetChild(i).gameObject.activeInHierarchy == true)
                {
                    alllist.Add(g.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                alllist.Add(g.transform.GetChild(i).gameObject);
            }
            if (isFindAll == true)
            {
                if (isHaveActive == false)
                {
                    if (g.transform.GetChild(i).gameObject.activeInHierarchy == false)
                    {
                        continue;
                    }
                }
                if (g.transform.GetChild(i).childCount > 0)
                {
                    List<GameObject> glist = GetObjAll(g.transform.GetChild(i).gameObject);
                    alllist.AddRange(glist);
                }
            }

        }
        return alllist;
    }


    List<GameObject> GetObjAll(GameObject g)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            if (isHaveActive == false)
            {
                if (g.transform.GetChild(i).gameObject.active == true)
                {
                    alllist.Add(g.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                alllist.Add(g.transform.GetChild(i).gameObject);
            }
            if (isHaveActive == false)
            {
                if (g.transform.GetChild(i).gameObject.active == false)
                {
                    continue;
                }
            }
            if (g.transform.GetChild(i).childCount > 0)
            {
                alllist.AddRange(GetObjAll(g.transform.GetChild(i).gameObject));
            }
        }
        return alllist;
    }

    objinfo ToInfo(GameObject g)
    {
        objinfo info = new objinfo();

        info.name = g.name;
        float x = -g.transform.position.x;
        float y = g.transform.position.y;
        float z = g.transform.position.z;
        info.pos = x + "," + y + "," + z;
        info.angle = g.transform.eulerAngles.x + "," + ((-g.transform.eulerAngles.y)) + "," + g.transform.eulerAngles.z;
        info.scale= g.transform.lossyScale.x + "," + g.transform.lossyScale.y + "," + g.transform.lossyScale.z;
        string[] strlist = info.name.Split('_');
        info.objtype = strlist[0];
        if (strlist.Length > 0)
        {
            for (int i = 1; i < strlist.Length; i++)
            {
                info.data.Add(strlist[i]);
            }
        }
        return info;
    }


    public string ReadJson(string path="")
    {
        string json = "";
        TextAsset text = Resources.Load<TextAsset>(path);
        json = text.text;
        if (string.IsNullOrEmpty(json)) return null;
        return json;

    }

    public List<string> LoadAll(string path)
    {
        List<string> json = new List<string>();
        Debug.Log("加载游戏json：文件夹："+path);

       TextAsset[] text =Resources.LoadAll<TextAsset>(path);
        for (int i = 0; i < text.Length; i++)
        {
            Debug.Log("加载游戏json：" + text[i].name);
            json.Add(text[i].text);
        }
        
        return json;
    }
}

[Serializable]
public class objinfo
{
    public string name;
    public string pos;
    public string angle;
    public string scale;
    public string objtype;
    public List<string> data=new List<string>();
}
//[Serializable]
//public class objdir
//{
//    public string name;
//    public List<objinfo> objinfo = new List<objinfo>();
    
//}

[Serializable]
public class objParent
{
    public string name;
    public List<objinfo> objinfo = new List<objinfo>();

}


[Serializable]
public class ConfigMap
{
    public List<objParent> mapinfo = new List<objParent>();
    
}