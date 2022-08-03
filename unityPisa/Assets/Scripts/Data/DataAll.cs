using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataAll
{
    private static DataAll _instance;
    public static DataAll Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataAll();
            }

            return _instance;
        }
    }

    public void init()
    {
        //PlayerPrefs.DeleteAll();
        isOnePlay = PlayerPrefs.GetInt("isOnePlay");

        PlayerPrefs.SetInt("oneplay", 1);
        //Debug.Log("金币：" + PlayerPrefs.GetInt("gold"));
        gold = PlayerPrefs.GetInt("gold");
        if (isOnePlay < 3)
        {
            isOnePlay = 0;
            gold = 0;
        }
        //gold = 10000;
        isMusic = PlayerPrefs.GetInt("isMusic");
        isZhen = PlayerPrefs.GetInt("isZhen");

        timeLi = PlayerPrefs.GetString("timeLi");
        if (timeLi == "")
        {
            timeLi = Tools.Instance.getTime().ToString();
        }
        //Debug.LogError("时间：：：" + timeLi);

        initMap();
    }

    #region 系统

    /// <summary>
    /// 声音大小
    /// </summary>
    int _isMusic = 0;

    /// <summary>
    /// 声音大小 0开，1关
    /// </summary>
    public int isMusic
    {
        get
        {
            return _isMusic;
        }
        set
        {
            _isMusic = value;
            PlayerPrefs.SetInt("isMusic", value);
        }
    }

    /// <summary>
    /// 声音大小
    /// </summary>
    int _isZhen = 0;

    /// <summary>
    /// 震动 0开，1关
    /// </summary>
    public int isZhen
    {
        get
        {
            return _isZhen;
        }
        set
        {
            _isZhen = value;
            PlayerPrefs.SetInt("isZhen", value);
        }
    }

    /// <summary>
    /// 金币
    /// </summary>
    int _gold = 0;

    public int gold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            PlayerPrefs.SetInt("gold", value);
            if (Tools.Instance != null)
            {

                Tools.Instance.sendMessage("updateGold");
            }
        }
    }


    #endregion

    #region 人物的属性
    int _speed = 0;

    /// <summary>
    /// 速度
    /// </summary>
    public int speed
    {
        get
        { return _speed; }
        set
        {
            _speed = value;
            PlayerPrefs.SetInt("speed", value);
        }
    }

    int _newGold = 0;

    /// <summary>
    /// 速度
    /// </summary>
    public int newGold
    {
        get
        { return _newGold; }
        set
        {
            _newGold = value;
            PlayerPrefs.SetInt( "newGold", value);
        }
    }

    int _addObj = 0;
    /// <summary>
    /// 增加拿着的披萨
    /// </summary>
    public int addObj
    {
        get
        { return _addObj; }
        set
        {
            _addObj = value;
            PlayerPrefs.SetInt( "addObj", value);
        }
    }

    #endregion

    #region ai的属性
    int _addObjAi = 0;

    /// <summary>
    /// 增加拿着的披萨
    /// </summary>
    public int addObjAi
    {
        get
        { return _addObjAi; }
        set
        {
            _addObjAi = value;
            PlayerPrefs.SetInt(level + "addObjAi", value);
        }
    }


    int _addHuman = 0;

    /// <summary>
    /// 增加员工
    /// </summary>
    public int addHuman
    {
        get
        { return _addHuman; }
        set
        {
            _addHuman = value;
            PlayerPrefs.SetInt(level + "addHuman", value);
        }
    }

    int _speedAi = 0;
    /// <summary>
    /// 速度
    /// </summary>
    public int speedAi
    {
        get
        { return _speedAi; }
        set
        {
            _speedAi = value;
            PlayerPrefs.SetInt(level + "speedAi", value);
        }
    }


    #endregion

    #region 地图的进度
    void initMap()
    {
        level = PlayerPrefs.GetInt("level");
        _openLevel = PlayerPrefs.GetString("openLevel");
        if (_openLevel == "")
        {
            _openLevel = "0";
        }

        _openMen= PlayerPrefs.GetInt("openMen");
    }

    private int _level = 0;
    public int level
    {
        get
        {
            return _level;
        }
        set
        {
            _level = value;

            speed = PlayerPrefs.GetInt("speed");
            addObj = PlayerPrefs.GetInt("addObj");
            newGold = PlayerPrefs.GetInt("newGold");

            addHuman = PlayerPrefs.GetInt(level + "addHuman");
            addObjAi = PlayerPrefs.GetInt(level + "addObjAi");
            speedAi = PlayerPrefs.GetInt(level + "speedAi");

            _deskIndex = PlayerPrefs.GetInt(level + "_deskIndex");
            _waimai = PlayerPrefs.GetString(level + "_waimai");
            _getFood = PlayerPrefs.GetString(level + "_getFood");
            if (_getFood == "")
            {
                _getFood = "0";
            }
            PlayerPrefs.SetInt("level", value);

            _foodLevel= PlayerPrefs.GetString(level + "_foodLevel", _foodLevel);
        }
    }

    private string _openLevel = "0";
    public string openLevel
    {
        get
        {
            return _openLevel;
        }
        set
        {
            string[] str = value.Split(',');
            List<string> ss = new List<string>();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != "")
                {
                    if (ss.Contains(str[i])) ;
                    ss.Add(str[i]);
                }
            }
            string sss = "";
            for (int i = 0; i < ss.Count; i++)
            {
                if (i != ss.Count - 1)
                {
                    sss += ss[i] + ",";
                }
                else
                {
                    sss += ss[i];
                }

            }
            _openLevel = sss;
            Debug.Log("保存：：：：：：：：" + sss);
            PlayerPrefs.SetString("openLevel", sss);
        }
    }
    public List<string> getOpenLevel()
    {
        string[] str = _openLevel.Split(',');
        List<string> ss = new List<string>();
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != "")
            {
                if (ss.Contains(str[i])) ;
                ss.Add(str[i]);
            }
        }
        return ss;
    }

    int _deskIndex = 0;
    /// <summary>
    /// 当前开启的桌子
    /// </summary>
    public int deskIndex
    {
        get
        {
            return _deskIndex;
        }
        set
        {
            PlayerPrefs.SetInt(level + "_deskIndex", value);
            _deskIndex = value;
        }
    }

    string _waimai = "";
    /// <summary>
    /// 当前开启的外卖
    /// </summary>
    public List<int> waimai
    {
        get
        {
            string[] str = _waimai.Split(',');
            List<int> idlist = new List<int>();
            for (int i = 0; i < str.Length; i++)
            {
                string v = str[i];
                if (v != "")
                {
                    int id = int.Parse(str[i]);
                    if (idlist.Contains(id) == false)
                    {
                        idlist.Add(id);
                    }
                }
            }
            return idlist;
        }
        set
        {
            string str = "";
            for (int i = 0; i < value.Count; i++)
            {
                str += value[i] + ",";
            }
            _waimai = str;
            PlayerPrefs.SetString(level + "_waimai", str);
        }
    }

    string _getFood = "";
    /// <summary>
    /// 当前开启的外卖
    /// </summary>
    public List<int> getFood
    {
        get
        {
            string[] str = _getFood.Split(',');
            List<int> idlist = new List<int>();
            for (int i = 0; i < str.Length; i++)
            {
                string v = str[i];
                if (v != "")
                {
                    int id = int.Parse(str[i]);
                    if (idlist.Contains(id) == false)
                    {
                        idlist.Add(id);
                    }
                }
            }
            return idlist;
        }
        set
        {
            string str = "";
            for (int i = 0; i < value.Count; i++)
            {
                str += value[i] + ",";
            }
            _getFood = str;
            Debug.Log("设置食物：：" + str + value.Count);
            PlayerPrefs.SetString(level + "_getFood", str);
        }
    }

    public string _foodLevel = "";

    public void setFoodLevel(int index, int lv)
    {

        //string ss = ","+index + ":" + lv;
        string[] str = _foodLevel.Split(',');
        Dictionary<int, int> l = new Dictionary<int, int>();
        bool isHave = false;
        for (int i = 0; i < str.Length; i++)
        {
            string value = str[i];
            if (value != "")
            {
                string[] key = value.Split(':');
                int indexy = int.Parse(key[0]);
                int valuey= int.Parse(key[1]);
                if (indexy == index)
                {
                    l[indexy] = valuey;
                    if (valuey < lv)
                    {
                        l[indexy] = lv;
                    }
                    isHave = true;
                }
                else
                {
                    l[index] = lv;
                }
            }
        }
        string save = "";

        foreach (KeyValuePair<int, int> item in l)//注意类型是KeyValuePair
        {
            int key = item.Key;
            int value = item.Value;
            save += key + ":" + value + ",";
        }
        if (isHave == false)
        {
            Debug.Log("保存：ssssss：：：：" + _foodLevel);
            save += index + ":" + lv;
        }
        _foodLevel = save;
        Debug.Log("保存：：：：：" + _foodLevel);
        PlayerPrefs.SetString(level+ "_foodLevel",_foodLevel);
    }

    public Dictionary<int, int> getFoolLevel (){
        _foodLevel= PlayerPrefs.GetString(level + "_foodLevel", _foodLevel);
        string[] str = _foodLevel.Split(',');
        Dictionary<int, int> l = new Dictionary<int, int>();
        for (int i = 0; i < str.Length; i++)
        {
            string value= str[i];
            if (value != "")
            {
                string[] key= value.Split(':');
                l[int.Parse(key[0])] = int.Parse(key[1]);
                Debug.Log("数据：：：：" + int.Parse(key[0]) + "        " + int.Parse(key[1]));
            }
        }
        return l;
    }
    #endregion

    int _openMen = 0;

    /// <summary>
    /// 增加员工
    /// </summary>
    public int openMen
    {
        get
        { return _openMen; }
        set
        {
            _openMen = value;
            PlayerPrefs.SetInt("openMen", value);
        }
    }

    //是否是第一次玩
    int _isOnePlay = 0;

    public int isOnePlay
    {
        get
        {
            return _isOnePlay;
        }
        set
        {
            _isOnePlay = value;
            PlayerPrefs.SetInt("isOnePlay",_isOnePlay);
        }
    }


    string _timeLi = "";

    public string timeLi
    {
        get
        {
            return _timeLi;
        }
        set
        {
            _timeLi = value;
            PlayerPrefs.SetString("timeLi", value);
        }
    }

}
