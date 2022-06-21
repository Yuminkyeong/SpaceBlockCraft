using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginDlg : WindowUI<LoginDlg>
{

    CWMainGame.LoginFunc Func;


    public InputField m_kInput1;
    public InputField m_kInput2;

    public void Show(CWMainGame.LoginFunc LoginFuc)
    {

        m_kInput1.text = PlayerPrefs.GetString("Name");
        m_kInput2.text = PlayerPrefs.GetString("Pass");

        Func = LoginFuc;
        Open();
    }

    public void OnOK()
    {

        //m_kInput1.text = CWPrefsManager.Instance.GetName();
        string szName = m_kInput1.text;
        string szPass = m_kInput2.text;

        Func(szName,szPass);

        Close();

    }
}
