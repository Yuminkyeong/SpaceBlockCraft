using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NoticeMessage : MessageWindow<NoticeMessage>
{

    public Text m_Message;

    void CheckHeight()
    {
        if (CWMobManager.Instance == null) return;
        if (CWMobManager.Instance.GetCount() > 0)
        {
            Vector3 vPos = m_visible[0].transform.localPosition;
            vPos.y = 250;
            m_visible[0].transform.localPosition = vPos;
        }
        else
        {
            Vector3 vPos = m_visible[0].transform.localPosition;
            vPos.y = 0;
            m_visible[0].transform.localPosition = vPos;
        }

    }
    public void Show( string szMessage,float ftime)
    {
        if (m_bShow) return;
        // 전투중
        CheckHeight();
        m_Message.text = szMessage;
        m_Message.transform.localScale = new Vector3(1, 0, 1);
        m_Message.transform.DOScaleY(1, 0.4f);
        Invoke("Close", ftime);

        

        base.Open();
    }
    public void Show(string szMessage)
    {
        if (m_bShow) return;
        CheckHeight();
        m_Message.text = szMessage;
        m_Message.transform.localScale = new Vector3(1, 0, 1);
        m_Message.transform.DOScaleY(1, 0.4f);
        Invoke("Close", 3f);
        base.Open();
    }
    public void Show(string szMessage,CBClose cbclose )
    {
        if (m_bShow) return;
        CheckHeight();
        CloseFuction = cbclose;
        m_Message.text = szMessage;
        m_Message.transform.localScale = new Vector3(1, 0, 1);
        m_Message.transform.DOScaleY(1, 0.4f);
        Invoke("Close", 3f);
        base.Open();
    }

    public void Show(float h, string szMessage, float ftime = 3f)
    {
        RectTransform rt =m_visible[0].GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0,h);
        Show(szMessage, ftime);
    }

    public override void Close()
    {
        RectTransform rt = m_visible[0].GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2();
        base.Close();
    }

}
