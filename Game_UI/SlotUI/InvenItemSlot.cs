using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

public class InvenItemSlot : SlotItemUI
{
    public GameObject m_gCheck;
    public override bool UpdateData()
    {
        if(m_gCheck!=null)
        {
            int n = m_gList.GetInt(m_nNumber, "ItemID");// 대소문자 무시 
            if (n == 0)
            {
                m_gCheck.SetActive(false);
                //string szval2 = m_gList.GetString(m_nNumber, "Check");//
            }
            else
            {
                m_gCheck.SetActive(true);
            }

        }

        return base.UpdateData();
    }
}
