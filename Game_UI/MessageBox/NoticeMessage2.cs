using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NoticeMessage2 : MessageWindow<NoticeMessage2>
{
    
    public Text m_Message;
    public Text m_Message2;
    public void Show(string szMessage, string szMessage2)
    {
        if (m_bShow) return;

        m_Message.text = szMessage;
        m_Message2.text = szMessage2;
        Invoke("Close", 3f);
        base.Open();
    }
  
}
