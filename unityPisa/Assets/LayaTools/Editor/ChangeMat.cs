using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ChangeMat : EditorWindow
{
    public LayaShader Tty = LayaShader.BlinnPhong;

    public LayaShader1 Tty1 = LayaShader1.all;

    [SerializeField]//必须要加
    protected List<GameObject> Gameobjectlist = new List<GameObject>();
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;

    public string changeStr="";

    //[MenuItem("Tools/changeMaterial")]
    //添加菜单栏用于打开窗口
    ChangeMat()
    {
        this.titleContent = new GUIContent("选择修改材质");
    }
    [MenuItem("Tools/选择修改材质")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(ChangeMat));
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

        //Rect fileRect1 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        //Tty = (LayaShader)EditorGUILayout.EnumPopup(Tty);//EnumFlagsField

        //if (GUILayout.Button("修改选择的材质"))
        //{
        //    change();
        //}

        Rect fileRect2 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        Tty = (LayaShader)EditorGUILayout.EnumPopup(Tty);//EnumFlagsField

        Rect fileRect3 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        Tty1 = (LayaShader1)EditorGUILayout.EnumPopup(Tty1);//EnumFlagsField

        if (GUILayout.Button("修改空材质"))
        {
            change1();
        }



        if (GUILayout.Button("修改选择的材质"))
        {
            change2();
        }

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



        if (GUILayout.Button("修改物体下所有材质"))
        {

            Debug.Log("ssgfsgsgs：");
            for (int i = 0; i < Gameobjectlist.Count; i++)
            {
                GameObject g = Gameobjectlist[i];
                List<GameObject> glist = LayaTools.getAllObj(g, true);
                Debug.Log("sssssssssssssss：" + glist.Count);
                for (int j = 0; j < glist.Count; j++)
                {
                    SetMat(glist[j]);
                }
                
            }
        }


        changeStr = EditorGUILayout.TextField(changeStr);
        if (GUILayout.Button("修改指定的材质"))
        {
            changeStr1();
        }

        if (GUILayout.Button("修改场景里指定的材质"))
        {
            for (int i = 0; i < Gameobjectlist.Count; i++)
            {
                GameObject g = Gameobjectlist[i];
                List<GameObject> glist = LayaTools.getAllObj(g, true);
                for (int j = 0; j < glist.Count; j++)
                {
                    SetMat3(glist[j]);
                }

            }
        }

    }

    void SetMat3(GameObject g)
    {
        List<Material> mlist = new List<Material>();
        MeshRenderer meshR = g.GetComponent<MeshRenderer>();
        if (meshR != null)
        {
            for (int i = 0; i < meshR.sharedMaterials.Length; i++)
            {
                mlist.Add(meshR.sharedMaterials[i]);
            }
        }
        if (mlist.Count > 0)
        {
            for (int i = 0; i < mlist.Count; i++)
            {
                if (mlist[i].shader != null)
                {
                    setMat4(mlist[i]);

                }
            }


        }
        mlist = new List<Material>();

        SkinnedMeshRenderer meshS = g.GetComponent<SkinnedMeshRenderer>();
        if (meshS != null)
        {
            for (int i = 0; i < meshS.sharedMaterials.Length; i++)
            {
                mlist.Add(meshS.sharedMaterials[i]);
            }
        }

        if (mlist.Count > 0)
        {
            for (int i = 0; i < mlist.Count; i++)
            {
                setMat4(mlist[i]);

            }
        }
    }

    void setMat4(Material mat)
    {
        string shaderPath = "";
        //if (mat.shader.name == "")
        //{
        //    return;
        //}
        string[] shader = mat.shader.name.Split('/');
        switch (Tty)
        {
            case LayaShader.BlinnPhong:
                shaderPath = "LayaAir3D/Mesh/BlinnPhong";
                break;
            case LayaShader.Effect:
                shaderPath = "LayaAir3D/Mesh/Effect";
                break;
            case LayaShader.PBRSpecularMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRSpecularMaterial";
                break;
            case LayaShader.PBRStandardMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRStandardMaterial";
                break;
            case LayaShader.Unlit:
                shaderPath = "LayaAir3D/Mesh/Unlit";
                break;
            case LayaShader.Particle_ShurikenParticle:
                shaderPath = "LayaAir3D/Particle/ShurikenParticle";
                break;
            default:
                break;
        }

        List<UnityEngine.Object> list = new List<UnityEngine.Object>();
        if (mat.shader.name == changeStr)
        {
            FindMater2(mat, shaderPath);
        }

    }


    private void changeStr1()
    {
        UnityEngine.Object[] m_objects = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);//选择的所以对象  

        //if (m_objects.length != 1)
        //{
        //    debug.log("选择的材质不唯一");
        //    return;
        //}
        string shaderPath = "";
        switch (Tty)
        {
            case LayaShader.BlinnPhong:
                shaderPath = "LayaAir3D/Mesh/BlinnPhong";
                break;
            case LayaShader.Effect:
                shaderPath = "LayaAir3D/Mesh/Effect";
                break;
            case LayaShader.PBRSpecularMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRSpecularMaterial";
                break;
            case LayaShader.PBRStandardMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRStandardMaterial";
                break;
            case LayaShader.Unlit:
                shaderPath = "LayaAir3D/Mesh/Unlit";
                break;
            case LayaShader.Particle_ShurikenParticle:
                shaderPath = "LayaAir3D/Particle/ShurikenParticle";
                break;
            default:
                break;
        }

        List<UnityEngine.Object> list = new List<UnityEngine.Object>();

        foreach (UnityEngine.Object go in m_objects)
        {
            Material mat = go as Material;

            if (mat.shader.name == changeStr)
            {
                list.Add(mat);
            }
        }
        Debug.Log("材质的名字：" + list.Count+">>>>>>"+changeStr);
        for (int i = 0; i < list.Count; i++)
        {
            FindMater2(list[i] as Material, shaderPath);
        }
    }


    void SetMat(GameObject g)
    {
        List<Material> mlist = new List<Material>();
        MeshRenderer meshR = g.GetComponent<MeshRenderer>();
        if (meshR != null)
        {
            for (int i = 0; i < meshR.sharedMaterials.Length; i++)
            {
                mlist.Add(meshR.sharedMaterials[i]);
            }
        }
        if (mlist.Count > 0)
        {
            for (int i = 0; i < mlist.Count; i++)
            {
                if (mlist[i].shader != null)
                {
                    setMat1(mlist[i]);

                }
            }


        }
        mlist = new List<Material>();

        SkinnedMeshRenderer meshS = g.GetComponent<SkinnedMeshRenderer>();
        if (meshS != null)
        {
            for (int i = 0; i < meshS.sharedMaterials.Length; i++)
            {
                mlist.Add(meshS.sharedMaterials[i]);
            }
        }

        if (mlist.Count > 0)
        {
            for (int i = 0; i < mlist.Count; i++)
            {
                setMat1(mlist[i]);

            }
        }
    }

    private void setMat1(Material mat)
    {
        Debug.Log("aaaaaaaaaaaaaaaaaa：" + mat.shader.name);
        string shaderPath = "";
        if (mat.shader.name == "")
        {
            return;
        }
        string[] shader = mat.shader.name.Split('/');
        switch (Tty)
        {
            case LayaShader.BlinnPhong:
                shaderPath = "LayaAir3D/Mesh/BlinnPhong";
                break;
            case LayaShader.Effect:
                shaderPath = "LayaAir3D/Mesh/Effect";
                break;
            case LayaShader.PBRSpecularMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRSpecularMaterial";
                break;
            case LayaShader.PBRStandardMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRStandardMaterial";
                break;
            case LayaShader.Unlit:
                shaderPath = "LayaAir3D/Mesh/Unlit";
                break;
            case LayaShader.Particle_ShurikenParticle:
                shaderPath = "LayaAir3D/Particle/ShurikenParticle";
                break;
            default:
                break;
        }

        List<UnityEngine.Object> list = new List<UnityEngine.Object>();
        Debug.Log("sssssssssssssss:" + shader[0]);
        if(shader[0]== "LayaAir3D")
        {

        }
        else
        {
            FindMater2(mat, shaderPath);
        }

    }



    public void change2()
    {
        UnityEngine.Object[] m_objects = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);//选择的所以对象  

        //if (m_objects.length != 1)
        //{
        //    debug.log("选择的材质不唯一");
        //    return;
        //}
        string shaderPath = "";
        switch (Tty)
        {
            case LayaShader.BlinnPhong:
                shaderPath = "LayaAir3D/Mesh/BlinnPhong";
                break;
            case LayaShader.Effect:
                shaderPath = "LayaAir3D/Mesh/Effect";
                break;
            case LayaShader.PBRSpecularMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRSpecularMaterial";
                break;
            case LayaShader.PBRStandardMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRStandardMaterial";
                break;
            case LayaShader.Unlit:
                shaderPath = "LayaAir3D/Mesh/Unlit";
                break;
            case LayaShader.Particle_ShurikenParticle:
                shaderPath = "LayaAir3D/Particle/ShurikenParticle";
                break;
            case LayaShader.Standard:
                shaderPath = "Standard";
                break;
            default:
                break;
        }

        foreach (UnityEngine.Object go in m_objects)
        {
            FindMater2(go as Material, shaderPath);
        }


    }


    public void change1()
    {
        UnityEngine.Object[] m_objects = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);//选择的所以对象  

        //if (m_objects.length != 1)
        //{
        //    debug.log("选择的材质不唯一");
        //    return;
        //}
        string shaderPath = "";
        switch (Tty)
        {
            case LayaShader.BlinnPhong:
                shaderPath = "LayaAir3D/Mesh/BlinnPhong";
                break;
            case LayaShader.Effect:
                shaderPath = "LayaAir3D/Mesh/Effect";
                break;
            case LayaShader.PBRSpecularMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRSpecularMaterial";
                break;
            case LayaShader.PBRStandardMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRStandardMaterial";
                break;
            case LayaShader.Unlit:
                shaderPath = "LayaAir3D/Mesh/Unlit";
                break;
            case LayaShader.Particle_ShurikenParticle:
                shaderPath = "LayaAir3D/Particle/ShurikenParticle";
                break;
            case LayaShader.Standard:
                shaderPath = "Standard";
                break;
            default:
                break;
        }

        foreach (UnityEngine.Object go in m_objects)
        {
            FindMater(go as Material, shaderPath);
        }

    }

    public void change()
    {
        UnityEngine.Object[] m_objects = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);//选择的所以对象  

        //if (m_objects.length != 1)
        //{
        //    debug.log("选择的材质不唯一");
        //    return;
        //}
        string shaderPath = "";
        switch (Tty)
        {
            case LayaShader.BlinnPhong:
                shaderPath = "LayaAir3D/Mesh/BlinnPhong";
                break;
            case LayaShader.Effect:
                shaderPath = "LayaAir3D/Mesh/Effect";
                break;
            case LayaShader.PBRSpecularMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRSpecularMaterial";
                break;
            case LayaShader.PBRStandardMaterial:
                shaderPath = "LayaAir3D/Mesh/PBRStandardMaterial";
                break;
            case LayaShader.Unlit:
                shaderPath = "LayaAir3D/Mesh/Unlit";
                break;
            case LayaShader.Particle_ShurikenParticle:
                shaderPath = "LayaAir3D/Particle/ShurikenParticle";
                break;
            default:
                break;
        }

        foreach (UnityEngine.Object go in m_objects)
        {
            FindMater(go as Material, shaderPath);
        }

    }

    public void FindMater2(Material mat, string shader)
    {
        //Debug.Log("ssss:"+mat.shader.name);
        Texture t = GetMainTex(mat);
        Color c = getColor(mat);
        mat.shader = Shader.Find(shader);
        mat.mainTexture = t;
        mat.SetTexture("albedoTexture", t);
        mat.color = c;
        mat.SetColor("albedoColor", c);

    }

    public void FindMater(Material mat, string shader)
    {
        //Debug.Log("ssss:"+mat.shader.name);
        if (mat.shader.name == "")
        {
            Texture t = GetMainTex(mat);
            Color c = getColor(mat);
            mat.shader = Shader.Find(shader);
            mat.mainTexture = t;
            mat.SetTexture("albedoTexture", t);
            mat.color = c;
            mat.SetColor("albedoColor", c);
        }

    }

    public static Texture GetMainTex(Material mat)
    {
        if (mat != null)
        {
            Texture t = null;
            string[] str = mat.shader.name.Split('/');
            string n = str[str.Length - 1];
            if (n == "PBRSpecularMaterial" || n == "PBRStandardMaterial")
            {
                t = mat.GetTexture("albedoTexture");
            }
            else
            {
                t = mat.mainTexture;
            }
            if (t == null)
            {
                string[] str1 = mat.GetTexturePropertyNames();
                for (int i = 0; i < str1.Length; i++)
                {
                    Texture tt = mat.GetTexture(str1[i]);
                    if (tt != null)
                    {
                        t = tt;
                        break;
                    }
                }
            }
            return t;
        }
        else
        {
            return null;
        }
    }
    
    Color getColor(Material mat)
    {
        Color t=Color.white;
        string[] str = mat.shader.name.Split('/');
        string n = str[str.Length - 1];
        if (n == "PBRSpecularMaterial" || n == "PBRStandardMaterial")
        {
            t = mat.GetColor("albedoColor");
        }
        else
        {
            
            t = mat.color;
        }
        if (t.a == 0)
        {
            t= Color.white;
        }
        Debug.Log("color:" + t);
        return t;
    }
    

}
//LayaAir3D/Mesh/BlinnPhong
[Flags]
public enum LayaShader
{
    BlinnPhong = 0,
    Effect = 1,
    PBRSpecularMaterial = 2,
    PBRStandardMaterial = 3,
    Unlit = 4,
    Particle_ShurikenParticle = 5,
    Standard = 6,

}

public enum LayaShader1
{
    BlinnPhong = 0,
    Effect = 1,
    PBRSpecularMaterial = 2,
    PBRStandardMaterial = 3,
    Unlit = 4,
    Particle_ShurikenParticle = 5,
    Standard = 6,

    all=10,

}