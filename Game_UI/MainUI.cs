using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWStruct;
using CWEnum;
using CWUnityLib;
public class MainUI : CWSingleton<MainUI>
{



    //public BaseUI m_gStartOpen;

    ////public GameObject m_gClickCursor;
    ////public GameObject m_gTempUI;
    ////void ClickCursor(Vector3 vPos)
    ////{

    ////    GameObject gg = Instantiate(m_gClickCursor);
    ////    gg.GetComponent<RectTransform>().SetParent(m_gTempUI.transform);
    ////    gg.GetComponent<RectTransform>().anchoredPosition = CWMath.ConvertByMousePos(vPos);

    ////}

    //void Start()
    //{
    //    if(m_gStartOpen)
    //        m_gStartOpen.Open();

    //}

    public BaseUI m_gStartOpen;

    public GameObject m_gClickCursor;
    public GameObject m_gTempUI;
    void ClickCursor(Vector3 vPos)
    {

        GameObject gg = Instantiate(m_gClickCursor);
        gg.GetComponent<RectTransform>().SetParent(m_gTempUI.transform);
        gg.GetComponent<RectTransform>().anchoredPosition = CWMath.ConvertByMousePos(vPos);

    }
    // 마우스 위치로 UI를 위치한다
    public void ShowMousePosUI(GameObject gg)
    {
        gg.GetComponent<RectTransform>().SetParent(m_gTempUI.transform);
        gg.GetComponent<RectTransform>().anchoredPosition = CWMath.ConvertByMousePos(Input.mousePosition);

    }
    void Start()
    {
        if (m_gStartOpen)
            m_gStartOpen.Open();

    }
    private void Update()
    {
        if (m_gClickCursor == null) return;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    ClickCursor(Input.mousePosition);
        //}


    }

}
