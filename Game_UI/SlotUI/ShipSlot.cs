using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;
using CWEnum;
public class ShipSlot : SlotItemUI
{

    
    public RawImage m_kIcon;
    public GameObject m_gRepair;
    public GameObject m_gQuest;// 느낌표
    public Text m_kTimer;

    public GameObject[] m_Bttns;
    public Text m_kCount;
         

    bool TimerUpdate()
    {
        int nID = m_gList.GetInt(m_nNumber, "_id");
        int Repair = m_gList.GetInt(m_nNumber, "Repair");
        DateTime dt = m_gList.GetDateTime(m_nNumber, "Repairdate");
        if (CWHeroManager.Instance.CheckRepair(Repair, dt))
        {


            TimeSpan ss = DateTime.Now - dt;
            int RepairTime = (100 - Repair) * CWGlobal.REPAIRTIME;

            int rest =  RepairTime - (int)ss.TotalSeconds;
            m_kTimer.text = CWLib.GetTimeString((float)rest); 

            if (ss.TotalSeconds >= RepairTime)
            {


                m_kTimer.text = "";
                m_Bttns[1].SetActive(true);
                m_Bttns[0].SetActive(false);
                return true;
            }



        }

        return false;

    }
    public void OnReapiarEndTask()
    {
        int nID = m_gList.GetInt(m_nNumber, "_id");
        CWSocketManager.Instance.UpdateRepairAir(nID, 100);// 수리 완료
        NoticeMessage.Instance.Show("수리가 완료되었습니다");
        m_gRepair.SetActive(false);
        m_gQuest.SetActive(false);
        m_gList.UpdateData();
        if (nID == CWHeroManager.Instance.m_nAirSlotID)
        {
            CWHeroManager.Instance.ClearRepair();// 수리 완료
            return;
        }

        
    }
    // 즉시 
    public void OnRepairFast()
    {

        MessageBoxDlg.Instance.Show(() => {

            CWSocketManager.Instance.UseCoinEx(COIN.GEM, -CWGlobal.REPAIR_PRICE, () => {

                OnReapiarEndTask();

            });

        }, null, "", "즉시 복구를 하시겠습니까?");

    }
    public void UpdateSlotData()
    {

        m_gList.SetString(m_nNumber, "Repairdate", DateTime.Now.ToString());
        int nID = m_gList.GetInt(m_nNumber, "_id");
        if(CWHeroManager.Instance.m_nAirSlotID==nID)
        {
            CWHeroManager.Instance.Repairtime = DateTime.Now;
        }

        UpdateData();
    }
    public override bool UpdateData()
    {

        foreach(var v in m_Bttns)
        {
            v.SetActive(false);
        }

        int nID = m_gList.GetInt(m_nNumber, "_id");
        m_kIcon.texture = CWLib.LoadImage(string.Format("Ship_{0}",nID), CWGlobal.ICONIMAGESIZE, CWGlobal.ICONIMAGESIZE);
        if(m_kIcon.texture==null)
        {
            m_kIcon.texture = CWResourceManager.Instance.GetTexture("chobo_1");
        }

        // tcnt = GameEdit.Instance.m_kAirObject.NBlockCount;
        // string.Format("{0}/{1}", tcnt, GameEdit.Instance.m_nAirBlockCount);//

        int maxcount = m_gList.GetSelectValueInt("MaxCount");
        int count = m_gList.GetSelectValueInt("Count");

        m_kCount.text = string.Format("{0}/{1}",count,maxcount);

        int Repair = m_gList.GetInt(m_nNumber, "Repair");
        DateTime dt = m_gList.GetDateTime(m_nNumber, "Repairdate");
        // 수리 중!
        if (dt!=DateTime.MinValue)// 수리 
        {
            // 수리가 남아있다
            
            
            TimeSpan ss = DateTime.Now - dt;

            m_kTimer.text = CWLib.GetTimeString((float)ss.TotalSeconds);
            int RepairTime = (100 - Repair) * CWGlobal.REPAIRTIME;
            if (ss.TotalSeconds >= RepairTime)
            {
                if (nID == CWHeroManager.Instance.m_nAirSlotID)
                {
                    m_kTimer.text = "";
                    m_Bttns[1].SetActive(true);
                }
            }
            else
            {
                
                StartCoroutine("IRun");
                
            }
            m_Bttns[0].SetActive(true);
            m_gRepair.SetActive(true);
            m_gQuest.SetActive(false);
        }
        else
        {
            m_gRepair.SetActive(false);
            if(Repair==0)
            {
                m_gQuest.SetActive(true);
            }
            m_Bttns[1].SetActive(true);
        }


        return base.UpdateData();
    }
    IEnumerator IRun()
    {
        while(true)
        {
            
            yield return null;
            if(TimerUpdate())
            {
                break;
            }
                
        }
    }
    //수리
    public void OnRepair()
    {
        int ID = m_gList.GetInt(m_nNumber, "_id");
        int HP = m_gList.GetInt(m_nNumber, "HP");
        int Repair = m_gList.GetInt(m_nNumber, "Repair");

        int Sec = (100 - Repair) * CWGlobal.REPAIRTIME;
        ShipRepairDlg.Instance.Show(ID,HP,Repair,Sec,this);
    }
    //강화
    public void OnEnchant()
    {
        int ID = m_gList.GetInt(m_nNumber, "_id");
        int Level = m_gList.GetInt(m_nNumber, "Level");

        SlotUpgradeDlg.Instance.Show(ID, Level);
    }
    public void OnWeaponEnchant()
    {
        ShipSlotDlg.Instance.Close();
        int ID = m_gList.GetInt(m_nNumber, "_id");

        byte[] buffer = m_gList.GetBytes(m_nNumber, "BlockData");
        if(buffer==null)
        {
            TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
            buffer = aa.bytes;
        }
        WeaponUpgradeDlg.Instance.Show(ID, buffer);
        WeaponUpgradeDlg.Instance.CloseFuction = ShipSlotDlg.Instance.Open;


    }

    public override void OnSelect()
    {
        //    int nID = m_gScrollList.GetSelectValueInt("_id");

        int nID = m_gList.GetInt(m_nNumber, "_id");
        if (nID != CWHeroManager.Instance.m_nAirSlotID)
        {
            CWHeroManager.Instance.SelectAirSlot(nID);
            UpdateData();
        }
        base.OnSelect();
    }
    public void OnEdit()
    {
        byte[] bBuffer = null;
        int nID = m_gList.GetInt(m_nNumber, "_id");
        if (nID == CWHeroManager.Instance.m_nAirSlotID)
        {
            bBuffer = CWHero.Instance.GetBuffer();
        }
        else
        {
            bBuffer = m_gList.GetBytes(m_nNumber, "BlockData");
        }
        int maxcount = m_gList.GetSelectValueInt("MaxCount");
        GameEdit.Instance.Show(nID, maxcount, bBuffer);
        GameEdit.Instance.CloseFuction = ShipSlotDlg.Instance.Open;
        ShipSlotDlg.Instance.Close();
    }
}
