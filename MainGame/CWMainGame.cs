using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CWUnityLib;
using DG.Tweening;
using CWStruct;
using CWEnum;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif



public class CWMainGame : MonoBehaviour
{

    public delegate void LoginFunc(string szName, string szPass);

    public Canvas m_kMainCanvas;

    public bool m_bSelectIP;// 지정한 아이피로 모바일 포함


    

    
    public bool m_bLocalFile = true;

    public bool m_bVibration = false;
    public bool m_bSoundOn;
    public bool m_bRandomTest = false;
    public bool m_bManagerTool = false;
    public bool m_bSavePacket = false;
    public bool m_bGuestLogin = false;
    public bool m_bCheatVersion = false;

    
    public string m_FacebookID = "";
    public string m_GoogleID = "";
    public string m_GameCenterID = "";
    public string m_szName="PC";
    public string m_szPass;
    



    public bool m_bTestmode = false;

    public int m_nPort=30001;   // 서버 Port
    public string m_szLocalIP = "127.0.0.1";
    public string m_szUDPIP = "127.0.0.1";


    public static CWMainGame Instance;
    protected void Awake()
    {


#if UNITY_EDITOR|| UNITY_STANDALONE
        //PlayerPrefs.DeleteAll();

        Application.runInBackground = true;
#endif
        Instance = this;
        //GamePot.initPlugin();
    }

    public class NUserInfo
    {
        public string name { get; set; }
        public string userid { get; set; }
        public string logintype { get; set; }
        public string pass { get; set; }

    }

    static public NUserInfo m_kUserInfo=null;



#region 로그인


    // 게스트 로그인
    void CBLoginFuc(string szName,string szPass)
    {
        string szID = PlayerPrefs.GetString("Guest");

        if(CWLib.IsString( m_GoogleID))
        {
            szID = m_GoogleID;
        }

        string szName2 = PlayerPrefs.GetString("Name");
        if (szName != szName2)// 다른 이름으로 들어간다면
        {
            DateTime tt = DateTime.Now;
            int rr = UnityEngine.Random.Range(1, 100000000);
            // 완전 유니크한 수!!
            szID = string.Format("{0}{1}{2}{3}{4}{5}{6}", tt.Year, tt.Month, tt.Day, tt.Hour, tt.Second, tt.Millisecond, rr); //"Guest_" + rr.ToString();
        }
        PlayerPrefs.SetString("Guest", szID);
        PlayerPrefs.SetString("Name", szName);
        PlayerPrefs.SetString("szPass", szPass);
        LoginGame("Guest", szID, szName, szPass, false, true);

    }
    public void ReLogin()
    {
        LoginGame(GetLoginType(), GetLoginID(), m_szName, m_szPass);
    }
    // 자동 로그인 
    public void AutoLogin()
    {
        LoginDlg.Instance.Show(CBLoginFuc);


    }
    public void GameLogin()
    {

        if(m_kUserInfo!=null)
        {
            m_szName = m_kUserInfo.name;
            m_szPass = m_kUserInfo.pass;

            Debug.LogFormat("Login => {0} {1}", GetLoginType(), GetLoginID());

            LoginGame(GetLoginType(), GetLoginID(), m_szName, m_szPass);
        }
        else
        {
            if (CWLib.IsString(m_GoogleID))
            {
                LoginGame("Google", m_GoogleID, m_szName, m_szPass);
                return;
            }

            LoginDlg.Instance.Show(CBLoginFuc);
            PlayerPrefs.SetString("LoginType", "Guest");
        }

    }

    public void Login()
    {

    


        /////////////////////////////////////////////
        //#if !UNITY_EDITOR

        //        if(m_bGuestLogin)
        //        {
        //            LoginDlg.Instance.Show(CBLoginFuc);
        //           return;
        //        }
        //        GameLogin();
        //        return;
        //#else

        //        if (CWLib.IsString(m_GoogleID))
        //        {
        //            LoginGame("google", m_GoogleID, m_szName, "");
        //        }
        //        else
        //        {
        //            GameLogin();
        //        }

        //#endif
        if (m_bGuestLogin)
        {
            LoginDlg.Instance.Show(CBLoginFuc);
            return;
        }
        GameLogin();

    }
#if UNITY_ANDROID && !UNITY_EDITOR
    IEnumerator IGRun()
    {
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //.Build();
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.DebugLogEnabled = true;
        //PlayGamesPlatform.Activate();


        Debug.Log("PlayGamesPlatform 초기화");

        yield return new WaitForSeconds(1f);

        Debug.Log("Social.localUser.authenticated 진행");


        if (!Social.localUser.authenticated) // 로그인 되어 있지 않다면
        {
            Social.localUser.Authenticate(success => {
                if (success)
                {
                    Debug.Log("success");
                    CWSocketManager.Instance.ChangeLogin(Social.localUser.id, CWHero.Instance.name, m_szPass, (jData) => {
                        if (jData["Result"].ToString() == "ok")
                        {

                            Debug.Log("구글로 전환");
                            NoticeMessage.Instance.Show("구글로 전환하였습니다!");

                            PlayerPrefs.SetString("Google", Social.localUser.id);
                            PlayerPrefs.SetString("LoginType", "Google");

                            if (CWHero.Instance.name.Substring(0,5) =="Guest_")
                            {
                                CWHero.Instance.name = Social.localUser.userName;
                                CWSocketManager.Instance.UpdateUser("Name", Social.localUser.userName);

                            }

                        }
                        if (jData["Result"].ToString() == "googleLogin")
                        {
                            Debug.Log("구글 다시 로그인");
                            // 구글로 다시 로그인
                            PlayerPrefs.SetString("LoginType", "Google");
                            PlayerPrefs.SetString("Google", Social.localUser.id);
                            SceneManager.LoadScene("GLogin");
                        }
                        if (jData["Result"].ToString() == "fail")
                        {
                            NoticeMessage.Instance.Show("구글 접속을 실패하였습니다!");
                            Debug.Log("실패");
                        }


                    });
                }
                else
                {

                    NoticeMessage.Instance.Show("구글 접속을 실패하였습니다!");
                    Debug.Log("구글 접속을 실패하였습니다!");
                }
            });
            

        }



    }
#endif

    public void OnChangeGoogle()
    {
        //#if UNITY_ANDROID 
        //        Debug.Log("OnChangeGoogle()");
        //        StartCoroutine("IGRun");
        //#endif


#if UNITY_EDITOR

        //Debug.Log("success");
        string googleid = "30000";
        CWSocketManager.Instance.ChangeLogin(googleid, CWHero.Instance.name, m_szPass, (jData) => {
            if (jData["Result"].ToString() == "ok")
            {
                Debug.Log("구글로 전환");
                NoticeMessage.Instance.Show("구글로 전환하였습니다!");
                PlayerPrefs.SetString("Google", googleid);
                PlayerPrefs.SetString("LoginType", "Google");
                if (CWHero.Instance.name.Substring(0, 5) == "Guest_")
                {
                    CWHero.Instance.name = Social.localUser.userName;
                    CWSocketManager.Instance.UpdateUser("Name", Social.localUser.userName);
                }

            }
            if (jData["Result"].ToString() == "googleLogin")
            {
                Debug.Log("구글 다시 로그인");
                // 구글로 다시 로그인
                PlayerPrefs.SetString("LoginType", "Google");
                PlayerPrefs.SetString("Google", googleid);
                SceneManager.LoadScene("GLogin");
            }
            if (jData["Result"].ToString() == "fail")
            {
                NoticeMessage.Instance.Show("구글 접속을 실패하였습니다!");
                Debug.Log("실패");
            }


        });
#else
        if (!Social.localUser.authenticated) // 로그인 되어 있지 않다면
        {
            Social.localUser.Authenticate(success => {
                if (success)
                {
                    Debug.Log("success");
                    CWSocketManager.Instance.ChangeLogin(Social.localUser.id, CWHero.Instance.name, m_szPass, (jData) => {
                        if (jData["Result"].ToString() == "ok")
                        {

                            Debug.Log("구글로 전환");
                            NoticeMessage.Instance.Show("구글로 전환하였습니다!");

                            PlayerPrefs.SetString("Google", Social.localUser.id);
                            PlayerPrefs.SetString("LoginType", "Google");

                            if (CWHero.Instance.name.Substring(0,5) =="Guest_")
                            {
                                CWHero.Instance.name = Social.localUser.userName;
                                CWSocketManager.Instance.UpdateUser("Name", Social.localUser.userName);

                            }

                        }
                        if (jData["Result"].ToString() == "googleLogin")
                        {
                            Debug.Log("구글 다시 로그인");
                            // 구글로 다시 로그인
                            PlayerPrefs.SetString("LoginType", "Google");
                            PlayerPrefs.SetString("Google", Social.localUser.id);
                            SceneManager.LoadScene("GLogin");
                        }
                        if (jData["Result"].ToString() == "fail")
                        {
                            NoticeMessage.Instance.Show("구글 접속을 실패하였습니다!");
                            Debug.Log("실패");
                        }


                    });
                }
                else
                {

                    NoticeMessage.Instance.Show("구글 접속을 실패하였습니다!");
                    Debug.Log("구글 접속을 실패하였습니다!");
                }
            });
            

        }
#endif

    }
    

#endregion

    

    void OnEnable()
    {
#if UNITY_EDITOR
       
        EditorApplication.playModeStateChanged += OnUnityPlayModeChanged;
#endif
    }
    //void OnApplicationFocus(bool focusStatus)
    //{
    //    isPaused = focusStatus;
    //    if (!NoticeMessage.Instance) return;
    //    if(isPaused)
    //    {
    //        NoticeMessage.Instance.Show("화면 정지");

    //    }
    //    else
    //    {
    //        NoticeMessage.Instance.Show("화면 풀림");
    //    }
        
    //}

    private void OnApplicationQuit()
    {



        if (CWMapManager.SelectMap)
            CWMapManager.SelectMap.Close();
        if(CWUdpManager.Instance)
            CWUdpManager.Instance.CloseUDP();
        if (CWSocketManager.Instance)
            CWSocketManager.Instance.Close();

    }
#if UNITY_EDITOR
    void OnUnityPlayModeChanged(PlayModeStateChange state)
    {
        //if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
        //{
        //    //

        //}

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            CWGlobal.G_bGameStart = false;
            CWLocalization.Instance.Save();
            if(CWUdpManager.Instance)
                CWUdpManager.Instance.CloseUDP();
            if (CWMapManager.SelectMap)
                CWMapManager.SelectMap.Close();
        }
        CWUnityLib.DebugX.Log("state = " + state.ToString());
    }
#endif
    string GetText(string szName, string sz)
    {
        string szstr = szName;
        szstr += "  :   ";
        szstr += sz;
        szstr += "\n";
        return szstr;
    }
    void CheckSystem()
    {
        //#if !UNITY_EDITOR// 프레임 고정
        // Application.targetFrameRate = 30;
        Application.targetFrameRate = 60;
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        CWDebugManager.Instance.Log(GetText("텍스쳐 크기", SystemInfo.maxTextureSize.ToString()));
        CWDebugManager.Instance.Log(GetText("메모리", SystemInfo.systemMemorySize.ToString()));
        CWDebugManager.Instance.Log(GetText("그래픽 메모리", SystemInfo.graphicsMemorySize.ToString()));

        CWGlobal.g_SystemState = CWGlobal.SYSTEMSTATE.GOOD;
        if (SystemInfo.graphicsMemorySize>2000)
        {
            if (SystemInfo.systemMemorySize > 5000)
            {
                // 최상위
                CWGlobal.g_SystemState = CWGlobal.SYSTEMSTATE.BEST;
                CWDebugManager.Instance.Log(GetText("베스트시스템", ""));

                
                QualitySettings.masterTextureLimit = 0;
            }
        }
        else
        {
            if (SystemInfo.systemMemorySize < 3000)
            {
                CWGlobal.g_SystemState = CWGlobal.SYSTEMSTATE.BAD;
                CWDebugManager.Instance.Log(GetText("하위시스템", ""));
                Application.targetFrameRate = 30;
                QualitySettings.masterTextureLimit = 1;
                

            }

        }

        //CWGlobal.g_SystemState = CWGlobal.SYSTEMSTATE.GOOD;
    }

    void Start()
    {

         
        //GamePot.setListener(GamePotInterface.cs 상속받은 class );

         // ex) GamePot.setListener(new GamePotSampleListener());
        //시스템
        CheckSystem();
        

        List<string> klist = new List<string>();

        CWPrefsManager.Instance.LoadPrefs();
        CWJSon kSjon = new CWJSon();

#if UNITY_ANDROID && !UNITY_EDITOR

        if (kSjon.LoadFile("Config"))
        {
            string szIP = kSjon.GetString("ServerIP");
            klist.Add(szIP);
            string szIP2 = kSjon.GetString("ServerIP2");
            klist.Add(szIP2);

           CWUdpManager.Instance.m_szUDPIP = m_szUDPIP;


            m_szUDPIP=szIP;// 나중에 정한다!


            if(kSjon.GetInt("Release")==1)
            {
                m_bTestmode = false;
                m_bGuestLogin=false;
                m_bSelectIP=false; // 테스트 모드는 무조건 아이피를 직접 선택한다
            }
            else
            {
                m_bTestmode=true;// 테스트모드 
             ///   m_bGuestLogin=true;
                m_bSelectIP=true; // 테스트 모드는 무조건 아이피를 직접 선택한다
            }
            

           // CWDebugManager.Instance.Log(string.Format("ServerIP = {0}",szIP));



        }
        else
        {
           // CWDebugManager.Instance.Log(string.Format("서버아이피가 없다!!!! "));
        }



#else


#endif


        CWGlobal.g_bSoundOn = m_bSoundOn;
        CWGlobal.g_bBgmOn = m_bSoundOn;
        CWGlobal.g_bVibration = m_bVibration;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tChild = transform.GetChild(i);
            if (tChild)
            {
                if(tChild.gameObject.activeSelf)
                    tChild.SendMessage("Create");
            }
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        CWUdpManager.Instance.m_szUDPIP = m_szUDPIP;

#else
        if (!m_bSelectIP)
        {
            m_szLocalIP = "127.0.0.1";
            
        }
        m_szUDPIP = m_szLocalIP;
        klist.Add(m_szLocalIP);
        klist.Add(m_szLocalIP);
        CWUdpManager.Instance.m_szUDPIP = m_szUDPIP;

#endif

        // 공통
        {
        
            CWSocketManager.Instance.CreateSocket(klist, m_nPort);
            
        }




    }
    //void CountryCheck()
    //{
    //    Locale locale;
    //    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N)
    //    {
    //        locale = getApplicationContext().getResources().getConfiguration().getLocales().get(0); } else { locale = getApplicationContext().getResources().getConfiguration().locale; }
    //    String displayCountry = systemLocale.getDisplayCountry(); String country = systemLocale.getCountry(); String language = systemLocale.getLanguage();


    //}

    public void LoginGame(string szLoginType,string szID, string szUserName,string szPass="",bool breconnect=false,bool bTest=false)
    {
        
        m_szName = szUserName;
        m_szPass = szPass;
        if(szLoginType== "Google")
        {
            m_GoogleID = szID;
        }
        if (!CWLib.IsString(szID))
        {
            DateTime tt = DateTime.Now;
            int rr = UnityEngine.Random.Range(1, 100000000);
            // 완전 유니크한 수!!
            szID = string.Format("{0}{1}{2}{3}{4}{5}{6}", tt.Year, tt.Month, tt.Day, tt.Hour, tt.Second, tt.Millisecond, rr); //"Guest_" + rr.ToString();
            szUserName = "Guest_" + rr.ToString();
            szLoginType = "Guest";
        }
        CWSocketManager.Instance.Login(szLoginType,szID, szUserName, szPass, breconnect, bTest);

        PlayerPrefs.SetString("Name", szUserName);
        PlayerPrefs.SetString("Pass", szPass);


    }

    // 구글로 전환 
    // 게스트에서 구글로 넘어갈때, 데이타 이전을 할 것인가?


    public int GetVersion()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 기능 추가만 관리, 
        string[] szstr = Application.version.Split('.');
        if(szstr.Length==3)
        {
            int Version1 = CWLib.ConvertInt(szstr[0]);
            int Version2 = CWLib.ConvertInt(szstr[1]);
            
            return Version1 * 100 + Version2 ;//
            
        }
        return 0;
#else
        return 10000;
#endif
    }
    
    public string GetLoginType()
    {
        return PlayerPrefs.GetString("LoginType");
    }
    public string GetLoginID()
    {
        return PlayerPrefs.GetString(GetLoginType());
    }
    public void Restart()
    {

    }
    public void Quit()
    {

        // NetworkMgr.Instance.UpdatePlayerStage();#if !(UNITY_EDITOR || UNITY_STANDALONE)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    protected void OnExit()
    {
        

        BaseUI.m_gPrevPage.OnEscKey();
        
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            OnExit();
        }
    }





}
