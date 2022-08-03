using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManage : BaseScript
{ 
    public static MusicManage Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    public int MusicId = 0;
    /// <summary>
    /// 所有声音对象
    /// </summary>
    public Dictionary<string, GameObject> Allmusic = new Dictionary<string, GameObject>();

    public Dictionary<string, AudioClip> AllClip = new Dictionary<string, AudioClip>();

    public GameObject play(MusicType ty,string name,bool isLoop=false)
    {
        if (DataAll.Instance.isMusic == 1)
        {
            return null;
        }
        MusicId++;
        string musicstr = "music_" + MusicId;
       // Debug.Log("音乐id：1：" + MusicId+","+ name);

        string path = "";
        switch (ty)
        {
            case MusicType.music:
                path = "Music/";
                break;
            case MusicType.audio:
                path = "audio/";
                break;
            default:
                break;
        }
        path = path + name;
        AudioClip clip = null;
        if (AllClip.ContainsKey(path))
        {
            clip = AllClip[path];
        }
        else
        {
            clip = Resources.Load<AudioClip>(path);
            AllClip.Add(path, clip);
        }
        float t = clip.length;
        GameObject g = getMusicObj();
        g.name = musicstr;
        AudioSource au = g.GetComponent<AudioSource>();
        au.clip = clip;
        au.loop = isLoop;
        if (isLoop == false)
        {
            StartCoroutine(OneRemove(t, g));
        }
        au.Play();
        Allmusic.Add(musicstr, g);
        return g;
    }

    GameObject bgMusic = null;
    public void playBG(string bg)
    {
        GameObject g = bgMusic;
        if (bgMusic == null)
        {
            MusicId++;
            //Debug.Log("音乐id：1：" + MusicId);
            string musicstr = "music_" + MusicId;
            g = getMusicObj();
            g.name = musicstr;
            bgMusic = g;
            Allmusic.Add(musicstr, g);
        }
        string path = "audio/"+ bg;
        AudioSource au = g.GetComponent<AudioSource>();
        au.loop = true;
        au.clip = Resources.Load<AudioClip>(path);
        au.Play();
    }
    public void StopBg()
    {
        if (bgMusic != null)
        {
            AudioSource au = bgMusic.GetComponent<AudioSource>();
            au.Stop();
        }
    }

    public void ReMove(string n)
    {
        if (Allmusic.ContainsKey(n))
        {
            GameObject g = Allmusic[n];
            g.GetComponent<AudioSource>().Stop();
            g.GetComponent<AudioSource>().clip = null;
            Allmusic.Remove(n);
        }
    }

    GameObject getMusicObj()
    {
        GameObject g = null;

        if (oneRemoveList.Count!=0)
        {
            g = oneRemoveList[0];
            oneRemoveList.RemoveAt(0);
        }

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    AudioSource au = transform.GetChild(i).GetComponent<AudioSource>();
        //    if (au.clip == null)
        //    {
        //        g = transform.GetChild(i).gameObject;
        //    }
        //}
        if (g == null)
        {
            g = new GameObject();
            g.transform.parent = transform;
            AudioSource au = g.AddComponent<AudioSource>();
        }
        
        return g;
    }

    List<GameObject> oneRemoveList = new List<GameObject>();

    IEnumerator OneRemove(float t,GameObject g)
    {
        yield return new WaitForSeconds(t);
        if (g != null)
        {
            AudioSource au = g.AddComponent<AudioSource>();
            au.clip = null;
            //Debug.Log("移除音乐>>>>：" + g.name + au.clip);
            oneRemoveList.Add(g);
            if (Allmusic.ContainsKey(g.name))
            {
                Allmusic.Remove(g.name);
            }
        }
    }
}

public enum MusicType{
    path = 0,
    music=1,
    audio=2,
}


