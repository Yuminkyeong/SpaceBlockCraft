
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

public class ShipRepairDlg : WindowUI<ShipRepairDlg>
{
    public Text m_kRepair;
    public Text m_kPrice;
    public Text m_kTimer;
    int m_nID = 0;
    int m_nPrice;

    int m_nRepair;
    ShipSlot m_Slot = null;

    public void Show(int nID,int HP,int nRepair,int Sec, ShipSlot slot)
    {
        if(nRepair==100)
        {
            NoticeMessage.Instance.Show("내구력이 완벽합니다!");
            return;
        }
        m_Slot = slot;
        m_nRepair = nRepair;
        // 가격 결정
        int values = HP*CWGlobal.REPARIPRICE;
        m_nPrice = (values * (100-nRepair));
        m_nID = nID;

        m_kPrice.text = m_nPrice.ToString();
        m_kRepair.text = nRepair.ToString()+"%";


        m_kTimer.text = CWLib.GetTimeString(Sec);

        Open();
    }
    protected override void _Open()
    {
        
        m_nGroupType = 1;
        base._Open();
    }

    public override void Close()
    {
        base.Close();
    }
    
    public void OnRepair()
    {

        CWSocketManager.Instance.UseCoinEx(COIN.GOLD, -m_nPrice, () =>
        {
            
            CWSocketManager.Instance.UpdateRepairAir_Begin(m_nID);
            
            NoticeMessage.Instance.Show("내구력 복구를 시작합니다!", Close);
            //ShipSlotDlg.Instance.UpdateData();
            m_Slot.UpdateSlotData();

        });

        Close();

    }
    public void OnRepairItem()
    {
        if(CWInvenManager.Instance.GetItemTotalCount((int)GITEM.Repair)==0)
        {
            NoticeMessage.Instance.Show("수리 아이템이 없습니다!");
            return;
        }
        CWInvenManager.Instance.DelItem((int)GITEM.Repair, 1);
        m_Slot.OnReapiarEndTask();
        Close();

    }

}
