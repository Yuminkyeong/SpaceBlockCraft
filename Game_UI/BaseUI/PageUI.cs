using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWStruct;
using CWEnum;
public class PageUI<T> : WindowUI<T>
{

    public bool m_bCloseMainPage;// 종료후 메인페이지 복귀
    public Material m_kSkyMat;
    public GameObject m_gEnvironment;


    
    protected override void _Open()
    {


        if (m_gPrevPage != null && m_gPrevPage!=this)
        {
            m_gPrevPage.BaseClose();
        }
        m_gPrevPage = this;



        if (ClickHelpDlg.Instance)
            ClickHelpDlg.Instance.Close();

      //  RenderSettings.skybox = m_kSkyMat;
        if (m_gEnvironment)
        {
            Transform tParent = m_gEnvironment.transform.parent;
            for (int i = 0; i < tParent.childCount; i++)
            {
                Transform tChild = tParent.GetChild(i);
                tChild.gameObject.SetActive(false);
            }

            m_gEnvironment.SetActive(true);
        }

        if (m_bCoinInfo)
        {
            if(CoininfoDlg.Instance)
                CoininfoDlg.Instance.Open();
        }
        else
        {
            if (CoininfoDlg.Instance)
                CoininfoDlg.Instance.Close();
        }
        EnvironmaenON();
        base._Open();
    }
  
    public override void Close()
    {
        if (ClickHelpDlg.Instance)
            ClickHelpDlg.Instance.Close();
        

        base.Close();

        if (m_bCloseMainPage)
        {
            Space_Map.Instance.Open();
        }

    }
    // 환경을 킨다 
    public virtual void EnvironmaenON()
    {
        RenderSettings.skybox = m_kSkyMat;
        if (m_gEnvironment)
            m_gEnvironment.SetActive(true);

        //m_kSkyMat.color = Color.black;

        //StartCoroutine("IEnvRun");
    }

    Color m_Start = new Color(0, 0, 0, 0.5f);
    Color m_End = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    IEnumerator IEnvRun()
    {
        yield return null;
        float fDelayTime = 14f;
        float fRate = 0;
        while (true)
        {
            
            fRate += Time.deltaTime/ fDelayTime;
            Color kk = Color.Lerp(m_Start, m_End, fRate);
            m_kSkyMat.SetColor("_Tint", kk);
            m_kSkyMat.color = kk;

            yield return null;
        }

    }

}
