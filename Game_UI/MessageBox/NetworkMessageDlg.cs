using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMessageDlg : MessageWindow<NetworkMessageDlg>
{

    
    public void OnGameQuit()
    {
        Close();
        CWMainGame.Instance.Quit();
        
    }
         
}
