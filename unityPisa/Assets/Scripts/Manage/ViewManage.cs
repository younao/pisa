using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ViewManage 
{
    public Dictionary<string,GameObject> ViewPreList = new Dictionary<string, GameObject>();

    public static ViewManage Instance = null;

    public GameObject open_anim = null;

    public GameObject Ui = null;

    public GameObject UiPre = null;

    public List<GameObject> UiList = new List<GameObject>();

    public GameObject View = null;

    public string viewPath= "View/";

    public ViewManage()
    {
        
        ViewManage.Instance = this;
        initAll();
    }

    public GameObject newUi(int order)
    {
        GameObject g = null;
        for (int i = 0; i < this.View.transform.childCount; i++)
        {
            Canvas ui = View.transform.GetChild(i).GetComponent<Canvas>();
            if (ui.sortingOrder == order)
            {
                g = ui.gameObject;
                break;
            }
        }

        if (g == null)
        {
            
            g = GameObject.Instantiate(UiPre);
            Tools.Instance.ClearObj(g);
            g.transform.SetParent(this.View.transform,false);
        }
        g.GetComponent<Canvas>().sortingOrder = order;
        return g;
    }

    private void initAll()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] glist= scene.GetRootGameObjects();
        for (int i = 0; i < glist.Length; i++)
        {
            if(glist[i].name== "View")
            {
                this.View = glist[i];
                
                break;
            }
        }
        this.Ui = Tools.Instance.GetGameObjectByName("Ui", this.View);
        UiPre = GameObject.Instantiate(this.Ui);
        Tools.Instance.ClearObj(UiPre);
        open_anim= Tools.Instance.GetGameObjectByName("open_anim", this.View);
    }

    private GameObject _LoadView(string viewname,int order)
    {
        GameObject view = null;

        GameObject uipar = newUi(order);

        if (uipar == null)
        {
            return null;
        }
        for (int i = 0; i < uipar.transform.childCount; i++)
        {
            if (uipar.transform.GetChild(i).name == viewname)
            {
                view = uipar.transform.GetChild(i).gameObject;
            }
        }
        if (view == null)
        {
            GameObject pre = null;
            if (ViewPreList.ContainsKey(viewname) == false)
            {
                pre = Resources.Load<GameObject>(this.viewPath + viewname);
                ViewPreList[viewname] = pre;
            }
            else
            {
                pre = ViewPreList[viewname];
            }
            view = GameObject.Instantiate(pre);
            view.transform.SetParent(uipar.transform,false);

        }
        view.name = viewname;
        return view;
        
    }

    public GameObject LoadView(string viewname, bool isTop = true, int order = 0)
    {
        if (ViewManage.Instance.Ui == null)
        {
            return null;
        }
        GameObject view = ViewManage.Instance._LoadView(viewname, order);
        if (isTop == true)
        {
            view.transform.SetAsLastSibling();
        }
        view.SetActive(true);
        return view;
    }

    public void ShowView(GameObject view, bool isTop = true, int order = 0)
    {
        GameObject uipar = newUi(order);
        if (uipar != view.transform.parent)
        {
            view.transform.SetParent(uipar.transform);
        }
        if (isTop == true)
        {
            view.transform.SetAsLastSibling();
        }
        view.SetActive(true);
    }

}