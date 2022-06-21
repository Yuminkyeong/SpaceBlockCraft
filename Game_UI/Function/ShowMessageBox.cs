using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class ShowMessageBox : MonoBehaviour
{

    public string m_szMessage;
    private void Start()
    {
        
    }
    private void Update()
    {
        if (!CWGlobal.G_bGameStart) return;
        if (Game_App.Instance.g_bDirecting) return ;
        //HelpMessageDlg.Instance.Show(m_szMessage);
        NoticeMessage.Instance.Show(m_szMessage);
        Destroy(this);
    }

    //protected override bool OnceRun()
    //{
    //    if (Game_App.Instance.g_bMoveGalaxy) return false;
    //    HelpMessageDlg.Instance.Show(m_szMessage);
    //    Destroy(this);
    //    return base.OnceRun();
    //}


}
