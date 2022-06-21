using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;
using CWUnityLib;
public class TutoMissionDlg : WindowUI<TutoMissionDlg>
{
    public TutoMissionMove uiMove;
    public override void Close()
    {
        uiMove.MoveToHide();
        Invoke("MissionClose", 1.2f);
    }
    private void MissionClose()
    {
        
        base.Close();
    }
}
