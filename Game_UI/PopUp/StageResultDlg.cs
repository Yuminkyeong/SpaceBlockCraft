using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using CWEnum;
using CWUnityLib;
using CWStruct;
public class StageResultDlg : WindowUI<StageResultDlg>
{

    


    


    

    CBClose m_CBClose1;

    bool m_bVictory = false;

    

    public Transform m_tVictoryText;
    public Transform m_tParming;
    public Sprite[] m_kSprite;
    public GameObject m_gPlnet;
    public RawImage m_kTicket;
    public RawImage m_kGift;

    public GameObject[] m_gImage1;
    public GameObject[] m_gImage2;

    public GameObject m_gRewardAD;

    public Text m_kADText;

    public Text m_kGold;
    
    public Text m_kTicketCount;

    public Text m_KADText2;
    public Text m_KADText3;
    public GameObject ui1;
    public GameObject ui2;

    int m_bDoubleCount = 1;
    int numberCount = 0;
    CWButton m_ADBtn;

    public override void UpdateData(bool bselect = true)
    {
        CWArrayManager.PlanetData kPlanetData = CWArrayManager.Instance.GetPlanetData(CWHeroManager.Instance.m_nPlanetID);
        int nGold = kPlanetData.m_nGold;
        int nTicket = kPlanetData.m_nTicket;
        if (m_bDoubleCount>1)
        {
            //m_kTicketCount.text = string.Format("{0}x{1}",nTicket, m_bDoubleCount);// nTicket.ToString() + " " + m_bDoubleCount.ToString()+ "배!!";
            //m_kGold.text = string.Format("{0}x{1}", nGold, m_bDoubleCount);//nGold.ToString()+" " + m_bDoubleCount.ToString()+ "배!!";
            numberCount++;
            ui1.SetActive(true);
            ui2.SetActive(true);
            m_KADText2.text = string.Format("{0}배!", m_bDoubleCount);
            m_KADText3.text = string.Format("{0}배!", m_bDoubleCount);
            m_kTicketCount.text = string.Format("{0}", nTicket*m_bDoubleCount);// nTicket.ToString() + " " + m_bDoubleCount.ToString()+ "배!!";
            m_kGold.text = string.Format("{0}", nGold*m_bDoubleCount);//nGold.ToString()+" " + m_bDoubleCount.ToString()+ "배!!";
            StartCoroutine("IRun2");
            m_kADText.text = string.Format("({0}/5)", numberCount);

        }
        else
        {
            m_KADText2.text = "";
            m_KADText3.text = "";
            m_kADText.text = "(0/5)";
            ui1.SetActive(false);
            ui2.SetActive(false);
            m_kTicketCount.text = nTicket.ToString();
            m_kGold.text = nGold.ToString();

        }

        base.UpdateData(bselect);
    }


    private void Start()
    {
        m_gPlnet.SetActive(false);
    }
    void RewardFunc()
    {
        CWArrayManager.PlanetData kPlanetData = CWArrayManager.Instance.GetPlanetData(CWHeroManager.Instance.m_nPlanetID);
        int nGold = kPlanetData.m_nGold;
        int nTicket = kPlanetData.m_nTicket;

        nGold *= m_bDoubleCount;
        nTicket *= m_bDoubleCount;
        m_kTicketCount.text = nTicket.ToString();
        m_kGold.text = nGold.ToString();
        CWSocketManager.Instance.UseCoin2(COIN.TICKET, nTicket, COIN.GOLD, nGold, CWSocketManager.UseCoin_ResultFuc, "UseCoin_ResultFuc");

    }
    public override void Open()
    {

        CWGlobal.g_GameStop = true;
        m_bDoubleCount = 1;
        numberCount = 0;

        m_gRewardAD.SetActive(true);
        m_ADBtn = m_gRewardAD.GetComponentInChildren<CWButton>(); ;//.GetComponent<CWButton>().SetDisable(false, "최대치 입니다!");
        m_ADBtn.SetDisable(false, "최대치 입니다!");

        Image ii = m_tParming.GetComponent<Image>();
        ii.sprite = m_kSprite[0];
        m_gPlnet.SetActive(true);
        for (int i=0;i< m_gPlnet.transform.childCount;i++)
        {
            m_gPlnet.transform.GetChild(i).gameObject.SetActive(true);
            PlanetCheckslot ps = m_gPlnet.transform.GetChild(i).GetComponent<PlanetCheckslot>();
            ps.m_fEffect.SetActive(false);

        }

        

        StartCoroutine("IRun");

        


        base.Open();
       
    }
    public void Show( CBClose kClose1)
    {

        GamePlay.Instance.SetShow(false);

        m_CBClose1 = kClose1;
        Open();
        if (CWHeroManager.Instance.IsVictoryPlanet(Space_Map.Instance.GetPlanetID()))
        {
            
            
            CloseFuction = VictoryPlanetDlg.Instance.Open;
            if(Space_Map.Instance.GetPlanetID()==1)
            {
                CWQuestManager.Instance.CheckUpdateData(51, 1);//
                CWSocketManager.Instance.UpdateDayLog(DAYLOG.Storycount1);
            }
            if (Space_Map.Instance.GetPlanetID() == 2)
            {
                CWQuestManager.Instance.CheckUpdateData(52, 1);//
                CWSocketManager.Instance.UpdateDayLog(DAYLOG.Storycount2);
            }
            if (Space_Map.Instance.GetPlanetID() == 3)
            {
                CWQuestManager.Instance.CheckUpdateData(53, 1);//
                CWSocketManager.Instance.UpdateDayLog(DAYLOG.Storycount3);
            }
            if (Space_Map.Instance.GetPlanetID() == 4)
            {
                CWQuestManager.Instance.CheckUpdateData(54, 1);//
                CWSocketManager.Instance.UpdateDayLog(DAYLOG.Storycount4);
            }
            if (Space_Map.Instance.GetPlanetID() == 5)
            {
                CWQuestManager.Instance.CheckUpdateData(55, 1);//
                CWSocketManager.Instance.UpdateDayLog(DAYLOG.Storycount5);
            }
            if (Space_Map.Instance.GetPlanetID() == 6)
            {
                CWQuestManager.Instance.CheckUpdateData(56, 1);//
                CWSocketManager.Instance.UpdateDayLog(DAYLOG.Storycount6);
            }



            m_bVictory = true;
            


        }
        else
        {
            m_bVictory = false;
            

        }

        int num= Space_Map.Instance.GetStageID();
        if(num==1)
        {
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.FirstStory1);
        }
        if (num == 2)
        {
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.FirstStory2);
        }
        if (num == 3)
        {
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.FirstStory3);
        }
        if (num == 4)
        {
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.FirstStory4);
        }
        if (num == 5)
        {
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.FirstStory5);
        }



    }
    public override void Close()
    {
        base.Close();
        


        int acnt = CWHeroManager.Instance.GetEndTaskPlanet(Space_Map.Instance.GetPlanetID());
        if(acnt==6)
        {
          //  CWQuestManager.Instance.CheckUpdateData(8, 1);//행성정복
            
        }
        m_gPlnet.SetActive(false);

        Image ii=m_tParming.GetComponent<Image>();
        ii.sprite = m_kSprite[0];
        // CWADManager.Instance.Show();
        CWGlobal.g_GameStop = false;
    }
    public override void OnExit()
    {
        base.OnExit();

        RewardFunc();
        GamePlay.Instance.SetShow(false);
        if(m_bVictory)
        {
            return;
        }
        GamePlay.Instance.Close();
      //  Space_Map.Instance.Show();
        
    }
   
    public void OnParming()
    {
        CloseFuction = m_CBClose1;
        Close();
        GamePlay.Instance.SetShow(true);
        CWBgmManager.Instance.PlayDigg();
        RewardFunc();
    }
   // 광고 보상
    public void OnDoubleAD()
    {
        CWADManager.Instance.RewardShow(() => {

            m_bDoubleCount *= 2;
            AnalyticsLog.Print("ADLog", "DoubleAD", m_bDoubleCount.ToString());
            UpdateData();
            if(m_bDoubleCount>16)
            {
                //m_gRewardAD.GetComponent<CWButton>().SetDisable(true, "최대치 입니다!");
                m_ADBtn.SetDisable(true, "최대치 입니다!");
            }
            
        });
        
    }
    IEnumerator IRun()
    {

        foreach(var v in m_gImage1)
        {
            v.SetActive(false);
        }
        foreach(var v in m_gImage2)
        {
            v.SetActive(false);
        }


        yield return new WaitForSeconds(0.5f);
        m_tVictoryText.DOScale(4,0).OnComplete(()=> {

            m_tVictoryText.DOScale(1, 0.3f);
        });
        yield return new WaitForSeconds(1f);


        for (int i = 0; i < m_gPlnet.transform.childCount; i++)
        {
            int nStage = (CWHeroManager.Instance.m_nPlanetID - 1) * 6 + i + 1;
            if(CWHeroManager.Instance.IsEndTask(nStage))
            {
                m_gImage1[i].SetActive(true);
                m_gImage2[i].SetActive(false);
            }
            else
            {
                m_gImage1[i].SetActive(false);
                m_gImage2[i].SetActive(true);

            }
        }

        for (int i = 0; i < m_gPlnet.transform.childCount; i++)
        {
            int nStage = (CWHeroManager.Instance.m_nPlanetID-1)*6+i+1;
            //if(CWHeroManager.Instance.IsEndTask(nStage))
            if(Space_Map.Instance.GetStageID()==nStage)
            {
                PlanetCheckslot ps1 = m_gPlnet.transform.GetChild(i).GetComponent<PlanetCheckslot>();
                ps1.m_fEffect.SetActive(true);
                CWResourceManager.Instance.PlaySound("drum1");
                break;
            }
        }
        yield return new WaitForSeconds(0.5f);



        m_tParming.DOScaleX(0, 0.4f).OnComplete(() => {
            Image ii = m_tParming.GetComponent<Image>();
            ii.sprite = m_kSprite[1];
            m_tParming.DOScaleX(1, 0.4f);
        });

        yield return new WaitForSeconds(0.5f);

       m_kTicket.transform.DOScale(4, 0).OnComplete(() => {
            m_kTicket.transform.DOScale(1, 0.3f);
            CWResourceManager.Instance.PlaySound("drum1");
        });
        yield return new WaitForSeconds(0.5f);

        m_kGift.transform.DOScale(4, 0).OnComplete(() => {
            m_kGift.transform.DOScale(1, 0.3f);
            CWResourceManager.Instance.PlaySound("drum1");
        });
        yield return new WaitForSeconds(1f);


    }

    IEnumerator IRun2()
    {
        ui1.transform.DOScale(2, 0).OnComplete(() => {
           ui1.transform.DOScale(1, 0.3f);
      
        });
        yield return new WaitForSeconds(0.5f);

        ui2.transform.DOScale(2, 0).OnComplete(() => {
            ui2.transform.DOScale(1, 0.3f);

        });
        yield return new WaitForSeconds(0.5f);
        m_kTicket.transform.DOScale(4, 0).OnComplete(() => {
            m_kTicket.transform.DOScale(1, 0.3f);
            CWResourceManager.Instance.PlaySound("drum1");
        });
        yield return new WaitForSeconds(0.5f);

        m_kGift.transform.DOScale(4, 0).OnComplete(() => {
            m_kGift.transform.DOScale(1, 0.3f);
            CWResourceManager.Instance.PlaySound("drum1");
        });
        yield return new WaitForSeconds(1f);
    }
}
