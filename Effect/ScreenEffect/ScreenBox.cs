using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;

public class ScreenBox : CWSingleton<ScreenBox>
{
    // UI 타입 , :  에디터,상점,랭킹,지도 


    UITYPE m_kUIType;
    int m_nPrevNumber = 0;
    int m_nSelectNumber = 0;

    public float m_fLifetime = 1f;
    public Renderer[] m_gRenderer;

    public GameObject m_visible;


    CallBackFunction Callbackfucntion;
    CallBackFunction CallbackStart;



    Vector3[] m_kRotate =
    {
        new Vector3(0,0,180), // main
        new Vector3(0,90,180), // edit
        new Vector3(0,-90,180), // map
        new Vector3(90,0,180), //store
        new Vector3(-90,0,180),//ranking

    };

    Vector3 GetRotate(UITYPE ktype)
    {
        if (ktype == UITYPE.MAIN) return m_kRotate[0];
        if (ktype == UITYPE.EDIT) return m_kRotate[1];
        if (ktype == UITYPE.MAP) return m_kRotate[2];
        if (ktype == UITYPE.STORE) return m_kRotate[3];
        if (ktype == UITYPE.RANKING) return m_kRotate[4];
        return Vector3.zero;
    }
    int GetFace(UITYPE ktype)
    {
        if (ktype == UITYPE.MAIN) return 0;
        if (ktype == UITYPE.EDIT) return 1;
        if (ktype == UITYPE.MAP) return 2;
        if (ktype == UITYPE.STORE) return 3;
        if (ktype == UITYPE.RANKING) return 4;
        return 0;

    }

    private void OnEnable()
    {
        m_visible.SetActive(false);
        StartCoroutine("IRun");
    }

    public void SetScreen(UITYPE kType, Texture2D kTexture,CallBackFunction Func, CallBackFunction Func2)
    {
        m_kUIType = kType;
        CallbackStart = Func;
        Callbackfucntion = Func2;
        int num=GetFace(kType);
        m_nSelectNumber = num;
        //m_gRenderer[num].material.SetTexture("_MainTex", kTexture);
        StartCoroutine("ChangeScreen");
    }
    IEnumerator IRun()
    {

        yield return new WaitForEndOfFrame();
        Canvas cs = GetComponentInParent<Canvas>();
        RectTransform rt = cs.GetComponent<RectTransform>();
        transform.localScale = new Vector3(rt.sizeDelta.x, rt.sizeDelta.y, rt.sizeDelta.x);
        //        m_kCapture = ScreenCapture.CaptureScreenshotAsTexture();
        //      m_kImage.texture = m_kCapture;
        //        Play(0, MType, fTime);
    }


    bool m_bEndflag = false;
    IEnumerator ChangeScreen()
    {
        yield return new WaitForEndOfFrame();
        m_bEndflag = false;
        m_visible.SetActive(true);
        Texture2D _kCapture = ScreenCapture.CaptureScreenshotAsTexture();
        m_gRenderer[m_nPrevNumber].material.SetTexture("_MainTex", _kCapture);
        if(CallbackStart!=null)
            CallbackStart();

        m_visible.transform.DOLocalRotate(GetRotate(m_kUIType), m_fLifetime).OnComplete(()=> {

            m_bEndflag = true;
        });
     
        while(!m_bEndflag)
        {
            yield return null;
        }
        if (Callbackfucntion != null)
            Callbackfucntion();

        yield return new WaitForEndOfFrame();


        //m_visible.transform.localEulerAngles = new Vector3(0, 0, 180);
        m_visible.SetActive(false);
        m_nPrevNumber = m_nSelectNumber;

    }

}
