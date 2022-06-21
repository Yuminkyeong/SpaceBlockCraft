
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;
using CWStruct;
using System;
// 모든 게임의 변수를 UI로 표현하는 개념
// 사용되는 모든 문장을 외어야 한다

public class ValueUI : MonoBehaviour
{
    static public float g_fStartTimer=0;
    static public float g_fLifeTimer=0; // 정해진시간

    static public string g_szGoodsName = "";
    static public string g_szGoodsHelp = "";
    static public int g_nCoinType = 0;
    static public string g_szPrice="";


    static public GITEMDATA g_kSelectItemData=new GITEMDATA();
    

    public string m_szValues;
    public string m_szFormat;
    public string m_szMaxValues; // 최대치가 있다

    float m_fDelay = 0.3f;



    public enum UITYPE {AUTO, TEXT,SLIDER,FACE,TEXTURE,ICON ,SPRITE, SPRITESLIDER,ITEMSLOT,SHOWHIDE, INC_NUMBER };// INC_NUMBER : 증가하는 숫자
    public UITYPE m_Type = UITYPE.AUTO;

    Text m_kText;
    Slider m_kSlider;
    RawImage m_kRawImage;
    Image m_kSprite;

    ItemInfoSlot m_kItemInfo;


    int m_nTempValue = 0;
    int m_nTempMaxValue = 0;
    bool m_bTempRun=false;
    private void Awake()
    {

        m_kItemInfo = GetComponent<ItemInfoSlot>();
        if(m_kItemInfo)
        {
            if (m_Type == UITYPE.AUTO)
                m_Type = UITYPE.ITEMSLOT;

        }

        m_kText = GetComponent<Text>();
        if (m_kText != null)
        {
            if(m_Type==UITYPE.AUTO)
                m_Type = UITYPE.TEXT;
        }
        m_kSlider = GetComponent<Slider>();
        if (m_kSlider != null)
        {
            if (m_Type == UITYPE.AUTO)
                m_Type = UITYPE.SLIDER;
        }
        m_kRawImage = GetComponent<RawImage>();
        if (m_kRawImage != null)
        {
            if (m_Type == UITYPE.AUTO)
                m_Type = UITYPE.TEXTURE;
        }
        m_kSprite = GetComponent<Image>();
        if(m_kSprite!=null)
        {
            if (m_Type == UITYPE.AUTO)
            {
                
                if(m_kSprite.type==Image.Type.Filled)
                {
                    m_Type = UITYPE.SPRITESLIDER;
                }
                else
                {
                    m_Type = UITYPE.SPRITE;
                }

            }
                

        }


    }
    private void OnEnable()
    {
        StopCoroutine("IRun");
        StartCoroutine("IRun");
     
    }
    private void OnDisable()
    {
        EndTask();
    }

    string GetValueString(string szValue)
    {
      
        if(szValue == "도움패키지시간")
        {
            if (CWPlayerPrefs.HasKey("HelpPackage"))
            {
                string strdate = CWPlayerPrefs.GetString("HelpPackage");
                CWGlobal.g_kHelpPackageDate = DateTime.Parse(strdate);
                TimeSpan ss = CWGlobal.g_kHelpPackageDate - DateTime.Now;
                if (ss.TotalSeconds < 1)
                {
                    // 종료
                    
                    CWPlayerPrefs.SetInt("Use", 1);
                    return "";
                }
                return CWLib.GetDateStringbyTime((int)ss.TotalSeconds);
            }

            return "";
        }
        //나의승률
        if (szValue == "나의승률")
        {
            if (CWHeroManager.Instance.m_nPVPTotal > 0)
            {
                float rate = (CWHeroManager.Instance.m_nPVPWin / (float)CWHeroManager.Instance.m_nPVPTotal) * 100f;
                return string.Format("{0}%", (int)rate);
            }
            else return "";
             

        }
        if (szValue == "나의전체승률")
        {
            if (CWHeroManager.Instance.m_nPVPTotal > 0)
            {
                return string.Format("총 {0} 판 중", CWHeroManager.Instance.m_nPVPTotal);
            }
            else return "";


        }
        if (szValue == "나의승률표시")
        {
            if (CWHeroManager.Instance.m_nPVPTotal > 0)
            {
                return string.Format("{0}승 {1}패", CWHeroManager.Instance.m_nPVPWin, CWHeroManager.Instance.m_nPVPTotal- CWHeroManager.Instance.m_nPVPWin);
            }
            else return "";


        }

        //즉시수리가격
        if (szValue == "즉시수리가격")
        {

            return CWGlobal.REPAIR_PRICE.ToString();
        }

        if (szValue== "출석보상이 있는가?")
        {
            foreach(var v in CWHeroManager.Instance.m_kDailyList)
            {
                if(v==1)
                {
                    return "1";
                }
            }
            return "0";
        }
        if(szValue== "캐릭터가격")
        {

            return CWGlobal.CHAR_PRICE.ToString();
        }
        if (szValue== "출석월")
        {

            DateTime tt = DateTime.Now;
            return string.Format("{0:00}", tt.Month);
        }

        if(szValue== "내캐릭터")
        {

            return string.Format("Char_{0}",CWHeroManager.Instance.m_nCharNumber);
        }
        if(szValue== "현재슬롯")
        {

        }
        if (szValue == "현재행성")
        {
            return CWGlobal.GetPlanetName(Space_Map.Instance.GetStageID());
        }
        if (szValue== "이전행성")
        {

            
            return CWGlobal.GetGradestring(Space_Map.Instance.GetPlanetID()-1); 
        }
        if (szValue == "다음행성")
        {
            return CWGlobal.GetGradestring(Space_Map.Instance.GetPlanetID()+1);
        }

       

        if (szValue== "미션 티켓 개수")
        {

            return "X" +  CWGlobal.MISSIONTICKET.ToString();
        }
        if(szValue== "승급남은시간")
        {
            
            TimeSpan ts = CWGlobal.RANKRESETTIME -DateTime.Now ;
            int nSec =  (int)ts.TotalSeconds;
            if (nSec <= 0)
            {
                // 리셋개념
                return "";
            }
            int nVal1 = nSec / 3600;
            int nVal2 = (nSec / 60) % 60;
            int nVal3 = nSec % 60;
            
            return string.Format("{0:00}:{1:00}:{2:00}", nVal1, nVal2, nVal3);


        }
        if(szValue=="다음공격력업그레이드가격")
        {
            return CWHeroManager.Instance.NextUpgradeGold.ToString();
        }
        
        if(szValue== "잔여블록")
        {

            int nStage = Space_Map.Instance.GetStageID();
            float fRate =1- CWHeroManager.Instance.GetStageRate(nStage);
            return string.Format(CWLocalization.Instance.GetLanguage("{0:0.00}%"), (fRate*100));

        }

        if (szValue == "일일미션티켓보상")
        {
            return CWGlobal.DAYMISSIONTICKET.ToString();
        }



        if (szValue == "일일미션보상")
        {
            return CWGlobal.DAYMISSION.ToString();
        }


        if (szValue == "일일상점리셋가격")
        {
            return CWGlobal.TODAY_RESET.ToString();
        }
        if (szValue == "시즌기간")
        {
            return string.Format("{0}~{1}",CWGlobal.EVENTSEASON1, CWGlobal.EVENTSEASON2);
        }
        //시즌종료타임
        if (szValue == "시즌종료타임")
        {
            if(CWLib.IsString(CWGlobal.EVENTSEASON2))
            {
                DateTime dt = DateTime.Parse(CWGlobal.EVENTSEASON2);
                TimeSpan ss = dt - DateTime.Now;
                return CWLib.GetDateString((int)ss.TotalSeconds);

            }
            return "";
        }

        if (szValue=="정복회수")
        {
            int acnt = CWHeroManager.Instance.GetEndTaskPlanet(Space_Map.Instance.GetPlanetID());

            return acnt.ToString();
        }

        if (szValue == "메일이있는가?")
        {
            if (CWGlobal.g_bIsMail) return "1";
            return "0";
        }
        if (szValue == "강화할수있는가?")
        {

            if(Check_Enchant())
            {
                return "1";
            }
            return "0";
        }
        if (szValue == "블록교환가능한가?")
        {
            if (CWInvenManager.Instance.IsChangeBlock()) return "1";
            return "0";
        }
        if (szValue == "비행기창 알람")
        {
            if (CWInvenManager.Instance.IsChangeBlock()) return "1";
            if (Check_Enchant())
            {
                return "1";
            }
            return "0";

        }

        if (szValue== "인벤에 블록이 없다.")
        {
            if (CWInvenManager.Instance.IsHaveItem("shipblock")) return "0";
            return "1";
        }
        if(szValue =="게임이처음인가?")
        {
            if(CWGlobal.g_bGameBegin)
            {
                return "1";
            }
            return "0";
        }

        if (szValue == "퀘스트 보상있음")
        {

            if(CWQuestManager.Instance.IsHaveReward())
            {
                return "1";
            }
            return "0";

        }
       

      

        if (szValue == "체크변수활성화1")
        {
            if(CWGlobal.g_bCheckValue1)
            {
                CWGlobal.g_bCheckValue1 = false;
                return "1";
            }
            return "0";
        }
        if (szValue == "체크변수활성화2")
        {
            if (CWGlobal.g_bCheckValue2)
            {
                CWGlobal.g_bCheckValue2 = false;
                return "1";
            }
            return "0";
        }
        if (szValue == "체크변수활성화3")
        {
            if (CWGlobal.g_bCheckValue3)
            {
                CWGlobal.g_bCheckValue3 = false;
                return "1";
            }
            return "0";
        }

        if(szValue== "계속하기 멀티")
        {
            return CWGlobal.MULTI_CONTINUE.ToString();
        }
      


        if (szValue == "입장권개수")// 
        {
            CWArrayManager.PlanetData gPlanetData = CWArrayManager.Instance.GetPlanetData(Space_Map.Instance.GetPlanetID());
            return "x " + gPlanetData.m_nTicket.ToString();
        }
        if (szValue == "행성보기인가")// 
        {
            if (Space_Map.Instance.m_nType == 1||Space_Map.Instance.m_nType == 2 || Space_Map.Instance.m_nType == 3 || Space_Map.Instance.m_nType == 4) return "0";
            else return "1";
        }

        
        // 멀티행성 제외
        if (szValue == "행성보기")// 
        {
            if (Space_Map.Instance.m_nType == 0||Space_Map.Instance.m_nType == 1|| Space_Map.Instance.m_nType == 2) return "1";
            else return "0";
        }
        if (szValue == "멀티행성보기")// 
        {
            if (Space_Map.Instance.m_nType == 4) return "1";
            else return "0";
        }
        if (szValue == "은하보기")// 
        {
            if (Space_Map.Instance.m_nType == 2 || Space_Map.Instance.m_nType == 3) return "1";
            else return "0";
        }


        if (szValue == "정복했는가")// 
        {
            if (CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID())) return "1";
            return "0";
        }

        if(szValue =="광고보상입장권")
        {
            return CWGlobal.REWARDTICKET.ToString();
        }

      
       
        if (szValue == "보석")
        {

            int nPlanet = (CWHeroManager.Instance.m_nPlanetID - 1) % 6 + 1;
            int nID = CWTableManager.Instance.GetTableInt("BAC_아이템 - 보석", "ID", nPlanet);
            if (m_Type == UITYPE.TEXT)
            {
                int nvalue= CWInvenManager.Instance.GetItemTotalCount(nID);
                return nvalue.ToString();

            }
            if (m_Type == UITYPE.SPRITE)
            {

                return string.Format("gem_{0}", nPlanet);
            }
            
        }
     

        if (szValue== "UPLOADFILE")
        {
            return CWGlobal.UPLOADCASH.ToString();
        }
        if(szValue== "OnlyBuyDesign")
        {
            return CWGlobal.OnlyBuyDesign.ToString();
        }

        if (szValue == "남은시간")
        {
            float ff = g_fLifeTimer- (Time.time - g_fStartTimer);
            return CWLib.GetTimeString(ff);
        }

        if (szValue == "타이머")
        {
            float ff = Time.time- g_fStartTimer;
            return CWLib.GetTimeString(ff);
        }

        if (szValue == "비행기레벨")//
        {
            return string.Format("{0}", CWHeroManager.Instance.NSlotLevel);//

        }

        if (szValue == "에디터 블록수")//
        {
            
            int tcnt = GameEdit.Instance.m_kAirObject.NBlockCount;
            return string.Format("{0}/{1}", tcnt, GameEdit.Instance.m_nAirBlockCount);//

        }
        if (szValue == "에디터 HP")//
        {
            return string.Format("{0}", GameEdit.Instance.m_kAirObject.GetMaxHP());//

        }
        if (szValue == "에디터 Damage")//
        {
            return string.Format("{0}", GameEdit.Instance.m_kAirObject.GetDamage());//
        }
        if (szValue == "에디터 Speed")//
        {
            return string.Format("{0}", GameEdit.Instance.m_kAirObject.GetSpeed());//

        }



        if (szValue == "주인공 블록수")//
        {
            int tcnt =CWHero.Instance.NBlockCount;
            return string.Format("{0}/{1}", tcnt, CWHeroManager.Instance.m_nAirBlockCount);//

        }
        if (szValue == "주인공 HP")//
        {
            return string.Format("{0}",CWHero.Instance.GetMaxHP());//

        }
        if (szValue == "주인공 Damage")//
        {
            return string.Format("{0}", CWHero.Instance.GetDamage());//
        }
        if (szValue == "광고 Damage")//
        {
            return string.Format("{0}", CWHero.Instance.GetDamage()*2);//
        }
        if (szValue == "주인공 내구력")//
        {
            return string.Format("{0}", CWHeroManager.Instance.m_nRepairValue);//

        }

        if (szValue == "주인공 Speed")//
        {
            return string.Format("{0}", CWHero.Instance.GetSpeed());//

        }


        if(szValue=="구매상품")
        {
            return CWLocalization.Instance.GetLanguage(ValueUI.g_szGoodsName);
        }
        if (szValue == "구매코인종류")
        {

            return CWGlobal.GetCoinSpriteStr(g_nCoinType);

        }
        if (szValue == "구매가격")
        {
            return g_szPrice.ToString();
        }

        if (szValue == "구매상품설명")
        {

            return CWLocalization.Instance.GetLanguage(g_szGoodsHelp);
        }

        if (szValue == "SelectItem_HP")
        {
            
            return g_kSelectItemData.hp.ToString();
        }
        if (szValue == "SelectItem_Price")
        {
            
            return g_kSelectItemData.pricesell.ToString();
        }
     

        if (szValue == "SelectItem_info")
        {
            
            return g_kSelectItemData.szInfo;
        }

        if (szValue == "SelectItem_Name")
        {
            
            return g_kSelectItemData.m_szTitle;
        }
        if (szValue == "SelectItem_Deamge")
        {
            
            WEAPON ws = CWArrayManager.Instance.GetWeapon(g_kSelectItemData.nID);
            return ws.Damage.ToString();
        }
        if (szValue == "SelectItem_Speed")
        {
            
            BUSTER bs =CWArrayManager.Instance.GetBuster(g_kSelectItemData.nID);
            return bs.Speed.ToString();
        }
    
        if (szValue=="SelectItem")
        {
            return g_kSelectItemData.nID.ToString();
        }

        if (szValue == "SelectGrade")//선택한 아이템 등급 
        {
        
            return CWGlobal.GetGradeItemName(g_kSelectItemData.level);
        }
      

        if (szValue == "SelectItemCount")
        {
            int ncnt= CWInvenManager.Instance.GetItemTotalCount(g_kSelectItemData.nID);
            return ncnt.ToString();
        }

        if (szValue == "Target_HP")
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWPower kPower = CWObject.g_kSelectObject.KPower;
            return kPower.GetHP().ToString();
        }

        if (szValue == "Target_Energy")
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWPower kPower = CWObject.g_kSelectObject.KPower;
            return kPower.GetEnergy().ToString();
        }
      
     
        if (szValue == "Leveltext")//레벨
        {
            return "Lv. "+ CWHero.Instance.NLevel.ToString();
        }
        if (szValue == "Level")// 레벨
        {
            return CWHero.Instance.NLevel.ToString();
        }




        if (szValue == "Select_Buster")
        {
            return "+";// 1씩 증가하는 속도 
        }


      
      
        if (szValue == "Select_WeaponDamage")
        {
            WEAPON ws = CWArrayManager.Instance.GetWeapon(g_kSelectItemData.nID);
            return ws.Damage.ToString();
        }



        if (szValue == "Select_Leveltext")//레벨
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;
            return "Lv. " + aa.NLevel.ToString();
        }
        if (szValue == "Select_Level")// 레벨
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;
            return aa.NLevel.ToString();
        }
        if (szValue == "Select_Damage")
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;
            
            return aa.GetDamage().ToString();
        }

        if (szValue == "Select_HP")
        {
            if (CWObject.g_kSelectObject == null) return "";

            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;

            return string.Format("{0}", aa.GetHP());
        }
        if (szValue == "Select_Airlevel")// 선택한 비행기 레벨
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;

            int level = aa.NBlockCount - 99;
            if (level <= 0) level = 1;
            return string.Format("{0}", level);

        }

        if (szValue == "Select_BlockCount")
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;
            
            return string.Format("{0}", aa.NBlockCount);
            
        }
        // 비행기 가격
        if (szValue == "Select_AirPrice")
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;

            
            return string.Format("{0}", aa.GetPrice());

        }

        if (szValue == "Select_title")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                return ScrollListUI.g_kSelectScrol.GetSelectValue("제목");
            }
        }
        if (szValue == "Select_help")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                return ScrollListUI.g_kSelectScrol.GetSelectValue("설명");
            }
        }


        if (szValue == "Select_Ranking")
        {
            //if (ScrollListUI.g_kSelectScrol)
            //{

            //    string szValues=  CWLocalization.Instance.GetLanguage("순위 #{0}");
            //    return string.Format(szValues, ScrollListUI.g_kSelectScrol.GetSelectValue("Ranking")); 
            //}

            int nRanking = ScrollListUI.g_kSelectScrol.GetSelectValueInt("Ranking");
            if (m_Type == UITYPE.SPRITE)
            {
                if (nRanking == 1)
                {
                    return "icon_rank_gold";
                }
                if (nRanking == 2)
                {
                    return "icon_rank_silver";
                }
                if (nRanking == 3)
                {
                    return "icon_rank_bronze";
                }
                return "icon_rank_default";
            }
            if (nRanking == 1)
            {
                return "";
            }
            if (nRanking == 2)
            {
                return "";
            }
            if (nRanking == 3)
            {
                return "";
            }

            return nRanking.ToString();


        }

    

        if (szValue == "Select_UserName")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                return ScrollListUI.g_kSelectScrol.GetSelectValue("Name");
            }
        }
        if (szValue == "Select_Score")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                return ScrollListUI.g_kSelectScrol.GetSelectValue("RankPoint");
            }
        }
        if (szValue == "Select_CoinType")
        {
            if (m_Type == UITYPE.SPRITE)
            {
                if (ScrollListUI.g_kSelectScrol)
                {
                    int nn = ScrollListUI.g_kSelectScrol.GetSelectValueInt("코인유형");//
                    return CWGlobal.GetCoinSpriteStr(nn);
                }
            }
        }

        if (szValue == "Select_Tip")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                return ScrollListUI.g_kSelectScrol.GetSelectValue("Tip");
            }
        }

        if (szValue == "Select_UserLike")
        {
            if(ScrollListUI.g_kSelectScrol)
            {
             return   ScrollListUI.g_kSelectScrol.GetSelectValue("Like");
            }
        }
        // 차액
     
            // 선택한 비행기 레벨
        if (szValue == "Select_BlockLv")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                return ScrollListUI.g_kSelectScrol.GetSelectValue("blocklevel");
            }
        }


        if (szValue == "Select_Price")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                return ScrollListUI.g_kSelectScrol.GetSelectValue("Price");
            }
        }
      
       


        if (szValue == "Select_Grade")
        {
            if (ScrollListUI.g_kSelectScrol)
            {

                int p=ScrollListUI.g_kSelectScrol.GetSelectValueInt("RankPoint");// 대소문자 무시 
                int nGrade = CWGlobal.GetGrade(p);
                if (m_Type== UITYPE.TEXT)
                {
                    return CWGlobal.GetGradestring(nGrade);
                }
                if (m_Type == UITYPE.TEXTURE)
                {
                    return CWGlobal.GetGradeFileName(nGrade);
                }

                
            }
        }
        if (szValue == "현재행성스토리")//
        {
            int num = Space_Map.Instance.m_nPlanetNumber + 1;
            string str= CWTableManager.Instance.GetTable("블록이름,스토리 - 행성","story",num);

            int nID = CWArrayManager.Instance.GetBlockLevel(3, Space_Map.Instance.GetStageID());
            

            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nID);
            return string.Format(str, gData.m_szTitle);

        }
        if (szValue == "현재행성블록1")//
        {
            int nID = CWArrayManager.Instance.GetBlockLevel(1, Space_Map.Instance.GetStageID());
            return nID.ToString();
        }
        if (szValue == "현재행성블록2")//
        {
            int nID = CWArrayManager.Instance.GetBlockLevel(2, Space_Map.Instance.GetStageID()); ;
            return nID.ToString();
        }
        if (szValue == "현재행성블록3")//
        {
            int nID = CWArrayManager.Instance.GetBlockLevel(3, Space_Map.Instance.GetStageID()) ;
            return nID.ToString();
        }

        if (szValue == "현재행성블록이름2")//
        {
            int nID = CWArrayManager.Instance.GetBlockLevel(2, Space_Map.Instance.GetStageID());
            return CWArrayManager.Instance.GetItemData(nID).m_szTitle;
        }
        if (szValue == "현재행성블록이름3")//
        {
            int nID = CWArrayManager.Instance.GetBlockLevel(3, Space_Map.Instance.GetStageID()); ;
            return CWArrayManager.Instance.GetItemData(nID).m_szTitle;
        }
        if (szValue == "블록레벨2")//
        {
            
            int nID = CWArrayManager.Instance.GetBlockLevel(2, Space_Map.Instance.GetStageID());
            return string.Format("LV. "+ CWArrayManager.Instance.GetItemData(nID).level);
        }
        if (szValue == "블록레벨3")//
        {

            int nID = CWArrayManager.Instance.GetBlockLevel(3, Space_Map.Instance.GetStageID());
            return string.Format("LV. " + CWArrayManager.Instance.GetItemData(nID).level);
        }
        if (szValue == "블록hp2")//
        {
            int nID = CWArrayManager.Instance.GetBlockLevel(2, Space_Map.Instance.GetStageID()); ;
            return string.Format("HP +" + CWArrayManager.Instance.GetItemData(nID).hp);
        }
        if (szValue == "블록hp3")//
        {
            int nID = CWArrayManager.Instance.GetBlockLevel(3, Space_Map.Instance.GetStageID()); ;
            return string.Format("HP +" + CWArrayManager.Instance.GetItemData(nID).hp);
        }

        if (szValue == "남아있는블록2")//
        {
            int nStage = Space_Map.Instance.GetStageID();

            CWArrayManager.StageData kk= CWArrayManager.Instance.GetStageData(nStage);
            int nCount = CWHeroManager.Instance.GetRes2(nStage); //PlayerPrefs.GetInt(szAddCount);
            int nMaxCount = kk.m_nCount2;
            return nCount.ToString() + "/" + nMaxCount.ToString();
        }
        if (szValue == "남아있는블록3")//
        {
            int nStage = Space_Map.Instance.GetStageID();
            CWArrayManager.StageData kk = CWArrayManager.Instance.GetStageData(nStage);
            int nCount = CWHeroManager.Instance.GetRes3(nStage); //PlayerPrefs.GetInt(szAddCount);
            int nMaxCount = kk.m_nCount3;
            return nCount.ToString() + "/" + nMaxCount.ToString();
        }
        
        //남아있는블록2
        //남아있는블록3
        //잔여블록
        if (szValue == "현재지역이름")//
        {
            return Space_Map.Instance.CurrentAreaName();
        }

        
        if (szValue == "현재행성이름")
        {
            return Space_Map.Instance.GetCurrentPlanet();
        }
        if (szValue == "현재몇번째면")
        {
            return Space_Map.Instance.GetCurrentArea();
        }

        if (szValue == "현재행성별칭")
        {
            int num = Space_Map.Instance.GetPlanetID();
            return CWTableManager.Instance.GetTable("블록이름,스토리 - 행성", "name", num);///
        }

        if (szValue == "현재스테이지")
        {
            int num = Space_Map.Instance.GetPlanetID();
            return CWTableManager.Instance.GetTable("블록이름,스토리 - 행성", "stage", num);///
        }


        if (szValue == "Select_Leveltext_Next")
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWAirObject aa = (CWAirObject)CWObject.g_kSelectObject;
            return "Lv. " + (aa.NLevel+1).ToString();
        }
      



        // HP는 2가지 10/100 rate 두개 존재 슬라이더는 rate text 10/100 로 통일
        if (szValue=="HP")
        {
            CWPower kPower = CWHero.Instance.KPower;
            if (m_Type == UITYPE.SLIDER)
            {
                return kPower.FhpRate.ToString();
            }
            return string.Format("{0}/{1}", kPower.GetHP(), kPower.m_nHp);
        }
        if(szValue=="Target_HP2")
        {
            
            if (CWObject.g_kSelectObject == null) return "";

            CWPower kPower = CWObject.g_kSelectObject.KPower;
            return string.Format("{0}/{1}", kPower.GetHP(), kPower.m_nHp);
        }

        if (szValue == "Energy")
        {
            CWPower kPower = CWHero.Instance.KPower;
            if (m_Type == UITYPE.SLIDER)
            {
                return kPower.FEnRate.ToString();
            }
            return string.Format("{0}/{1}", (int)kPower.GetEnergy(),(int) kPower.m_fEnergy);
        }
        if (szValue == "Target_Energy")
        {
            if (CWObject.g_kSelectObject == null) return "";
            CWPower kPower = CWObject.g_kSelectObject.KPower;
            return string.Format("{0}/{1}", (int)kPower.GetEnergy(), (int)kPower.m_fEnergy);

        }
       
        if (szValue=="Gold")
        {
            if (m_Type == UITYPE.SPRITE)
            {
                return "gold";
            }

            int val = CWCoinManager.Instance.GetCoin(COIN.GOLD);
            return val.ToString();
        }
        if (szValue == "Gem")
        {
            if (m_Type == UITYPE.SPRITE)
            {
                return "gem";
            }

            int val = CWCoinManager.Instance.GetCoin(COIN.GEM);
            return val.ToString();
        }

        if (szValue == "입장권")
        {
            if (m_Type == UITYPE.SPRITE)
            {
                return "ticket";
            }
            return CWCoinManager.Instance.GetCoin(COIN.TICKET).ToString();
        }
        if (szValue == "PVP_TICKET")
        {
            return "x" + CWGlobal.PVP_TICKET.ToString();
        }
        // 다음 업그레이드 가격을 줌!!
        if (szValue == "PVP_GOLD")
        {
            if(CWGlobal.g_bADDouble)
            {
                return "+" + CWGlobal.PVP_GOLD.ToString() + "X 2배"; 
            }
            return "+" + CWGlobal.PVP_GOLD.ToString();
        }

        if (szValue == "PVPSCORE")
        {
            if (CWGlobal.g_bADDouble)
            {
                return "+" + CWGlobal.PVP_SCORE.ToString() + "X 2배";
            }
            return "+" + CWGlobal.PVP_SCORE.ToString();
        }
        if (szValue == "-PVPSCORE")
        {
            return "-"+(CWGlobal.PVP_SCORE-1).ToString();
        }
        if (szValue == "PVP점수획득")
        {
            return string.Format(CWLocalization.Instance.GetLanguage("{0}점을 획득하였습니다!!"), CWGlobal.PVP_SCORE);
        }
        if (szValue == "PVP점수뺏김")
        {
            return string.Format(CWLocalization.Instance.GetLanguage("{0}점을 잃어버렸습니다"), CWGlobal.PVP_SCORE-1);
        }
        if (szValue == "name")
        {
            return CWHero.Instance.name;
        }
        if (szValue == "Target_name")
        {
            if (CWObject.g_kSelectObject == null) return "";
            return CWObject.g_kSelectObject.name;
        }
        if (szValue == "PVPTarget_name")
        {
            return PVPDlg.Instance.m_szTargetname;
        }

        if (szValue == "Face")
        {

            return CWHero.Instance.m_szFace;
        }
        if (szValue == "Target_Face")
        {
            if (CWObject.g_kSelectObject == null) return "";
            return CWObject.g_kSelectObject.m_szFace;
        }

        if (szValue == "MyLike")
        {
            return CWHeroManager.Instance.m_nLike.ToString();
        }
        if (szValue == "MyPrice")
        {
            return CWHeroManager.Instance.m_nPrice.ToString();
        }
        if (szValue == "MyPriceF")
        {
            //return CWHero.Instance.GetPricef().ToString();
            return string.Format("{0:0.00}", CWHero.Instance.GetPricef());
        }

        if (szValue == "MyScore")
        {
            return CWHeroManager.Instance.m_nRankPoint.ToString();
        }
        
        if (szValue == "나의 등수")
        {
            if(CWHeroManager.Instance.m_nRanking==0)
            {
                return "--";
            }
            return CWHeroManager.Instance.m_nRanking.ToString();
        }
        if (szValue == "MyRank")
        {

            if (m_Type == UITYPE.SPRITE)
            {
                if(CWHeroManager.Instance.m_nRanking==1)
                {
                    return "icon_rank_gold";
                }
                if (CWHeroManager.Instance.m_nRanking == 2)
                {
                    return "icon_rank_silver";
                }
                if (CWHeroManager.Instance.m_nRanking == 3)
                {
                    return "icon_rank_bronze";
                }
                return "icon_rank_default";
            }
            if (CWHeroManager.Instance.m_nRanking == 1)
            {
                return "";
            }
            if (CWHeroManager.Instance.m_nRanking == 2)
            {
                return "";
            }
            if (CWHeroManager.Instance.m_nRanking == 3)
            {
                return "";
            }

            return CWHeroManager.Instance.m_nRanking.ToString();
        }
        


        if (szValue == "Target_Score")
        {
            if (CWObject.g_kSelectObject == null) return "";
            int ncnt = CWObject.g_kSelectObject.m_nRankPoint;
            return ncnt.ToString();
        }
       
        if (szValue == "GradeName")// 계급이름
        {
            return CWGlobal.GetGradestring(CWHeroManager.Instance.m_nGrade);
        }
        if (szValue == "GradeName next")// 계급이름
        {
            return CWGlobal.GetGradestring(CWHeroManager.Instance.m_nGrade+1);
        }



        if (szValue == "Grade")
        {
            return CWGlobal.GetGradeFileName(CWHeroManager.Instance.m_nGrade);
        }
       
        if(szValue == "GradeCircle")
        {
            return CWGlobal.GetGradeCircleFileName(CWHeroManager.Instance.m_nGrade);
        }

      


        if (szValue == "Grade next")
        {
            return CWGlobal.GetGradeFileName(CWHeroManager.Instance.m_nGrade+1);
        }

        if (szValue == "Grade 1")
        {
            return CWGlobal.GetGradeFileName((CWHeroManager.Instance.m_nGrade/3)*3);
        }
        if (szValue == "Grade 2")
        {
            return CWGlobal.GetGradeFileName((CWHeroManager.Instance.m_nGrade / 3) * 3+1);
        }
        if (szValue == "Grade 3")
        {
            return CWGlobal.GetGradeFileName((CWHeroManager.Instance.m_nGrade / 3) * 3+2);
        }



        if (szValue == "Target_Grade")
        {
            if (CWObject.g_kSelectObject == null) return "";
            return CWGlobal.GetGradeFileName(CWObject.g_kSelectObject.GetGrade());
        }

        if (szValue == "멀티회수")
        {
            return CWHeroManager.Instance.MultiCount.ToString();
        }
        if (szValue == "최대멀티회수")
        {
            return CWGlobal.MULTICOUNT.ToString();
        }
        if (szValue == "멀티회수표현")
        {
            return string.Format("({0}/{1})", CWHeroManager.Instance.MultiCount, CWGlobal.MULTICOUNT);
        }

        if (szValue == "멀티입장료")
        {
            return CWHeroManager.Instance.MultiPrice.ToString();
        }
        if (szValue == "일일상점존재하나")
        {
         
            return "0";
        }
        if (szValue=="랜덤")
        {
            int RR= CWLib.ConvertInt(m_szMaxValues);
            CWLib.Random(0, RR);
            if(RR==0)
            {
                return "1";
            }
            return "0";
        }


        return "";
    }
    float GetValueRate(string szValue)//0~1으로 처리 하는 개념만
    {
        if (szValue == "남은시간")
        {
            return (Time.time - g_fStartTimer) / g_fLifeTimer;//0~1
        }



        string szval = GetValueString(szValue);
        return CWLib.ConvertFloat(szval);
    }
    bool m_bDWTFlag = false;
    void UpdateData()
    {
        if (CWArrayManager.Instance == null) return;
//        if (CWHero.Instance == null) return;
        if(CWHeroManager.Instance == null) return;
        if (m_Type == UITYPE.TEXT)
        {
            // 개념 설정
            // 쉼표등 표현이 필요한 문자는, 정수로 바꿔서 출력한다
            if(CWLib.IsString(m_szMaxValues))// 최대치가 있다면
            {
                string sz1 =  CWLocalization.Instance.GetLanguage(GetValueString(m_szValues));
                string sz2 = GetValueString(m_szMaxValues);
                m_kText.text = string.Format("{0}/{1}",sz1,sz2);
            }
            else
            {
                if (CWLib.IsString(m_szFormat))
                {
                    string sz1 = CWLocalization.Instance.GetLanguage(m_szFormat);

                    m_kText.text = string.Format(sz1, CWLocalization.Instance.GetLanguage(GetValueString(m_szValues)));
                }
                else
                {
                    m_kText.text = GetValueString(m_szValues);
                }

            }

            return;
        }
        if (m_Type == UITYPE.SLIDER)
        {
            if (CWLib.IsString(m_szMaxValues))// 최대치가 있다면
            {
                string sz1 = GetValueString(m_szValues);
                string sz2 = GetValueString(m_szMaxValues);
                float f1 = CWLib.ConvertFloat(sz1);
                float f2 = CWLib.ConvertFloat(sz2);
                
                m_kSlider.value =f1/f2;
                if (m_kSlider.value > 1) m_kSlider.value = 1;
                if (m_kSlider.value < 0) m_kSlider.value = 0;

            }
            else
            {
                m_kSlider.value = GetValueRate(m_szValues);
            }
                


        }
        if (m_Type == UITYPE.TEXTURE)
        {
            
            m_kRawImage.texture = CWResourceManager.Instance.GetTexture(GetValueString(m_szValues)); //.GetFaceTexture(GetValueString(m_szValues));
        }
        if (m_Type == UITYPE.ICON)
        {
            int nID = CWLib.ConvertInt(GetValueString(m_szValues));
            m_kRawImage.texture = CWResourceManager.Instance.GetItemIcon(nID);
        }
        if(m_Type ==UITYPE.SPRITE)
        {
            m_kSprite.sprite = CWResourceManager.Instance.GetSprite(GetValueString(m_szValues));
        }
        if (m_Type == UITYPE.SPRITESLIDER)
        {
            m_kSprite.fillAmount = GetValueRate(m_szValues);
        }
        if(m_Type==UITYPE.ITEMSLOT)
        {
            m_kItemInfo.m_nItem = CWLib.ConvertInt(GetValueString(m_szValues));
            m_kItemInfo.UpdateData();
        }
        if(m_Type==UITYPE.SHOWHIDE)
        {
            
            
            GameObject gg = CWLib.FindChild(gameObject, "visible");
            if (gg == null) return;
            int nRet = CWLib.ConvertInt(GetValueString(m_szValues));
            if(nRet==1)
            {
                if(gg.activeSelf==false)
                {
                    gg.SetActive(true);

                    if(!m_bDWTFlag)
                    {
                        gg.transform.DOScale(0, 0).OnComplete(() => {
                            gg.transform.DOScale(1, 0.2f).OnComplete(() => {

                                m_bDWTFlag = false;
                            });

                        });

                    }
                    m_bDWTFlag = true;

                }


            }
            else
            {
                if (gg.activeSelf == true)
                {
                    if(!m_bDWTFlag)
                    {
                        gg.transform.DOScale(0, 0.2f).OnComplete(() => {
                            gg.SetActive(false);
                            gg.transform.DOScale(1, 0);
                            m_bDWTFlag = false;
                        });

                    }
                    m_bDWTFlag = true;
                }

            }
        }
        if (m_Type == UITYPE.INC_NUMBER)
        {
            // 증가하는 텍스쳐
            if(m_bTempRun==false)
            {
                string szValues = GetValueString(m_szValues);
                //m_kText.text = szValues;

                int nValue = CWLib.ConvertInt(szValues);
                if (nValue==0 || nValue != m_nTempValue)
                {
                    m_nTempMaxValue = nValue;
                    m_bTempRun = true;

                   // EndTask();
                    StopCoroutine("IncRun");
                    StartCoroutine("IncRun");
                }
            }

        }

    }
    void EndTask()
    {
        if(m_bTempRun)
        {
            m_kText.text = m_nTempMaxValue.ToString();
            m_nTempValue = m_nTempMaxValue;
        }
        m_nTempMaxValue = 0;
        m_bTempRun = false;

    }
    IEnumerator IncRun()
    {
        // 0.3초 변화 
        // 맥스 시간 1초 
        // 간격 
        float fMaxTime = 0.5f;
        float fStartTime = Time.time;
        float fMaxValue = m_nTempMaxValue -m_nTempValue;
        float fStartValue = m_nTempValue;
        
        while (true)
        {
            float ftime = Time.time- fStartTime;
            if(ftime>fMaxTime)
            {
               
                break;
            }
            float fRate = ftime / fMaxTime;
            float fValue = fStartValue + fMaxValue*fRate;
            m_kText.text = ((int)fValue).ToString();

            
            yield return null;
        }
        EndTask();
        
    }
    IEnumerator IRun()
    {
        while (true)
        {
            if (CWGlobal.G_bGameStart)
            {
                UpdateData();

            }
            yield return new WaitForSeconds(m_fDelay);
        }
    }
    private void Start()
    {
        StartCoroutine("IRun");
    }

    #region 서브 함수

    // 강화 할 수 있는가?
    bool Check_Enchant()
    {
        int nGold = CWCoinManager.Instance.GetCoin(COIN.GOLD);
        if (nGold > 1000)
        {
            int Damagelevel1 = CWHeroManager.Instance.GetWeaponDamageLevel(1);

            int nGold1 = CWHeroManager.Instance.GetWeaponDamageGold(CWHero.Instance.m_nSelectWeaponType, Damagelevel1);
            if (nGold >= nGold1) return true;



        }
        return false;
    }
    #endregion
}
