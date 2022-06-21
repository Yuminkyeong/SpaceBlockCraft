using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;

public class Dailymission : WindowUI<Dailymission>
{
    const int MAXCOUNT =4;

    class MDATA
    {
        public int m_nKey;
        public int m_nCount;// 달성률
        public int m_nMaxCount;//
        public int m_nReward; //결과를 받았는가?
    }


    Dictionary<DAYMTYPE, MDATA> m_kData = new Dictionary<DAYMTYPE, MDATA>();

    CWJSon m_kTodayData = new CWJSon();

    void MakeNumber()
    {
        int tcnt = CWTableManager.Instance.GetTableCount(m_gScrollList.m_szValues); // m_gScrollList.GetCount();

        List<MDATA> kTemp = new List<MDATA>();


        for (int i = 0; i < tcnt; i++)
        {
            int nval = CWTableManager.Instance.GetTableInt(m_gScrollList.m_szValues, "Use", i + 1); //m_gScrollList.GetInt(i, "Use");
            if (nval == 0) continue;

            MDATA mm = new MDATA();
            mm.m_nKey = CWTableManager.Instance.GetTableInt(m_gScrollList.m_szValues, "key", i + 1); //m_gScrollList.GetInt(i, "key");
            mm.m_nCount = 0;
            mm.m_nMaxCount = CWTableManager.Instance.GetTableInt(m_gScrollList.m_szValues, "MaxCount", i + 1); //m_gScrollList.GetInt(i, "key");
            kTemp.Add(mm);
        }


        SRandom.Create(kTemp.Count);//0~4의 숫자 k
        for (int i = 0; i < MAXCOUNT; i++)
        {
            int vv = SRandom.GetNextValue();//1,4,3 이런식의 숫자가 표현
            

            MDATA mm = new MDATA();
            mm.m_nKey = kTemp[vv].m_nKey;
            mm.m_nMaxCount= kTemp[vv].m_nMaxCount;
            mm.m_nCount = 0;
            mm.m_nReward = 0;
            DAYMTYPE key = (DAYMTYPE)mm.m_nKey;
            m_kData.Add(key, mm);

        }
    }

    bool IsContainNumber(int num)
    {
        
        if (m_kData.ContainsKey((DAYMTYPE)num))
        {
            return true;
        }
        return false;
    }

    // 하루에 한번 
    public void Create()
    {
        MakeNumber();
        
        Save();
    }
    public void Save()
    {

        JArray ja = new JArray();
        foreach(var v in m_kData)
        {
            JObject jj = new JObject();
            jj.Add("Key", (int)v.Key);
            jj.Add("Count", v.Value.m_nCount);
            jj.Add("MaxCount", v.Value.m_nMaxCount);
            jj.Add("Reward", v.Value.m_nReward);
            ja.Add(jj);
        }
        m_kTodayData.Add("Array", ja);
        PlayerPrefs.SetString("Dailymission", m_kTodayData.ToString());
    }
    public void Load()
    {
        m_kData = new Dictionary<DAYMTYPE, MDATA>();

        string szfile = PlayerPrefs.GetString("Dailymission");
        if (!CWLib.IsString(szfile))
        {
            Create();
            return;
        }
            


        m_kTodayData.LoadString(szfile);

        JArray ja = (JArray)m_kTodayData.GetJson("Array");
        for(int i=0;i<ja.Count;i++)
        {
            int Key= CWJSon.GetInt(ja[i], "Key");
            int Count= CWJSon.GetInt(ja[i], "Count");
            int MaxCount = CWJSon.GetInt(ja[i], "MaxCount");
            int Reward = CWJSon.GetInt(ja[i], "Reward");

            MDATA mm = new MDATA();
            mm.m_nKey = Key;
            mm.m_nCount = Count;
            mm.m_nMaxCount = MaxCount;
            mm.m_nReward = Reward;
            DAYMTYPE key = (DAYMTYPE)mm.m_nKey;
            m_kData.Add(key, mm);
        }


    }
    bool CheckData(JToken kToken)
    {
        CWJSon jj = new CWJSon((JObject)kToken);
        if (jj.GetInt("USE") == 0) return false;
        int nKey = jj.GetInt("KEY");
        if(IsContainNumber(nKey))
        {
            return true;
        }

        return false;
    }
    int SortFunction(JToken a, JToken b)
    {
        int v1 = 0;
        int v2 = 0;
        int key1 = CWJSon.GetInt(a, "KEY");
        int key2= CWJSon.GetInt(b, "KEY");

        int mc1 = GetMaxCount(key1);
        int mc2 = GetMaxCount(key2);

        int c1 = GetCount(key1);
        int c2 = GetCount(key2);

        if(c1>=mc1)
        {
            v1 = 100;
            if (IsReward(key1))
            {
                v1 = 0;
            }
        }
        else
        {
            v1 = 10;
        }
        if (c2 >= mc2)
        {
            v2 = 100;
            if (IsReward(key2))
            {
                v2 = 0;
            }

        }
        else
        {
            v2 = 10;
        }


        return v2 -v1;
    }
    protected override void _Open()
    {

        m_gScrollList.CBCheckData = CheckData;
        m_gScrollList.CBSortFunction = SortFunction;
        base._Open();
    }

    public bool  IsHaveReward()
    {
        foreach(var v in m_kData)
        {
            if(v.Value.m_nReward==0)
            {
                if(v.Value.m_nCount>=v.Value.m_nMaxCount)
                {
                    return true;
                }
            }
        }

        return false;
    }
    // 달성회수
    public int GetCount(int nKey)
    {
        if (m_kData.ContainsKey((DAYMTYPE)nKey))
        {
            return m_kData[(DAYMTYPE)nKey].m_nCount;
        }
        return 0;
    }
    public int GetMaxCount(int nKey)
    {
        if (m_kData.ContainsKey((DAYMTYPE)nKey))
        {
            return m_kData[(DAYMTYPE)nKey].m_nMaxCount;
        }
        return 0;
    }
    public bool IsReward(int nKey)
    {
        if (m_kData.ContainsKey((DAYMTYPE)nKey))
        {
            if (m_kData[(DAYMTYPE)nKey].m_nReward == 1) return true;
        }

        return false;
    }
    public void OnReward(int nKey,int Gold,int Gem)
    {
        if (m_kData.ContainsKey((DAYMTYPE)nKey))
        {
            CWSocketManager.Instance.UseCoin2(COIN.GOLD, Gold, COIN.GEM, Gem, CWSocketManager.UseCoin_ResultFuc, "UseCoin_ResultFuc");
            m_kData[(DAYMTYPE)nKey].m_nReward = 1;
        }

        Save();
    }
    public bool CheckUpdate( DAYMTYPE nType, int count)
    {
        if (count < 0) count = -count;

        if (m_kData.ContainsKey(nType))
        {
            m_kData[nType].m_nCount += count;
            
            Save();
        }
        return true;
    }
}
