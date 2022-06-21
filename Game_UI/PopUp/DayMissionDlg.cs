using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;

public class DayMissionDlg : PageUI<DayMissionDlg>
{

    protected override int GetUINumber()
    {
        return 13;
    }


    #region 랜덤 생성

    List<int> m_kResult = new List<int>();
    void InitRandom()
    {

        m_kResult = new List<int>();
        List<int> kTemp = new List<int>();
        for (int i = 0; i < COUNT; i++)
        {
            kTemp.Add(i);
        }
        for (int i = 0; i < COUNT; i++)
        {
         
            int TT = Random.Range(0, kTemp.Count);
            m_kResult.Add(kTemp[TT]);
            kTemp.RemoveAt(TT);
        }

    }
    int GetRandom(int num)
    {
        return m_kResult[num];
    }


    #endregion

    const int ACTIVECOUNT = 2;// 2개의 미션을 제공한다
    const int COUNT = 7;
    


    DAYMTYPE m_nType;

    class Mission
    {
        public int m_nType;
        public string m_szTitle;
        public int m_nMaxCount;
        public int m_nCount;
        public int m_nReward;// 보상 받았나?
    }

    Mission[] m_kMission = new Mission[COUNT];

    int[] m_nNowMission = new int[ACTIVECOUNT];

    CWJSon m_kMissonData = new CWJSon();

    int GetMaxGoldCount()
    {
        int MaxCount = 1000;
        for (int i = 0; i < 10; i++)
        {
            int lv1 = CWHeroManager.Instance.GetWeaponDamageLevel(i);
            MaxCount += CWHeroManager.Instance.GetWeaponDamageGold(0,lv1);
            int lv2 = CWHeroManager.Instance.GetWeaponRangeLevel(i);
            MaxCount += CWHeroManager.Instance.GetWeaponRangeGold(lv2);
            int lv3 = CWHeroManager.Instance.GetWeaponSpeedLevel(i);
            MaxCount += CWHeroManager.Instance.GetWeaponSpeedGold(lv3);
        }

        return MaxCount * 10;

    }

    int GetMultiCount()
    {
        if(CWHero.Instance.GetHP()<50)
        {
            return 4;
        }
        return 8;
    }
    int GetPVPCount()
    {
        if (CWHero.Instance.GetHP() < 25)
        {
            return 2;
        }
        if (CWHero.Instance.GetHP() < 50)
        {
            return 3;
        }
        if (CWHero.Instance.GetHP() < 100)
        {
            return 4;
        }
        return 5;

    }
    int GetMaxBlockCount()
    {
        int MaxCount = 1;
        for(int i=0;i<10;i++)
        {
            int lv = CWHeroManager.Instance.GetWeaponRangeLevel(i);
            if (lv == 0) break;
            MaxCount += CWHeroManager.Instance.GetWeaponRange(lv);
        }
        return MaxCount*30;

    }
    // 현재 미션인가?

    public void ResetData()
    {
        m_kMissonData = new CWJSon();
        InitRandom();
        m_kMission = new Mission[COUNT];
        for(int i=0;i<COUNT;i++)
        {
            m_kMission[i] = new Mission();
        }
        
        // 이름을 정한다
        string str1 = CWLocalization.Instance.GetLanguage("골드{0}소진하기");
        string str2 = CWLocalization.Instance.GetLanguage("블록{0}개 획득하기");
        string str3 = CWLocalization.Instance.GetLanguage("행성 1개 정복하기");
        string str4 = CWLocalization.Instance.GetLanguage("멀티에서 {0} 유저 파괴하기");
        string str5 = CWLocalization.Instance.GetLanguage("1:1대전 {0}승 하기");
        string str6 = CWLocalization.Instance.GetLanguage("좋아요 5번 누르기");
        string str7 = CWLocalization.Instance.GetLanguage("금속블록 4개 획득하기");//

        // 좋아요 10번 누르기

        // 개수를 설정한다

        m_kMission[0].m_nMaxCount = GetMaxGoldCount();
        m_kMission[1].m_nMaxCount = 100000;//십만개 이상!! 자동 파밍으로 얻어야 한다!!
        m_kMission[2].m_nMaxCount = 1;
        m_kMission[3].m_nMaxCount = GetMultiCount();
        m_kMission[4].m_nMaxCount = GetPVPCount();
        m_kMission[5].m_nMaxCount = 5;
        m_kMission[6].m_nMaxCount = 4;

        // string.Format("{0:0,0}", CWLib.ConvertInt(base.text));

        string szva1 = string.Format("{0:0,0}", m_kMission[0].m_nMaxCount);
        string szva2 = string.Format("{0:0,0}", m_kMission[1].m_nMaxCount);

        m_kMission[0].m_szTitle = string.Format(str1, szva1);
        m_kMission[1].m_szTitle = string.Format(str2, szva2);

        m_kMission[2].m_szTitle = str3;
        m_kMission[3].m_szTitle = string.Format(str4, m_kMission[3].m_nMaxCount);
        m_kMission[4].m_szTitle = string.Format(str5, m_kMission[4].m_nMaxCount);

        m_kMission[5].m_szTitle = str6;
        m_kMission[6].m_szTitle = str7;


        for (int i = 0; i < ACTIVECOUNT; i++)
        {
            m_nNowMission[i] = -1;
        }
        for (int i=0;i<ACTIVECOUNT;i++)
        {
            
            m_nNowMission[i] = GetRandom(i);
        }
        Save();
        UpdateData();
    }
    bool IsNowMission(int num)
    {

        if(num==5)
        {
            if (CWHeroManager.Instance.GetLikeCount() > 20)
            {
                return true;
            }
        }
        foreach(var v in m_nNowMission)
        {
            if (v == num) return true;
        }
        return false;
    }
    public void ChangeMission(int num)
    {
        
        int RR = Random.Range(0, 10);
        
        for(int i= RR; i<COUNT+ RR; i++)
        {
            int kk = i % COUNT;
            if(!IsNowMission(m_kResult[kk]))
            {
                m_nNowMission[num] = m_kResult[kk];

                Mission kMission = m_kMission[m_nNowMission[num]];
                if(kMission==null)
                {
                    return;
                }
                kMission.m_nCount = 0;
                kMission.m_nReward = 0;

                Save();
                return;
            }
            
        }

        
    }

    Mission GetMission(int num)
    {
        if (num < 0) return null;
        if (num >= ACTIVECOUNT) return null;
        int n = m_nNowMission[num];
        return m_kMission[n];
    }

    //GetMissionSubTitle
    public string GetMissionSubTitle(int num)
    {
        if (num < 0) return "";
        if (num >= ACTIVECOUNT) return "";

        int n = m_nNowMission[num];

        if(m_kMission[n].m_nMaxCount < 1000)
        {
          return   string.Format("{0}/{1}", m_kMission[n].m_nCount, m_kMission[n].m_nMaxCount);
        }
        return string.Format("{0:0,0}/{1:0,0}", m_kMission[n].m_nCount, m_kMission[n].m_nMaxCount);
    }
    public string GetMissionTitle(int num)
    {
        if (num < 0) return "";
        if (num >= ACTIVECOUNT) return "";

        int n = m_nNowMission[num];
        return m_kMission[n].m_szTitle;
    }

    //0 진행중 1 보상받기 대기, 2 완료
    public int GetState(int num)
    {
        Mission kMission = GetMission(num);
        if (kMission == null) return 0;
        
        if (kMission.m_nReward==1) return 2;

        if(kMission.m_nCount==kMission.m_nMaxCount)
        {
            return 1;
        }
        return 0;
    }
    public void OnReward(int num)
    {
        Mission kMission = GetMission(num);
        kMission.m_nReward = 1;
        Save();
        UpdateData();
    }
    
    public bool Load()
    {
        string szStr = string.Format("DayMission_N2{0}",CWHero.Instance.m_nID);
        if(!m_kMissonData.LoadString(PlayerPrefs.GetString(szStr)))
        {
            return false;
        }



        {
            JArray ja=(JArray) m_kMissonData.GetJson("NowMission");
            for(int i=0;i<ja.Count;i++)
            {
                m_nNowMission[i] = ja[i].Value<int>();
            }
        }
        {
            JArray ja = (JArray)m_kMissonData.GetJson("Mission");
            for (int i = 0; i < COUNT; i++)
            {
                m_kMission[i] = new Mission();
                JObject jData = new JObject();
                if (ja.Count > i)
                {
                    m_kMission[i].m_nType = CWJSon.GetInt(ja[i], "Type");
                    m_kMission[i].m_nMaxCount = CWJSon.GetInt(ja[i], "MaxCount");
                    m_kMission[i].m_nCount = CWJSon.GetInt(ja[i], "Count");
                    m_kMission[i].m_nReward = CWJSon.GetInt(ja[i], "Reward");
                }
                else
                {
                    Debug.Log("");
                }
            }


            string str1 = CWLocalization.Instance.GetLanguage("골드{0}소진하기");
            string str2 = CWLocalization.Instance.GetLanguage("블록{0}개 획득하기");
            string str3 = CWLocalization.Instance.GetLanguage("행성 1개 정복하기");
            string str4 = CWLocalization.Instance.GetLanguage("멀티에서 {0} 유저 파괴하기");
            string str5 = CWLocalization.Instance.GetLanguage("1:1대전 {0}승 하기");

            string str6 = CWLocalization.Instance.GetLanguage("좋아요 5번 누르기");
            string str7 = CWLocalization.Instance.GetLanguage("금속블록 4개 획득하기");//


            // 개수를 설정한다

            string szva1 = string.Format("{0:0,0}", m_kMission[0].m_nMaxCount);
            string szva2 = string.Format("{0:0,0}", m_kMission[1].m_nMaxCount);

            m_kMission[0].m_szTitle = string.Format(str1, szva1);
            m_kMission[1].m_szTitle = string.Format(str2, szva2);

            m_kMission[2].m_szTitle = str3;
            m_kMission[3].m_szTitle = string.Format(str4, m_kMission[3].m_nMaxCount);
            m_kMission[4].m_szTitle = string.Format(str5, m_kMission[4].m_nMaxCount);

            m_kMission[5].m_szTitle = str6;
            m_kMission[6].m_szTitle = str7;



        }
        {
            JArray ja = (JArray)m_kMissonData.GetJson("Result");
            for (int i = 0; i < ja.Count; i++)
            {
                
                m_kResult.Add(ja[i].Value<int>());
            }

        }

        UpdateData();

        return true;

    }
    void Save()
    {
        m_kMissonData = new CWJSon();

        {
            JArray jj = new JArray();
            for (int i = 0; i < ACTIVECOUNT; i++)
            {
                jj.Add(m_nNowMission[i]);
            }
            m_kMissonData.Add("NowMission", jj);

        }

        {
            JArray jj = new JArray();
            for (int i = 0; i < COUNT; i++)
            {
                JObject jData =new JObject();
                
                jData.Add("Type", m_kMission[i].m_nType);
                jData.Add("MaxCount", m_kMission[i].m_nMaxCount);

                jData.Add("Count", m_kMission[i].m_nCount);

                jData.Add("Reward", m_kMission[i].m_nReward);
                jj.Add(jData);
            }
            m_kMissonData.Add("Mission", jj);

        }
        {
            JArray jj = new JArray();
            for(int i=0;i<m_kResult.Count;i++)
            {
                jj.Add(m_kResult[i]);
            }
            m_kMissonData.Add("Result", jj);
        }

        string szStr = string.Format("DayMission_N2{0}", CWHero.Instance.m_nID);

        PlayerPrefs.SetString(szStr, m_kMissonData.ToString());

    }

    public override void UpdateData(bool bselect = true)
    {
        DayMissionSlot[] Slot = GetComponentsInChildren<DayMissionSlot>();
        foreach(var v in Slot)
        {
            v.UpdateData();
        }
    }

    public void CheckUpdate(DAYMTYPE nType,int nCount)
    {
        if (!IsNowMission((int)nType)) return;
        m_kMission[(int)nType].m_nCount += Mathf.Abs(nCount);
        if(m_kMission[(int)nType].m_nCount>= m_kMission[(int)nType].m_nMaxCount)
        {
            m_kMission[(int)nType].m_nCount = m_kMission[(int)nType].m_nMaxCount;
        }

        Save();
    }
    public bool IsHaveReward()
    {
        if (GetState(0) == 1) return true;
        if (GetState(1) == 1) return true;

        return false;

    }

    protected override void _Open()
    {
        base._Open();

        string str1 = CWLocalization.Instance.GetLanguage("골드{0}소진하기");
        string str2 = CWLocalization.Instance.GetLanguage("블록{0}개 획득하기");
        string str3 = CWLocalization.Instance.GetLanguage("행성 1개 정복하기");
        string str4 = CWLocalization.Instance.GetLanguage("멀티에서 {0} 유저 파괴하기");
        string str5 = CWLocalization.Instance.GetLanguage("1:1대전 {0}승 하기");
        string str6 = CWLocalization.Instance.GetLanguage("좋아요 5번 누르기");
        string str7 = CWLocalization.Instance.GetLanguage("금속블록 4개 획득하기");//

        // 개수를 설정한다

        string szva1 = string.Format("{0:0,0}", m_kMission[0].m_nMaxCount);
        string szva2 = string.Format("{0:0,0}", m_kMission[1].m_nMaxCount);

        m_kMission[0].m_szTitle = string.Format(str1, szva1);
        m_kMission[1].m_szTitle = string.Format(str2, szva2);

        m_kMission[2].m_szTitle = str3;
        m_kMission[3].m_szTitle = string.Format(str4, m_kMission[3].m_nMaxCount);
        m_kMission[4].m_szTitle = string.Format(str5, m_kMission[4].m_nMaxCount);

        m_kMission[5].m_szTitle = str6;
        m_kMission[6].m_szTitle = str7;


    }
}
