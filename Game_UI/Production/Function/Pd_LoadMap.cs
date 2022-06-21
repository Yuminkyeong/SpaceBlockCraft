using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using Newtonsoft.Json.Linq;
using CWStruct;
using CWEnum;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;


public class Pd_LoadMap : MonoBehaviour
{


    #region 맵로딩
    
    void LoadMap()
    {
        int nMapID = 0;
        if (Space_Map.Instance.IsMulti())
        {
            nMapID = CWGlobal.MULTIMAPID_1;
        }
        
        else
        {
            nMapID = CWArrayManager.Instance.GetMapID(Space_Map.Instance.GetStageID());
        }
        LoadingDlg.Instance.Show(true);


        string szlocalfile = CWGlobal.GetLocalFile();
        if (CWGlobal.CheckLoacalfile(szlocalfile))
        {
            CWMapManager.Instance.LoadLocalMap(szlocalfile);
        }
        else
        {
            CWMapManager.Instance.LoadMap(nMapID);
        }
        
        GamePlay.Instance.EnvironmaenON();


        CWProductionPage pt = GetComponentInParent<CWProductionPage>();
        pt.OnClose();

    }
    private void Start()
    {
        
        LoadMap();
        

    }

    #endregion

}
