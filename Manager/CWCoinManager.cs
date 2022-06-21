using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWEnum;

public class CWCoinManager : CWManager<CWCoinManager>
{
    
    byte[] m_Buffer;
    int[] m_nCoins = new int[4];

    public void SetData(JToken _Data)
    {
        if (_Data == null) return;
        if (!_Data.HasValues) return;

        JObject jj = new JObject();
        jj.Add("array", _Data);
        m_Buffer = CWLib.ConvertBin(jj);

        

        m_nCoins[0] = GetCoin(COIN.TICKET);
        m_nCoins[1] = GetCoin(COIN.GOLD);
        m_nCoins[2] = GetCoin(COIN.GEM);
        //        m_nCoins[3] = GetCoin(COIN.ENERGY);
        CoininfoDlg.Instance.UpdateReAction();
        
    }
    public int GetCoinbyFast(COIN ntype)
    {
        return m_nCoins[(int)ntype];
    }
    public int GetCoin(COIN ntype)
    {
        if(m_bONEFlag)
        {
            if (ntype == m_ONEType) return m_nONECoin;
        }
        if (m_Buffer == null) return 0;
        int num = (int)ntype;
        
        JObject jj = (JObject)CWLib.ConvertJSon(m_Buffer);

        

        JArray ja = CWJSon.GetArray(jj, "array");
        if (ja == null) return 0;
        if (ja.Count <= num) return 0;
        string sz= ja[num].Value<string>();
        return CWLib.ConvertInt(sz);
    }

   
   
    public bool CheckCoin(COIN ntype, int val)
    {
        int n=GetCoin(ntype);
        if (n < Mathf.Abs(val)) return false;
        return true;
    }

    #region 한번에 값을 넘긴다

    bool m_bONEFlag = false;
    int m_nONECoin=0;
    COIN m_ONEType;
    public bool IsOneFlag(COIN nType)
    {
        if(m_ONEType==nType)
        {
            return m_bONEFlag;
        }

        return false;
        
    }
    public void BeginONECoin(COIN nType)
    {
        m_nONECoin = GetCoin(nType);
        m_bONEFlag = true;
        m_ONEType = nType;
        
    }
    public bool UseCoinONE(COIN nType, int nCoin, Action cbAciton = null)
    {

        if (nCoin < 0)
        {
            if (m_nONECoin <  -nCoin)
            {
                if (nType == COIN.GOLD)
                {
                    NoticeMessage.Instance.Show("골드가 모자랍니다.");
                }
                if (nType == COIN.GEM)
                {
                    NoticeMessage.Instance.Show("보석이 모자랍니다.");
                }
                if (nType == COIN.ENERGY)
                {
                    NoticeMessage.Instance.Show("오일이 모자랍니다.");
                }
                if (nType == COIN.TICKET)
                {
                    NoticeMessage.Instance.Show("입장권이 모자랍니다");
                }
                return false;
            }
        }
        m_nONECoin += nCoin;
        if(cbAciton!=null) cbAciton();

        return true;
    }
    public void SaveONECoin()
    {
        CWSocketManager.Instance.SetCoin(m_ONEType, m_nONECoin);
        m_bONEFlag = false;
    }


    #endregion

}






