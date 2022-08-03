using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow self = null;

    public GameObject view1;
    public GameObject player;

    private void Awake()
    {
        self = this;
        if (view1 != null)
        {
            oripos1.x =  view1.transform.position.x-player.transform.position.x;
            oripos1.y =  view1.transform.position.y-player.transform.position.y;
            oripos1.z =  view1.transform.position.z-player.transform.position.z;
            oriposRo1.x = view1.transform.eulerAngles.x;
            oriposRo1.y = view1.transform.eulerAngles.y;
            oriposRo1.z = view1.transform.eulerAngles.z;
        }
    }

    int shakeCount = 0;
    public void shake(int count = 10)
    {
        currentPosition = transform.localPosition;
        //设置抖动次数
        this.shakeCount = count;

    }

    Vector3 currentPosition = new Vector3();
    Vector3 huan = new Vector3();
    void douNoMove()
    {
        this.shakeCount--;
        float shakeDelta = 0.05f;
        //抖动最后一次时设置为都动前记录的位置
        if (this.shakeCount == 0)
        {
            this.transform.localPosition = this.currentPosition;
        }
        else
        {
            huan.x = this.currentPosition.x + Random.Range(0, 1.0f) * shakeDelta;
            huan.y = this.currentPosition.y + Random.Range(0, 1.0f) * shakeDelta;
            huan.z = this.currentPosition.z;
            transform.localPosition = huan;

        }
    }


    void Start()
    {
        currentPosition = transform.localPosition;
    }
    bool _isView1 = false;
    public bool isView1
    {
        get
        {
            return _isView1;
        }
        set
        {
            if (value == false)
            {
                transform.eulerAngles = oriposRo;
            }
            else
            {
                transform.eulerAngles = oriposRo1;
            }
            
            _isView1 = value;
        }
    }
    public GameObject followObj = null;
    public bool isFollow = false;
    Vector3 oripos = new Vector3();
    Vector3 oriposRo = new Vector3();

    Vector3 oriposRo1 = new Vector3();
    Vector3 oripos1 = new Vector3();
    public void follow(GameObject g)
    {
        isFollow = true;
        oripos.x = transform.position.x - g.transform.position.x;
        oripos.y = transform.position.y - g.transform.position.y;
        oripos.z = transform.position.z - g.transform.position.z;
        oriposRo.x = transform.eulerAngles.x;
        oriposRo.y = transform.eulerAngles.y;
        oriposRo.z = transform.eulerAngles.z;
        followObj = g;
    }
    Vector3 huan1 = new Vector3();
    void updateFollow()
    {
        if (isView1 == true)
        {
            huan1.x = followObj.transform.position.x + oripos1.x;
            huan1.y = followObj.transform.position.y + oripos1.y;
            huan1.z = followObj.transform.position.z + oripos1.z;
        }
        else
        {
            huan1.x = followObj.transform.position.x + oripos.x;
            huan1.y = followObj.transform.position.y + oripos.y;
            huan1.z = followObj.transform.position.z + oripos.z;
        }

        transform.position = huan1;
    }

    public GameObject moveObj = null;
    public bool isMoveGo = false;
    public void moveTarget(GameObject g)
    {
        isMoveGo = true;
        moveObj = g;
    }

    Vector3 huan2 = new Vector3();
    void updateMove()
    {
        huan1.x = (moveObj.transform.position.x + oripos.x - transform.position.x) * 0.01f;
        huan1.y = (moveObj.transform.position.y + oripos.y - transform.position.y) * 0.01f;
        huan1.z = (moveObj.transform.position.z + oripos.z - transform.position.z) * 0.01f;
        //Debug.Log("坐标：：" + oripos + "  :" + oripos1+" :"+huan1);
        transform.position += huan1;
    }
    // Update is called once per frame
    void Update()
    {
        if (this.shakeCount > 0)
        {
            this.douNoMove();
        }
        if (isMoveGo == true)
        {
            updateMove();
        }
        else
        {
            if (followObj != null)
            {
                if (isFollow == true)
                {
                    this.updateFollow();
                }
            }
        }


    }
}
