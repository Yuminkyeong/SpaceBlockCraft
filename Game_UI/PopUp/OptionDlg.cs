using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class OptionDlg : WindowUI<OptionDlg>
{
    public Text m_kVerion;
    public CWButton m_kLogin;
    int m_nLanguage = 0;
    public override void Open()
    {
        m_kVerion.text = Application.version; //PlayerSettings.bundleVersion;
        if (CWMainGame.Instance.GetLoginType() == "Google")
        {
            m_kLogin.SetDisable(true,"이미 로그인 했습니다");
        }
        m_nLanguage = CWPrefsManager.Instance.GetLanguage();
        base.Open();
    }

    public void OnTrueOrFalse(GameObject OnOff)
    {

        if(CWGlobal.g_bBgmOn == true)
        {
            GameObject d = CWLib.FindChild(OnOff, "ON");
        }
        else
        {
            CWLib.FindChild(OnOff, "OFF");
        }
    }

    #region  언어

    
    public void OnPrevLang()
    {
        m_nLanguage--;
        if (m_nLanguage < 0) m_nLanguage = CWLocalization.Instance.m_LanguageList.Count-1;
        CWLocalization.Instance.SetLanguage(m_nLanguage);


    }
    public void OnNextLang()
    {
        m_nLanguage++;
        if (m_nLanguage >= CWLocalization.Instance.m_LanguageList.Count) m_nLanguage = 0;

        CWLocalization.Instance.SetLanguage(m_nLanguage);
    }
    public void OnBgSoundON()
    {
        CWGlobal.g_bBgmOn = true;
    }
    public void OnBgSoundOFF()
    {
        CWGlobal.g_bBgmOn = false;
    }
    public void OnBgToggle()
    {
        CWGlobal.g_bBgmOn = !CWGlobal.g_bBgmOn;
    }


    public void OnEffSoundON()
    {
        CWGlobal.g_bSoundOn = true;
    }
    public void OnEffSoundOFF()
    {
        CWGlobal.g_bSoundOn = false;
    }
    public void OnEffSoundToggle()
    {
        CWGlobal.g_bSoundOn =!CWGlobal.g_bSoundOn;
    }

    public void OnEffVibrationON()
    {
        CWGlobal.g_bVibration = true;
    }
    public void OnEffVibrationFF()
    {
        CWGlobal.g_bVibration = false;
    }
    public void OnEffVibratiToggle()
    {
        CWGlobal.g_bVibration =!CWGlobal.g_bVibration;
    }

    public void OnGoogle()
    {
        if(CWMainGame.Instance.GetLoginType()== "Google")
        {
            NoticeMessage.Instance.Show("이미 로그인 했습니다!");
            return;
        }
        CWMainGame.Instance.OnChangeGoogle();
        Close();
//        GameTitle.Instance.Open();
    }

    #endregion

}
