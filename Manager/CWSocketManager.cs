
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using System.Linq;
using SimpleJSON;

using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

using CWStruct;
using CWUnityLib;
using CWEnum;
/// <summary>
///  0번 소켓 
///     온라인 서버
///     채팅서버 
///     월드 서버
///     끊어지면 지속적으로 기다리며 연결을 한다 
///  1번 이상: 
///    데이타 베이스 서버
///    끊어지면 다른 서버로 접속을 시도 한다
///      
/// </summary>
public class CWSocketManager : CWManager<CWSocketManager>
{

    

    public List<string>IPlist=new List<string>();
    public CWSocket[] m_kSock;
    public int m_Port;
    public void CreateSocket(List<string> List,int port)
    {
        Debug.Log("IP List Count "+List.Count.ToString());
        for(int i=0;i<List.Count;i++)
        {
            Debug.Log(List[i]);
        }
        m_Port = port;
        IPlist.AddRange(List);


        m_kSock[0].Create(List, m_Port, CWSocket.SOCKETTYPE.ONLINE);
        m_kSock[1].Create(List, m_Port, CWSocket.SOCKETTYPE.DATA);
        // 수시 연결, 데이타 연결

    }
    public override void Create()
    {
        base.Create();
    }
    public void Close()
    {
        foreach(var v in m_kSock)
        {
            v.Close();
        }
    }
    public void Send(int num, JObject JData, RECEIVEFUCION rec=null)
    {
        m_kSock[num].Send(JData, rec);
    }
    public void OnlineConnect()
    {
        m_kSock[(int)CWSocket.SOCKETTYPE.ONLINE].Connect();
    }


    #region Receive 함수들


    // 차단 유저
    void BlockingUser()
    {
        MessageOneBoxDlg.Instance.Show(_closeClient, "메세지", "해킹 시도 하였습니다!");
    }

    // 클랜 전달 메세지 
    void ClanMessage(JObject jData)
    {
        // 나중에 메세지를 바꿔야 함
        int CT = CWJSon.GetInt(jData, "CT");
        if (CT == 3) // 거절
        {
            NoticeMessage.Instance.Show("회원 신청이 있습니다!");
        }

        if (CT == 3) // 거절
        {
            NoticeMessage.Instance.Show("거절 당했습니다!");
        }
        if (CT == 4) // 수락
        {
            NoticeMessage.Instance.Show("가입이 되었습니다");
        }
        if (CT == 5) //강퇴
        {
            NoticeMessage.Instance.Show("클랜에서 강퇴를 당했습니다!!!");
        }
        if (CT == 6) //탈퇴
        {
            NoticeMessage.Instance.Show("클랜 탈퇴를 했습니다!!");
        }


    }

    void RestoreBlockMap(JObject jData)
    {
        NoticeMessage.Instance.Show("맵이 복원 중입니다!");
        CWJSon jj = new CWJSon(jData);
        byte[] bTemp = jj.GetBytes("BlockBuffer");
        CWMapManager.SelectMap.RestoreBlock(bTemp);

    }

    void AddOwner(JObject _Data)
    {
        int nAiID = CWJSon.GetInt(_Data, "AiId");
        int nID = CWJSon.GetInt(_Data, "id");

        CWAIOwnerManager.Instance.AddOwnerList(nAiID, nID);
    }
    void DelOwner(JObject _Data)
    {
        int nAiID = CWJSon.GetInt(_Data, "AiId");
        CWAIOwnerManager.Instance.RemoveOwner(nAiID);
    }
    void WorldOut(JObject _Data)
    {
        int nID = CWJSon.GetInt(_Data, "id");
        CWUserManager.Instance.CloseUser(nID);
        CWAIOwnerManager.Instance.DeleteOWnerID(nID);
    }
    void UserRest(JObject jData)
    {
        int nID = CWJSon.GetInt(jData, "id");
        CWAirObject kUser = (CWAirObject)CWUserManager.Instance.GetUser(nID);
        if (kUser != null)
        {
            if (kUser == CWHero.Instance) return;
            int nRest = CWJSon.GetInt(jData, "Rest");
            float fHP = CWJSon.GetFloat(jData, "fHP");
            float x = CWJSon.GetFloat(jData, "X");
            float y = CWJSon.GetFloat(jData, "Y");
            float z = CWJSon.GetFloat(jData, "Z");
            kUser.SetPos(new Vector3(x, y, z));
            kUser.SetRest(nRest);
            kUser.SetHPRate(fHP);

        }
    }
    void SetEmoticon(JObject jData)
    {
        int UserId = CWJSon.GetInt(jData, "UserId");
        int Etype = CWJSon.GetInt(jData, "Etype");
        CWObject kUser = CWUserManager.Instance.GetUser(UserId);
        if (kUser)
        {
            kUser.SetEmoticon((Emoticon)Etype);
        }


    }
    void Musuk(JObject jData)
    {
        int Id = CWJSon.GetInt(jData, "Id");
        int Musuk = CWJSon.GetInt(jData, "Musuk");
        CWObject User = CWUserManager.Instance.GetUser(Id);
        if (User)
        {
            User.SetMusukBar(Musuk);
        }



    }

    public delegate void ResultPVP(bool bflag);
     ResultPVP CBResultPVP=null;

    // PVP 요청
    void PVPAsk(JObject jData)
    {
        int TargetID = CWJSon.GetInt(jData, "TargetID");
        string szname = CWJSon.GetString(jData, "Name");
        string szStr = string.Format("{0}님이 1:1 대결 신청을 하였습니다!",szname);
        // 로비 있을 때만 받는다
        if(Space_Map.Instance.IsShow())
        {
            MessageBoxDlg.Instance.Show(() => {

                SendAcceptPvp(TargetID);// 허락

            }, () => {

                SendAcceptRefuse(TargetID);//거절
            }, "PVP 요청!", szStr);
        }
        else
        {
            // 거절
        }

    }
    // PVP 허락
    void PVPAccept(JObject jData)
    {
        int UserID1 = CWJSon.GetInt(jData, "UserID1");
        int UserID2 = CWJSon.GetInt(jData, "UserID2");
        int roomnumber = CWJSon.GetInt(jData, "roomnumber");
        int TargetID = UserID1;
        bool Hometeam = false;
        if(TargetID==CWHero.Instance.m_nID)
        {
            Hometeam = true;
            TargetID = UserID2;
        }
        CBResultPVP?.Invoke(true);
        CBResultPVP = null;
        PVPDlg.Instance.ShowOnline(TargetID, roomnumber, Hometeam);
    }
    // PVP 거절
    void PVPRefuse(JObject jData)
    {
        CBResultPVP?.Invoke(false);
        CBResultPVP = null;
        MessageOneBoxDlg.Instance.Show("거절", "상대가 거절하였습니다!");
    }

    void PvPStart(JObject jData)
    {
        int TargetId = CWJSon.GetInt(jData, "TargetId");
        int Hitter = CWJSon.GetInt(jData, "Hitter");
        if (TargetId == 0 && Hitter == 0)
        {
            Musuk(jData); // 임시 작업  추후 변경
            return;
        }

        CWObject EUser = CWUserManager.Instance.GetUser(Hitter);
        CWObject kUser = CWUserManager.Instance.GetUser(TargetId);
        if (!EUser) return;
        if (!kUser) return;
        if (EUser)
        {
            EUser.PVPStart();
        }
        if (kUser)
        {
            kUser.PVPStart();
        }
        if (TargetId == CWHero.Instance.m_nID)// 공격받음
        {
            if (EUser)
            {
                NoticeMessage.Instance.Show(string.Format("{0}와 전투를 시작합니다.", EUser.name));
                CWHero.Instance.SetEnemy(EUser);
            }

        }
        if (Hitter == CWHero.Instance.m_nID)// 공격시작
        {
            {
                NoticeMessage.Instance.Show(string.Format("{0}와 전투를 시작합니다.", kUser.name));
                CWHero.Instance.SetEnemy(kUser);
            }
        }

        if (CWAIOwnerManager.Instance.IsMyOwner(TargetId))
        {

            CWAIEntity cs = kUser.GetComponent<CWAIEntity>();
            if (cs)
            {
                cs.SetEnemy(EUser);
            }

        }


    }
    void UserDamage(JObject jData)
    {
        int nID = CWJSon.GetInt(jData, "id");
        int Hitter = CWJSon.GetInt(jData, "Hitter");
        float fHP = CWJSon.GetFloat(jData, "fHP");
        int Damage = CWJSon.GetInt(jData, "Damage");
        CWObject kUser = CWUserManager.Instance.GetUser(nID);
        if (kUser == null) return;
        if (Damage == -1)
        {
            NoticeMessage.Instance.Show("아직 전투 준비가 안된 유저입니다");
            return;
        }
        if (Damage == 0)
        {
            if (Hitter == 0)
            {
                // HP 상승 물약 먹음
                kUser.SetHPRate(fHP);
                return;
            }

            kUser.PVPStart();
            if (Hitter == CWHero.Instance.m_nID)
            {
                NoticeMessage.Instance.Show("상대가 공격 준비 중입니다.");
            }
            return;
        }

        if (kUser != null)
        {
            kUser.SetDamage(Hitter, Damage, fHP);
        }


    }

   

    void UserDie(JObject _Data)
    {
        int nID = CWJSon.GetInt(_Data, "id");
        int Hitter = CWJSon.GetInt(_Data, "Hitter");
        int Count = CWJSon.GetInt(_Data, "Count");
        CWObject kUser = CWUserManager.Instance.GetUser(nID);
        if (kUser != null)
        {
            kUser.m_gKiller = CWUserManager.Instance.GetUser(Hitter);
            kUser.SetDie();
        }

        if (Hitter == CWHero.Instance.m_nID)
        {
            GamePlay.Instance.MultiKill(Count);
        }


    }

    CWObject GetUser(int nID)
    {
        CWObject oUser = CWUserManager.Instance.GetUser(nID);
        if (oUser) return oUser;

        oUser = CWMobManager.Instance.GetObject(nID);
        if (oUser) return oUser;
        return null;
    }


    void ShootFunc(JObject _Data)
    {
        int Shooter = CWJSon.GetInt(_Data, "Shooter");
        int Hitter = CWJSon.GetInt(_Data, "Hitter");
        int SelectWeapon = CWJSon.GetInt(_Data, "SelectWeapon");
        CWObject pgShooter = GetUser(Shooter);

        if (pgShooter && pgShooter != CWHero.Instance)
        {
            CWObject pgHitter = GetUser(Hitter);
            pgShooter.m_nSelectWeaponType = SelectWeapon;
            pgShooter.AIShoot(false, pgHitter);
        }
    }
    void ShootPosFunc(JObject _Data)
    {
        int Shooter = CWJSon.GetInt(_Data, "Shooter");
        float X = CWJSon.GetFloat(_Data, "X");
        float Y = CWJSon.GetFloat(_Data, "Y");
        float Z = CWJSon.GetFloat(_Data, "Z");
        int SelectWeapon = CWJSon.GetInt(_Data, "SelectWeapon");

        CWObject pgShooter = GetUser(Shooter);
        if (pgShooter && pgShooter != CWHero.Instance)
        {
            pgShooter.m_nSelectWeaponType = SelectWeapon;
            pgShooter.AIShootPos(false, new Vector3(X, Y, Z));
        }

    }





    void pingReceive(JObject _Data)
    {
        if (_Data["Result"].ToString() == "ok")
        {

        }
        else
        {
            print(" fail!!");
        }


    }
    void ReceiveKill(JObject jData)
    {
        CWJSon jj = new CWJSon(jData);
        int nHitID = jj.GetInt("HitID");
        int nExp = jj.GetInt("exp");
        int Energy = jj.GetInt("Energy");
        int nKillerID = jj.GetInt("KillerID");
        if (nKillerID == CWHero.Instance.m_nID)
        {

            string str = "";
            str = string.Format("{0}의 경험치와 {1}에너지를 얻었습니다", nExp, Energy);
            CWChattingManager.Instance.SystemMessage(str);
            CWCoinManager.Instance.SetData(jData["Coins"]);
        }
        CWObject gUser = CWHeroManager.Instance.GetObject(nHitID);
        if (gUser == null) return;
        if (gUser.IsDie())
        {
            return;
        }
        gUser.SetDie();

        CWChattingManager.Instance.SystemMessage("ReceiveKill!!");

    }
    void AddUserFacker(JObject _Data)
    {
        if (!GamePlay.Instance.IsMultiPlay()) return;

        int Id = CWJSon.GetInt(_Data, "Id");

        float x = CWJSon.GetFloat(_Data, "X");
        float y = CWJSon.GetFloat(_Data, "Y");
        float z = CWJSon.GetFloat(_Data, "Z");

        int HP = CWJSon.GetInt(_Data, "HP");
        int NHP = CWJSon.GetInt(_Data, "NHP");

        CWUserManager.Instance.MakeFakeUser(Id, new Vector3(x, y, z), 0, HP, NHP);

    }

    void AddUser(JObject _Data)
    {
        
        int Id = CWJSon.GetInt(_Data, "Id");
        int HP = CWJSon.GetInt(_Data, "HP");
        int NHP = CWJSon.GetInt(_Data, "NHP");

        float x = CWJSon.GetFloat(_Data, "X");
        float y = CWJSon.GetFloat(_Data, "Y");
        float z = CWJSon.GetFloat(_Data, "Z");

        CWUserManager.Instance.MakeUser(Id, new Vector3(x, y, z), 0, HP, NHP);



    }
    void _ClanDelete()
    {



    }
    void ClanDelete(JObject _Data)
    {

        CWSocketManager.Instance.UpdateUser("Clan", "0");

    }
    void DeleteMapOwner(JObject _Data)
    {
        // 맵 소유 상실
    }
    void DeleteNpc(JObject _Data)
    {
        int NpcId = CWJSon.GetInt(_Data, "NpcId");
        CWObject gUser = CWMobManager.Instance.GetObject(NpcId);
        if (gUser)
            gUser.SetDie();
        //Debug.Log(" deleteNpc(JObject _Data)");

    }
    
    void ReceveDelBlock(JObject _Data)
    {

        CWMapManager.SelectMap.ReceveDelBlock(_Data);
    }
    void ChattingMsg(JObject _Data)
    {
        int ID = CWJSon.GetInt(_Data, "ID");
        int CharNumber = CWJSon.GetInt(_Data, "CharNumber");
        string Name = CWJSon.GetString(_Data, "Name");
        string ChattingMsg = CWJSon.GetString(_Data, "ChattingMsg");
        
        DateTime dTime = CWJSon.GetTime(_Data, "ChatTime");

        int Grade = CWJSon.GetInt(_Data, "Grade");

        CWChattingManager.Instance.AddChatt(ID,CharNumber,Name,ChattingMsg, dTime.ToString(),Grade);
        if(ID==CWHero.Instance.m_nID)
        {
            ChattingDlg.Instance.ResetCursor();
        }
    }
    // 점검
    void _SystemFunc()
    {
        CWMainGame.Instance.Quit();
    }
    public Action _LoginEvent = null;
    void ToolLoginResult(JObject _Data)
    {
        // PingFuc();
    }
    void UpdatePage()
    {
        CWMainGame.Instance.Quit();
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.cwgames.spaceblockcraft");

    }
    int m_nTodayCount = 0;
    float m_fPlaytime = 0;

    void LoginResult(JObject _Data)
    {

        CWDebugManager.Instance.Log("로그 성공 - LoginResult");

        if ((_Data["Result"].ToString() == "system"))
        {


            MessageOneBoxDlg.Instance.Show(_SystemFunc, "서버 점검", "죄송합니다. 서버 점검 중입니다. 잠시 뒤에 접속해 주세요.");
            return;


        }
        if ((_Data["Result"].ToString() == "vision"))
        {
            //#if !UNITY_EDITOR

            MessageOneBoxDlg.Instance.Show(UpdatePage, "업데이트", "새버전이 있습니다. 업데이트 해주세요");
            return;
            //#endif
        }
        if ((_Data["Result"].ToString() == "fail"))
        {
            // GameLoading.Instance.Close();
            MessageOneBoxDlg.Instance.Show(_SystemFunc, "서버 점검", "접속 에러입니다");
            return;
        }

        if ((_Data["Result"].ToString() == "block"))
        {
            // GameLoading.Instance.Close();
            //NetWorkErrorDlg.Instance.Show(_SystemFunc, "차단유저", "차단 유저 대상입니다");
            MessageOneBoxDlg.Instance.Show(_closeClient, "차단유저", "차단된 유저입니다!");
            return;
        }


        if (_Data["Result"].ToString() == "Overlap")
        {

            //JToken tt= J
            // 중복 처리!
            CWDebugManager.Instance.Log("중복 !");
            MessageOneBoxDlg.Instance.Show(CWMainGame.Instance.Restart, "재시작", "중복이 있습니다.");//


        }
        else
        {

            //m_gCheckTime = CWLib.Random()
            CWHeroManager.Instance.m_nRanking = CWLib.ConvertIntbyJson(_Data["Ranking"]);
            m_fPlaytime = Time.time;
            int nToday = CWLib.ConvertIntbyJson(_Data["Todaycheck"]);


            if (nToday == 1)
            {
                CWGlobal.g_bToday = true;
                m_nTodayCount = 0;
                CWSocketManager.Instance.UpdateDayLog(DAYLOG.UserCount);

            }
            else
            {
                CWGlobal.g_bToday = false;
                int num = PlayerPrefs.GetInt("TodayCount");
                m_nTodayCount = num + 1;
            }

            CWSocketManager.Instance.UpdateDayLog(DAYLOG.UsertotalCount);

            PlayerPrefs.SetInt("TodayCount", m_nTodayCount);




            CWDebugManager.Instance.Log("Login ok!");
            
            CWHeroManager.Instance.m_nRepairValue = CWLib.ConvertIntbyJson(_Data["Repair"]);
            CWHeroManager.Instance.Repairtime = CWJSon.GetTime(_Data, "Repairdate"); //CWLib.ConvertIntbyJson(_Data["Repair_ing"]);
            
            //if (CWHeroManager.Instance.Repairtime== DateTime.MinValue)
            //{
            //    CWHeroManager.Instance.Repairtime = DateTime.MinValue;
            //}

            CWHeroManager.Instance.UpdateSelectAir(_Data["BlockBuffer"]);
            CWHeroManager.Instance.UpdateData(_Data["UserData"]);

            CWHeroManager.Instance.NSlotLevel = CWJSon.GetInt(_Data, "SlotLevel");


    
            
            CWHeroManager.Instance.TodayWork();


            // 로그인 정상
            // 게임 아이디
            // 블록 정보
            // 인벤 정보

            //서버변수
            CWGlobal.MULTI_HP = CWJSon.GetInt(_Data["Config"], "MULTI_HP");// 멀티 HP
            CWGlobal.MULTI_ATTACK = CWJSon.GetInt(_Data["Config"], "MULTI_ATTACK");// 멀티  Attack

            CWGlobal.DRINK_BY_GEM = CWJSon.GetFloat(_Data["Config"], "DRINK_BY_GEM");// 젬당 드링크 가격
            CWGlobal.REPAIR_PRICE = CWJSon.GetInt(_Data["Config"], "REPAIR_PRICE");// 즉시 수리가격
            CWGlobal.REPAIRTIME = CWJSon.GetInt(_Data["Config"], "REPAIRTIME");// 업그레이드 보석 가격
            CWGlobal.UPGRADE_GEM = CWJSon.GetInt(_Data["Config"], "UPGRADE_GEM");// 업그레이드 보석 가격
            CWGlobal.CHAR_PRICE = CWJSon.GetInt(_Data["Config"], "CHAR_PRICE");// 캐릭터 가격
            CWGlobal.DAYMISSION = CWJSon.GetInt(_Data["Config"], "DAYMISSION");// 일일미션 가격
            CWGlobal.DAYMISSIONTICKET = CWJSon.GetInt(_Data["Config"], "DAYMISSIONTICKET");// 일일미션 가격
            CWGlobal.TODAY_RESET = CWJSon.GetInt(_Data["Config"], "TODAY_RESET");// 일일상점 리셋

            CWGlobal.MISSIONTICKET = CWJSon.GetInt(_Data["Config"], "MISSIONTICKET");// 미션 변환 티켓 가격
            CWGlobal.GET_TICKETTIME = CWJSon.GetInt(_Data["Config"], "GET_TICKETTIME");// 자동 생산 티켓 시간(초)
            CWGlobal.TICKETTIME_MAX = CWJSon.GetInt(_Data["Config"], "TICKETTIME_MAX");// 자동 티켓 최대 개수 

            CWGlobal.MULTI_TICKETRANDOM = CWJSon.GetInt(_Data["Config"], "MULTI_TICKETRANDOM");//멀티 아이템 확률
            CWGlobal.MULTI_RESETTIMER = CWJSon.GetInt(_Data["Config"], "MULTI_RESETTIMER");//멀티 리셋 밀리세커 

            CWGlobal.REWARDTICKET = CWJSon.GetInt(_Data["Config"], "REWARDTICKET");//광고후 입장권 개수
            CWGlobal.UPLOADCASH = CWJSon.GetInt(_Data["Config"], "UPLOADCASH");// 업로드 가격
           // CWGlobal.MULTIMAPID_1 = CWJSon.GetInt(_Data["Config"], "MULTIMAPID_1");// 멀티맵 번호 
           


            CWGlobal.MULTICOUNT = CWJSon.GetInt(_Data["Config"], "MULTICOUNT");// 멀티 최대 회수 
            CWGlobal.MULTIENTERPRICE = CWJSon.GetInt(_Data["Config"], "MULTIENTERPRICE");// 입장료 가격

            
            CWGlobal.WAR_TIME = CWJSon.GetInt(_Data["Config"], "WAR_TIME");// 전투시간
            CWGlobal.MULTI_TIME = CWJSon.GetInt(_Data["Config"], "MULTI_TIME");// 
            CWGlobal.PVP_TIME = CWJSon.GetInt(_Data["Config"], "PVP_TIME");// 

            

            CWGlobal.MULTI_CONTINUE = CWJSon.GetInt(_Data["Config"], "MULTI_CONTINUE");// 연장비용
            CWGlobal.PVP_SCORE = CWJSon.GetInt(_Data["Config"], "PVP_SCORE");// 연장비용
            CWGlobal.PVP_TICKET = CWJSon.GetInt(_Data["Config"], "PVP_TICKET");// PVP 티켓 
            CWGlobal.PVP_GOLD = CWJSon.GetInt(_Data["Config"], "PVP_GOLD");// PVP 티켓 

            JArray ja = CWJSon.GetArray(_Data["Config"], "RANKING_POINT");// PVP 티켓 
            for(int i=0;i<ja.Count;i++)
            {
                CWGlobal.RANKING_POINT[i] =  CWLib.ConvertInt( ja[i].ToString());
            }

            




            CWGlobal.MAXWEAPONCOUNT = CWJSon.GetInt(_Data["Config"], "MAXWEAPONCOUNT");// 

            //CWGlobal.SLOTMAXLEVEL = CWJSon.GetInt(_Data["Config"], "SLOTMAXLEVEL");// 


            CWGlobal.RANKRESETTIME = CWJSon.GetTime(_Data["Config"], "RANKRESETTIME");

            //GRADEVALUE


            JToken jt = _Data["Config"];
            if (jt != null)
            {
                {
                    JArray ja1 = (JArray)jt["EVENTSEASON"];
                    if (ja1 != null)
                    {
                        if (ja1.Count == 2)
                        {
                            string[] ss = ja1.ToObject<string[]>();
                            CWGlobal.EVENTSEASON1 = ss[0]; //
                            CWGlobal.EVENTSEASON2 = ss[1]; //
                        }
                    }

                }

            }

            CWGlobal.EVENTSEASONTITLE = CWJSon.GetString(_Data["Config"], "EVENTSEASONTITLE"); //
            if (_LoginEvent != null)
                _LoginEvent();
            CWSocketManager.Instance.CheckMail();
        }

        // 로그인 화면
        GameTitle.Instance.LoginOK();
        CWPlayerPrefs.Instance.InitPlayerPrefs();

        
    }

    static public void UseCoin_ResultFuc(JObject jData)
    {

        if (jData["Result"].ToString() == "ok")
        {
            CWCoinManager.Instance.SetData(jData["Coins"]);
        }
        else
        {
            //faile!!
            print("shop fail!!");
        }
    }

    void _closeClient()
    {
        CWMainGame.Instance.Quit();
    }
    // 서버에서 종료 메세지를 보냈다
    //void ExitHeckerMessage(JObject jData)
    //{
    //    MessageOneBoxDlg.Instance.Show(UpdatePage, "업데이트", "새버전이 있습니다. 업데이트 해주세요");
    //}
    void ExitGame(JObject jData)
    {
        if (CWJSon.GetInt(jData, "Message") == 610)
        {
            MessageOneBoxDlg.Instance.Show(_SystemFunc, "동시접속", "중복 접속된 계정입니다.");
        }
        else
        {
            MessageOneBoxDlg.Instance.Show(_SystemFunc, "서버 점검", "죄송합니다. 서버 점검 중입니다. 잠시 뒤에 접속해 주세요.");
        }

    }
    void ExitClient(JObject jData)
    {
        MessageOneBoxDlg.Instance.Show(UpdatePage, "업데이트", "게임이 마음에 드셨다면 다운로드 받아주세요!");

    }

#endregion
#region Send함수들
    
    #endregion

    #region  Send 온라인 함수

    #endregion

    #region  Send 데이타 서버 
    public void Login(string szType, string szID, string szname, string szPass = "", bool bReconnect = false, bool bTest=false)
    {

        // 
            JObject JData = new JObject();
            JData.Add("File", "./file/login");
            JData.Add("Version", CWMainGame.Instance.GetVersion());
            JData.Add("Type", szType);
            JData.Add("ID", szID);
            JData.Add("Name", szname);
            if(bTest)
            {
                JData.Add("Test", "1");
            }

            JData.Add("Pass", szPass);
            if (bReconnect)
            {
                JData.Add("ReConnect", 1);
            }
            else
            {
                JData.Add("ReConnect", 0);
            }
            Send((int)CWSocket.SOCKETTYPE.DATA,JData, LoginResult);
        

    }


    public void SendLog()
    {
        
        // 

            JObject JData = new JObject();
            JData.Add("File", "./file/PingLog");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Todaycount", m_nTodayCount);
            int Playtime = (int)(Time.time - m_fPlaytime);
            JData.Add("Playtime", Playtime);

            JArray ja = new JArray();
            for (int i = 0; i < BaseUI.g_kOpenList.Count; i++)
            {
                ja.Add(BaseUI.g_kOpenList[i]);
            }
            JData.Add("Log", ja);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, pingReceive);
    }


    public void SendCoreLevel(int CoreLevel)
    {
        // 


            string fucname = "UnitUpdate";
            JObject JData = new JObject();
            JData.Add("File", "./file/" + fucname);
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("CT", 2);
            JData.Add("CoreLevel", CoreLevel);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);

    }
    public void UpdateInvenAll(RECEIVEFUCION fuc)
    {
//         

            string fucname = "UpdateInvenAll";
            JObject JData = new JObject();
            JData.Add("File", "./file/" + fucname);
            JData.Add("IDX", CWHero.Instance.m_nID);


            JArray array = new JArray();
            foreach (var v in CWInvenManager.Instance.m_nInvenDB)
            {
                v.m_bUpdated = false;

                JObject jj = new JObject();
                jj.Add("Slot", v.m_nSlot);
                jj.Add("Item", v.NItem);
                jj.Add("Count", v.NCount);
                array.Add(jj);
            }
            JData.Add("inven", array);
        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

    }
    public void UpdateInven(List<SLOTITEM> List)
    {

        if (CWHero.Instance == null)
        {
            return;
        }
        bool bSendflag = false;
        JArray array = new JArray();
        if (List != null)
        {
            foreach (var v in List)
            {
                if (v.m_bUpdated)
                {
                    JObject jj = new JObject();
                    jj.Add("Slot", v.m_nSlot);
                    jj.Add("Item", v.NItem);
                    jj.Add("Count", v.NCount);
                    array.Add(jj);
                    v.m_bUpdated = false;
                    bSendflag = true;
                }
            }
        }
        if (bSendflag == false) return; /// 보낼 것이 없다

        // 
            string fucname = "UpdateInven";
            JObject JData = new JObject();
            JData.Add("File", "./file/" + fucname);
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("inven", array);
        //  Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);
        Send((int)CWSocket.SOCKETTYPE.DATA, JData);
        //});

    }
    public void UpdateAirObject(int AirSlotID,int maxhp,int Damage,int blockcount, byte[] bbb,Action func)
    {
        // 
            string fucname = "UpdateAirObject";
            JObject JData = new JObject();
            JData.Add("File", "./file/" + fucname);
            JData.Add("SlotID", AirSlotID);
            JData.Add("BlockData", bbb);
            JData.Add("MaxHP", maxhp);
            JData.Add("Damage", Damage);
            JData.Add("Count", blockcount);

        Send((int)CWSocket.SOCKETTYPE.DATA, JData, (jData) =>
        {

            if (AirSlotID == CWHeroManager.Instance.m_nAirSlotID)
            {
                CWHero.Instance.CopyBuffer(bbb);
            }
            func();
        });

    

    }

    public void UpdateUser(string szColumn, string szValues)
    {

            string fucname = "updateUser";
            JObject JData = new JObject();
            JData.Add("File", "./file/" + fucname);
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Column", szColumn);
            JData.Add("Values", szValues);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);

    }

    //public void UpdateStage(int nStage)
    //{

    //    CWHeroManager.Instance.NStage = nStage;
    //    string fucname = "UpdateStage";
    //    JObject JData = new JObject();
    //    JData.Add("File", "./file/" + fucname);
    //    JData.Add("IDX", CWHero.Instance.m_nID);
    //    JData.Add("Stage", CWHeroManager.Instance.NStage);

    //    PacketData _Data = new PacketData(JData, fucname, null);
    //    AddDatatList(_Data);


    //}
    public void Sendkill(int nKillerID, int nHitID, int nptype, int nLevel1, int nLevel2)
    {

            string szfucname = "ReceiveKill";

            JObject JData = new JObject();
            JData.Add("File", "./file/" + szfucname);
            JData.Add("KillerID", nKillerID);
            JData.Add("HitID", nHitID);
            JData.Add("ptype", nptype);
            JData.Add("Level1", nLevel1);
            JData.Add("Level2", nLevel2);
            JData.Add("Msg", szfucname);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);

    }
    public bool UpdateShape(int number, RECEIVEFUCION fuc, string fucname)
    {

            JObject JData = new JObject();
            JData.Add("File", "./file/ShapeUpdate");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Number", number);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData,fuc);
            return true;

    }
   

    public bool UseCoin2(COIN nType, int nCoin, COIN nType2, int nCoin2, RECEIVEFUCION fuc, string fucname)
    {


        if (nCoin < 0)
        {
            if (!CWCoinManager.Instance.CheckCoin(nType, nCoin))
            {
                if (nType == COIN.GOLD)
                {
                    NoticeMessage.Instance.Show("골드가 모자랍니다.");
                }
                if (nType == COIN.GEM)
                {
                    NoticeMessage.Instance.Show("보석이 모자랍니다.");
                }
                if (nType == COIN.ENERGY)
                {
                    NoticeMessage.Instance.Show("오일이 모자랍니다.");
                }
                if (nType == COIN.TICKET)
                {
                    NoticeMessage.Instance.Show("입장권이 모자랍니다");
                }

                return false;
            }


        }
        //   print("fuc =" + fuc.ToString());
    
            JObject JData = new JObject();
            JData.Add("File", "./file/UseCoin");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("num1", (int)nType);
            JData.Add("Coin1", nCoin);

            JData.Add("num2", (int)nType2);
            JData.Add("Coin2", nCoin2);


            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);
        return true;

    }

    public bool UseCoin3( int ticket, int gem, int gold, RECEIVEFUCION fuc, string fucname)
    {


        
        //   print("fuc =" + fuc.ToString());
    
            JObject JData = new JObject();
            JData.Add("File", "./file/UseCoin3");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("ticket", ticket);
            JData.Add("gem", gem);
            JData.Add("gold", gold);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);
        

        return true;

    }
    bool m_bReceiveCheck = false;
    public bool UseCoinEx(COIN nType, int nCoin, Action cbAciton = null)
    {
        if(CWCoinManager.Instance.IsOneFlag(nType))
        {
            CWCoinManager.Instance.UseCoinONE(nType, nCoin, cbAciton);

            return true;
        }

        if (m_bReceiveCheck) return true;
        m_bReceiveCheck = true;
        bool bRet = UseCoin(nType, nCoin, (jData) => {

            if (jData["Result"].ToString() == "ok")
            {
                m_bReceiveCheck = false;
                CWCoinManager.Instance.SetData(jData["Coins"]);
                if (cbAciton != null) cbAciton();
            }
            else
            {
                //faile!!
                print("shop fail!!");
            }


        }, "UseCoinEx");
        if(bRet==false)
        {
            m_bReceiveCheck = false;
        }
        return bRet;
    }
    public void SetCoin(COIN nType, int nCoin)
    {
       
            JObject JData = new JObject();
            JData.Add("File", "./file/SetCoin");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("num1", (int)nType);
            JData.Add("Coin1", nCoin);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, (jData)=> {

            if (jData["Result"].ToString() == "ok")
            {
                CWCoinManager.Instance.SetData(jData["Coins"]);
            }
            else
            {
                //faile!!
                print("shop fail!!");
            }
            });


    }
    public bool UseCoin(COIN nType, int nCoin, RECEIVEFUCION fuc, string fucname)
    {
        if (nType == COIN.GOLD)
        {
            // 돈을 사용했을때
            if (nCoin <= 0)
            {
                Dailymission.Instance.CheckUpdate(DAYMTYPE.GOLD, nCoin);
            }

        }
        if (nType == COIN.TICKET)
        {
            // 
            if (nCoin <= 0)
            {
                Dailymission.Instance.CheckUpdate(DAYMTYPE.TICKET, nCoin);
            }

        }



        if (nCoin < 0)
        {
            if (!CWCoinManager.Instance.CheckCoin(nType, nCoin))
            {
                if (nType == COIN.GOLD)
                {
                    NoticeMessage.Instance.Show("골드가 모자랍니다.");
                }
                if (nType == COIN.GEM)
                {
                    NoticeMessage.Instance.Show("보석이 모자랍니다.");
                }
                if (nType == COIN.ENERGY)
                {
                    NoticeMessage.Instance.Show("오일이 모자랍니다.");
                }
                if (nType == COIN.TICKET)
                {
                    NoticeMessage.Instance.Show("입장권이 모자랍니다");
                }

                return false;
            }


        }
        //   print("fuc =" + fuc.ToString());

        
            JObject JData = new JObject();
            JData.Add("File", "./file/UseCoin");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("num1", (int)nType);
            JData.Add("Coin1", nCoin);

            JData.Add("num2", 0);
            JData.Add("Coin2", 0);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


            return true;

    }



    public void AskUserData(int nID, RECEIVEFUCION fuc, string fucname)
    {

            // 유저데이타 요청
            JObject JData = new JObject();
            JData.Add("File", "./file/ReceiveUserData");
            JData.Add("UserID", nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);
        

    }
    // 유저 상품을 요구한다
    public void AskUserGoodsData(int nID, RECEIVEFUCION fuc, string fucname)
    {

        
            // 유저데이타 요청
            JObject JData = new JObject();
            JData.Add("File", "./file/ReceiveUserGoods");
            JData.Add("GoodId", nID);// 상품번호 , 헷갈리면 안됨, 유저아이디가 아니다
        
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    

    }

    public void SendQueryList(JObject JData, string szfilename, RECEIVEFUCION fuc, string fucname)
    {

        
            JData.Add("File", "./file/" + szfilename);
             Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);
    }
    public void SendDlg(string szfilename, string Jstring, RECEIVEFUCION fuc, string fucname)
    {

        
            JObject JData;
            if (Jstring == "")
            {
                JData = new JObject();
            }
            else
            {
                JData = JObject.Parse(Jstring);
            }
            JData.Add("File", "./file/" + szfilename);
            JData.Add("IDX", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);
    }
    public void SendMailAll(string Query, int Limit, string Message, int nCash, string Maintext, List<int> Item, List<int> ItemCount, Action func)
    {

        
            
            JObject JData = new JObject();
            JData.Add("File", "./file/SendMailAll");

            JData.Add("Query", Message);
            JData.Add("Limit", Limit);
            JData.Add("Message", Message);
            JData.Add("Count", nCash);
            JData.Add("Maintext", Maintext);
            if (Item.Count > 0)
            {
                JArray JItem = new JArray();
                for (int i = 0; i < Item.Count; i++)
                {
                    JItem.Add(Item[i]);
                }
                JData.Add("Item", JItem);

                JArray JCount = new JArray();
                for (int i = 0; i < ItemCount.Count; i++)
                {
                    JCount.Add(ItemCount[i]);
                }
                JData.Add("ItemCount", JCount);
            }
        Send((int)CWSocket.SOCKETTYPE.DATA, JData, (jData) =>
         {

             func();
         });

    }
    public void SendMail(int nReceiveId, string Message, int nCash, string Maintext, List<int> Item, List<int> ItemCount)
    {

        
            
            JObject JData = new JObject();
            JData.Add("File", "./file/SendMail");

            JData.Add("ReceiveId", nReceiveId);
            JData.Add("Message", Message);
            JData.Add("Count", nCash);
            JData.Add("Maintext", Maintext);
            if (Item.Count > 0)
            {
                JArray JItem = new JArray();
                for (int i = 0; i < Item.Count; i++)
                {
                    JItem.Add(Item[i]);
                }
                JData.Add("Item", JItem);

                JArray JCount = new JArray();
                for (int i = 0; i < ItemCount.Count; i++)
                {
                    JCount.Add(ItemCount[i]);
                }
                JData.Add("ItemCount", JCount);
            }

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);
    

    }

    public void SendSystemMail(int nReceiveId, int szSubject, int szMaintext, int nCash, int nItemId, int nCount)
    {

 
            JObject JData = new JObject();
            JData.Add("File", "./file/SendMail");

            JData.Add("ReceiveId", nReceiveId);
            JData.Add("Subject", szSubject);
            JData.Add("Maintext", szMaintext);
            JData.Add("Type", 1);

            JData.Add("Cash", nCash);
            JData.Add("Item", nItemId);
            JData.Add("Count", nCount);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);
     

    }
    public void UseAllMail(RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/UseAllMail");
            JData.Add("IDX", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void UseMail(int nMailId, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/UseMail");
            JData.Add("MailId", nMailId);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    public void SendName(string szname, RECEIVEFUCION fuc)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/ReName");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Name", szname);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void CloseChatt()
    {
        JObject JData = new JObject();
        JData.Add("File", "./file/Chatting");
        JData.Add("CT", 3);
        JData.Add("IDX", CWHero.Instance.m_nID);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);
    }

    // 채팅 데이타를 받는다
    public void GetChattingList(RECEIVEFUCION fuc)
    {
         
            JObject JData = new JObject();
            JData.Add("File", "./file/Chatting");
            JData.Add("CT",1);
            JData.Add("IDX", CWHero.Instance.m_nID);

            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);

    }
    public void SendChatting(string szChatt)
    {
        if(!CWLib.IsString(szChatt))
        {
            return;
        }
         
            JObject JData = new JObject();
            JData.Add("File", "./file/Chatting");
            JData.Add("ID", CWHero.Instance.m_nID);
            JData.Add("CT", 2);
            JData.Add("Name", CWHero.Instance.name);
            JData.Add("CharNumber", CWHeroManager.Instance.m_nCharNumber);
            JData.Add("ChattingMsg", szChatt);
            JData.Add("Grade",CWHero.Instance.m_nGrade);
            JData.Add("Msg", "ChattingMsg");
            JData.Add("ChatTime", DateTime.Now.ToString());
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);


    }
    public void SendUpgrade(int num, int nGem, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/Upgrade");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Num", num);
            JData.Add("Gem", nGem);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    public void SendTimerUpgrade(int Cooltime, int Term, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/TimerData");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Cooltime", Cooltime);
            JData.Add("Term", Term);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    // 친구 요청
    public void SendFriendAsk(int AskID, int flag, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/FriendAsk");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("AskID", AskID);
            JData.Add("flag", flag);//0 요청,1 요청 취소
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


        // CWQuestManager.Instance.CheckAddFriend();

    }
    // 친구 수락
    public void SendFriendAccept(int AskID, int nRefuse, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/FriendAdd");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("AskID", AskID);
            JData.Add("Refuse", nRefuse);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

    }
    // 친구취소
    public void SendFriendCancel(int UserID, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/FriendDel");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("UserID", UserID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

    }


    /*
     *  // 블록데이타 전송
       public void SendBlockData(int mx, int mz, List<MAPBLOCK> kBlocks)
       {
       
           
           string fucname = "ReceveDelBlock";
           JObject JData = new JObject();
           JData.Add("File", "./file/BlockMap");
           JData.Add("IDX", CWHero.Instance.m_nID);
           JData.Add("CT", 2);// 블록데이타 전송
           JData.Add("roomnumber", CWHeroManager.Instance.m_nRoomNumber);// 
           JData.Add("mx", mx);// 
           JData.Add("mz", mz);// 
           JData.Add("Msg", fucname);
           JArray array = new JArray();

           foreach (var v in kBlocks)
           {
               JObject jj = new JObject();
               jj.Add("x", v.x);
               jj.Add("y", v.y);
               jj.Add("z", v.z);
               jj.Add("block",v.nblock);
               array.Add(jj);
           }

           JData.Add("blocks", array);

           PacketData _Data = new PacketData(JData, fucname, null);
           AddDatatList(_Data);

#if UNITY_EDITOR

           Debug.Log("블록데이타 전송");

#endif
       }
      */

    // 자동 배틀 게임 


    //해당 룸에 들어 갈 수 있는가?
    public void CheckRoom(int roomnumber, RECEIVEFUCION fuc, string fucname)
    {
        if (CWGlobal.g_bSingleGame) return;

         

            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager"); // 파일함수와 fuc 전혀다르다 헷갈리지 않을것 
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("CT", 10);
            JData.Add("roomnumber", roomnumber);
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);


    }
    // 방번호를 배정받는다
    public void AskRoomNumberWorld(int nStep, RECEIVEFUCION fuc, string fucname)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager"); // 파일함수와 fuc 전혀다르다 헷갈리지 않을것 
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Grade", CWHeroManager.Instance.m_nGrade);
            JData.Add("Step", nStep);
            JData.Add("CT", 13);

            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);

    }
    public void ConnectWoarldbyTuto(RECEIVEFUCION fuc)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager"); // 파일함수와 fuc 전혀다르다 헷갈리지 않을것 
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("CT", 14);
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);


    }
    public void ConnectWorld(int nRoomnumber,Vector3 vPos, RECEIVEFUCION fuc, string fucname)
    {


        CWGlobal.m_nMultiRoomNumber = nRoomnumber;
        JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager"); // 파일함수와 fuc 전혀다르다 헷갈리지 않을것 
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("CT", 1);
            JData.Add("roomnumber", nRoomnumber);
            JData.Add("HP", (int)CWHero.Instance.GetMaxHP());
            JData.Add("NHP", (int)CWHero.Instance.GetHP());

            JData.Add("Grade", CWHero.Instance.GetGrade());
            JData.Add("Name", CWHero.Instance.name);



            JData.Add("X", vPos.x);// 위치 조절
            JData.Add("Y", vPos.y);
            JData.Add("Z", vPos.z);
            JData.Add("Yaw", CWHero.Instance.GetYaw());

            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);

    }
    public void SendEmoticon(int nType)
    {
        if (CWGlobal.g_bSingleGame) return;

         
            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager"); // 파일함수와 fuc 전혀다르다 헷갈리지 않을것 
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("CT", 7);
            JData.Add("Etype", nType);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);


    }


    public void SendWorldClose()
    {
        if (CWGlobal.g_bSingleGame) return;

         
            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager"); // 파일함수와 fuc 전혀다르다 헷갈리지 않을것 
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("CT", 2);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);


    }
    public void SendRemoveOwner(int nAiNumber)
    {
        if (CWGlobal.g_bSingleGame) return;

         


            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager"); // 파일함수와 fuc 전혀다르다 헷갈리지 않을것 
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("AiID", nAiNumber);
            JData.Add("CT", 4);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);


    }
    public void SendAskOwner(int nAiNumber)
    {
        if (CWGlobal.g_bSingleGame) return;

         

            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("AskID", nAiNumber);
            JData.Add("CT", 3);
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);;


        //  CWDebugManager.Instance.Log(string.Format("오너 요청 {0}", nAiNumber));

    }

    public void SendMusuk(bool bflag)
    {
        if (CWGlobal.g_bSingleGame) return;

         
            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager");
            JData.Add("IDX", CWHero.Instance.m_nID);
            if (bflag)
                JData.Add("Musuk", 1);
            else
                JData.Add("Musuk", 0);

            JData.Add("CT", 15);
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);;




    }


    public void SendUserAttackStart(int Hitter, int TargetId, RECEIVEFUCION fuc, string fucname)
    {
        if (CWGlobal.g_bSingleGame) return;

         
            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager");
            JData.Add("Hitter", Hitter);
            JData.Add("TargetId", TargetId);
            JData.Add("CT", 5);
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);




    }
    // 데미지 패킷을 활용한다
    public void SendResetHP()
    {
        CWHero.Instance.SetFullHP();
        SendUserDamage(0, CWHero.Instance.m_nID, 0);
    }
    public void SendUserDamage(int Hitter, int TargetId, int Damage, RECEIVEFUCION fuc = null)
    {
        if (CWGlobal.g_bSingleGame) return;

         
            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager");
            JData.Add("Hitter", Hitter);
            if (Hitter == CWHero.Instance.m_nID)
            {
                JData.Add("Grade", CWHeroManager.Instance.m_nGrade);
            }
            JData.Add("TargetId", TargetId);
            JData.Add("Damage", Damage);
            JData.Add("CT", 6);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);




    }


    public void SendShoot(int Shooter, int Hitter, int nSelectWeapon = 0)
    {
        if (CWGlobal.g_bSingleGame) return;

         
            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager");
            JData.Add("CT", 8);// 슈팅

            JData.Add("Shooter", Shooter);
            JData.Add("Hitter", Hitter);
            JData.Add("SelectWeapon", nSelectWeapon);
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);;


    }
    public void SendShootPOS(int Shooter, Vector3 vPos, int nSelectWeapon = 0)
    {
        if (CWGlobal.g_bSingleGame) return;

         
            JObject JData = new JObject();
            JData.Add("File", "./file/WorldManager");
            JData.Add("CT", 9);// 슈팅

            JData.Add("Shooter", Shooter);
            JData.Add("X", vPos.x);
            JData.Add("Y", vPos.y);
            JData.Add("Z", vPos.z);
            JData.Add("SelectWeapon", nSelectWeapon);
            Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);;


    }

    public void AskServerFile(int nID, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/ServerFile");
            JData.Add("Id", nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    // 얼굴아이콘을 등록한다 
    public void RegFaceImage(byte[] bImage, RECEIVEFUCION fuc, string fucname)
    {
        if (bImage == null) return;
         

            JObject JData = new JObject();
            JData.Add("File", "./file/RegFaceimage");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Name", CWHero.Instance.m_nID.ToString());
            JData.Add("Filedata", bImage);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

        CWDebugManager.Instance.Log(string.Format("upload file size {0}", bImage.Length));

    }

    // 보상 
    public bool BattleSuccess(int nGold, int nReward, int nExp, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/battlesucess");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Gold", nGold);
            JData.Add("Reward", nReward);
            JData.Add("Exp", nExp);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


        return true;

    }

    // 친구보상
    public bool FriendRewardMail(string szName, string szface, int nFriendID, RECEIVEFUCION fuc, string fucname)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/FriendReward");
            JData.Add("SendId", CWHero.Instance.m_nID);
            JData.Add("ReceiveId", nFriendID);
            JData.Add("Face", szface);
            JData.Add("Name", szName);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

        return true;

    }

    public bool UserList(string szfind, string szSelect, string szSort, string szLimit, string szStart, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/UserList");

            JData.Add("find", szfind);


            if (CWLib.IsString(szSelect))
                JData.Add("select", szSelect);
            if (CWLib.IsString(szSort))
                JData.Add("sort", szSort);
            if (CWLib.IsString(szLimit))
                JData.Add("limit", szLimit);

            if (CWLib.IsString(szStart))
                JData.Add("start", szStart);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


        return true;

    }

    public bool AutoBattleRestTime(int nRestTime)
    {

         

            JObject JData = new JObject();
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("File", "./file/AutoBattle");
            JData.Add("AutoBattleRestTime", nRestTime);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


        return true;

    }
    public bool UpdateCSV(RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/updatecsv");
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

        return true;

    }
    // 알람 상황 
    public bool RequestAlarm(RECEIVEFUCION fuc, string fucname)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/RequestAlarm");
            JData.Add("IDX", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


        return true;

    }

    public bool UpdateQuest(int nSelect, int Count)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/Quest");
            JData.Add("IDX", CWHero.Instance.m_nID);

            JData.Add("Select", nSelect);
            JData.Add("Count", Count);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


        return true;

    }

    public bool UseADShop()
    {
#if !UNITY_EDITOR
        
        JObject JData = new JObject();
        JData.Add("File", "./file/UseADLog");
        Send((int)CWSocket.SOCKETTYPE.DATA, JData);;

#endif
        CWHeroManager.Instance.ADCount++;
        UpdateUser("ADCount", CWHeroManager.Instance.ADCount.ToString());
        return true;

    }

    // 무슨 상품을 샀는가?
    public bool UseShop(string szShopID)
    {


#if !UNITY_EDITOR
         
        JObject JData = new JObject();
        JData.Add("File", "./file/UseShop");
        JData.Add("IDX", CWHero.Instance.m_nID);
        JData.Add("ShopID", szShopID);

        Send((int)CWSocket.SOCKETTYPE.DATA, JData);;

#endif
        return true;

    }
    public bool UseGemShop(string szType, int nCount)
    {


#if !UNITY_EDITOR
         
        JObject JData = new JObject();
        JData.Add("File", "./file/UseGemShop");
        JData.Add("IDX", CWHero.Instance.m_nID);
        JData.Add("Type", szType);
        JData.Add("Count", (int)nCount);
        Send((int)CWSocket.SOCKETTYPE.DATA, JData);;

#endif
        return true;

    }
    // 점수 계산 개념
    // 행성레벨 * 별개수 
    



    public void ChangeLogin(string szID, string szName,string szPass, RECEIVEFUCION fuc)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/ChangeLogin");
            JData.Add("ID", szID);
            JData.Add("Name", szName);
        //JData.Add("Pass", szPass);


        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

    }
    public void Rankinglist(RECEIVEFUCION fuc, string fucname)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/Rankinglist");
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

    }

   

    // AI가 건물을 짓는다



    // 유저 건물  구입



    public void SendRepairStage(int BuildId)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 8);
            JData.Add("File", "./file/UnitManager");
            JData.Add("UnitId", BuildId);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }

    // 정거장 에너지 충전
    public void SendChargeStage(int BuildId)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 6);
            JData.Add("File", "./file/UnitManager");
            JData.Add("UnitId", BuildId);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }
    // 정거장 일꾼 미네랄 채취
    public void SendTakeMineral(int BuildId)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 7);
            JData.Add("File", "./file/UnitManager");
            JData.Add("UnitId", BuildId);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }





    // 맵 오너  등록
    public void SendRegMapOwner(string szTitle, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 1);
            JData.Add("File", "./file/MapOwner");

            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("roomnumber", CWHeroManager.Instance.GetRoomNumber());// 
            JData.Add("Title", szTitle);// 
            JData.Add("Level", CWHero.Instance.m_nLevel);// 
            JData.Add("Name", CWHero.Instance.name);// 
            JData.Add("Face", CWHero.Instance.m_szFace);// 

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    // 맵 연장
    public void SendContinueMapOwner(RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 2);
            JData.Add("File", "./file/MapOwner");

            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("roomnumber", CWHeroManager.Instance.GetRoomNumber());// 
            JData.Add("ClanId", CWHeroManager.Instance.m_nClanID);// 

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    // 맵 이름 바꾸기
    public void SendChangeNameMapOwner(string sztitle, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 3);
            JData.Add("File", "./file/MapOwner");

            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("roomnumber", CWHeroManager.Instance.GetRoomNumber());// 
            JData.Add("Title", sztitle);// 

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }




#region 클랜
    public void SendClanInfo(RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 1);
            JData.Add("File", "./file/ClanManager");
            JData.Add("ClanId", CWHeroManager.Instance.m_nClanID);
            JData.Add("roomnumber", CWHeroManager.Instance.GetRoomNumber());
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    public void SendClanAsk(int nOwnerID, RECEIVEFUCION fuc, string fucname) // 회원 요청
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 2);
            JData.Add("File", "./file/ClanManager");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("LeaderId", nOwnerID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }


    public void SendClanAskRefuse(int AskID, RECEIVEFUCION fuc, string fucname) // 회원 요청 거절
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 3);
            JData.Add("File", "./file/ClanManager");
            JData.Add("AskID", AskID);
            JData.Add("ClanId", CWHeroManager.Instance.m_nClanID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void SendClanAskAccept(int AskID, RECEIVEFUCION fuc, string fucname) // 회원 요청 수락
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 4);
            JData.Add("File", "./file/ClanManager");
            JData.Add("AskID", AskID);
            JData.Add("ClanId", CWHeroManager.Instance.m_nClanID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void SendClanKickUser(int UserId, RECEIVEFUCION fuc, string fucname) // 회원 강퇴
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 5);
            JData.Add("File", "./file/ClanManager");
            JData.Add("UserId", UserId);
            JData.Add("ClanId", CWHeroManager.Instance.m_nClanID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    public void SendClanOut(RECEIVEFUCION fuc, string fucname) // 탈퇴
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 6);
            JData.Add("File", "./file/ClanManager");
            JData.Add("UserId", CWHero.Instance.m_nID);
            JData.Add("ClanId", CWHeroManager.Instance.m_nClanID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }






    public void SendUserBlock(List<MAPBLOCK> kBlocks)
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 1);
            JData.Add("File", "./file/UserBlockDBManager");

            JArray array = new JArray();
            foreach (var v in kBlocks)
            {
                JObject jj = new JObject();
                jj.Add("x", v.x);
                jj.Add("y", v.y);
                jj.Add("z", v.z);
                jj.Add("block", v.nblock);
                array.Add(jj);
            }
            JData.Add("BlockArray", array);
            JData.Add("roomnumber", CWHeroManager.Instance.GetRoomNumber());

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;

    }
    public void SendRestoreUserBlock()
    {

         
            JObject JData = new JObject();
            JData.Add("CT", 2);
            JData.Add("File", "./file/UserBlockDBManager");
            JData.Add("roomnumber", CWHeroManager.Instance.GetRoomNumber());

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }

    public void SendUserPlanetInfo(int Galaxy, RECEIVEFUCION fuc, string fucname) // 행성 정보 
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/UserPlanetInfo");
            JData.Add("Galaxy", Galaxy);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    // 행성을 정복했다
    public void SendUpdatePlanet(int GalaxyNumber, int PlanetNumber) // 행성 정보 
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/UpdatePlanet");
            JData.Add("GalaxyNumber", GalaxyNumber);
            JData.Add("PlanetNumber", PlanetNumber);
            JData.Add("UserID", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }

#endregion

    //무기 업그레이드 
    public void SendWeaponSlotUpgrade(int ntype, int nSlot, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("type", ntype);
            JData.Add("File", "./file/WeaponSlotUpgrade");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Slot", nSlot);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    //부스터 업그레이드 
    public void SendBusterSlotUpgrade(int nSlot, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();

            JData.Add("File", "./file/BusterSlotUpgrade");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Slot", nSlot);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    //부스터 업그레이드 
    public void SendBusterSlotRLUpgrade(int nSlot, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();

            JData.Add("File", "./file/BusterSlotRLUpgrade");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Slot", nSlot);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }



    // 좋아요 
    public void SendAddLike(int UserID, bool bFlag, List<int> kList, RECEIVEFUCION fuc)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/AddLike");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("LikeUser", UserID);
            JData.Add("Grade", CWHeroManager.Instance.m_nGrade);
#if UNITY_EDITOR
            JData.Add("Value", 3);
            Dailymission.Instance.CheckUpdate(DAYMTYPE.LIKE, 1);
#else
            if (bFlag)// 이미 존재
            {
                JData.Add("Value", -1);
            }
            else
            {
                JData.Add("Value", 1);
                Dailymission.Instance.CheckUpdate(DAYMTYPE.LIKE, 1);
            }

#endif

            JArray array = new JArray();
            foreach (var v in kList)
            {
                array.Add(v);
            }
            JData.Add("LikeList", array);


        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);



    }

    // 좋아요 리스트 가져오기 

    public void SendTakeLikeDB(RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/ReceiveLikeDB");
            JData.Add("IDX", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    // 비행기 전시
    public void SendAddUserGoods(Texture2D kIcon, string szName)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/MgrUserGoods");
            JData.Add("CT", 1);
            JData.Add("UserID", CWHero.Instance.m_nID);
            JData.Add("Name", szName);
            JData.Add("HP", CWHero.Instance.GetHP());
            JData.Add("Price", CWHero.Instance.GetPrice());
            JData.Add("BlockCount", CWHero.Instance.NBlockCount);
            JData.Add("BlockData", CWHero.Instance.GetBuffer());
            JData.Add("Icon", kIcon.EncodeToPNG());
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }
    public void SendDeleteMyAir(int nID, RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/MgrUserGoods");
            JData.Add("CT", 2);
            JData.Add("Id", nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    public void GetUploadCount(RECEIVEFUCION fuc, string fucname)
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/GetUploadCount");
            JData.Add("IDX", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);




    }

    // 게임 시간을 저장을 한다. 블록 채취 시간만 저장

    public void SendUseGamePlay(float fSec) //
    {

         
            JObject JData = new JObject();
            JData.Add("File", "./file/UseGamePlay");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("UseTime", fSec);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }
    public void SendFirstData(int num)
    {

        //        if (!m_bLogined) return;


        if (CWHeroManager.Instance.m_bFirstData[num]) return;
        CWHeroManager.Instance.m_bFirstData[num] = true;

         

            JObject JData = new JObject();
            JData.Add("File", "./file/FirstDataUpdate");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Num", num);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }
    public void CheckMail()
    {
        __CheckMail((JData) => {

            if (JData["Result"].ToString() == "ok")
            {
                CWGlobal.g_bIsMail = true;
            }
            else
            {
                CWGlobal.g_bIsMail = false;
            }

        }, "CheckMail");

    }

    void __CheckMail(RECEIVEFUCION fuc, string fucname)
    {


        if (CWHero.Instance == null) return;
        if (CWHero.Instance.m_nID == 0) return;
         

            JObject JData = new JObject();
            JData.Add("File", "./file/CheckMail");
            JData.Add("IDX", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);

    }
    public void SendMyRanking(RECEIVEFUCION fuc)
    {

        if (CWHero.Instance == null) return;
        if (CWHero.Instance.m_nID == 0) return;

         

            JObject JData = new JObject();
            JData.Add("File", "./file/MyRanking");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Grade", CWHeroManager.Instance.m_nGrade);

        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
#region 내 슬롯 
    
    

    public void UpdateWeaponSlot(int Gold, RECEIVEFUCION fuc)
    {


        if (CWHero.Instance == null) return;
        if (CWHero.Instance.m_nID == 0) return;

        
        

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdateWeaponSlot2");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Gold", Gold);

            JArray array = new JArray();
            foreach (var v in CWHeroManager.Instance.m_kWeaponSlot)
            {
                JObject jj = new JObject();
                jj.Add("Damage", v.DamageLv);
                jj.Add("Range", v.RangeLv);
                jj.Add("Speed", v.SpeedLv);
                array.Add(jj);
            }

            JData.Add("WeaponSlot", array);


            JArray array2 = new JArray();
            foreach (var v in CWHeroManager.Instance.m_kBusterSlot)
            {
                array2.Add(v);
            }
            JData.Add("BusterSlot", array2);


            JData.Add("SlotLevel", CWHeroManager.Instance.NSlotLevel);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    // 멀티 관련 ct,0조회, 1 증가 , 2 리셋
    public void SendMultiTimer(int CT, RECEIVEFUCION fuc)
    {


        if (CWHero.Instance == null) return;
        if (CWHero.Instance.m_nID == 0) return;



        JObject JData = new JObject();
        JData.Add("File", "./file/MultiCheckTime");
        JData.Add("IDX", CWHero.Instance.m_nID);
        JData.Add("CT", CT);
        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
#endregion

#region 운영툴

    public void SendManagerTool(string szDay, RECEIVEFUCION fuc, string fucname)
    {


        JObject JData = new JObject();
        JData.Add("File", "./file/ManagerTool");
        JData.Add("Day", szDay);
        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);



    }
    public void SendResetFunction(string szfile)
    {
                    JObject JData = new JObject();
                    JData.Add("File", "./file/refresh");
                    JData.Add("refile", szfile);
                    Send((int)CWSocket.SOCKETTYPE.DATA, JData); 

    }
    public void SendUpdateLog()
    {



    }


#endregion

    public void FindPVPUser(RECEIVEFUCION fuc)
    {


         
            JObject JData = new JObject();
            JData.Add("File", "./file/SearchPVPUser");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("BlockCount", CWHero.Instance.NBlockCount+5);
           Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);



    }

    public void UpdatePrice()
    {

         


            JObject JData = new JObject();
            JData.Add("File", "./file/UpdatePrice");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("Grade", CWHeroManager.Instance.m_nGrade);
            JData.Add("Price", CWHeroManager.Instance.m_nPrice);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;



    }

    public bool UpdatePVP(bool bWin,bool bDouble, RECEIVEFUCION fuc)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdatePVP");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("PVP_SCORE", CWGlobal.PVP_SCORE);
            if (bWin)
            {

                JData.Add("Win", 1);
            }
            else
            {
                JData.Add("Win", 0);
            }
            if(bDouble)
            {
                JData.Add("Double",1);
            }
            JData.Add("Ranking", CWHeroManager.Instance.m_nRanking);
            JData.Add("Like", CWHeroManager.Instance.m_nLike);
            JData.Add("Price", CWHeroManager.Instance.m_nPrice);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


            return true;

    }

    public bool SaveMapData(int stage, int total, int add, int res2, int res3)
    {



         

            JObject JData = new JObject();
            JData.Add("File", "./file/SaveMapData");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("stage", stage);

            JData.Add("total", total);
            JData.Add("add", add);
            JData.Add("res2", res2);
            JData.Add("res3", res3);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


        return true;
    }



   

    public void SendRefreshFile(string szFile)
    {

         

            JObject JData = new JObject();
            JData.Add("File", "./file/refresh");
            JData.Add("refile", "./file/" + szFile);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;



    }

#region 유저 메이킹 툴

    public void SendAskMakeruser(RECEIVEFUCION fuc)
    {
        // 
        //    JObject JData = new JObject();
        //    JData.Add("File", "./file/AskMakeruser");
        //    
        //}, fuc);



    }

#endregion




    public void SendUpdateDailCheck(int num, RECEIVEFUCION fuc)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/DailyCheck");
            JData.Add("IDX", CWHero.Instance.m_nID);

            CWHeroManager.Instance.m_kDailyList[num] = 2;

            JArray ja = new JArray();
            for (int i = 0; i < CWHeroManager.Instance.m_kDailyList.Count; i++)
            {
                ja.Add(CWHeroManager.Instance.m_kDailyList[i]);
            }
            JData.Add("DailyReward", ja);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void TakeMyWeaponList(RECEIVEFUCION fuc)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/TakeMyWeaponList");
            JData.Add("IDX", CWHero.Instance.m_nID);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    // 수리 시작
    public void UpdateRepairAir_Begin(int SlotID)
    {
         

            

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdateAirSlot");
            JData.Add("SlotID", SlotID);
            JData.Add("Repair_ing", 1);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }

    public void UpdateRepairAir(int SlotID, int Repair)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdateAirSlot");
            JData.Add("SlotID", SlotID);
            JData.Add("Repair", Repair);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }
    public void UpdateAirSlotLevel(int SlotID, int level,Action func)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdateAirSlot");
            JData.Add("SlotID", SlotID);
            JData.Add("Level", level);

            int MaxCount= CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "blockcount", level); 

            JData.Add("MaxCount", MaxCount);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData, (jData) =>
            {
                func();
            });


    }

    public void CreateAirSlot(int HP,int Attack,int Count,int MaxCount,int Level,byte[] BlockData, RECEIVEFUCION fuc)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/CreateAirSlot");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("HP", HP);
            JData.Add("Attack", Attack);
            JData.Add("Count", Count);
            JData.Add("MaxCount", MaxCount);
            JData.Add("Level", Level);
            JData.Add("BlockData", BlockData);
        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void SelectAirSlot(int SlotID, RECEIVEFUCION fuc)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/SelectAirSlot");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("SlotID", SlotID);

        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }
    // 수령하다
    public void UpdateDicItem(int num, int val, RECEIVEFUCION fuc)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdateDicItem");
            JData.Add("IDX", CWHero.Instance.m_nID);
            JData.Add("num", num);
            JData.Add("val", val);

        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void UpdateDayLog(DAYLOG dlog)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdateDayLog");
            JData.Add("Day", string.Format("{0}_{1}_{2}", DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day));
            JData.Add(dlog.ToString(), 1);

            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }

    public void GetPlayerData(RECEIVEFUCION fuc)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/GetPlayerData");
            JData.Add("IDX", CWHero.Instance.m_nID);
        Send((int)CWSocket.SOCKETTYPE.DATA, JData, fuc);


    }

    public void UpdatePlayerData(Dictionary<string, string> Playerdata)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/UpdatePlayerData");
            JData.Add("IDX", CWHero.Instance.m_nID);

            JArray ja = new JArray();
            foreach(var v in Playerdata)
            {
                JObject jo = new JObject();
                jo.Add("key",v.Key);
                jo.Add("value",v.Value);

                ja.Add(jo);
            }
            JData.Add("PData", ja);
            Send((int)CWSocket.SOCKETTYPE.DATA, JData);;


    }
    public void AskUserInfo(int UserID, RECEIVEFUCION fuc)
    {
         

            JObject JData = new JObject();
            JData.Add("File", "./file/AskUserInfo");
            JData.Add("UserID", UserID);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData, fuc);


    }

    public void SendAskPvp(int TargetID, ResultPVP func)
    {
        JObject JData = new JObject();
        JData.Add("File", "./file/PVPManager");
        JData.Add("IDX", CWHero.Instance.m_nID);
        JData.Add("CT",1);
        JData.Add("Name", CWHero.Instance.name);
        JData.Add("RankPoint", CWHeroManager.Instance.m_nRankPoint);
        JData.Add("PvpTotal", CWHeroManager.Instance.m_nPVPTotal);
        JData.Add("PvpWin", CWHeroManager.Instance.m_nPVPWin);
        JData.Add("TargetID", TargetID);
        JData.Add("CharNumber", CWHeroManager.Instance.m_nCharNumber);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);

        CBResultPVP = func;

    }
    // 허락
    public void SendAcceptPvp(int TargetID)
    {
        JObject JData = new JObject();
        JData.Add("File", "./file/PVPManager");
        JData.Add("CT", 2);
        JData.Add("UserID1", TargetID);
        JData.Add("UserID2", CWHero.Instance.m_nID);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);

    }
    public void SendAcceptRefuse(int TargetID)// 거절
    {
        JObject JData = new JObject();
        JData.Add("File", "./file/PVPManager");
        JData.Add("CT", 3);
        JData.Add("TargetID", TargetID);
        Send((int)CWSocket.SOCKETTYPE.ONLINE, JData);

    }

    #endregion


}
