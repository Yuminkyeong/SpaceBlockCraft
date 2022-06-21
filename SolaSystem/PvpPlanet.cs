using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvpPlanet : EventPlanet
{

    private void Awake()
    {
        pType = Planettype.PVP;
    }

    public override void SetMap()
    {

        int nMapID = PVPDlg.Instance.GetPVPMap();
        m_kFilter.sharedMesh = CWMeshManager.Instance.GetMeshLOD2(nMapID,"");// CWResourceManager.Instance.GetMeshAsset(nMapID.ToString());

    }
}
