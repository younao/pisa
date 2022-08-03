using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[Serializable]
public class MusicInfo
{
    public int MusicId = 0;
    public bool isTimeJi = false;
    public string name = "";
    public string author = "";
    public string MusicUrl = "";
    public float gao = 0;
    public float jiepai = 0.6f;
    public float startT = 0;
    public bool Isbuy = false;
    public int buyGold = 800;
    public string texurl = "";
    public List<MusicData> info = new List<MusicData>();
}


[Serializable]
public class MusicData
{
    public float t = 0;
    public int pos = 0;
    public int jieT = 0;
    public int ty = 0;
}