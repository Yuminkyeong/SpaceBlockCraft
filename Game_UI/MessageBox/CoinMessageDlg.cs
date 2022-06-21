using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;
using CWUnityLib;
public class CoinMessageDlg : WindowUI<CoinMessageDlg>
{
    public Text m_kMessage;
    public Text m_kPrice;
    public Image m_kCoin;
    COIN m_nCoin;
    int m_nPrice;
    public void Show(COIN coin,int Price,string szMessage, CBFunction cfunc)
    {
        string szCoin = "gold";
        if(coin==COIN.GEM) szCoin = "gem";
        m_kCoin.sprite = CWResourceManager.Instance.GetSprite(szCoin);

        m_kPrice.text = Price.ToString();
        m_kMessage.text = szMessage;
        m_CBFuction = cfunc;
        m_nPrice = Price;
        m_nCoin = coin;
        Open();
    }
    public void OnClick()
    {
        CWSocketManager.Instance.UseCoin(m_nCoin, -m_nPrice, (jData) => {

            if (jData["Result"].ToString() == "ok")
            {
                CWCoinManager.Instance.SetData(jData["Coins"]);
                m_CBFuction();
                
            }
            else
            {
                //faile!!
                print("shop fail!!");
            }
            Close();

        }, "CoinMessageDlg");
    }

}
