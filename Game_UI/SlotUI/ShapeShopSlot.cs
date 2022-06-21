using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

public class ShapeShopSlot : SlotItemUI
{
    public GameObject[] m_gObject;

    public override bool UpdateData()
    {
        foreach (var v in m_gObject) v.SetActive(false);

        bool flag= CWHeroManager.Instance.GetShape(m_nNumber + 1);
        if(flag)
        {
            m_gObject[1].SetActive(true);
        }
        else
        {
            m_gObject[0].SetActive(true);
        }
        return base.UpdateData();
    }
    public void OnSelectShape()
    {
        GameEdit.Instance.OnSelectShape(m_nNumber);
        ShapeShopDlg.Instance.Close();

    }
}
