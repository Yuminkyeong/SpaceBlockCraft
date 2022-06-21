using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;


public class GameTitle : PageUI<GameTitle>
{
    public Text m_gButton;
    public GameObject m_kClickBtn;
    bool m_bFlag = false;
    

    public Color m_kColor;
    public Color m_kEndColor;
    public float fspeed = 0.1f;

    public GameObject m_gLight;
    


    public GameObject m_gMyPlanet;
    void play()
    {
        m_bFlag = true;
        m_gButton.DOFade(1, 1);
    }
    public override void Open()
    {
        m_gLight.SetActive(true);
        ///m_kClickBtn.SetActive(false);
        //m_bFlag = false;
        CWGalaxy.Instance.OnTitleMap();
        base.Open();
        StartCoroutine("IRun");

    }
    IEnumerator IRun()
    {
        yield return null;

    }

    public void OnPlay()
    {

        

        CWTableManager.Instance.UpdateCSV(() =>
        {
            CWSocketManager.Instance.OnlineConnect();
            CWMainGame.Instance.Login();
        });

    }

    protected override void _Close()
    {
        m_gLight.SetActive(false);
        base._Close();
    }


    bool m_bNotice = false;
    string m_szTitle;
    string m_szText;
    string m_szDate;
    int m_nReward=0;
    int m_nType = 0;
    public void ShowNotice(int nType,string szTitle, string sztext,string szDate,int Reward)
    {
        m_nType = nType;
        m_szDate = szDate;
        m_bNotice = true;
        m_szTitle = szTitle;
        m_nReward=Reward;
        m_szText = sztext;
    }
    public void LoginOK()
    {
        Space_Map.Instance.Show(0);

    }

    public override void OnEscKey()
    {
        
        MessageBoxDlg.Instance.Show(CWMainGame.Instance.Quit, null, "종료", "종료하시겠습니까?");
    }


}
