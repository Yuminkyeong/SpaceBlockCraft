using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWStruct;
using CWUnityLib;
using CWEnum;

#if !NO_SERVICE
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class CWQuestManager : CWManager<CWQuestManager>
{

    public const int MAXCOUNT = 100000;// 최대 달성 개수
    public const int MAXQUEST = 100;


    int m_nCount = 0;
    public QUESTDATA [] m_kQuestData = new QUESTDATA[MAXQUEST];

    
    public bool m_bReceive = false;


    // 보상이 있다
    public bool IsHaveReward()
    {
        foreach(var v in m_kQuestData)
        {
            
            if(GetState(v.m_nID)==1)
            {
                return true;
            }
        }
        return false;
    }

    public QUESTDATA GetQuest(int num)
    {
        
        return m_kQuestData[num];

    }
    public QUESTDATA GetQuestByID(int nID)
    {
        foreach(var v in m_kQuestData)
        {
            if (v.m_nID == nID) return v;
        }
        return null;
    }
    public int GetCount()
    {
        return m_nCount;
    }
    public string GetRateString(int nID)
    {
        if (nID == 0) return "";
        QUESTDATA qData = GetQuestByID(nID);
        return string.Format("{0}/{1}", qData.m_nCount, qData.m_nMaxCount);

    }

    // 진행율
    public float GetRate(int nID)
    {
        if (nID == 0) return 0;
        QUESTDATA qData = GetQuestByID(nID);
        if (qData == null)
        {
            return 0;
        }
        if (qData.m_nCount == MAXCOUNT) return 1;
        return (float)qData.m_nCount / qData.m_nMaxCount;

    }
    //2 완료, 1 보상 0 진행중 3: 퀘스트 끝났음

    public int GetState(int nID)
    {
        if (nID == 0) return 0;
        QUESTDATA qData = GetQuestByID(nID);
        if(qData==null)
        {
            return 0;
        }
        if (qData.m_nCount == MAXCOUNT) return 2;
        if (qData.m_nCount >= qData.m_nMaxCount)
        {
            return 1;// 보상대기
        }

        return 0; // 미완료
    }
    public void RewardQuest(int nID,BaseUI.CBClose func)
    {
        QUESTDATA qData = GetQuestByID(nID);
        qData.m_nCount = MAXCOUNT;// 보상 완료
        CWSocketManager.Instance.UpdateQuest(nID, qData.m_nCount);

        CWSocketManager.Instance.UseCoinEx(COIN.GEM, qData.m_nRewardCount, () => {
        });
        

        TakeItemDlg.Instance.ShowCoin(COIN.GEM, func);

    }
    void UpdateQuest(int nID,int nCount)
    {
        QUESTDATA qData = GetQuestByID(nID);
        if(qData==null)
        {
            return;
        }
        qData.m_nCount += nCount;
        if(qData.m_nCount>= qData.m_nMaxCount)
        {
            qData.m_nCount = qData.m_nMaxCount;
        }
        if(nID==30|| nID == 31 || nID == 32 || nID == 33)
        {
            qData.m_nCount = CWHeroManager.Instance.m_nPVPWin;
        }


        CWSocketManager.Instance.UpdateQuest(nID, qData.m_nCount);
        double drate = (double)qData.m_nCount/ qData.m_nMaxCount;

#if !NO_SERVICE
        if (Social.localUser.authenticated)
        {
            //NoticeMessage.Instance.Show("구글업적!");
            Social.ReportProgress(qData.m_szGoogleID, drate, (ret) => {


            });

        }
#endif

    }

    public void UpdatePvpData()
    {


        QUESTDATA q1=GetQuestByID(30);
        q1.m_nCount = CWHeroManager.Instance.m_nPVPWin;

        QUESTDATA q2 = GetQuestByID(31);
        q2.m_nCount = CWHeroManager.Instance.m_nPVPWin;

        QUESTDATA q3 = GetQuestByID(32);
        q3.m_nCount = CWHeroManager.Instance.m_nPVPWin;
        QUESTDATA q4 = GetQuestByID(33);
        q4.m_nCount = CWHeroManager.Instance.m_nPVPWin;

    }
    public void ReceiveData(JArray ja)
    {
        m_bReceive = true;
        if (ja.Count > 0)
        {
            for (int i = 1; i < ja.Count; i++)
            {

                QUESTDATA qData = GetQuestByID(i);
                if(qData==null)
                {
                    continue;
                }
                qData.m_nCount = CWLib.ConvertInt(ja[i].ToString());
                if(qData.m_nCount>MAXQUEST)
                {
                    qData.m_nRewardEnd = 1;
                }

            }
        }
    }

  

    public void LoadCSV()
    {
        
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("퀘스트 - 업적");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {

                QUESTDATA kData = new QUESTDATA();
                
                kData.m_nID = cs.GetInt(v.Key, "ID");
                kData.m_szTitle = cs.GetString(v.Key, "title");
                kData.m_szHelp = cs.GetString(v.Key, "subtitle");
                kData.m_szGoogleID = cs.GetString(v.Key, "google");
                kData.m_nRewardCount = cs.GetInt(v.Key, "reward");
                kData.m_nMaxCount = cs.GetInt(v.Key, "Count");
                kData.m_szIcon = cs.GetString(v.Key, "Icon");
                kData.m_nCount =0;
                kData.m_nRewardEnd = 0;
                m_kQuestData[m_nCount] = kData;
                m_nCount++;
            }
        }


    }
    public override void Create()
    {
        for(int i=0;i< m_kQuestData.Length; i++)
        {
            m_kQuestData[i] = new QUESTDATA();
        }
        base.Create();
    }

    public void CheckUpdateData(int nID,int nCount=1)
    {
        int nState =GetState(nID);
        if (nState != 0) return;
        UpdateQuest(nID, nCount);

    }




}
