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

public class WPUpgradeBoxDlg : WindowUI<WPUpgradeBoxDlg>
{
    public GameObject[] m_gEffect;


    public GameObject[] m_gResult;


    public RawImage selectWeaponImg;

    public RawImage selectWeaponImg1;
    public RawImage selectWeaponImg2;


    public Text successPercentageTxt;
    public Text weaponNameTxt;
    public Text weaponNameTxt1;
    public Text weaponNameTxt2;


    public Text originUpgradeNumTxt;
    public Text upgradeTxt;
    public Text GemText;
    public Text GoldText;

    public Text m_kCount;

    const int MAXCOUNT = 3;

    int m_nPrice;

    int m_nItem;
    int m_nLevel;
    int m_nSlotNumber;// 블록에 붙어 있는 순서 
    float m_fRate=1;
    float m_fOneRate=0.1f;//10%확률

    int m_nCount = 0;

    CWAirObject m_kAirObject;
    int m_SlotID;

    int  GetRate()
    {
        return Mathf.Min(100, (int)( (m_fRate) * 100f + ( (float)m_nCount/10)*100f  ));

        
    }
    
    void UpdateInfo()
    {
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nItem);
        m_nPrice = WeaponUpgradeDlg.Instance.GetPrice(m_nItem, m_nLevel);

        m_fRate = CWTableManager.Instance.GetTableFloat("무기업그레이드 - 시트1", "확률", m_nLevel);


        selectWeaponImg.texture = CWResourceManager.Instance.GetItemIcon(m_nItem);

        selectWeaponImg1.texture = CWResourceManager.Instance.GetItemIcon(m_nItem);
        selectWeaponImg2.texture = CWResourceManager.Instance.GetItemIcon(m_nItem);

        successPercentageTxt.text = string.Format(CWLocalization.Instance.GetLanguage("성공 확률 {0}%!"), GetRate());
        weaponNameTxt.text = gData.m_szTitle;

        weaponNameTxt1.text = gData.m_szTitle;
        weaponNameTxt2.text = gData.m_szTitle;


        m_kCount.text = string.Format("{0}/{1}",m_nCount, MAXCOUNT);

        originUpgradeNumTxt.text = string.Format(CWLocalization.Instance.GetLanguage("+{0}"),  m_nLevel.ToString());
        upgradeTxt.text = string.Format(CWLocalization.Instance.GetLanguage("1회 마다 {0}% 확률 UP"),(int)(m_fOneRate*100));
        CWGlobal.UPGRADE_GEM = int.Parse(GemText.text);
        GoldText.text = m_nPrice.ToString();




    }

    public void Show(int nItem,int nLevel,int slotnumber,int SlotID, CWAirObject kAirObject)
    {

        
        
        m_SlotID = SlotID;
        m_kAirObject =kAirObject;
        m_nItem = nItem;


        
        m_nLevel= nLevel;
        m_nSlotNumber = slotnumber;
        Open();
    }

    protected override void _Open()
    {
        foreach (var v in m_gEffect) v.gameObject.SetActive(false);
        foreach (var v in m_gResult) v.gameObject.SetActive(false);
        



        m_bExit = false;
        UpdateInfo();
        base._Open();

    }
    public override void Close()
    {
        foreach (var v in m_gEffect) v.gameObject.SetActive(false);
        foreach (var v in m_gResult) v.gameObject.SetActive(false);

        WeaponUpgradeDlg.Instance.UpdateData();
        base.Close();
    }
    bool m_bExit = false;

    public void OnUpGradeGem()
    {

        if (m_nCount >= MAXCOUNT)
        {
            NoticeMessage.Instance.Show( string.Format("{0}회가 최대입니다! ", MAXCOUNT));
            return;
        }


        CWSocketManager.Instance.UseCoinEx(COIN.GEM, -CWGlobal.UPGRADE_GEM, () =>
        {
            m_nCount++;
            UpdateInfo();
        });


    }
    public void OnUpGradeAD()
    {
       
        if (m_nCount >= MAXCOUNT)
        {
            NoticeMessage.Instance.Show(string.Format("{0}회가 최대입니다! ", MAXCOUNT));
            return;
        }
            

        CWADManager.Instance.RewardShow(()=> {

            m_nCount++;
            UpdateInfo();

            AnalyticsLog.Print("ADLog", "WeaponUpgrade", m_nCount.ToString());
        });


    }
    public void OnUpGrade()
    {
        if (m_bExit) return;
        m_bExit = true;
        CWSocketManager.Instance.UseCoinEx(COIN.GOLD, -m_nPrice, () =>
        {

            
            Result();
            
        });
    }
    void ResetPopup()
    {
        m_bExit = false;
        foreach (var v in m_gEffect) v.gameObject.SetActive(false);
    }

    IEnumerator ISuccess()
    {
        CWResourceManager.Instance.PlaySound("weapon_suc");
        m_gEffect[0].SetActive(true);
        m_gEffect[1].SetActive(false);
        yield return new WaitForSeconds(2.4f);
        m_nLevel++;
        m_kAirObject.UpGradeBlock(m_SlotID, m_nSlotNumber);
        // 현재 비행기와 같은 비행기라면, 카피를 해야 한다
        if (m_SlotID == CWHeroManager.Instance.m_nAirSlotID)
        {
            byte[] buffer = m_kAirObject.GetJSonByte();
            CWHero.Instance.CopyBuffer(buffer);
        }

        
        yield return new WaitForSeconds(0.01f);
        m_gResult[0].SetActive(true);
        m_gResult[1].SetActive(false);
        yield return new WaitForSeconds(0.01f);
        ResetPopup();
        UpdateInfo();
        Dailymission.Instance.CheckUpdate(DAYMTYPE.WEAPONUP,1);
        CWQuestManager.Instance.CheckUpdateData(5);//무기공격력강화

        if(m_nLevel==10)
        {
            CWQuestManager.Instance.CheckUpdateData(46);//무기공격력강화
        }
        if (m_nLevel == 15)
        {
            CWQuestManager.Instance.CheckUpdateData(47);//무기공격력강화
        }
        if (m_nLevel == 18)
        {
            CWQuestManager.Instance.CheckUpdateData(48);//무기공격력강화
        }
        if (m_nLevel == 19)
        {
            CWQuestManager.Instance.CheckUpdateData(49);//무기공격력강화
        }
        if (m_nLevel == 25)// 최대
        {
            CWQuestManager.Instance.CheckUpdateData(50);//무기공격력강화
        }




        WeaponUpgradeDlg.Instance.UpdateData();
        CheckMaxLevel();

    }
    private void CheckMaxLevel()
    {
        if (25 == m_nLevel)
            Close();
    }
    IEnumerator IFail()
    {
        CWResourceManager.Instance.PlaySound("weapon_fail");
        m_gEffect[1].SetActive(true);
        m_gResult[0].SetActive(false);

        yield return new WaitForSeconds(2.4f);

        m_gResult[0].SetActive(false);
        m_gResult[1].SetActive(true);
        yield return new WaitForSeconds(0.01f);
        ResetPopup();
    }



    void SuccessFunc()
    {

        StartCoroutine("ISuccess");
    }
    void FailFunc()
    {
        StartCoroutine("IFail");
        //m_gEffect[1].SetActive(true);
        //m_gResult[0].SetActive(false);
        //Invoke("Exit", 4f);
    }

    void Result()
    {
        int nRate = GetRate();
        int nn=  Random.Range(0, 100);

        if(nRate>=nn)
        {
            // 성공
            SuccessFunc();
        }
        else
        {
            FailFunc();
        }
        m_nCount = 0;



    }




}
