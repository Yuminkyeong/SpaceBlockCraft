using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
// 광고로 살 수 있는 제품 관련

public class AskBuydlg : WindowUI<AskBuydlg>
{

    public Text m_ktitle;
    public Text m_kMessage;

    Action m_Func;

    public void Show(string szTitle, string szMessage,Action fuc)
    {
        m_ktitle.text = szTitle;
        m_kMessage.text = szMessage;
        m_Func = fuc;
        Open();
    }
    protected override void _Open()
    {
        CWGlobal.g_GameStop = true;
        base._Open();
    }
    protected override void _Close()
    {
        CWGlobal.g_GameStop = false;
        base._Close();
    }
    public void OnADBuy()
    {
        if (!CWSocketManager.Instance.UseCoinEx(COIN.TICKET, -1, () => {

            m_Func();

        }))
        {
          //  CWADManager.Instance.RewardShow(m_Func);
        }
        Close();
    }
    public void OnGemBuy()
    {
        if(!CWSocketManager.Instance.UseCoinEx(COIN.GEM, -5, m_Func))
        {
            //CWADManager.Instance.RewardShow(m_Func);
        }
        Close();
    }


}
