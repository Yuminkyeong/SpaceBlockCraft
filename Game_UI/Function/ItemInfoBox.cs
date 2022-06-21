using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
using CWStruct;
using CWEnum;
using System;

public class ItemInfoBox : MonoBehaviour
{

    public GITEM m_nItemID;

    private void OnEnable()
    {

        StartCoroutine("IRun");
    }

    void UpdateData()
    {
        RawImage rr = GetComponentInChildren<RawImage>();
        rr.texture = CWResourceManager.Instance.GetItemIcon((int)m_nItemID);

        Text[] tt = GetComponentsInChildren<Text>();

        foreach (var v in tt)
        {
            if (v.name.Equals("Count", StringComparison.CurrentCultureIgnoreCase))// 
            {
                v.text = CWInvenManager.Instance.GetItemTotalCount((int)m_nItemID).ToString();
            }
            if (v.name.Equals("Title", StringComparison.CurrentCultureIgnoreCase))// 
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData((int)m_nItemID);
                v.text = gData.m_szTitle;
            }

        }



    }

    IEnumerator IRun()
    {
        while(true)
        {
            UpdateData();
            yield return new WaitForSeconds(1f);
        }
    }

}
