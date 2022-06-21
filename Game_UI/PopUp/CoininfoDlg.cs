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

public class CoininfoDlg : WindowUI<CoininfoDlg>
{

    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }
    public override void SetShow(bool bflag)
    {
        base.SetShow(bflag);
    }
    public void OnGold()
    {
        
        if (GamePlay.Instance.IsShow()) return;
        ///NewShopDlg.Instance.Show(4);
        StoreDlg.Instance.Show(2);
    }
    public void OnGem()
    {
      
        if (GamePlay.Instance.IsShow()) return;
        StoreDlg.Instance.Show(3);
    }
    public void OnTicket()
    {

        DrinkBuyDlg.Instance.Open();
    }

    public GameObject[] m_gCoin;
    int[] m_nCoin = new int[4];

    bool m_bOnce = false;
    private void ResetAction()
    {
        for (int i = 0; i < 3; i++)
        {
            m_gCoin[i].transform.DOScale(1, 0.3f);
        }
    }
    public void UpdateReAction()
    {
        if(m_bOnce)
        {
            int[] nCoin = new int[4];
            for (int i = 0; i < 3; i++)
            {
                nCoin[i] = CWCoinManager.Instance.GetCoin((COIN)i);
                if (m_nCoin[i] != nCoin[i])
                {
                    m_gCoin[i].transform.DOScale(2, 0.2f).OnComplete(() =>
                    {

                        ResetAction();
                    });

                }
                m_nCoin[i] = nCoin[i];
            }

        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                m_nCoin[i] = CWCoinManager.Instance.GetCoin((COIN)i);
            }

        }

        m_bOnce = true;

    }



}
