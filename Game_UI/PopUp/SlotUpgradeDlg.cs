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

public class SlotUpgradeDlg : WindowUI<SlotUpgradeDlg>
{

    /*public Text m_ktitle;
    public Text m_kMessage;

    Action m_Func;*/

    /*public void Show(string szTitle, string szMessage,Action fuc)
    {
        m_ktitle.text = szTitle;
        m_kMessage.text = szMessage;
        m_Func = fuc;
        Open();
    }*/

    public Text m_kLevel1;
    public Text m_kLevel2;
    public Text m_kCount1;
    public Text m_kCount2;

    public Text m_kPrice;
    int m_nPrice;
    int m_nLevel = 0;
    int m_nID;
    public void Show(int nID, int level)
    {
        m_nID = nID;
        if(level>=6)
        {
            NoticeMessage.Instance.Show("최대 슬롯입니다!");
            return;
        }
        m_nLevel = level;

        m_kLevel1.text = level.ToString();
        m_kLevel2.text = (level+1).ToString();

        int nCount1 = CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "blockcount", level);
        int nCount2 = CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "blockcount", level+1);

        
        int nPrice2 = CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "Price", level + 1);

        m_nPrice = nPrice2;
        m_kPrice.text = nPrice2.ToString();
        m_kCount1.text = nCount1.ToString();
        m_kCount2.text = nCount2.ToString();
        Open();
    }

    void CloseUpdate()
    {

        ShipSlotDlg.Instance.UpdateData();

        Close();
    }
    public void OnBuyItem()
    {
        CWSocketManager.Instance.UseCoinEx(COIN.GOLD, -m_nPrice, () => {

            CWSocketManager.Instance.UpdateAirSlotLevel(m_nID, m_nLevel + 1,()=> {

                NoticeMessage.Instance.Show("슬롯이 업그레이드 되었습니다", CloseUpdate);
            });
            

        });
    }
}
