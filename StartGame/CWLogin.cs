using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
//#endif
using CWUnityLib;

public class CWLogin : MonoBehaviour
{

    public GameObject m_gMenu;

    private void Awake()
    {
        
    }
    void Start()
    {

       
        m_gMenu.SetActive(false);
#if UNITY_ANDROID
#if UNITY_EDITOR
      
#endif
        CWMainGame.m_kUserInfo = new CWMainGame.NUserInfo();
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
#endif
        Debug.Log("PlayerPrefs.HasKey");
        if (PlayerPrefs.HasKey("LoginType"))
        {
            
            string szLogintype= PlayerPrefs.GetString("LoginType");
            string sz2= PlayerPrefs.GetString(szLogintype);
            if(CWLib.IsString(sz2))
            {
                Debug.LogFormat("log type= {0} {1}", szLogintype, sz2);
                Debug.Log(string.Format("log type= {0} {1}", szLogintype, sz2));
                CWMainGame.m_kUserInfo.name = PlayerPrefs.GetString("Name");
                CWMainGame.m_kUserInfo.pass = PlayerPrefs.GetString("Pass");

                
                //SceneManager.LoadScene("RGame");
                StartCoroutine(LoadYourAsyncScene());
                return;
            }
        }

        m_gMenu.SetActive(true);


    }
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        LoginLoading.Instance.Open();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("RGame");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        LoginLoading.Instance.Close();
    }

    public void OnGoogle()
    {

        

        StartMessage.Instance.Show("구글 계정으로 접속합니다.");
        m_gMenu.SetActive(false);
#if UNITY_ANDROID 
        if (!Social.localUser.authenticated) // 로그인 되어 있지 않다면
        {
            Social.localUser.Authenticate(success => {
                if (success)
                {
                    Debug.Log("success");

                    //Social.localUser.id
                    CWMainGame.m_kUserInfo.userid = Social.localUser.id;
                    CWMainGame.m_kUserInfo.name = Social.localUser.userName;
                    CWMainGame.m_kUserInfo.logintype = "Google";
                    Debug.Log("구글로그인  login");


                    PlayerPrefs.SetString("LoginType", "Google");
                    PlayerPrefs.SetString("Google", CWMainGame.m_kUserInfo.userid);
                    PlayerPrefs.SetString("Name", CWMainGame.m_kUserInfo.name);
                    PlayerPrefs.SetString("Pass", "1111");
                    StartCoroutine(LoadYourAsyncScene());

                }
                else
                {
                    StartMessage.Instance.Show("구글계정 연동에 실패하였습니다.");
                    Debug.Log("구글계정 연동에 실패하였습니다!");
                    m_gMenu.SetActive(true);
                }
            });


        }
#else
        SceneManager.LoadScene("RGame");
#endif


    }
    public void OnGuest()
    {
        m_gMenu.SetActive(false);
        DateTime tt = DateTime.Now;
        int rr = UnityEngine.Random.Range(1, 100000000);
        // 완전 유니크한 수!!
        CWMainGame.m_kUserInfo.userid = string.Format("{0}{1}{2}{3}{4}{5}{6}", tt.Year, tt.Month, tt.Day, tt.Hour, tt.Second, tt.Millisecond, rr); //"Guest_" + rr.ToString();
        CWMainGame.m_kUserInfo.name = "Guest_" + rr.ToString();
        CWMainGame.m_kUserInfo.logintype = "Guest";
        CWMainGame.m_kUserInfo.pass = "1111";

        Debug.LogFormat("게스트 저장한다!");
        Debug.LogFormat("LoginType Guest");
        Debug.LogFormat("Guest 저장 {0}", CWMainGame.m_kUserInfo.userid);


        PlayerPrefs.SetString("Guest", CWMainGame.m_kUserInfo.userid);
        PlayerPrefs.SetString("Name", CWMainGame.m_kUserInfo.name);
        PlayerPrefs.SetString("Pass", "1111");
        PlayerPrefs.SetString("LoginType", "Guest");
        Debug.Log("게스트 생성");
        StartCoroutine(LoadYourAsyncScene());
    }


}
