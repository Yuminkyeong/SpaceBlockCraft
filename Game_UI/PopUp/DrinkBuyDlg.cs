using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWStruct;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class DrinkBuyDlg : WindowUI<DrinkBuyDlg>
{
    public Text m_kPrice;
    public Text m_kCount;
    

    int m_nCount = 1;

    protected override void _Open()
    {
        m_nCount = 1;
        m_kCount.text = m_nCount.ToString();
        m_kPrice.text = "";
        base._Open();
    }

    public override void UpdateData(bool bselect = true)
    {
        m_kPrice.text = string.Format("{0}", CWGlobal.DRINK_BY_GEM * m_nCount);
        m_kCount.text = m_nCount.ToString();
        base.UpdateData(bselect);
    }
    public void OnBuy()
    {
        int gem = (int)(CWGlobal.DRINK_BY_GEM * m_nCount);
        int drink = m_nCount;

        CWSocketManager.Instance.UseCoin2(COIN.GEM, -gem, COIN.TICKET,drink, CWSocketManager.UseCoin_ResultFuc, "UseCoin_ResultFuc");

        //  m_kImage.transform.DOShakeRotation(0.2,10)
   //     DOTweenAnimation dt = gameObject.GetComponentInChildren<DOTweenAnimation>();
        //dt.DOPlay();
        //dt.DORewind();
        

       Close();
    }
    public void OnLeft()
    {
        m_nCount--;
        if (m_nCount <= 0) m_nCount = 0;
        
        UpdateData();
    }

    // 광고 제거를 하면, 티켓은 무한으로 공급?
    public void OnAD()
    {
        //int nTNowCount = CWHeroManager.Instance.GetStoreTimerCount("Drink");
        //DateTime tDate = CWHeroManager.Instance.GetStoreTimer("Drink");
        //TimeSpan ts = DateTime.Now - tDate;
        //if (ts.TotalSeconds > 0.5 && ts.TotalSeconds < 24 * 3600)
        //{

        //}

         CWADManager.Instance.RewardShow(()=> {

           CWSocketManager.Instance.UseCoinEx(COIN.TICKET, 3);

             NoticeMessage.Instance.Show("티켓을 획득하였습니다!");

             AnalyticsLog.Print("ADLog", "DrinkAD");
             Close();
        
        });
    }

    public void OnRight()
    {
        m_nCount++;
        
        UpdateData();
    }

}
