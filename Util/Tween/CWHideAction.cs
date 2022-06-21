using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CWUnityLib;
/*
 *  HideAction 은
 *  객체가 사라지는 표현을 하고
 *  종료가 되면, 원래 위치로 돌아간다 
 *  
 *  확대가 되고,
 *  위로 올라갔다가, 점점작아지면서 사라진다 
 *  
 * */

public class CWHideAction : MonoBehaviour
{

    

    Vector3 m_vStart;
    Quaternion m_qStart;

    public CallBackFunction m_CBFunc;

    void Close()
    {
        RectTransform rr = GetComponent<RectTransform>();
        transform.localRotation = m_qStart;
        transform.localScale = Vector3.one;
        rr.anchoredPosition = m_vStart;
        gameObject.SetActive(false);
        Destroy(this);
        m_CBFunc();
    }
    void Start()
    {

        RectTransform rr=GetComponent<RectTransform>();

        m_vStart = rr.anchoredPosition;// transform.localPosition;
        m_qStart = rr.localRotation;//  transform.localRotation;

        Vector3 vPos = m_vStart;
        vPos.y += 350;// Game_App.Instance.Test1;// 얼마나올라가나?

        //transform.DOShakeRotation(0.2f).OnComplete(() => {

        //    transform.DOMove(vPos, 1f).OnComplete(() => {

        //    });
        //    transform.DOScale(Game_App.Instance.Test2, 0.2f).OnComplete(() => {

        //        transform.DOScale(0, 0.6f).OnComplete(() => {

        //            Close();
        //        });

        //    });


        //});

        rr.DOAnchorPos(vPos, 0.7f).OnComplete(() => {
            
        });
        transform.DOScale(2, 0.3f).OnComplete(() => {

            transform.DOScale(0.2f, 0.6f).OnComplete(() => {

            });

        });

        Invoke("Close", 1.2f);

    }

}
