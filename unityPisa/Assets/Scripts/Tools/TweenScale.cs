using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TweenScale : MonoBehaviour
{

    // Use this for initialization
    public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);

    public float time = 1;

    public Vector3 form = Vector3.zero;
    public Vector3 to = Vector3.one;

    public float startTime = 0;
    public TweenMore more = TweenMore.one;

    public Action<GameObject> Finish;

    public bool ActionRePlay = true;

    public string Data = "";

    private Transform tran;

    private void Awake()
    {
        tran = transform.GetComponent<Transform>();

        form = tran.transform.localScale;
        Debug.Log("大小：：：：：" + form);

    }

    void Start()
    {
        //if (gameObject.active == true)
        //{
        //    if (startIe != null)
        //    {
        //        StopCoroutine(startIe);
        //        startIe = null;
        //    }
        //    startIe = StartTween(startTime);

        //    StartCoroutine(startIe);
        //}
    }

    IEnumerator startIe = null;

    IEnumerator StartTween(float t)
    {
        isPlay = true;
        yield return new WaitForSeconds(t);
        if (curve != null)
        {
            if (ScaleMoveIe != null)
            {
                StopCoroutine(ScaleMoveIe);
                ScaleMoveIe = null;
            }
            ScaleMoveIe = ScaleMove(more);
            StartCoroutine(ScaleMoveIe);
        }
    }
    public bool isPlay = false;
    public void stopPlay()
    {
        isPlay=false;
        if (ScaleMoveIe != null)
        {
            StopCoroutine(ScaleMoveIe);
            ScaleMoveIe = null;
        }
    }

    IEnumerator ScaleMoveIe = null;
    private IEnumerator ScaleMove(TweenMore m)
    {
        float startT = 0;
        if (m == TweenMore.one)
        {
            while (startT < time)
            {
                if (tran != null)
                {
                    float x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
                    float y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
                    float z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
                    tran.localScale = new Vector3(x, y, z);
                }
                else
                {
                    startT = 1000000;
                }
                startT += 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if (m == TweenMore.loop)
        {
            bool isrun = true;
            while (isrun)
            {
                if (startT > time)
                {
                    startT = 0;
                }
                if (tran != null)
                {
                    float x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
                    float y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
                    float z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
                    tran.localScale = new Vector3(x, y, z);
                }
                else
                {
                    isrun = false;
                }
                startT += 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if (m == TweenMore.lineOne)
        {
            bool isrun = true;
            float fixT = 0.02f;
            while (isrun)
            {
                if (startT > time)
                {
                    fixT = -0.02f;
                }
                if (startT < 0)
                {
                    tran.localScale = form;
                    isrun = false;
                }
                if (tran != null)
                {
                    float x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
                    float y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
                    float z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
                    tran.localScale = new Vector3(x, y, z);
                }
                else
                {
                    isrun = false;
                }
                startT =startT +fixT;
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if (m == TweenMore.lineloop)
        {
            bool isrun = true;
            float fixT = 0.02f;
            while (isrun)
            {
                if (startT > time)
                {
                    fixT = -0.02f;
                }
                if (startT < 0)
                {
                    fixT = 0.02f;
                }
                if (tran != null)
                {
                    float x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
                    float y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
                    float z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
                    tran.localScale = new Vector3(x, y, z);
                }
                else
                {
                    isrun = false;
                }
                startT = startT + fixT;
                yield return new WaitForSeconds(0.02f);
            }
        }
        if (Finish != null)
        {
            Finish(gameObject);
        }
    }

    public void RePlay()
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        isPlay = true;
        tran.localScale = form;
        if (startIe != null)
        {
            StopCoroutine(startIe);
            startIe = null;
        }
        startIe = StartTween(startTime);

        StartCoroutine(StartTween(startTime));
    }

    private void OnEnable()
    {
        if (ActionRePlay == true)
        {
            RePlay();
        }
  
    }
    private void OnDisable()
    {
        //if (ScaleMoveIe != null)
        //{
        //    StopCoroutine(ScaleMoveIe);
        //}
        //if (startIe != null)
        //{
        //    StartCoroutine(startIe);
        //}
    }

    private void FixedUpdate()
    {

    }

}

/// <summary>
/// tween类型
/// </summary>
public enum TweenMore : byte
{
    /// <summary>
    /// 运行一次
    /// </summary>
    one = 0,

    /// <summary>
    /// 循环
    /// </summary>
    loop = 1,

    /// <summary>
    /// 反向重复
    /// </summary>
    lineloop = 2,

    /// <summary>
    /// 一次反向重复
    /// </summary>
    lineOne = 3,

}
