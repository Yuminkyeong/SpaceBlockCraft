using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class RankingDlg : WindowUI<RankingDlg>
{
    protected override int GetUINumber()
    {
        return 6;
    }


    public GameObject[] m_gGroup;
    public Slider m_kSlider;
    public GameObject LikeTapList;

    
    const int MaxCount = 30;//6개씩 보냄

    int m_prnum = -1;

    protected override void OnceFunction()
    {
        m_kUIType = UITYPE.RANKING;
        base.OnceFunction();
    }
    // 파워랭킹


    public void OnSaveRanking()
    {
        CWCaptureBox.Instance.ClearAir();
        m_prnum = -1;
        StopAllCoroutines();

        m_nGroupType = 2;


        m_gScrollList.m_Type = ScrollListUI.TYPE.DB;
        m_gScrollList.m_szValues = "SaveRankingReceive";
        m_gScrollList.m_szCondition = "";

        string m_szQueryRanking = @"{{Limit:{0},Start: {1} }}";

        m_gScrollList.m_szDBParam = string.Format(m_szQueryRanking, MaxCount, 0);
        UpdateData();

        //StartCoroutine("IDBRun");

        CWSocketManager.Instance.SendMyRanking((JData) => {

            if (JData["Result"].ToString() == "ok")
            {
                CWHeroManager.Instance.m_nRanking = CWLib.ConvertIntbyJson(JData["Ranking"]);
            }

        });


    }
    public void OnPriceRanking()
    {
        CWCaptureBox.Instance.ClearAir();
        m_prnum = -1;
        StopAllCoroutines();

        m_nGroupType = 3;


        m_gScrollList.m_Type = ScrollListUI.TYPE.DB;
        m_gScrollList.m_szValues = "PriceRankingReceive";
        m_gScrollList.m_szCondition = "";

        string m_szQueryRanking = @"{{Limit:{0},Start: {1} }}";

        m_gScrollList.m_szDBParam = string.Format(m_szQueryRanking, MaxCount, 0);
        UpdateData();

        //StartCoroutine("IDBRun");

        CWSocketManager.Instance.SendMyRanking((JData) => {

            if (JData["Result"].ToString() == "ok")
            {
                CWHeroManager.Instance.m_nRanking = CWLib.ConvertIntbyJson(JData["Ranking"]);
            }

        });


    }

    
    public void OnRanking_ST()
    {
        LikeTapList.SetActive(true);
        ToggleButton tl = LikeTapList.GetComponent<ToggleButton>();
        tl.Toggle_ButtonList(0);

        OnSaveRanking();
    }

    public void OnLikeRanking()
    {
        

        CWCaptureBox.Instance.ClearAir();
        m_prnum = -1;
        StopAllCoroutines();

        m_nGroupType = 2;

        m_gScrollList.m_Type = ScrollListUI.TYPE.DB;  
        m_gScrollList.m_szValues = "LikeRankingReceive";
        m_gScrollList.m_szCondition = "";

        string m_szQueryRanking = @"{{Limit:{0},Start: {1} }}";

        m_gScrollList.m_szDBParam = string.Format(m_szQueryRanking, MaxCount, 0);
        UpdateData();

     //   StartCoroutine("IDBRun");

        CWSocketManager.Instance.SendMyRanking((JData) =>
        {
            if (JData["Result"].ToString() == "ok")
            {
                CWHeroManager.Instance.m_nRanking = CWLib.ConvertIntbyJson(JData["Ranking"]);
            }
        });
    }
    // 티어 랭킹
    void _OnRanking(int Grade)
    {
        LikeTapList.SetActive(false);
        //if(Grade==0)
        //{
        //    _OnChoboRanking();
        //    return;
        //}
        CWCaptureBox.Instance.ClearAir();
        m_prnum = -1;
        StopAllCoroutines();

        m_nGroupType = 1;

        
        m_gScrollList.m_Type = ScrollListUI.TYPE.DB;
        m_gScrollList.m_szValues = "RankingReceive";
        m_gScrollList.m_szCondition = "";

        string m_szQueryRanking = @"{{Limit:{0},Start: {1} }}";

        m_gScrollList.m_szDBParam = string.Format(m_szQueryRanking, MaxCount, 0);
        UpdateData();

        //StartCoroutine("IDBRun");

        CWSocketManager.Instance.SendMyRanking((JData) => {

            if (JData["Result"].ToString() == "ok")
            {
                CWHeroManager.Instance.m_nRanking = CWLib.ConvertIntbyJson(JData["Ranking"]);
            }

        });

    }
    public void OnRanking()
    {
        _OnRanking(6);

    }
    public void OnMyRoom()
    {

    }
  

    IEnumerator IDBRun()
    {
        m_gScrollList.m_nSelect = 0;
        int page = MaxCount;
        while (true)
        {
            yield return new WaitForSeconds(4f);

            if (m_gScrollList.IsSendOK())
            {
                string m_szQueryRanking = @"{{Limit:{0},Start: {1} ,Sort :{{Ranking : 1}}}}";
                m_gScrollList.m_szDBParam = string.Format(m_szQueryRanking, MaxCount, page); page += MaxCount;

                if (m_gScrollList.AskDBList() == false)
                {
                    break;
                }
            }
        }              
    }
    
    public override void OnSelect(int num)
    {
        if (m_prnum == num) return;
        m_prnum = num;
        int nID = m_gScrollList.GetInt(m_gScrollList.m_nSelect, "Id");
      
        CWCaptureBox.Instance.ShowUser(nID);

        //Debug.Log(string.Format("Ranking!!! CWCaptureBox.Instance.ShowUser({0}) ", nID));
        m_kSlider.value = 0.7f;


        base.OnSelect(num);
    }
    public override void Open()
    {

        CWHero.Instance.gameObject.SetActive(false);
        
        m_kSlider.value = 0.7f;
        base.Open();
        OnRanking();
    }
    public override void Close()
    {
        CWHero.Instance.gameObject.SetActive(true);
        CWCaptureBox.Instance.Close();
        base.Close();

    }
    public void OnScaleChange()
    {
        if (!CWCaptureBox.Instance) return;
        if (!CWCaptureBox.Instance.m_gAirObject) return;

        float fscale = 0.2f+(1 - m_kSlider.value);

        CWCaptureBox.Instance.m_gAirObject.transform.localScale = new Vector3(fscale, fscale, fscale);
    }

    public override void UpdateData(bool bselect = true)
    {
        foreach (var v in m_gGroup) v.SetActive(false);

        m_gGroup[m_nGroupType-1].SetActive(true);

        m_kSlider.value = 0.7f;
        base.UpdateData(bselect);
    }

    public void OnRank_bronze()
    {
        _OnRanking(1);
    }
    public void OnRank_silver()
    {
        _OnRanking(2);
    }
    public void OnRank_gold()
    {
        _OnRanking(3);
    }
    public void OnRank_Platinum()
    {
        _OnRanking(4);
    }
    public void OnRank_Diamond()
    {
        _OnRanking(5);
    }
    public void OnRank_Master()
    {
        _OnRanking(6);
    }
    public void OnRank_GrandMaster()
    {
        _OnRanking(7);
    }

    public void OnAddLike()
    {
        int nID = m_gScrollList.GetInt(m_gScrollList.m_nSelect, "Id");
        if (nID == CWHero.Instance.m_nID) return;
        int nLike = m_gScrollList.GetInt(m_gScrollList.m_nSelect, "Like");

        if (!CWHeroManager.Instance.SendAddLike(nID))
        {
            nLike++;
        }
        else
        {
            nLike--;
        }


        m_gScrollList.SetString(m_gScrollList.m_nSelect,"Like",nLike.ToString());
    }
    /*
    #region   초보 랭킹 개념 

    class CHODATA
    {
        public int Id;
        public string Name;
        public int RankPoint;
        public int Grade;
        public int Like;
        public int Price;
        public int Ranking;
    }
    List<CHODATA> m_kChoboDB= new List<CHODATA>();
    

    CHODATA m_kCHODATAByHero=null;

    public void InitChoboData()
    {
        if (m_kCHODATAByHero != null) return;

        string szQueryRanking = @"{{Limit:{0},Start: {1},Grade: {2} }}";
        string szDBParam = string.Format(szQueryRanking, MaxCount, 0, 0);
        
        CWSocketManager.Instance.SendDlg("RankingReceive", szDBParam, (jData)=> {

            JArray ja = (JArray)jData["List"];
            CHODATA hh = new CHODATA();
            hh.Name = CWHero.Instance.name;
            hh.Id = CWHero.Instance.m_nID;
            hh.Grade = 0;
            hh.RankPoint = CWHeroManager.Instance.m_nRankPoint;
            hh.Price = CWHero.Instance.GetPrice();
            hh.Like = 0;
            hh.Ranking = 0;
            m_kChoboDB.Add(hh);
            m_kCHODATAByHero = hh;

            for (int i=0;i<ja.Count;i++)
            {
                CHODATA cc = new CHODATA();
                cc.Id = CWJSon.GetInt(ja[i], "Id");
                if (m_kCHODATAByHero.Id == cc.Id) continue;

                cc.RankPoint = CWJSon.GetInt(ja[i], "RankPoint");
                cc.Grade = CWJSon.GetInt(ja[i], "Grade");
                cc.Like = CWJSon.GetInt(ja[i], "Like");
                cc.Price = CWJSon.GetInt(ja[i], "Price");
                cc.Name = CWJSon.GetString(ja[i], "Name");
                cc.Ranking = 0;
                m_kChoboDB.Add(cc);
            }
        }, "InitChoboData");
    }
    int GetListCount()
    {
        return m_kChoboDB.Count;
    }
    int GetListColumnCount()
    {
        return 7;//
    }
    string GetListColumn(int Col)
    {
        if (Col == 0) return "Id";
        if (Col == 1) return "Name";
        if (Col == 2) return "RankPoint";
        if (Col == 3) return "Grade";
        if (Col == 4) return "Like";
        if (Col == 5) return "Price";
        if (Col == 6) return "Ranking";
        
        return "";
    }
    string GetListValue(int Raw, int Col)
    {
        if (Col == 0)
        {
            return m_kChoboDB[Raw].Id.ToString();
        }
        if (Col == 1)
        {
            return m_kChoboDB[Raw].Name.ToString();
        }
        if (Col == 2)
        {
            return m_kChoboDB[Raw].RankPoint.ToString();
        }
        if (Col == 3)
        {
            return m_kChoboDB[Raw].Grade.ToString();
        }
        if (Col == 4)
        {
            return m_kChoboDB[Raw].Like.ToString();
        }
        if (Col == 5)
        {
            return m_kChoboDB[Raw].Price.ToString();
        }
        if (Col == 6)
        {
            return m_kChoboDB[Raw].Ranking.ToString();
        }

        return "";
    }

    public void MakeChoboData()
    {

        int tcnt = 0;
        foreach(var v in m_kChoboDB)
        {
            v.RankPoint = Random.Range(1, 24);
            if (v!= m_kCHODATAByHero)
            {
                tcnt++;
                if (tcnt >= 5) continue;
                if (tcnt <= 2)
                {
                    v.RankPoint = Random.Range(70, 120);
                }
                else
                {
                    v.RankPoint = Random.Range(25, 50);
                }
                
            }
            
        }

        m_kCHODATAByHero.RankPoint = CWHeroManager.Instance.m_nRankPoint;


        m_kChoboDB.Sort((CHODATA a, CHODATA b) => {
            return b.RankPoint - a.RankPoint;
        });
        int Ranking  = 1;
        int RankPoint = 0;
        m_kChoboDB[0].Ranking=1;

        for (int i = 0; i < m_kChoboDB.Count; i++)
        {
            m_kChoboDB[i].Ranking = Ranking;
            RankPoint = m_kChoboDB[i].RankPoint;
            if (i< m_kChoboDB.Count-1)
            {
                if (RankPoint == m_kChoboDB[i + 1].RankPoint)
                {
                    continue;
                }
            }
            Ranking = i+2;

        }

        CWHeroManager.Instance.m_nRanking = m_kCHODATAByHero.Ranking;
    }

    void _OnChoboRanking()
    {

        MakeChoboData();

        CWCaptureBox.Instance.ClearAir();
        m_prnum = -1;
        StopAllCoroutines();
        m_nGroupType = 1;
        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_szCondition = "";
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        UpdateData();



    }

    #endregion
    */

    public void RankingRewardOpen()
    {
        RankingRewardDlg.Instance.Open();
    }
}
