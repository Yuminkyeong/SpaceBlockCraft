using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class SendMailDlg : WindowUI<SendMailDlg>
{

    public Text m_kName;

    public InputField m_kInput1;
    public InputField m_kInput2;

    int m_nUserID;
    public void Show(int nID,string szName)
    {
        m_nUserID = nID;
        m_kName.text = string.Format("{0}({1})",szName,nID);

        Open();
    }

    public void OnSendMail()
    {

        int gemcount = CWLib.ConvertInt(m_kInput2.text);
        if (!CWLib.IsString(m_kInput1.text)) return;
        if (!CWLib.IsString(m_kInput2.text)) return;

   //     CWSocketManager.Instance.SendMail(m_nUserID, m_kInput1.text, gemcount);
        Close();
    }

}
