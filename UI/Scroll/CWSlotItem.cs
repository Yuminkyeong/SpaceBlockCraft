using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;
using CWEnum;
public class CWSlotItem : DynamicScrollItem
{

    public delegate void CBClickEvent(int number);
    public CBClickEvent CBClickFunction;
    static GameObject g_SelectBox = null;
    public override void SetScreen()
    {
        // 어떤 리스트를 참조하고 
        // 해당 번호에 맞는 것을 가져 온다 
        //UI text에 값을 넣는다 
        // parent를 찾으려면 하나만 존재 해야되는 개념이 존재
        // 그러므도 별도의 스크롤리스트에 있어야 한다
        CWBridgeList kBridge = gameObject.GetComponentInParent<CWBridgeList>();
        if (kBridge)
        {
            if (kBridge.m_Type == CWBridgeList.TYPE.NONE) return;

        }
        if (kBridge == null) return;
        UILabel[] la = gameObject.GetComponentsInChildren<UILabel>(true);
        if (la.Length > 0)
        {
            foreach (var v in la)
            {
                if(kBridge.IsRecord(m_Num, v.name))
                {
                    string sz = kBridge.GetRecord(m_Num, v.name);
                    if (sz != null && sz.Length > 0)
                    {
                        v.text = sz;
                    }
                    else
                    {
                        v.text = "";
                    }

                }
            }

        }
        UITexture[] ta = gameObject.GetComponentsInChildren<UITexture>(true);
        if (ta.Length > 0)
        {
            foreach (var v in ta)
            {
                string szval = kBridge.GetRecord(m_Num, v.name);
                if (v.name == "GAMEICON")
                {
                    int nID = CWLib.ConvertInt(szval);
                    v.mainTexture = CWResourceManager.Instance.GetItemIcon(nID);
                   
                    continue;
                }
                
                if (v.name=="Icon")// 유저 얼굴 아이콘
                {
                //    CWResourceManager.Instance.GetFaceImage(szval, v);
                    continue;
                }
                
                if (szval.Length > 1)
                {
                    v.mainTexture = CWResourceManager.Instance.GetTexture(szval);
                }

            }

        }

        base.SetScreen();
    }
    public virtual void SetSelect()
    {
        if (g_SelectBox != null)
        {
            g_SelectBox.SetActive(false);
        }
        GameObject gg = CWLib.FindChild(gameObject, "SelectBox");
        if (gg)
        {
            gg.SetActive(true);
            g_SelectBox = gg;
        }

    }
    public override void OnClick()
    {
        if (g_SelectBox != null)
        {
            g_SelectBox.SetActive(false);
        }
        GameObject gg = CWLib.FindChild(gameObject, "SelectBox");
        if (gg)
        {
            gg.SetActive(true);
            g_SelectBox = gg;
            CWBridgeList kBridge = gameObject.GetComponentInParent<CWBridgeList>();
            if(kBridge!=null)
                kBridge.NCursor = GetNum();

        }
        base.OnClick();

    }


    //public int GetGItemID()
    //{
    //    CWBridgeList kBridge = gameObject.GetComponentInParent<CWBridgeList>();
    //    string szval = kBridge.GetRecord(m_Num, "GAMEICON");
    //    return CWLib.ConvertInt(szval);
    //}
}