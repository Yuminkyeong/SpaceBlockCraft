using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;


public class Pd_LoadUserMap : MonoBehaviour
{

    public static int g_UserID;

    private void Start()
    {

        LoadMap();
    }
    void LoadMap()
    {
        int nMapID = 0;
        nMapID = CWGlobal.MYPLANETMAPID;


        LoadingDlg.Instance.Show(true);

        string szfile= CWGlobal.GetMyLocalName();
        if (CWGlobal.CheckLoacalfile(szfile))
        {
            CWMapManager.Instance.LoadLocalMap(szfile);
        }
        else
        {
            CWMapManager.Instance.LoadMap(nMapID);
        }
        GamePlay.Instance.EnvironmaenON();


        CWProductionPage pt = GetComponentInParent<CWProductionPage>();
        pt.OnClose();

    }


}
