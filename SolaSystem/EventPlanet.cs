using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPlanet : BasePlanet
{
    public enum Planettype {MULTY,PVP,MYPLANET };
    public Planettype pType;
    public MeshFilter m_kFilter;
    public override void SetMap()
    {
        int nMapID = CWGlobal.MULTIMAPID_1;// 멀티맵 
        m_kFilter.sharedMesh = CWMeshManager.Instance.GetMeshLOD2(nMapID,"");// CWResourceManager.Instance.GetMeshAsset(nMapID.ToString());

    }
    public override void Rotate(Vector3 vRot)
    {
        //base.Rotate(vRot);
    }

}
