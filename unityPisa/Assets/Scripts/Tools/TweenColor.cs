using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TweenColor : MonoBehaviour
{

    // Use this for initialization
    public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);

    public float time = 1;

    public Color form = Color.white;
    public Color to = Color.white;

    public float startTime = 0;
    public TweenMore more = TweenMore.one;

    public Action<GameObject> Finish;

    public bool ActionRePlay = true;

    public string Data = "";

    //private RectTransform tran;
    private Image image;
    private Text text;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
        text = gameObject.GetComponent<Text>();
        //tran.localScale = form;
        if (image != null)
        {
            image.color = form;
            
        }
        if (text != null)
        {
            text.color = form;
        }
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

    IEnumerator ScaleMoveIe = null;
    private IEnumerator ScaleMove(TweenMore m)
    {
        float startT = 0;
        if (m == TweenMore.one)
        {
            while (startT < time)
            {
                if (image != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    image.color = new Color(r,g,b,a);
                }


                if (text != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    text.color = new Color(r, g, b, a);
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
                if (image != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    image.color = new Color(r, g, b, a);
                }


                if (text != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    text.color = new Color(r, g, b, a);
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
                    image.color = form;
                    text.color = form;
                    isrun = false;
                }
                if (image != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    image.color = new Color(r, g, b, a);
                }

                if (text != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    text.color = new Color(r, g, b, a);
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
                if (image != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    image.color = new Color(r, g, b, a);
                }
                if (text != null)
                {
                    float r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
                    float g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
                    float b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
                    float a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
                    text.color = new Color(r, g, b, a);
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
        if (image != null)
        {
            image.color = form;
        }
        if (text != null)
        {
            text.color = form;
        }
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


