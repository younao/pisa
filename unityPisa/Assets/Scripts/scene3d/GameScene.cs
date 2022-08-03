using UnityEngine;
using UnityEditor;
using System.Collections;

public class GameScene : BaseScript
{
    public static GameScene self = null;

    public string mapPath = "Map/";

    public Player player = null;
    public Vector3 playerOri = new Vector3();

    public GameObject pre = null;

    public MapControl mapControl = null;

    public GameObject mouse = null;

    public float addPlayerY = -0.566f;

    private void Awake()
    {
        GameScene.self = this;
        pre = GetOneByName("pre");
        pre.SetActive(false);
    }

    private void Start()
    {
        player = GetGameObjectByName("player").GetComponent<Player>();
        playerOri.x = player.transform.position.x;
        playerOri.y = player.transform.position.y;
        playerOri.z = player.transform.position.z;
        CameraFollow.self.follow(player.gameObject);
        loadMap(DataAll.Instance.level);
    }

    private void Update()
    {
        updateT1 += Time.deltaTime;
        if (updateT1 > 0.5)
        {
            updateT1 = 0;
            update1();
        }
    }

    float updateT1=0;
    /// <summary>
    /// 0.5秒执行一次
    /// </summary>
    void update1()
    {
        if (CameraFollow.self != null)
        {
            if (player != null)
            {
                if (CameraFollow.self.followObj == null)
                {
                    
                }
            }
        }
    }

    public void loadMap(int level)
    {
        if (mapControl != null)
        {
            GameObject.Destroy(mapControl.gameObject);
            mapControl = null;
        }
        DataAll.Instance.level = level;
        GameObject pre= Resources.Load<GameObject>(mapPath + "level_"+level);
        
        GameObject map = GameObject.Instantiate(pre);
        map.transform.SetParent(transform);
        mapControl= map.AddComponent<MapControl>();
        if (player != null)
        {
            if (DataAll.Instance.isOnePlay > 0)
            {
                player.transform.position = playerOri;
            }
        }
    }
}