using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using CWEnum;


public class VictoryPlanetDlg : WindowUI<VictoryPlanetDlg>
{

    public override void Open()
    {

        CWInvenManager.Instance.AddItem((int)GITEM.Repair, 3);

        base.Open();
    }
    public override void Close()
    {
        Space_Map.Instance.Show();
        
        base.Close();
    }
    public void OnClickClose()
    {
        Close();
    }

}
