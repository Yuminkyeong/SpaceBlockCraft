using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#if UNITY_ANDROID
using UnityEngine.Advertisements;
using UnityEngine.Events;
public class CWADManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
{

    static CWADManager _Instance;
    public static CWADManager Instance
    {
        get
        {
            return _Instance;
        }
    }
    protected void Awake()
    {
        _Instance = this;
        InitializeAds();
    }




    [SerializeField] string _androidGameId;
    [SerializeField] string _iOsGameId;
    [SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId;

    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOsAdUnitId = "Rewarded_iOS";

    Action ReturnFunction;
    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, false);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void RewardShow(Action func)
    {
        ReturnFunction = func;
#if UNITY_EDITOR
        if (func != null) func();
        NoticeMessage.Instance.Show("광고를 시청하였습니다!");
#else
        if (CWHeroManager.Instance.m_bAdDel)
        {
            if (func != null) func();
            return;
        }

        Advertisement.Show(_androidAdUnitId, this);
        //NoticeMessage.Instance.Show("광고를 시청하였습니다!");
        //if (func != null) func();
#endif
    }
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
    }
  

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_androidAdUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.

            if (ReturnFunction == null) return;
            ReturnFunction();
            ReturnFunction = null;
            // Load another ad:
            Advertisement.Load(_androidAdUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }



    //public void OnUnityAdsAdLoaded(string adUnitId)
    //{
    //    Debug.Log("Ad Loaded: " + adUnitId);

    //    if (adUnitId.Equals(_adUnitId))
    //    {
    //        // Configure the button to call the ShowAd() method when clicked:
    //        _showAdButton.onClick.AddListener(ShowAd);
    //        // Enable the button for users to click:
    //        _showAdButton.interactable = true;
    //    }
    //}

}
/*
public class CWADManager : CWManager<CWADManager>
{

    public bool m_bTestMode = false;

    int m_nCount = 0;
    
    Action ResultFunction;

    void AdEndTask()
    {
        CWSocketManager.Instance.UseADShop();
        if(ResultFunction!=null)
        {
            ResultFunction();
            ResultFunction = null;
        }
    }

    //private void HandleShowResult(ShowResult result)
    //{
    //    switch (result)
    //    {
    //        case ShowResult.Finished:
    //            Debug.Log("The ad was successfully shown.");
    //            AdEndTask();
    //            break;
    //        case ShowResult.Skipped:
    //            Debug.Log("The ad was skipped before reaching the end.");
    //            break;
    //        case ShowResult.Failed:
    //            Debug.LogError("The ad failed to be shown.");
    //            break;
    //    }

        
    //}
    public void RewardShow(Action func)
    {

        if(CWHeroManager.Instance.m_bAdDel)
        {
            if (func != null) func();
            return;
        }
        ResultFunction = func;
        _RewardShow();
        //CWSocketManager.Instance.Close(_RewardShow);

    }
    void _RewardShow()
    {
      
        //if (Advertisement.IsReady("rewardedVideo"))
        //{
        //    BaseUI.g_kOpenList.Add(201);
        //    Debug.Log("Advertisement.IsReady() ok1!");
        //    Advertisement.Show("rewardedVideo", options);
        //}
        //else
        //{
        //    if (CWHeroManager.Instance.m_bAdDel)
        //    {

        //        MessageBoxDlg.Instance.Show(AdEndTask, AdEndTask, "광고완료", "광고가 없습니다.");
               

        //    }
           

                
        //}
        //m_fStart = Time.time;

    }

    float m_fStart = 0;
    // 그냥 광고 
    IEnumerator IRunShow()
    {
        
        yield return new WaitForSeconds(0.1f);
        //if (Advertisement.IsReady())
        //{
        //    Debug.Log("Advertisement.IsReady() ok1!");
        //    Advertisement.Show("video");
        //    yield return new WaitForSeconds(3f);
            
        //}


    }
    void EmptyFunc()
    {

    }
    
    public void Show(Action func = null)
    {
      
        ResultFunction = func;
        _Show();
    }
    void _RunShow()
    {
        StartCoroutine("IRunShow");
    }
    void _Show()
    {

        if (CWHeroManager.Instance.m_bAdDel)
        {
            if (ResultFunction != null)
            {
                ResultFunction();
            }
            ResultFunction = null;
            return;
        }
            
        // 튜토리얼  하면안됨
        if(CWHeroManager.Instance.m_bBlockUser)
        {
            RewardShow(EmptyFunc);
            return;
        }

        

        float fDelay = Time.time - m_fStart;
        
        m_fStart = Time.time;

        m_nCount++;
        BaseUI.g_kOpenList.Add(202);

        _RunShow();
        //CWSocketManager.Instance.Close(_RunShow);
        
        

    }

   

    public override void Create()
    {
        base.Create();
//        Advertisement.Initialize("4451411", m_bTestMode);
    }
}
*/
#else
    public class CWADManager : CWManager<CWADManager>
{
    public void Show(Action func = null)
    {
        if(CWHeroManager.Instance.m_bAdDel)
        {
            if (func != null) func();
            return;
        }
        NoticeMessage.Instance.Show("광고를 시청하였습니다!");
        if(func != null) func();

    }
    public void RewardShow(Action func)
    {
        if (CWHeroManager.Instance.m_bAdDel)
        {
            if (func != null) func();
            return;
        }
        NoticeMessage.Instance.Show("광고를 시청하였습니다!");
        if (func != null) func();

    }
    public override void Create(){}
}

#endif
