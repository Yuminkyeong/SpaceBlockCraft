using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class PVPDlg : WindowUI<PVPDlg>
{

    bool m_bOnline;// 온라인 모드 인가
    protected override int GetUINumber()
    {
        return 7;
    }

    bool m_bContinueGame = false;

    const int COUNT = 7;
     GameObject[] m_gHide=new GameObject[2];
    public int [] MAPARRAY;
    public int m_nMAXTime=100;// 시간 제한

    public GameObject [] m_gObject;
    public Text m_kCount;

    public Text m_kTimerText;

    int m_nTargetUser=0;

    public GameObject m_gDoubleText;

    
    public GameObject m_gInfo;

    public GameObject[] m_gPos;
    public GameObject[] m_gCamera;
    public GameObject[] m_gResult;

    Vector3[] m_vCmameraPos = null;


    public CWUser m_gTarget;


    public Text [] m_kName;
    public Text [] m_kScore;
    public Text [] m_kDamage;
    public Text [] m_kHP;
    public Text m_TargetRanking;

    public RawImage[] m_gGradeImage;

    
    

    public Slider m_kMyHP;
    public Slider m_kTgHP;

    public Transform m_tGoldPos;
    public RawImage m_gImage;

    public GameObject m_gStarPos;
    public Transform m_tStartStarPos;

    public GameObject m_bCloseBtn;


    public string m_szTargetname;

    bool m_bHome;
    Vector3 m_PVPHome = new Vector3(128, 60, 50);
    Vector3 m_PVPAway =new Vector3(128, 60, 200);
    Vector3 m_PVPPos = Vector3.zero;

    int m_nRoomnumber = 0;

    bool m_bResult = false;
    int m_nTargetScore;
    int m_nTargetGrade;
    int m_nTargetDamage;
    int m_nTargetHP;

    int RankPoint = 0;

    public int m_nPVPWinCount = 0;
    public int m_nCount;//대전 개수 
    

    float [] m_fCamPos=new float[2];

    int m_nGradeUpDown = 0;

    

    int m_nPVPMapID = 0;
    public GameObject m_gAdBtn;
    public int GetPVPMap()
    {
        return m_nPVPMapID;

    }
    public void MakeMap()
    {


         int num=CWLib.Random(0, MAPARRAY.Length);

         m_nPVPMapID = MAPARRAY[num];
        m_nPVPMapID = 129;
    }

    private void Start()
    {

        m_nPVPMapID = 129;
    }
    protected override void _Open()
    {
        CWGlobal.g_bADDouble = false;
        m_gDoubleText.SetActive(false);
        m_gAdBtn.SetActive(true);
        m_bCloseBtn.SetActive(true);
        m_nGradeUpDown = 0;
        m_gPos = new GameObject[2];
        m_gCamera = new GameObject[2];

        m_gPos[0] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpPos1");
        m_gPos[1] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpPos2");

        m_gCamera[0] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpcam1");
        m_gCamera[1] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpcam2");

        if(m_vCmameraPos==null)
        {
            m_vCmameraPos = new Vector3[2];
            m_vCmameraPos[0] = m_gCamera[0].transform.position;
            m_vCmameraPos[1] = m_gCamera[1].transform.position;

        }
        else
        {
            m_gCamera[0].transform.position = m_vCmameraPos[0];
            m_gCamera[1].transform.position = m_vCmameraPos[1];

        }

        m_fCamPos[0] = m_gCamera[0].transform.position.z;
        m_fCamPos[1] = m_gCamera[1].transform.position.z;


        


        base._Open();
    }
    void UpdateUserInfo()
    {
        if (m_gTarget == null) return;
        m_kName[0].text = CWHero.Instance.name;
        m_kName[1].text = m_gTarget.name;
        m_szTargetname = m_gTarget.name;

        m_kScore[0].text = CWHeroManager.Instance.m_nRankPoint.ToString();
        
        m_kScore[1].text = m_gTarget.m_nRankPoint.ToString();

        if(CWGlobal.g_bDamageDouble)
        {
            m_kDamage[0].text = string.Format("{0} + 두배 공격!", (CWHero.Instance.GetDamage()*2).ToString());
        }
        else
        {
            m_kDamage[0].text = CWHero.Instance.GetDamage().ToString();
        }
        

        m_kDamage[1].text = m_gTarget.GetDamage().ToString();

        m_kHP[0].text = CWHero.Instance.GetMaxHP().ToString();
        m_kHP[1].text = m_gTarget.GetMaxHP().ToString();

        m_TargetRanking.text = m_gTarget.m_nRanking.ToString();

        m_gGradeImage[0].texture = CWResourceManager.Instance.GetTexture(CWGlobal.GetGradeFileName(CWHero.Instance.GetGrade()));
        m_gGradeImage[1].texture = CWResourceManager.Instance.GetTexture(CWGlobal.GetGradeFileName(m_gTarget.GetGrade()));

    }

   
    void SetActiveObject(int nType)
    {
        
        foreach (var v in m_gObject) v.SetActive(false);
        m_gObject[nType].SetActive(true);
    }

    public override void Close()
    {
        m_nCount++;
        m_OnExit = false;
        m_bContinueGame = false;
        
        StopAllCoroutines();
        GamePlay.Instance.Close();


        Space_Map.Instance.m_nPlanetType = 2;
        Space_Map.Instance.Show(100);
        


        foreach (var v in m_gHide)
        {
            if(v)v.SetActive(true);
        }
            
        base.Close();
    }
    //Hometeam 홈팀인가?
    public void ShowOnline(int UserID,int roomnumber,bool Hometeam)
    {
        m_nRoomnumber = roomnumber;

        m_bHome = Hometeam;
        ChattingDlg.Instance.Close();
        m_bOnline = true;
        m_nTargetUser = UserID;
        CWGlobal.g_bSingleGame = false;

        CWGlobal.g_bWarmode = false;

        if (m_bHome)
        {

            m_PVPPos = m_PVPHome;
        }
        else
        {
            m_PVPPos = m_PVPAway;
            
        }
        CWHero.Instance.SetPos(Vector3.zero);
        Open();
        //CWSocketManager.Instance.ConnectWorld(roomnumber, vPos,(jData) => {

            

        //}, "ReceiveWorldData");



    }

    public void ShowSingle()
    {
        m_bOnline = false;
        // m_nTargetUser = ;
        CWGlobal.g_bSingleGame = true;
        Open();
    }
    

    public override void Open()
    {
        OpenGame();
        base.Open();
    }
    void OpenGame()
    {

        m_gHide = new GameObject[2];
        m_gHide[0] = CWLib.FindChild(GamePlay.Instance.gameObject, "farming_pd1");
        m_gHide[1] = CWLib.FindChild(GamePlay.Instance.gameObject, "Stage_War_LeftTop");



        m_gTarget = null;
        m_bCloseBtn.SetActive(false);
        StopAllCoroutines();
        m_kMyHP.value = 1;
        m_kTgHP.value = 1;
        m_kTimerText.text = CWLib.GetTimeString(m_nMAXTime);
        GamePlay.Instance.m_kMissionText.text = "상대 비행기를 격추 시키세요!";//
        foreach (var v in m_gHide)
        {
            if(v)
                v.SetActive(false);
        }
            
        SetActiveObject(0);
        Game_App.Instance.m_bTuboFlag = false;
        StartCoroutine("IRun");

    }

    public CWFightUser MakeUser(int nID)
    {
        if (nID == 0) return null;
        if (CWHero.Instance.m_nID == nID) return null;

        CWFightUser kUser = CWUserManager.Instance.MakePVPUser(nID); 
        return kUser;
    }


    // 유저를 찾는다
    IEnumerator LoadMap()
    {

        //if(!m_bContinueGame)
        //    CWHero.Instance.ResetHP();

        // 입장권 없이 그대로 HP 채우고 시작
        CWHero.Instance.ResetHP();

        // 맵을 고른다 
        LoadingDlg.Instance.Show(true);
        yield return null;
        CWMapManager.Instance.LoadMap(GetPVPMap());
        GamePlay.Instance.EnvironmaenON();

        float ts = Time.time;
        while(!CWMapManager.SelectMap.IsLoadEndTask())
        {
            if(Time.time-ts>4)
            {
                // 잘못되었음 

                Close();

                NoticeMessage.Instance.Show("File Error!");
                break;
            }

            yield return null;
        }

    }
    // 양쪽을 보여 준다
    IEnumerator CameraWalking()
    {
        m_gInfo.SetActive(true);

        m_gPos = new GameObject[2];
        m_gCamera = new GameObject[2];

        m_gPos[0] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpPos1");
        m_gPos[1] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpPos2");

        m_gCamera[0] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpcam1");
        m_gCamera[1] = CWLib.FindChild(CWMapManager.Instance.gameObject, "pvpcam2");

        Game_App.Instance.g_bDirecting = true;
        CWHero.Instance.Show(true);

        Camera.main.transform.position = m_gPos[0].transform.position;
        Camera.main.transform.eulerAngles = new Vector3(15, m_gPos[0].transform.eulerAngles.y, 0);


        CWHero.Instance.SetPos(m_gPos[0].transform.position);
        CWHero.Instance.SetYaw(m_gPos[0].transform.eulerAngles.y);

        if (m_gTarget==null)
        {
            Debug.Log("");


        }
        m_gTarget.SetPos(m_gPos[1].transform.position);
        m_gTarget.SetYaw(m_gPos[1].transform.eulerAngles.y);

        m_gCamera[0].SetActive(true);
        m_gCamera[1].SetActive(true);

        int ncount = COUNT;

        Vector3 v = m_gCamera[0].transform.position;
        v.z = m_fCamPos[0];
        m_gCamera[0].transform.position = v;

        float zvalue = m_gCamera[0].transform.position.z;
        m_gCamera[0].transform.DOMoveZ(zvalue - 10, ncount);

        v = m_gCamera[1].transform.position;
        v.z = m_fCamPos[1];
        m_gCamera[1].transform.position = v;
        zvalue = m_gCamera[1].transform.position.z;

        m_gCamera[1].transform.DOMoveZ(zvalue + 10, ncount);

        CWLookAt lt=m_gCamera[1].GetComponentInChildren<CWLookAt>();
        lt.m_gTarget = m_gTarget.gameObject;

        while (true)
        {
            m_kCount.text = ncount.ToString();
            if (ncount == 0)
            {
                
                break;
            }
            UpdateUserInfo();
            ncount--;
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.4f);
        //yield return new WaitForSeconds(1000f);
        Game_App.Instance.g_bDirecting = false;

        m_gInfo.SetActive(false);
        m_gCamera[0].SetActive(false);
        m_gCamera[1].SetActive(false);
        

    }
    IEnumerator StartEffect()
    {
        // 파이트 표시 
        GamePlay.Instance.StartWarMessage();
        
        yield return null;
        if(!m_bOnline)
        {
            CWFightUser ff = (CWFightUser)m_gTarget;
            ff.BeginAI();
        }
        
    }
    bool m_OnExit = false;
    public override void OnExit()
    {
        m_OnExit = true;
    }

    bool m_bExitflag = false;
    public void CloseEnd()
    {
        m_bExitflag = true;
    }
    IEnumerator ResultGame(bool bResult)
    {
        yield return new WaitForSeconds(1f);

        MakeMap();
        
        CWShowUI.g_bValueList[6] = true;
        SetActiveObject(2);

        while(!m_bExitflag)
        {
            yield return null;
        }
        m_bExitflag = false;
        SetActiveObject(3);
        m_bResult = bResult;
        if (bResult)
        {
            //CoininfoDlg.Instance.SetShow(true);
            m_gResult[0].SetActive(true);
            m_gResult[1].SetActive(false);
            //CWResourceManager.Instance.MoveObjectStar(m_gStarPos, 5, m_tStartStarPos);
            //Dailymission.Instance.CheckUpdate(DAYMTYPE.PVP, 1);
            //BaseUI.g_kOpenList.Add(601);// 결과


        }
        else
        {
            m_gImage.transform.DOShakePosition(2, 60, 20);
            m_gResult[0].SetActive(false);
            m_gResult[1].SetActive(true);
            BaseUI.g_kOpenList.Add(602);// 결과

        }
        m_bCloseBtn.SetActive(true);
        //        yield return new WaitForSeconds(1f);



    }
    

    IEnumerator IStartOffline()
    {
        bool bsuccess = false;

        for (int i = 0; i < 3; i++)
        {
            Debug.Log(string.Format("유저 검색 요청"));
            CWSocketManager.Instance.FindPVPUser((jData) => {
                if (jData["Result"].ToString() == "ok")
                {
                    m_nTargetUser = CWJSon.GetInt(jData, "id");
                    Debug.Log(string.Format("UserID=> {0}", m_nTargetUser));
                    m_gTarget = MakeUser(m_nTargetUser);
                    if (m_gTarget != null)
                    {
                        CWObject.g_kSelectObject = m_gTarget;

                    }
                }
                else
                {
                    Debug.LogError("데이타 에러!!!");
                }
            });

            float ft1 = Time.time;
            while (true)
            {
                if (Time.time - ft1 > 10f)
                {
                    Debug.Log("if (Time.time - ft1 > 2f)");
                    break;
                }
                if (m_gTarget != null)
                {
                    if (m_gTarget.m_bReceiveEnd)
                    {
                        bsuccess = true;
                        break;
                    }

                }
                yield return null;
            }
            if (bsuccess)
            {
                break;
            }
        }


        if (bsuccess == false)
        {
            MessageOneBoxDlg.Instance.Show(Close, "서버연결 실패", "서버 접속 에러입니다.");
        }

        yield return null;
    }
    IEnumerator IStartONline()
    {


        if (m_bHome)
        {
            CWHero.Instance.SetPos(m_PVPHome);
            CWHero.Instance.SetYaw(0);
            

        }
        else
        {
            CWHero.Instance.SetPos(m_PVPAway);
            CWHero.Instance.SetYaw(180);
        }


        CWSocketManager.Instance.ConnectWorld(m_nRoomnumber, m_PVPPos, (jData) => {

            

            {
                JArray ja = (JArray)jData["UserList"];
                // 유저 
                int tcnt = ja.Count;
                for (int i = 0; i < tcnt; i++)
                {
                    CWJSon jj = new CWJSon((JObject)ja[i]);
                    int nId = jj.GetInt("Id");
                    int NHP = jj.GetInt("NHP");
                    int HP = jj.GetInt("HP");
                    int X = jj.GetInt("X");
                    int Y = jj.GetInt("Y");
                    int Z = jj.GetInt("Z");
                    float fYaw = jj.GetFloat("Yaw");
                    int Facker = jj.GetInt("Facker");
                    CWUser user= CWUserManager.Instance.RegUser(nId, Facker, new Vector3(X, Y, Z),fYaw, HP, NHP);
                    if(user!=null)
                    {
                        m_gTarget = user;
                    }
                }

            }


        }, "ReceiveWorldData");
        while(true)
        {
            m_gTarget = (CWUser)CWUserManager.Instance.GetUser(m_nTargetUser);
            if(m_gTarget!=null)
            {
                CWObject.g_kSelectObject = m_gTarget;
                
                break;
            }

            yield return null;
        }

        yield return null;
    }



    IEnumerator IRun()
    {
        GamePlay.Instance.ShowPVP();
        yield return new WaitForSeconds(0.4f);
        CWUserManager.Instance.Clear();

        if(m_bOnline)
        {
            yield return StartCoroutine("IStartONline");
        }
        else
        {
            yield  return StartCoroutine("IStartOffline");
        }



        yield return new WaitForSeconds(1f);

        // 맵 로딩
        SetActiveObject(1);

        GamePlay.Instance.SetShow(false);
        CoininfoDlg.Instance.SetShow(false);

        yield return StartCoroutine(LoadMap());
        yield return new WaitForSeconds(1f);
        

        UpdateUserInfo();
        yield return StartCoroutine(CameraWalking());
        yield return StartCoroutine(StartEffect());

        GamePlay.Instance.SetShow(true);
       
        float fStarttime = Time.time;
        bool bResult = false;
        bool bflag = false;
        if (m_gTarget != null)
        {

            //시작 위치 값 조정
            if (m_bOnline)
            {
                if (m_bHome)
                {
                    CWHero.Instance.SetPos(m_PVPHome);
                    CWHero.Instance.SetYaw(0);
                    m_gTarget.SetPos(m_PVPAway);
                    m_gTarget.SetYaw(180);
                    GamePlay.Instance.m_HeroCamAction.m_fYaw = 180;
                }
                else
                {
                    CWHero.Instance.SetPos(m_PVPAway);
                    CWHero.Instance.SetYaw(180);

                    GamePlay.Instance.m_HeroCamAction.m_fYaw = 0;// fYaw + 180;

                    m_gTarget.SetPos(m_PVPHome);
                    m_gTarget.SetYaw(0);
                }

                CWUdpManager.Instance.StartUDP();

            }
            else
            {
                GamePlay.Instance.m_HeroCamAction.m_fYaw = 180;
            }
            CWDebugManager.Instance.m_bMusuk = false;


            while (!m_OnExit)
            {

                float ft = m_nMAXTime - (Time.time - fStarttime);
                if (ft <= 0)
                {
                    // 종료
                    break;
                }
                if (m_gTarget == null)// 삭제 
                {
                    bResult = true;//승리 
                    bflag = true;
                    NoticeMessage.Instance.Show("적기를 파괴하였습니다!!");

                    m_nPVPWinCount++;

                    m_kTgHP.value = 0;

                    break;
                }
                if (CWHero.Instance.IsDie())// 사망
                {
                    bResult = false;//패배
                    bflag = true;
                    NoticeMessage.Instance.Show("패배하였습니다.");
                    m_kMyHP.value = 0;
                    CWSocketManager.Instance.UpdateDayLog(DAYLOG.PVPDie);
                    break;

                }
                m_kMyHP.value = CWHero.Instance.GetHpRate();
                m_kTgHP.value = m_gTarget.GetHpRate();
                m_kTimerText.text = CWLib.GetTimeString(ft);

                yield return null;
            }
            CWPoolManager.Instance.Clear();

            // 경기가 시간이 지나서 끝남
            if (bflag == false)
            {
                //float f1= m_gTarget.GetHpRate();
                //float f2 = CWHero.Instance.GetHpRate();
                //if (f1 > f2) bResult = false;// 패배
                //else bResult = true;// 승리 

                bResult = false;// 패배

            }
        }
        yield return StartCoroutine(ResultGame(bResult));
    }

    public void OnContinuGame()
    {

        m_bContinueGame = true;

        foreach (var v in m_gHide)
        {
            if (v) v.SetActive(true);
        }

        OpenGame();
    }

    IEnumerator IUpdateClose()
    {
        m_bCloseBtn.SetActive(false);
        m_gAdBtn.SetActive(false);

        bool bflag = false;
        CWSocketManager.Instance.UpdatePVP(m_bResult, CWGlobal.g_bADDouble, (jData) => {

            CWGlobal.g_bADDouble = false;
            bflag = true;
            if (jData["Result"].ToString() == "ok")
            {
                int nGrade = CWHeroManager.Instance.m_nGrade;
                //CWCoinManager.Instance.SetData(jData["Coins"]);
                //CWHeroManager.Instance.m_nRankPoint = CWJSon.GetInt(jData, "RankPoint");
                RankPoint = CWJSon.GetInt(jData, "RankPoint");
                CWHeroManager.Instance.m_nRanking = CWJSon.GetInt(jData, "Ranking");
                CWHeroManager.Instance.m_nPVPTotal = CWJSon.GetInt(jData, "PvpTotal");
                CWHeroManager.Instance.m_nPVPWin = CWJSon.GetInt(jData, "PvpWin");

                if (nGrade < CWHeroManager.Instance.m_nGrade)
                {
                    NoticeMessage.Instance.Show("등급이 올라갔습니다");
                    m_nGradeUpDown = 1;
                }
                if (nGrade > CWHeroManager.Instance.m_nGrade)
                {
                    NoticeMessage.Instance.Show("등급이 내려갔습니다");
                    m_nGradeUpDown = 2;
                }


                CWQuestManager.Instance.CheckUpdateData(29, CWHeroManager.Instance.m_nPVPWin);
                CWQuestManager.Instance.CheckUpdateData(30, CWHeroManager.Instance.m_nPVPWin);
                CWQuestManager.Instance.CheckUpdateData(31, CWHeroManager.Instance.m_nPVPWin);
                CWQuestManager.Instance.CheckUpdateData(32, CWHeroManager.Instance.m_nPVPWin);
                CWQuestManager.Instance.CheckUpdateData(33, CWHeroManager.Instance.m_nPVPWin);

            }
            else
            {

                print("shop fail!!");
            }
        });
        while (!bflag)
        {
            yield return null;
        }
        

        

        CoininfoDlg.Instance.SetShow(true);

        yield return null;

        CWResourceManager.Instance.MoveObjectStar(m_gStarPos, 5, m_tStartStarPos);
        Dailymission.Instance.CheckUpdate(DAYMTYPE.PVP, 1);
        BaseUI.g_kOpenList.Add(601);// 결과
        CWSocketManager.Instance.UseCoinEx(COIN.GOLD, 0, () => {


            CWHeroManager.Instance.m_nRankPoint = RankPoint;
            if (m_nGradeUpDown != 0)
            {
                foreach (var v in m_gObject) v.SetActive(false);
                GamePlay.Instance.SetShow(false);
                GradeUpDlg.Instance.Show(m_nGradeUpDown);
                GradeUpDlg.Instance.CloseFuction = Close;
                m_nGradeUpDown = 0;
            }

        });
        if(m_bResult)
        {
            yield return new WaitForSeconds(3f);
        }
            
        Close();

    }

    public void OnClosePVP()
    {

        
        StartCoroutine("IUpdateClose");
        


    }
    public void OnADDouble()
    {
        CWADManager.Instance.RewardShow(() => {


            CWGlobal.g_bADDouble = true;
            m_gDoubleText.SetActive(true);
            StartCoroutine("IUpdateClose");
            
            AnalyticsLog.Print("ADLog", "PVPDouble");

        });
    }
}
