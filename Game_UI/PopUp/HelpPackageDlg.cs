
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CWEnum;
using CWUnityLib;
public class HelpPackageDlg : WindowUI<HelpPackageDlg>
{

    private void Start()
    {

    }
    public override void Open()
    {
        base.Open();
    }

    public void Show()
    {
        if (CWPlayerPrefs.HasKey("Use")) return;// 사용했음

        if(!CWPlayerPrefs.HasKey("HelpPackage"))
        {
            CWGlobal.g_kHelpPackageDate = DateTime.Now.AddDays(3);
            CWPlayerPrefs.SetString("HelpPackage", CWGlobal.g_kHelpPackageDate.ToString());
        }
        else
        {
            string strdate= CWPlayerPrefs.GetString("HelpPackage");
            CWGlobal.g_kHelpPackageDate = DateTime.Parse(strdate);
        }


        if (!CWPlayerPrefs.HasKey("Fist"))
        {
            CWPlayerPrefs.SetInt("Fist", 1);
            m_gScrollList.m_szCondition = "PID=g5900";
        }
        else
        {
            int ncount= CWPlayerPrefs.GetInt("Fist");
            CWPlayerPrefs.SetInt("Fist", ncount+1);
            if(ncount<2)
            {
                m_gScrollList.m_szCondition = "PID=g5900";
            }
            else
            {
                m_gScrollList.m_szCondition = "PID=g3900";
            }
            
        }
        

        Open();
    }
    public override void OnBuy(int num)
    {

        CWPlayerPrefs.SetInt("Use", 1);
        
        Close();
        base.OnBuy(num);
    }
}
