using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;
using CWUnityLib;

// 작업 요소
// 호출 할때마다 지속적으로 실행하는 개념
// 


public class TextEffectDlg : MessageWindow<TextEffectDlg>
{

    public Text m_kText;
    public UIOverlayPos m_kOverlay;
    public void Show(RectTransform tObject, string sztext)
    {
        GetComponent<RectTransform>().anchoredPosition=tObject.anchoredPosition;
        m_kText.text = sztext;
        Open();
    }
    public void Show(Vector3 vPos, string sztext)
    {
        GetComponent<RectTransform>().anchoredPosition = vPos;
        m_kText.text = sztext;
        Open();
    }
    public void Show3D(GameObject gObject, string sztext)
    {
        m_kOverlay.m_gTarget = gObject;
        m_kText.text = sztext;
        Open();
    }
    public override void Open()
    {
        Transform tt = m_visible[m_nSelectMode].transform;

        tt.DOLocalMoveY(600, 0.6f);
        m_kText.DOFade(1, 0).OnComplete(()=> {
            m_kText.DOFade(0, 0.6f);
        });
        

        base.Open();
    }
}
