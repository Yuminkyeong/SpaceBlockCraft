using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlanet : EventPlanet
{

    private void Awake()
    {
        pType = Planettype.MYPLANET;
    }

    public override void SetMap()
    {
        
        m_kFilter.sharedMesh = CWMeshManager.Instance.GetMesh_MyPlanet();// CWResourceManager.Instance.GetMeshAsset(nMapID.ToString());

    }
    public override void UpdateSelectPlanet()
    {
        m_kFilter.sharedMesh = CWMeshManager.Instance.GetMesh_MyPlanet();

    }
}
