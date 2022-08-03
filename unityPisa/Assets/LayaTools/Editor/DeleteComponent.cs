using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class DeleteComponent : EditorWindow
{
    // Start is called before the first frame update
    [SerializeField]//必须要加
    protected List<GameObject> Gameobjectlist = new List<GameObject>();
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;


    //是否有激活的active
    public bool isHaveActive = true;

    public ComponentT Tty = ComponentT.meshCollider;


    //利用构造函数来设置窗口名称
    DeleteComponent()
    {
        this.titleContent = new GUIContent("删除组件");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/删除组件")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(DeleteComponent));
    }
    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性
        _assetLstProperty = _serializedObject.FindProperty("Gameobjectlist");
    }
    void OnGUI()
    {

        Rect fileRect1 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        Tty = (ComponentT)EditorGUILayout.EnumPopup(Tty);//EnumFlagsField

        Rect fileRect2 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isHaveActive = EditorGUI.Toggle(fileRect2, "有激活的active", isHaveActive);

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

        if (GUILayout.Button("开始删除组件"))
        {
            Debug.Log("删除开始？？" + Gameobjectlist.Count);
            for (int i = 0; i < Gameobjectlist.Count; i++)
            {
                DeleteT(Tty, Gameobjectlist[i], isHaveActive);
            }
        }
    }

    public void DeleteT(ComponentT cty,GameObject g,bool ishaveActive)
    {
        switch (cty)
        {
            case ComponentT.meshCollider:
                List<GameObject> glist= LayaTools.getAllObj(g, ishaveActive);
                for (int i = 0; i < glist.Count; i++)
                {
                    MeshCollider mc= glist[i].GetComponent<MeshCollider>();
                    
                    if (mc != null)
                    {
                        Component.DestroyImmediate(mc);
                    }
                }
                break;
            case ComponentT.meshFilter:
                List<GameObject> mglist = LayaTools.getAllObj(g, ishaveActive);
                for (int i = 0; i < mglist.Count; i++)
                {
                    MeshFilter mc = mglist[i].GetComponent<MeshFilter>();

                    if (mc != null)
                    {
                        Component.DestroyImmediate(mc);
                    }
                }
                break;
            case ComponentT.meshRenderer:
                List<GameObject> rglist = LayaTools.getAllObj(g, ishaveActive);
                for (int i = 0; i < rglist.Count; i++)
                {
                    MeshRenderer mc = rglist[i].GetComponent<MeshRenderer>();
                    if (mc != null)
                    {
                        Component.DestroyImmediate(mc);
                    }
                }
                break;
            case ComponentT.rigidbody:
                List<GameObject> riglist = LayaTools.getAllObj(g, ishaveActive);
                for (int i = 0; i < riglist.Count; i++)
                {
                    Rigidbody mc = riglist[i].GetComponent<Rigidbody>();
                    if (mc != null)
                    {
                        Component.DestroyImmediate(mc);
                    }
                }
                break;
            case ComponentT.allPhysical:

                List<GameObject> pglist = LayaTools.getAllObj(g, ishaveActive);
                for (int i = 0; i < pglist.Count; i++)
                {
                    Rigidbody a = pglist[i].GetComponent<Rigidbody>();
                    BoxCollider b = pglist[i].GetComponent<BoxCollider>();
                    SphereCollider c = pglist[i].GetComponent<SphereCollider>();
                    MeshCollider d = pglist[i].GetComponent<MeshCollider>();
                    CapsuleCollider e = pglist[i].GetComponent<CapsuleCollider>();
                    Collider f = pglist[i].GetComponent<Collider>();
                    if (a != null)
                    {
                        Component.DestroyImmediate(a);
                    }
                    if (b != null)
                    {
                        Component.DestroyImmediate(b);
                    }
                    if (c != null)
                    {
                        Component.DestroyImmediate(c);
                    }
                    if (d != null)
                    {
                        Component.DestroyImmediate(d);
                    }
                    if (e != null)
                    {
                        Component.DestroyImmediate(e);
                    }
                    if (f != null)
                    {
                        Component.DestroyImmediate(f);
                    }
                }

                break;
            default:
                break;
        }
    }

}

[Flags]
public enum ComponentT
{
    meshCollider=0,
    meshFilter=1,
    meshRenderer=2,
    rigidbody=3,
    allPhysical=4,

}
