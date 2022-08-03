using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OpenAnim : MonoBehaviour
{
    public Slider value = null;
    // Use this for initialization
    void Start()
    {
        timeSpeed+= Random.Range(0, 1.0f);
        value.value = 0;
    }
    float t = 0;
    float timeSpeed = 0;
    // Update is called once per frame
    void Update()
    {
        value.value += (timeSpeed + Random.Range(0, 0.5f)) * Time.deltaTime;
        if (value.value >= 1)
        {
            showUi();
        }
    }
    void showUi()
    {
        GameObject.Destroy(ViewManage.Instance.open_anim);
        GameView.ShowView();
    }
}
