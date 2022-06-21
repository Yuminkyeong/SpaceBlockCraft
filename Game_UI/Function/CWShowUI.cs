using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;

public class CWShowUI : MonoBehaviour
{
    #region 전역 변수
    public static bool[] g_bValueList = new bool[100];


    #endregion

    public int m_nValueList=0;

    public int m_nTutoNumber=0;

    public GameObject[] m_gShowtrue;//true 일때 보임
    public GameObject[] m_gShowfalse;// false 일때 보임

    public bool m_bFirstUse = false;//1회용

    
    public string m_szValues;
    public void AllFalse()
    {
        foreach (var v in m_gShowtrue)
        {
            if (v == null) continue;
            v.SetActive(false);
        }
        foreach (var v in m_gShowfalse)
        {
            if (v == null) continue;
            v.SetActive(false);
        }

    }
    private void Awake()
    {
    
        foreach (var v in m_gShowtrue)
        {
            if (v == null) continue;
            v.SetActive(false);
        }
        foreach (var v in m_gShowfalse)
        {
            if (v == null) continue;
            v.SetActive(false);
        }

    }
    private void OnEnable()
    {
        StartCoroutine("IRun");
    }
    //int nRet = CWLib.ConvertInt(GetValueString(m_szValues));
    bool GetValue(string szValue)
    {

        if(m_nValueList>0)
        {
            if (Game_App.Instance.g_bDirecting) return false;

            return g_bValueList[m_nValueList];
        }
        if(m_nTutoNumber>0)
        {
            if (Game_App.Instance.g_bDirecting) return false;

            if (!CWHeroManager.Instance.m_bFirstData[m_nTutoNumber])
            {
                return true;
            }
            return false;
        }
        if(szValue== "보석광고체크")
        {
            return ADDialDlg.Instance.IsAdBtn();
        }
        if(szValue== "도움패키지")
        {
            if (CWPlayerPrefs.HasKey("Use")) return false;

            if (CWPlayerPrefs.HasKey("HelpPackage")) return true;
            return false;
        }
        if(szValue== "공격력2배")
        {
            return CWGlobal.g_bDamageDouble;
            
        }
        //보상받을 도감이 있음
        if (szValue == "현재보상받을 도감있음")
        {//ValueUI.g_kSelectItemData
            if (CWHeroManager.Instance.IsRewardItem(ValueUI.g_kSelectItemData.nID)) return true;
            return false;
        }
        if (szValue == "보상받을 도감이 있음")
        {
            if (CWHeroManager.Instance.IsRewardItems()) return true;
            return false;
        }
        if (szValue == "불러오기가능")
        {
            if(!CWHeroManager.Instance.m_bTuto)
            {
                if (CWHeroManager.Instance.m_nStageNumber < 3)
                {
                    return true;
                }
                if (Space_Map.Instance.m_nNowFace == 5) return true;

                return false;
            }
            else
            {
                return true;
            }

        }

        if (szValue == "스테이지 3 이하")
        {
            //불러오기가능
            if (CWHeroManager.Instance.m_nStageNumber<3)
            {
                return true;
            }
            return false;

        }
        if (szValue == "보스 깼음")
        {
            if (CWHeroManager.Instance.m_nStageNumber >= 6)
            {
                return true;
            }
            return false;

        }

        if (szValue == "블록합성할 블록있음")
        {
            if (CWInvenManager.Instance.IsHaveUpgrade())
            {
                return true;
            }
            return false;

        }

        if (szValue == "일일 보상있음")
        {
            if (Dailymission.Instance.IsHaveReward())
            {
                return true;
            }
            return false;

        }
        if (szValue == "전투불가인가?")
        {
            if(CWHeroManager.Instance.IsDontUseShip())
            {
                return true;
            }
            return false;
        }
        if (szValue == "수리중")
        {
            if (CWHeroManager.Instance.IsRepairIng())
            {
                return true;
            }
            return false;
        }

        //타이머해야 하는가?
        if (szValue == "타이머해야 하는가?")
        {
            if(GamePlay.Instance.m_nWType == GamePlay.WTYPE.SINGLE)
            {
                if (!CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID()))
                {
                    return true;
                }

            }
            if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.PVP)
            {
                return false;
            }
                // 추후에 멀티, 

                return false;
        }

        if (szValue== "BgmOn")
        {
            return CWGlobal.g_bBgmOn;
        }
        if (szValue == "SoundOn")
        {
            return CWGlobal.g_bSoundOn;
        }
        if (szValue == "Vibration")
        {
            return CWGlobal.g_bVibration;
        }


        if (szValue == "복구중인가?")
        {
            return CWHeroManager.Instance.IsDontUseShip();
        }
        if (szValue=="게임플레이인가?")
        {
            if(GamePlay.Instance.IsShow())
            {
                return true;
            }
            return false;
        }
        //0:블록 1: 색블록, 2 무기, 3 아이템
        if (szValue == "인벤토리_블록")
        {
            if (InventoryDlg.Instance.m_nType == 0) return true;
            return false;
        }
        if (szValue == "인벤토리_컬러")
        {
            if (InventoryDlg.Instance.m_nType == 1) return true;
            return false;
        }
        if (szValue == "인벤토리_무기")
        {
            if (InventoryDlg.Instance.m_nType == 2) return true;
            return false;
        }
        if (szValue == "인벤토리_아이템")
        {
            if (InventoryDlg.Instance.m_nType == 3) return true;
            return false;
        }


        if (szValue == "블록합성가능한가?")
        {
            if(ValueUI.g_kSelectItemData.type=="shipblock")
            {
                if(ValueUI.g_kSelectItemData.subtype ==1)
                {
                    return true;
                }
            }
            return false;

        }
        if (szValue == "캐릭터모드인가?")
        {
            if (GamePlay.Instance.CharMode)
            {
                  return true;
            }
            return false;
        }

        if (szValue == "싱글행성출현가능한가?")
        {
            if (CWHeroManager.Instance.m_bTuto)
            {
                if (PVPDlg.Instance.m_nCount <= 4)
                {
                    return false;
                }
            }
            return true;
        }
        if (szValue == "멀티행성입장가능한가?")
        {
            if (CWHeroManager.Instance.m_bTuto)
            {
                if(Space_Map.Instance.GetStageID() <= 3)
                {
                    return false;
                }
            }
            return true;
        }

        
        if (szValue == "비행기교환가능한가?")
        {
            if(CWHeroManager.Instance.m_bTuto)
            {
                if(PVPDlg.Instance.m_nCount==0)
                {
                    return false;
                }
            }
            return true;
        }
        
        if (szValue == "상점출현가능한가?")
        {
            if (CWHeroManager.Instance.m_bTuto)
            {
                if (PVPDlg.Instance.m_nCount <= 3)
                {
                    return false;
                }
            }
            return true;

        }
        
        if (szValue == "에디터출현가능한가?")
        {
            if (CWHeroManager.Instance.m_bTuto)
            {
                if (PVPDlg.Instance.m_nCount <= 1)
                {
                    return false;
                }
            }
            return true;

        }
      

        if (szValue == "강화출현가능한가?")
        {
            if (CWHeroManager.Instance.m_bTuto)
            {
                if (PVPDlg.Instance.m_nCount <=2)
                {
                    return false;
                }
            }
            return true;

        }

     
        if (szValue == "연출중")
        {
            return Game_App.Instance.g_bDirecting;
        }

        if (szValue=="행성들이 움직인다!")
        {
            return Game_App.Instance.g_bDirecting;
        }

        if (szValue == "터보활성인가?")
        {
            return Game_App.Instance.m_bTuboFlag;
        }

        if (szValue == "블록을 처음 얻었다")
        {
            if(!CWHeroManager.Instance.m_bFirstData[4])
            {
                if(EquipInvenList.Instance.IsAddBlock())
                {
                    return true;
                }
            }
            return false;
        }
        if(szValue== "폭발범위가 작은가")
        {
            int lv= CWHeroManager.Instance.GetWeaponRangeLevel(0);
            if(lv<4)
            {
                return true;
            }
            return false;

        }
        if (szValue == "마이룸인가?")
        {
            if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.MYROOM) return true;
            return false;
        }

        if (szValue == "멀티플레이인가")
        {
            if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.MULTI) return true;
            return false;
        }
        if (szValue== "PVP인가?")
        {
            if (Space_Map.Instance.m_nType == 5) return true;
            if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.PVP) return true;
            return false;
        }
        if (szValue == "멀티인가")
        {
            if (Space_Map.Instance.m_nType == 4) return true;
            return false;
        }

        if (szValue == "보스맵인가?")
        {
            if (Space_Map.Instance.IsBossWar()) return true;
            return false;
        }

        if (szValue == "좋아요눌렀나?")
        {
            if (ScrollListUI.g_kSelectScrol)
            {
                int nUserID= ScrollListUI.g_kSelectScrol.GetSelectValueInt("Id");
                return CWHeroManager.Instance.IsLikeUser(nUserID);
            }

            return false;
        }

        if (szValue == "한국어인가")
        {
            int nSelect = CWPrefsManager.Instance.GetLanguage();
            if (nSelect == 0) return true;
            return false;
        }
        if (szValue == "차단유저인가?")
        {
            return CWHeroManager.Instance.m_bBlockUser;
        }
        if (szValue == "광고제거하였나?")
        {
            return CWHeroManager.Instance.m_bAdDel;
        }
        // 정복 했거나, 정복할 것이 남았거나
        if (szValue == "입장가능한가?")
        {
            return CWHeroManager.Instance.IsEnterPlanet(Space_Map.Instance.GetPlanetID(), Space_Map.Instance.GetStageID());
        }
        if(szValue=="정복했는가?")
        {
            return CWHeroManager.Instance.IsVictoryPlanet(Space_Map.Instance.GetPlanetID());

        }
        if (szValue == "정복한 구역인가?")
        {
            if (CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID()))
            {
                // 행성 정복 
                return true; // 파밍 가능
            }
            return false;

        }
        if (szValue == "게임이처음인가?")
        {
            //스테이지 하나도 안깻나?
            if (CWHeroManager.Instance.GetMyStageCount()==0)
            {
                return true; // 파밍 가능
            }
            return false;
        }
        if (szValue == "왼쪽으로 갈수있나")// 
        {
            if (Space_Map.Instance.IsLeftPlanet()) return true;
            else return false; // 
        }
        if (szValue == "오른쪽으로 갈수있나")// 
        {
            if (Space_Map.Instance.IsRightPlanet()) return true;
            else return false; // 
        }

        if (szValue == "현재 행성보기")// 
        {
            

            if (Space_Map.Instance.m_nType == 0 ) return true;
            else return false; // 
        }

        if (szValue == "행성보기인가")// 
        {
            if (Space_Map.Instance.m_nType == 1 || Space_Map.Instance.m_nType == 2 || Space_Map.Instance.m_nType == 3) return true;
            else return false; // 
        }
        if (szValue == "무료멀티입장")
        {
            if(CWHeroManager.Instance.MultiCount< CWGlobal.MULTICOUNT)
            {
                return true;
            }
            return false;

        }

        if (szValue == "싱글맵인가?")
        {
            if (GamePlay.Instance.m_nWType==GamePlay.WTYPE.SINGLE)
            {
                return true;
            }
            return false;

        }

        if (szValue == "PVP와전투")
        {
            if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.SINGLE|| GamePlay.Instance.m_nWType == GamePlay.WTYPE.PVP)
            {
                return true;
            }
            return false;

        }

        if (szValue == "멀티맵인가")
        {
            if (Space_Map.Instance.m_nType == 4)
            {
                return true;
            }
            return false;

        }
        //6번째행성도달했나
        if (szValue == "6번째행성도달했나")
        {
            if(CWHeroManager.Instance.m_nPlanetID>5)
            {
                return true;
            }
            return false;
        }

        if (szValue == "튜토리얼중인가")
        {
            return CWHeroManager.Instance.m_bTuto;
        }

        if (szValue == "초보인가")
        {
            if (CWHeroManager.Instance.m_bTuto) return false;
            if (CWGlobal.g_bGameBegin)
            {
                return true;
            }
            return false;
        }
    

        return false;
    }
    void UpdateData()
    {
        if(GetValue(m_szValues))
        {
            ShowHelp sh = null;
            foreach (var v in m_gShowtrue)
            {
                if (v == null) continue;
                v.SetActive(true);
                if(v.transform.childCount>0)
                {
                    Transform tChild = v.transform.GetChild(0);
                    if (tChild)
                    {
                        sh = tChild.GetComponent<ShowHelp>();
                    }

                }
            }
            foreach (var v in m_gShowfalse)
            {
                if (v == null) continue;
                v.SetActive(false);
            }
            if (sh)
            {
                sh.Show();
                
            }

            if(m_bFirstUse)
            {
                Destroy(this, 0.1f);
            }


        }
        else
        {
            foreach (var v in m_gShowtrue)
            {
                if (v == null) continue;
                v.SetActive(false);
            }
            ShowHelp sh = null;
            foreach (var v in m_gShowfalse)
            {
                if (v == null) continue;
                v.SetActive(true);
                if (v.transform.childCount > 0)
                {
                    Transform tChild = v.transform.GetChild(0);
                    if (tChild)
                    {
                        sh = tChild.GetComponent<ShowHelp>();
                    }
                }
            }

            if (sh)
            {
                sh.Show();

            }


        }

    }
    IEnumerator IRun()
    {
        
        while(true)
        {
            if (CWGlobal.G_bGameStart)
            {
                UpdateData();
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

}
