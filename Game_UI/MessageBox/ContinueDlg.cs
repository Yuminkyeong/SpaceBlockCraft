using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;


public class ContinueDlg : MessageWindow<ContinueDlg>
{
    CBFunction m_CBFuction2;
    public void Show(CBFunction fFunc, CBFunction cbClose)
    {
        m_CBFuction = fFunc;
        m_CBFuction2 = cbClose;

        Open();
    }
    public void OnOK()
    {
        if(!CWSocketManager.Instance.UseCoinEx(COIN.TICKET, -1, () => {
            m_CBFuction();
            
        }))
        {

            m_CBFuction2();
        }
        Close();
    }
    public void OnCancel()
    {
        m_CBFuction2();
        Close();
    }
}
