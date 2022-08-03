using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

public class OrganizeResources : EditorWindow
{

    //是否有激活的active
    public bool isHaveActive = false;

    public string scenename = "scene";

    //利用构造函数来设置窗口名称
    OrganizeResources()
    {
        this.titleContent = new GUIContent("整理资源");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/整理资源")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(OrganizeResources));
    }
    protected void OnEnable()
    {
    
    }
    void OnGUI()
    {

        Rect fileRect1 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isHaveActive = EditorGUI.Toggle(fileRect1, "是否只含有激活的active", isHaveActive);

        if (GUILayout.Button("整理资源"))
        {
            Scene scene = SceneManager.GetActiveScene();
            this.scenename = scene.name;
            //string s = AssetDatabase.MoveAsset("Assets/LayaResource", "Assets/test");

            string nostr = "remove_";
            int noid = 0;

            bool isE = true;

            while (isE)
            {
                isE = System.IO.Directory.Exists(Application.dataPath + "/LayaResource/" + nostr + noid);

                if (isE == false)
                {
                    nostr = nostr + noid;
                }
                else
                {
                    noid++;
                }
            }
            string s = AssetDatabase.RenameAsset("Assets/LayaResource/"+ scenename, nostr);
            Debug.Log("所有："+ nostr + ">>>>>" + s);

            totleNum = 0;
            GameObject[] glist= scene.GetRootGameObjects();
            Debug.Log("整理的场景是：" + scene.name);

            bool isLayaResource = System.IO.Directory.Exists(Application.dataPath + "/LayaResource");
            if (isLayaResource == false)
            {
                AssetDatabase.CreateFolder("Assets", "LayaResource");
            }

            foreach (GameObject g in glist)
            {
                if (isHaveActive==false)
                {
                    Organize(g);
                }
                else
                {
                    if (g.activeInHierarchy == true)
                    {
                        Organize(g);
                    }
                }
                
            }
            //AssetDatabase.SaveAssets();
            Debug.Log("一共整理了的数目：" + totleNum);
        }
    }

    void Organize(GameObject g)
    {
        List<GameObject> glist= LayaTools.getAllObj(g, isHaveActive);
        

        for (int i = 0; i < glist.Count; i++)
        {
            getobjAsset(glist[i]);
        }
        
    }
    public  void getobjAsset(GameObject g)
    {
        SetMat(g);
        SetMesh(g);
        SetAmin(g);
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
            Material[] ml = new Material[mlist.Count];
            string path = creatFolder("mat");
            bool isAddM = false;
            for (int i = 0; i < mlist.Count; i++)
            {
                string hou= getHou(mlist[i]);
                if (hou==".mat")
                {
                    string an = getAssetName(mlist[i]);
                    if (an != "")
                    {
                        string pahtm = path + "/" + getAssetName(mlist[i]);
                        string str = AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(mlist[i]), pahtm);
                        string sname = "";
                        if (mlist[i].shader != null)
                        {
                            string[] sli = mlist[i].shader.name.Split('/');
                            sname = "_" + sli[sli.Length - 1];
                        }
                        string pn = g.name + sname + "_" + totleNum + hou;
                        //Debug.Log("修改的名字：" + pn.Trim());
                        AssetDatabase.RenameAsset(pahtm, pn.Trim());
                        Material m = AssetDatabase.LoadAssetAtPath<Material>(pahtm+"/"+pn);
                        ml[i] = m;
                    }
                }
                else
                {
                    isAddM = true;
                    string pn = g.name + "_PBRStandardMaterial"+ "_" + totleNum + ".mat";
                    Material material = new Material(Shader.Find("LayaAir3D/Mesh/PBRStandardMaterial"));
                    Texture t = ChangeMat.GetMainTex(mlist[i]);
                    material.SetTexture("albedoTexture", t);
                    string cmat = "Assets/LayaResource/" + scenename + "/mat/" + pn;
                    AssetDatabase.CreateAsset(material, cmat);
                    Material m= AssetDatabase.LoadAssetAtPath<Material>(cmat);
                    ml[i] = m;
                }

                string[] str1 = mlist[i].GetTexturePropertyNames();
                for (int j = 0; j < str1.Length; j++)
                {
                    Texture tt = mlist[i].GetTexture(str1[j]);
                    if (tt != null)
                    {
                        setFlie(tt, "tex", g.name);
                    }
                }
            }
            if (isAddM==true)
            {
                meshR.sharedMaterials = ml;
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
            Material[] ml = new Material[mlist.Count];
            string path = creatFolder("mat");
            bool isAddM = false;
            for (int i = 0; i < mlist.Count; i++)
            {
                if (mlist[i] == null)
                {
                    continue;
                }
                string hou = getHou(mlist[i]);
                if (hou == ".mat")
                {
                    string an = getAssetName(mlist[i]);
                    if (an != "")
                    {
                        string pahtm = path + "/" + getAssetName(mlist[i]);
                        string str = AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(mlist[i]), pahtm);
                        string sname = "";
                        if (mlist[i].shader != null)
                        {
                            string[] sli = mlist[i].shader.name.Split('/');
                            sname = "_" + sli[sli.Length - 1];
                        }
                        string pn = g.name + sname + "_" + totleNum + hou;
                        //Debug.Log("修改的名字：" + pn.Trim());
                        AssetDatabase.RenameAsset(pahtm, pn.Trim());
                        Material m = AssetDatabase.LoadAssetAtPath<Material>(pahtm + "/" + pn);
                        ml[i] = m;
                    }
                }
                else
                {
                    isAddM = true;
                    string pn = g.name + "_PBRStandardMaterial" + "_" + totleNum + ".mat";
                    Material material = new Material(Shader.Find("LayaAir3D/Mesh/PBRStandardMaterial"));
                    Texture t = ChangeMat.GetMainTex(mlist[i]);
                    material.SetTexture("albedoTexture", t);
                    string cmat = "Assets/LayaResource/" + scenename + "/mat/" + pn;
                    AssetDatabase.CreateAsset(material, cmat);
                    Material m = AssetDatabase.LoadAssetAtPath<Material>(cmat);
                    ml[i] = m;
                }

                string[] str1 = mlist[i].GetTexturePropertyNames();
                for (int j = 0; j < str1.Length; j++)
                {
                    Texture tt = mlist[i].GetTexture(str1[j]);
                    if (tt != null)
                    {
                        setFlie(tt, "tex", g.name);
                    }
                }
            }
            if (isAddM == true)
            {
                meshS.sharedMaterials = ml;
            }

        }
    }


    void SetMesh(GameObject g)
    {
        MeshFilter meshF = g.GetComponent<MeshFilter>();
        if (meshF != null)
        {
            if (meshF.sharedMesh != null)
            {
                //Debug.Log("获取的名字后缀：：：：：" + AssetDatabase.GetAssetPath(meshF.sharedMesh));
                string str= setFlie(meshF.sharedMesh, "mesh", g.name);
                //Debug.Log("mesh>>>>>>>>>>>>>>>>>>" + AssetDatabase.GetAssetPath(meshF.sharedMesh));
            }
        }

        SkinnedMeshRenderer meshS = g.GetComponent<SkinnedMeshRenderer>();
        if (meshS != null)
        {
            if (meshS.sharedMesh != null)
            {
                setFlie(meshS.sharedMesh, "mesh", g.name);
            }
        }

    }

    void SetAmin(GameObject g)
    {
        Animator tor = g.GetComponent<Animator>();
        if (tor != null)
        {
            if (tor.runtimeAnimatorController != null)
            {
                setFlie(tor.runtimeAnimatorController, "anim", g.name);

                for (int i = 0; i < tor.runtimeAnimatorController.animationClips.Length; i++)
                {
                    setFlie(tor.runtimeAnimatorController.animationClips[i], "anim", g.name);
                }
            }

            if (tor.avatar != null)
            {
                setFlie(tor.avatar, "anim", g.name);
            }
        }
    }

    string setFlie(UnityEngine.Object assetObject,string foldername,string n)
    {
        string path = creatFolder(foldername);
        string hou = getHou(assetObject);
        string str = "";
        
        if (hou != "")
        {
            string an = getAssetName(assetObject);
            //Debug.Log("获取的名字：：：：：" + an);
            if (an != "")
            {
                string pahtm = path + "/" + an;
                //Debug.Log("修改的前：：" + AssetDatabase.GetAssetPath(assetObject));
                str = AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(assetObject), pahtm);
                //Debug.Log("修改的后：：" + AssetDatabase.GetAssetPath(assetObject));
                if (str == "")
                {
                    string pt = n + "_" + totleNum + hou;
                    string strc= AssetDatabase.RenameAsset(pahtm, pt.Trim());
                    if (strc != "")
                    {
                        //Debug.Log("修改的名字出错：" + strc);
                    }
                }
            }
        }
        return str;
    }

    int totleNum = 0;

    string getHou(UnityEngine.Object assetObject)
    {
        
        string[] hlist = AssetDatabase.GetAssetPath(assetObject).Split('.');
        string hou ="."+ hlist[hlist.Length - 1];
        Debug.Log(">>>>>>>>>>>>>：" + AssetDatabase.GetAssetPath(assetObject)+">>>>>"+ hou);
        bool ishaveTy = false;
        for (int i = 0; i < objty.Count; i++)
        {
            Debug.Log("获取的后缀：" + objty[i]);
            if (objty[i] == hou)
            {
                ishaveTy = true;
            }
        }
        if (ishaveTy == false)
        {
            hou = "";
        }
        totleNum++;
        //Debug.Log("获取的后缀：" + hou+">>>>>>>>>>"+ assetObject.name);
        return hou;
    }

    string getAssetName(UnityEngine.Object assetObject)
    {
        string[] hlist = AssetDatabase.GetAssetPath(assetObject).Split('/');

        for (int i = 0; i < hlist.Length; i++)
        {
            if(hlist[i]== "StreamingAssets")
            {
                return "";
            }
        }

        return hlist[hlist.Length - 1];
    }

    List<string> objty = new List<string>() {
      ".FBX", ".mat",".fbx",".obj",".png",".jpg",".anim",".asset",".controller",
    };

    string creatFolder(string folderName)
    {
        string path = "Assets/LayaResource/"+ scenename +"/"+ folderName;
        bool isLayaResource = System.IO.Directory.Exists(Application.dataPath + "/LayaResource/"+ scenename);
        if (isLayaResource == false)
        {
            AssetDatabase.CreateFolder("Assets/LayaResource", scenename);
        }

        bool isLayaFolder = System.IO.Directory.Exists(Application.dataPath + "/LayaResource/" + scenename + "/" + folderName);
        if (isLayaFolder == false)
        {
            AssetDatabase.CreateFolder("Assets/LayaResource/" + scenename, folderName);
        }
        return path;
    }
}