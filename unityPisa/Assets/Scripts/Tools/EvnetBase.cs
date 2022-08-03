using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// ui事件
/// </summary>
public class EvnetBase : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IInitializePotentialDragHandler, IDropHandler
{
    public Action<GameObject> CallFunStart = null;
    private Action<PointerEventData> CallFun = null;
    public Action<PointerEventData> CallFunDrag = null;
    public bool _IsFollowDrag = false;
    public Action<GameObject> CallPressFun = null;
    public Action<PointerEventData> RaiseTheCallback = null;

    public Action<PointerEventData> CallStartPressFun = null;
    public double dateD = 0.02f;
    public bool IsPressMove;
    private Vector3 ori = Vector3.zero;

    public Action<GameObject> ExitFun = null;

    public void Initi(bool isFollowDrag, Action<PointerEventData> callback)
    {

        _IsFollowDrag = isFollowDrag;
        CallFun = callback;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("点击：：：：开始拖动：" + Input.touchCount);

        if (CallFunStart != null)
        {
            CallFunStart(gameObject);
        }


        //throw new System.NotImplementedException();
    }

    private float oriY = 0;

    private float oriX = 0;


    public bool IsDragX = true;
    public float _XRightOffset = 0;
    public void OnDrag(PointerEventData eventData)
    {
        if (_IsFollowDrag == true)
        {
            float maxY = Tools.Instance.ModuleY;
            Vector3 mousePos = Input.mousePosition;
#if UNITY_IOS || UNITY_ANDROID
            TouchData data = Tools.Instance.TouchInfo.Find(s => s.TouchObj == gameObject);
            if (data != null)
            {

                //data.TouchPos = Input.touches[data.TouchIndex].position;
                Vector2 tp = Input.touches[data.TouchIndex].position;
                mousePos = new Vector3(tp.x, tp.y, 0);

                //Debug.Log("拖动有物体抬起后，touch id：" + TouchInedx);
            }
#endif



            Vector3 pos = new Vector3(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2, 0) - ori;
            pos = Tools.Instance.AdaptPos(pos);
            RectTransform tran = gameObject.GetComponent<RectTransform>();

            if ((tran.localPosition.y) < (-(maxY / 2 - tran.sizeDelta.y / 2)))
            {
                //Debug.Log("drag:" + oriY + ":::" + pos.y+"sssss");
                oriY = (-maxY / 2 + tran.sizeDelta.y / 2) - pos.y;
            }
            else if ((tran.localPosition.y) > (maxY / 2 - tran.sizeDelta.y / 2))
            {
                //Debug.Log("drag:" + oriY + ":::" + pos.y);
                oriY = (maxY / 2 - tran.sizeDelta.y / 2) - pos.y;
            }

            if (IsDragX == false)
            {
                float maxX = Tools.Instance.ModuleX;

                if ((tran.localPosition.x) < (-(maxX / 2 - tran.sizeDelta.x / 2)))
                {
                    Debug.Log("drag:" + oriY + ":::" + pos.y + "sssss");
                    oriX = (-maxX / 2 + tran.sizeDelta.x / 2) - pos.x;
                }
                else if ((tran.localPosition.x) > (maxX / 2 - tran.sizeDelta.x / 2 + _XRightOffset))
                {
                    Debug.Log("drag:" + oriY + ":::" + pos.y);
                    oriX = (maxX / 2 - tran.sizeDelta.x / 2) - pos.x + _XRightOffset;
                }

            }

            tran.localPosition = new Vector3(pos.x + oriX, pos.y + oriY, pos.z);


        }
        if (CallFunDrag != null)
        {
            CallFunDrag(eventData);
        }

        //throw new System.NotImplementedException();
    }

    private DateTime dateDragt = DateTime.Now;
    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("点击：：：：拖动结束：" + Input.touchCount);
        //Debug.Log(",.,.,");
        oriY = 0;

        oriX = 0;

        if (CallFun != null)
        {

            if ((DateTime.Now - dateDragt).TotalSeconds > dateD)
            {
                // Debug.Log(",.,.,");
                dateDragt = DateTime.Now;
                CallFun(eventData);
            }

        }
        //throw new NotImplementedException();
    }
    Vector3 Dowmpos = Vector3.zero;

    private int TouchMax = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        //gameObject.GetComponent<RectTransform>().localPosition;
        //Debug.Log("按下》》》》" + Dowmpos);
        if (CallStartPressFun != null)
        {
            //Debug.Log("按下》》》》");

            datet = DateTime.Now;
            CallStartPressFun(eventData);

        }
        Dowmpos = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            TouchData data = null;
            data = Tools.Instance.TouchInfo.Find(s => s.TouchObj == gameObject);
            if (data == null)
            {

                data = new TouchData();
                data.TouchIndex = Input.touchCount - 1;
                data.TouchObj = gameObject;
                data.TouchPos = Input.touches[Input.touchCount - 1].position;
                TouchMax++;
                Tools.Instance.TouchInfo.Add(data);
            }
            else
            {


                for (int i = 0; i < Tools.Instance.TouchInfo.Count; i++)
                {
                    if (Tools.Instance.TouchInfo[i].IsUp == false && Tools.Instance.TouchInfo[i].TouchIndex >= Input.touches.Length - 1)
                    {
                        Tools.Instance.TouchInfo[i].TouchIndex++;
                    }
                }
                data.IsUp = false;
                data.TouchIndex = Input.touches.Length - 1;
            }
            PosYes();
            Dowmpos = new Vector3(data.TouchPos.x, data.TouchPos.y, 0);
            //Debug.Log("按下》》》》》》" + gameObject.name);
        }

        Dowmpos = Tools.Instance.AdaptPos(Dowmpos);

        if (_IsFollowDrag == true)
        {
            //CallFun(eventData);

            Vector3 mousePos = Input.mousePosition;

#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Vector2 tp = Input.touches[Input.touchCount - 1].position;
                mousePos = new Vector3(tp.x, tp.y, 0);
            }
#endif


            //mousePos= Tools.Instance.AdaptPos(mousePos);
            ori = new Vector3(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2, 0);
            //ori = Tools.Instance.AdaptPos(ori);
            ori = ori - Tools.Instance.AdaptPos(gameObject.GetComponent<RectTransform>().localPosition, false);

        }

        //Debug.Log("点击：：：：OnPointerDown：" + Input.touchCount);

        //throw new NotImplementedException();
    }
    private DateTime datet = DateTime.Now;
    public void OnPointerUp(PointerEventData eventData)
    {
        if (RaiseTheCallback != null)
        {
            RaiseTheCallback(eventData);

        }
        //Debug.Log("点击：：抬起：：OnPointerDown：" + Input.touchCount);
        //Debug.Log("抬起》》》》" + Dowmpos);

        //string str = "";
        Vector3 pos = Input.mousePosition;
        if (Input.touches.Length > 0)
        {
            //Debug.Log("点击：：抬起：："+ TouchInedx);

            //Input.touches[0].

            TouchData red = Tools.Instance.TouchInfo.Find(s => s.TouchObj == gameObject);
            if (red != null)
            {
                //Debug.Log("点击：：抬起：gameObj：" + red.TouchObj.name+"remove");
                int RemoveIndex = red.TouchIndex;
                red.IsUp = true;
                //Tools.Instance.TouchInfo.Remove(red);
                TouchData data = Tools.Instance.TouchInfo.Find(s => s.TouchObj == gameObject);
                if (data != null)
                {
                    pos = new Vector3(Input.touches[data.TouchIndex].position.x, Input.touches[data.TouchIndex].position.y, 0);
                }
                for (int i = 0; i < Tools.Instance.TouchInfo.Count; i++)
                {
                    if (Tools.Instance.TouchInfo[i].TouchIndex > RemoveIndex)
                    {
                        Tools.Instance.TouchInfo[i].TouchIndex = Tools.Instance.TouchInfo[i].TouchIndex - 1;

                    }
                    //str = str + "》》物体名字：" + Tools.Instance.TouchInfo[i].TouchObj.name + ";物体index：" + Tools.Instance.TouchInfo[i].TouchIndex;
                }


            }
            PosYes();
            // Debug.Log(str);
        }

        pos = Tools.Instance.AdaptPos(pos);

        if ((pos.x > Dowmpos.x - 5 && pos.x < Dowmpos.x + 5) && (pos.y > Dowmpos.y - 5 && pos.y < Dowmpos.y + 5))
        {
            if (CallPressFun != null)
            {
                //Debug.Log("按下》》》》");
                if ((DateTime.Now - datet).TotalSeconds > dateD)
                {
                    datet = DateTime.Now;
                    CallPressFun(gameObject);

                }

            }


        }


        string strN = "";
        //Debug.Log("抬起MMMMMMM信息:");
        for (int i = 0; i < Tools.Instance.TouchInfo.Count; i++)
        {
            strN = strN + "<<<ssss<<<<id:" + Tools.Instance.TouchInfo[i].TouchIndex + ">>pos:" + Tools.Instance.TouchInfo[i].TouchPos + ">>物体名字：" + Tools.Instance.TouchInfo[i].TouchObj.name + ">>";
        }
        //Debug.Log(strN);
        string strT = "";
        for (int i = 0; i < Input.touches.Length; i++)
        {
            strT = strT + "<< touch信》》ID：" + i + "  >>位置：" + Input.touches[i].position + ">>";
        }
        //Debug.Log(strT);
        //Debug.Log("按下》》》》");

        //throw new NotImplementedException();
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("点击：：：：OnPointerEnter：" + Input.touchCount);
        //throw new NotImplementedException();

    }

    private void PosYes()
    {
        //for (int i = 0; i < Input.touches.Length; i++)
        //{
        //    Vector2 mousePos = Input.touches[i].position;
        //    Vector3 YesMousePos = new Vector3(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2, 0);
        //    YesMousePos = Tools.Instance.AdaptPos(YesMousePos);
        //    for (int j = 0; j < Tools.Instance.TouchInfo.Count; j++)
        //    {
        //        GameObject g = Tools.Instance.TouchInfo[j].TouchObj;
        //        RectTransform rt = g.GetComponent<RectTransform>();
        //        if(YesMousePos.x>( rt.localPosition.x- rt.sizeDelta.x/2) && YesMousePos.x < (rt.localPosition.x + rt.sizeDelta.x / 2) &&
        //            YesMousePos.y > (rt.localPosition.y - rt.sizeDelta.y / 2) && YesMousePos.y < (rt.localPosition.y + rt.sizeDelta.y / 2)
        //            )
        //        {
        //            Tools.Instance.TouchInfo[j].TouchIndex = i;
        //        }
        //    }

        //}
    }

    //public int TouchInedx = 0;

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("点击：：：：OnPointerEnter：" + Input.touchCount);
        if (ExitFun != null)
        {
            ExitFun(gameObject);
        }
        //throw new NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("点击：：：：OnPointerClick：" + Input.touchCount);

        //throw new NotImplementedException();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        // Debug.Log("点击：：：：OnInitializePotentialDrag：" + Input.touchCount);
        //throw new NotImplementedException();
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("点击：：：：OnInitializePotentialDrag：" + Input.touchCount);
        //throw new NotImplementedException();
    }
    private int touchc = 0;

    private int ttttt = 0;

    private int TouchInsert = 0;

    private void FixedUpdate()
    {
        if (Input.touches.Length == 0)
        {
            Tools.Instance.TouchInfo.Clear();
            TouchMax = 0;
        }
        else
        {
            string strN = "";
            //Debug.Log("抬起MMMMMMM信息:");
            for (int i = 0; i < Tools.Instance.TouchInfo.Count; i++)
            {
                strN = strN + "<<<id:" + Tools.Instance.TouchInfo[i].TouchIndex;
            }
            string strT = "";
            for (int i = 0; i < Input.touches.Length; i++)
            {
                strT = strT + "<<ID：" + i + "  >>位置：" + Input.touches[i].position;// +">手指id："+ Input.touches[i].fingerId;

            }
            if (ttttt != Input.touches.Length)
            {
                Debug.Log(strT);
                Debug.Log(strN);
                ttttt = Input.touches.Length;
            }
        }

    }

}

public class TouchData
{
    public int TouchIndex = 0;
    public GameObject TouchObj;
    public Vector2 TouchPos = Vector2.zero;

    public bool IsUp = true;
}