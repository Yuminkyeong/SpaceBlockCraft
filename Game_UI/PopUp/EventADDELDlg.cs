using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CWEnum;

public class EventADDELDlg : WindowUI<EventADDELDlg>
{
    public Text m_kPrice;
    public Text m_kTimerCount;
    Action _CBclose_;
    public void Show(Action _cbclose)
    {
        _CBclose_ = _cbclose;
        Open();
    }
    public override void Close()
    {
        base.Close();
        if (_CBclose_ != null)
        {
            _CBclose_();
            _CBclose_ = null;
        }
    }
    public override void Open()
    {

        TimerText kt= m_kTimerCount.gameObject.AddComponent<TimerText>();

        kt.m_fMaxTime = 15;

        ValueUI.g_nCoinType = 4;
        
        ValueUI.g_szPrice = CWArrayManager.Instance.GetPrice("tier1");
        m_kPrice.text = ValueUI.g_szPrice;

        Invoke("Close", 15f);

        base.Open();
    }
    public void OnBuy()
    {
        
        Close();
    }


}
