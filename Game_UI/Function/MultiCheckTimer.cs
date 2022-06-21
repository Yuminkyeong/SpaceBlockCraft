
using System;
using System.Collections;
using System.Runtime.InteropServices;

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
using CWEnum;
public class MultiCheckTimer : CWSingleton<MultiCheckTimer>
{

    public Text m_kTimer;
    DateTime m_Startime;

    bool m_bflag = false;
    public  void Begin()
    {
        if (CWSocketManager.Instance == null) return;
        CWSocketManager.Instance.SendMultiTimer(0, (jData) => {

            if(jData["Result"].ToString()=="ok")
            {
                CWHeroManager.Instance.MultiCount = CWLib.ConvertIntbyJson(jData["MultiCount"]);
                m_Startime = jData["MultiStartdate"].Value<DateTime>();
                m_bflag = true;
                
            }
        });

        
    }

    public int m_nRemaintime;
    void _Reset()
    {
        CWSocketManager.Instance.SendMultiTimer(2, (jData) => {

            if (jData["Result"].ToString() == "ok")
            {
                CWHeroManager.Instance.MultiCount = CWLib.ConvertIntbyJson(jData["MultiCount"]);
                m_Startime = jData["MultiStartdate"].Value<DateTime>();

            }
        });

    }
    public void OnReset()
    {


        CoinMessageDlg.Instance.Show(COIN.GEM, CWGlobal.MULTIENTERPRICE, "멀티행성 입장 횟수를 초기화 시키겠습니까?", () => {

            _Reset();
        });

    }
    private void OnEnable()
    {
        if(CWSocketManager.Instance== null)return;
        if (CWSocketManager.Instance == null) return;

        StartCoroutine("IRun");

    }
    private void Start()
    {

        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {

        while(true)
        {
            yield return null;
            if (CWSocketManager.Instance == null) continue;
            if (CWHero.Instance == null) continue;
            if (CWHero.Instance.m_nID == 0) continue;
            break;
        }

        m_bflag = false;
        CWSocketManager.Instance.SendMultiTimer(0, (jData) => {

            if (jData["Result"].ToString() == "ok")
            {
                CWHeroManager.Instance.MultiCount = CWLib.ConvertIntbyJson(jData["MultiCount"]);
                m_Startime = jData["MultiStartdate"].Value<DateTime>();
                m_bflag = true;
            }
        });

        while (!m_bflag)
        {
            yield return null;
        }


        int Maxtimer = CWGlobal.MULTI_RESETTIMER / 1000;
        while (true)
        {
            TimeSpan ts = DateTime.Now - m_Startime;
            int nSec= Maxtimer - (int)ts.TotalSeconds;
            if(nSec<=0)
            {
                // 리셋개념
                _Reset();
            }
            int nVal1 = nSec / 3600;
            int nVal2 = (nSec / 60) % 60;
            int nVal3 = nSec % 60;

            m_nRemaintime = nSec/60;
      //      m_kTimer.text = string.Format("{0:00}:{1:00}:{2:00}",nVal1,nVal2,nVal3);
            yield return null;
        }
    }
    
}
