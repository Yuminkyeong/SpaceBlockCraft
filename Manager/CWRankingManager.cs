using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWUnityLib;
using System;
public class CWRankingManager : CWManager<CWRankingManager>
{
    struct _USERDATA_
    {
        public int nID;
        public string szName;
        public int nScore;
    }
    
    Dictionary<int, _USERDATA_> m_kData = new Dictionary<int, _USERDATA_>();
    int[] m_kUser = new int[1100];

    public override void Create()
    {
        base.Create();
        StartCoroutine("IRun");
    }

    public string GetUserName(int nID)
    {
        if(m_kData.ContainsKey(nID))
        {
            return m_kData[nID].szName;
        }
        return "";// 등수가 없다
    }
    // 등수대로 유저 출력
    // 1부터 시작
    public int GetRankUser(int Rank)
    {
        return m_kUser[Rank];
    }

    void ReceiveData(JObject jData)
    {
        Debug.Log("Ranking data " );
        m_kData.Clear();

        Array.Clear(m_kUser, 0, m_kUser.Length);

        if (jData["Result"].ToString() == "ok")
        {
            int nRank = 1;
            JArray ja = (JArray)jData["List"];
            for(int i=0;i<ja.Count;i++)
            {

                _USERDATA_ kData = new _USERDATA_();
                CWJSon jj = new CWJSon((JObject)ja[i]);
                int nID = jj.GetInt("_id");

                kData.nID = nID;
                kData.nScore = jj.GetInt("Score");
                kData.szName = jj.GetString("Name");
                m_kData.Add(nID, kData);
                m_kUser[nRank] = nID;
                nRank++;
            }
        }
    }
    void AskRank()
    {
        CWSocketManager.Instance.Rankinglist(ReceiveData, "ReceiveData");
    }

    IEnumerator IRun()
    {
        yield return new WaitForSeconds(5);
        while(true)
        {
            AskRank();
            yield return new WaitForSeconds(100f);
        }
    }
}
