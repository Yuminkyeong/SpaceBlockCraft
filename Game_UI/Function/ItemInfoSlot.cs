using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;
using CWUnityLib;
using System;

public class ItemInfoSlot : MonoBehaviour
{

    public int m_nItem;
    public int m_nCount;
    // 주요 능력치 타이틀
    string GetPowerTitle(int nItem)
    {
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        if(gData.type== "shipblock")
        {
            return "HP";
        }
        if (gData.type == "weapon")
        {
            return "공격";
        }
        if (gData.type == "보석")
        {
            return "";
        }
        if (gData.type == "color")
        {
            return "";
        }
        if (gData.type == "Buster")
        {
            return "속도";
        }
        if (gData.type == "폭탄")
        {
            return "범위";
        }

        return "";
    }
    int GetPower(int nItem)
    {
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        if (gData.type == "shipblock")
        {
            return gData.hp;
        }
        if (gData.type == "weapon")
        {
            WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nItem);
            return ws.Damage;
        }
        if (gData.type == "보석")
        {
            return 0;
        }
        if (gData.type == "color")
        {
            return 0;
        }
        if (gData.type == "Buster")
        {
            //WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nItem);
            //return ws.Speed;
        }
        if (gData.type == "폭탄")
        {
            //WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nItem);
            //return ws.Range;

        }

        return 0;

    }
    void Empty()
    {
        RawImage kImage = gameObject.GetComponentInChildren<RawImage>();
        if(kImage==null)
        {
            return;
        }
        kImage.texture = CWResourceManager.Instance.GetTexture("Empty");

        Text[] kTexts = gameObject.GetComponentsInChildren<Text>();


        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nItem);

        foreach (var v in kTexts)
        {
            v.text = "";
        }

    }
    public void UpdateData()
    {
        if (m_nItem==0)
        {
            Empty();
            return;
        }
        RawImage kImage = gameObject.GetComponentInChildren<RawImage>();
        kImage.texture = CWResourceManager.Instance.GetItemIcon(m_nItem);

        Text [] kTexts = gameObject.GetComponentsInChildren<Text>();


        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nItem);

        foreach(var v in kTexts)
        {
            if(v.name.Equals("Count", StringComparison.CurrentCultureIgnoreCase))
            {
                v.text = m_nCount.ToString();
            }
            if (v.name.Equals("Title", StringComparison.CurrentCultureIgnoreCase))
            {
                v.text = gData.m_szTitle;
            }
            if (v.name.Equals("Grade", StringComparison.CurrentCultureIgnoreCase))
            {
                v.text = CWGlobal.GetGradeItemName(gData.level);
            }
            if (v.name.Equals("HP", StringComparison.CurrentCultureIgnoreCase))
            {
                v.text = gData.hp.ToString();
            }
            if (v.name.Equals("Price", StringComparison.CurrentCultureIgnoreCase))
            {
                v.text = gData.price.ToString();
            }
            if (v.name.Equals("Damage", StringComparison.CurrentCultureIgnoreCase))
            {
                WEAPON ws=CWArrayManager.Instance.GetWeapon(m_nItem);
                v.text = ws.Damage.ToString();
            }
            //if (v.name.Equals("Speed", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nItem);
            //    v.text = ws.Speed.ToString();

            //}
            if (v.name.Equals("Range", StringComparison.CurrentCultureIgnoreCase))
            {
                //WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nItem);
                //v.text = ws.Range.ToString();

            }



            // 주요 타이틀
            if (v.name.Equals("Powertitle", StringComparison.CurrentCultureIgnoreCase))
            {
                string szTitle = GetPowerTitle(m_nItem);
                v.text = szTitle;

            }
            // 파워 
            if (v.name.Equals("Power", StringComparison.CurrentCultureIgnoreCase))
            {
                int nVal = GetPower(m_nItem);
                if(nVal==0)
                {
                    v.text = "";
                }
                else
                {
                    v.text = nVal.ToString();
                }

            }


        }


    }
    private void OnEnable()
    {
        if (CWResourceManager.Instance==null) return;
        UpdateData();
    }
}
