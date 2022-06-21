using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using System.Linq;
using SimpleJSON;

using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWStruct;
using CWUnityLib;
using CWEnum;
using UnityEngine.Networking;

public class ShopItemSlot : SlotItemUI
{
    public enum STORETYPE
    {
        DISCOUNT, PAKAGE, COIN, CRYSTAL, BLOCK, PLANE, WEAPON, ITEM, SHAPE, CHARACTER, COLOR
    }
    public STORETYPE StoreType;

    // 하루에 살 수 있는 개수 관리 

    // 타이머등 관리
    public GameObject m_gImpossible;// 구입 완료 혹은 구입 불가!

    public override bool UpdateData()
    {
        base.UpdateData();      

        if(StoreType == STORETYPE.PAKAGE)
        {
            string szStr1 = m_gList.GetString(m_nNumber, "ItemArray");
            string[] szArray = szStr1.Split(',');

            string szStrCnt = m_gList.GetString(m_nNumber, "CountArray");
            string[] szArrayCnt = szStrCnt.Split(',');

            GameObject[] Slots = this.gameObject.GetComponent<PackageComponents>().Slots;

            foreach (var v in Slots)
                v.SetActive(false);

            for (int i = 0; i < szArray.Length; i++)
            {
                Slots[i].SetActive(true);
                Slots[i].transform.Find("ItemIcon").gameObject.GetComponent<RawImage>().texture = CWResourceManager.Instance.GetItemIcon(Int32.Parse(szArray[i]));
                Slots[i].transform.Find("Num").gameObject.GetComponent<CWText>().text = szArrayCnt[i];
            }
        }

        int nTimer= m_gList.GetInt(m_nNumber, "Timer");
        if(nTimer > 0)
        {
            string szID = string.Format("{0}_{1}", m_gList.m_szValues, m_nNumber);

            int nTCount = m_gList.GetInt(m_nNumber, "TimeCount");
            int nTNowCount = 0;
            if (nTCount>0)
            {
                // 개수 존재 
                nTNowCount = CWHeroManager.Instance.GetStoreTimerCount(szID); 
            }
            DateTime tDate = CWHeroManager.Instance.GetStoreTimer(szID);
            TimeSpan ts = DateTime.Now - tDate;


            ///CWDebugManager.Instance.Log(string.Format("TotalSeconds {0}", ts.TotalSeconds));

            
            // 구입 시간 : 언제 까지 살 수 있다, 내가 산지 얼마가 됐다 
            if (ts.TotalSeconds>1 && ts.TotalSeconds < nTimer * 3600)
            {
                // 구입 불가
                if(m_gImpossible)
                    m_gImpossible.SetActive(true);

                Text[] tt = GetComponentsInChildren<Text>(true);
                foreach (var v in tt)
                {
                    if (v.name.ToUpper() == "TIMER")
                    {

                        float trest =(3600*24f) - (float) ts.TotalSeconds;
                        v.text = CWLib.GetTimeString(trest);
                    }
                    if (v.transform.parent.name.ToUpper() == "TIMECOUNT")
                    {
                        v.transform.parent.gameObject.SetActive(false);
                    }
                }


            }
            else
            {
                //구입 가능
                if (m_gImpossible)
                    m_gImpossible.SetActive(false);

                Text[] tt = GetComponentsInChildren<Text>(true);
                foreach (var v in tt)
                {
                    if (v.name.ToUpper() == "TIMER")
                    {
                        if (v.transform.parent.name.ToUpper() == "TIMER")
                        {
                            v.transform.parent.gameObject.SetActive(false);
                        }

                    }
                    if (v.name.ToUpper() == "TIMECOUNT")
                    {
                        v.text = string.Format("{0}/{1}", nTNowCount, nTCount);
                    }
                }

            }

        }
        else
        {
            Text[] tt = GetComponentsInChildren<Text>(true);
            foreach (var v in tt)
            {
                
                if (v.name.ToUpper() == "TIMER")
                {
                    if(v.transform.parent.name.ToUpper()== "TIMER")
                    {
                        v.transform.parent.gameObject.SetActive(false);
                    }
                }
                if (v.name.ToUpper() == "TIMECOUNT")
                {
                    if (v.transform.parent.name.ToUpper() == "TIMECOUNT")
                    {
                        v.transform.parent.gameObject.SetActive(false);
                    }

                }
            }

            if (m_gImpossible)
                m_gImpossible.SetActive(false);
        }


        return true;
    }

    public override void ResultEvent()
    {
        base.ResultEvent();
        int nTimer = m_gList.GetInt(m_nNumber, "Timer");
        if(nTimer>0)
        {

            string szID = string.Format("{0}_{1}", m_gList.m_szValues, m_nNumber);

            int nTCount = m_gList.GetInt(m_nNumber, "TimeCount");
            if(nTCount>0)
            {
                int nTNowCount = CWHeroManager.Instance.GetStoreTimerCount(szID)+1;
                CWHeroManager.Instance.SetStoreTimerCount(szID, nTNowCount );
                if (nTNowCount>= nTCount)
                {
                    // 오늘 구입 끝남 체크
                    CWHeroManager.Instance.SetStoreTimer(szID);
                }

            }
            else
            {
                CWHeroManager.Instance.SetStoreTimer(szID);
            }
            

        }

        StoreDlg.Instance.SelectActiveUI();

        BaseUI bs = GetComponentInParent<BaseUI>();
        bs.UpdateData();
        


        StoreDlg.Instance.UpdateData();
    }
    public override void OnBuy()
    {
        int nTimer = m_gList.GetInt(m_nNumber, "Timer");
        if (nTimer > 0)
        {
            string szID = string.Format("{0}_{1}", m_gList.m_szValues, m_nNumber);
            DateTime tDate = CWHeroManager.Instance.GetStoreTimer(szID);
            TimeSpan ts = DateTime.Now - tDate;
            if (ts.TotalSeconds > 0.5 && ts.TotalSeconds < nTimer * 3600)// 구입불가
            {
                //구입 불가 
                return;
            }

        }

        base.OnBuy();
    }

}
