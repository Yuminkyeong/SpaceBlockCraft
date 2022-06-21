using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CWUnityLib;
using Newtonsoft.Json.Linq;
using CWEnum;


public class CWShowAirObject : CWAirObject
{
    int m_nTypeAir=0;
    void ReceiveUserData(JObject jData)
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }


        if (jData["Result"].ToString() == "ok")
        {

            CWJSon jSon = new CWJSon(jData);
            int nID = jSon.GetInt("UserID");
            name = jSon.GetString("Name");
            byte[] BlockData = jSon.GetBytes("BlockData");
            if(BlockData==null)
            {
                TextAsset aa = CWResourceManager.Instance.GetAirObject("Firstship");
                SetBuffer(aa.bytes);
            }
            else
            {
                if (BlockData.Length < 10)
                {
                    TextAsset aa = CWResourceManager.Instance.GetAirObject("Firstship");
                    SetBuffer(aa.bytes);
                }
                else
                {
                    SetBuffer(BlockData);
                }

            }
            gameObject.SetActive(true);
            Create(m_nID);

        }
        else
        {
            // 에라 처리 
        }
    }

    public void ShowUser(int nID)
    {
        if (CWSocketManager.Instance)
            CWSocketManager.Instance.AskUserData(nID, ReceiveUserData, "ReceiveUserData");
        m_nLayer = GameLayer.Edit;
        m_nTypeAir = 1;
    }
    public void ShowUserGoods(int nID)
    {
        if (CWSocketManager.Instance)
            CWSocketManager.Instance.AskUserGoodsData(nID, ReceiveUserData, "ReceiveUserData");
        m_nLayer = GameLayer.Edit;
        m_nTypeAir = 2;
    }

    public void ShowUserSlotDB(int nID)
    {
        if (CWSocketManager.Instance)
            CWSocketManager.Instance.AskUserGoodsData(nID, ReceiveUserData, "ReceiveUserData");
        m_nLayer = GameLayer.Edit;
        m_nTypeAir = 2;
    }

    public void ShowAirObject(string szname)
    {
        m_nTypeAir = 0;
        m_nLayer = GameLayer.Edit;
        Load(szname);
    }

    public override void CopyObject(CWObject kObject)
    {
        base.CopyObject(kObject);
        FixPos();
    }

    protected override void MakeProgressBar()
    {

    }

    public override void FixPos()
    {
        float DEFAULT_scale = 20;
        float fy = m_vStart.y - 1;
        float fx = m_vSize.x;
        float fz = m_vSize.z;

        float fff = (fx + fz + m_vSize.y*2) ;
        float fdx = DEFAULT_scale / fff;

        if (fdx > 1) fdx = 1;
        fy *= fdx;
        if (m_gCenterObject == null) return;
        m_gCenterObject.transform.localScale = new Vector3(fdx, fdx, fdx);
        m_gCenterObject.transform.localPosition = new Vector3(0, -fy, 0);

        SetHitDummy();

    }
}
