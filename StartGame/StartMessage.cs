using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartMessage : LoginBaseWin<StartMessage>
{

    public Text m_Message;
    Action m_OnOk;
    Action m_OnCancel;
    bool m_bOk = false;
    
    public void Show(string szMessage)
    {
        m_bOk = false;
        m_Message.text = szMessage;
        m_OnOk = null;
        m_OnCancel = null;
        base.Open();
    }

    public void Click_OK()
    {
        if (m_OnOk != null)
        {
            m_OnOk();
        }
        Close();
    }
    public override void Close()
    {
        base.Close();
    }

}
