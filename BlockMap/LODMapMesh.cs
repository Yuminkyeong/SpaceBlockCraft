using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class LODMapMesh : MonoBehaviour
{
    public int m_nNumber;
    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;

        int nPlanetID = Space_Map.Instance.GetPlanetID();
        int nFace = ((nPlanetID - 1) * 6 + m_nNumber);// 1부터 시작 
        int nMapID = CWArrayManager.Instance.GetMapID(nFace);
        string szfile = nMapID.ToString();
        MeshFilter mf= GetComponent<MeshFilter>();
        mf.sharedMesh = CWMeshManager.Instance.GetMesh(nMapID); // CWResourceManager.Instance.GetMeshAsset(szfile);
    }
}
