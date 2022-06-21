using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;

public class CWPlanet : BasePlanet
{

    public int m_nNumber;
    public int m_nPlanetID;
    public int m_nNowStage=0;

    public void Setting(int nPlanetID)//1 부터 온다 
    {
        m_nPlanetID= nPlanetID;

    }
    // 현재 맵을 LOD2로 바꾼다
    protected void ChangeLOD2Map(string szlocalfile,bool bUpdated)
    {
        if (!Space_Map.Instance.IsShow()) return;
        if (m_nNumber != Space_Map.Instance.m_nPlanetNumber)
        {
            return;
        }
            // 개념 : 
            // 중복되는 값이있는지 확인
            // 현재 맵이 어떤 것인지 정확하게 알아야됨
        int num= Space_Map.Instance.m_nNowFace;


        int nStage = ((m_nPlanetID - 1) * 6 + num) + 1;// 1부터 시작 




        int nMapID = CWArrayManager.Instance.GetMapID(nStage);

        m_kMeshs[num].sharedMesh = CWMeshManager.Instance.GetMeshLOD2(nMapID, szlocalfile, bUpdated); //CWResourceManager.Instance.GetMeshAsset(szfile);


    }
    public override void Rotate(Vector3 vRot)
    {
        Quaternion qq = Quaternion.Euler(vRot);
        m_visible.transform.DORotateQuaternion(qq,2).OnComplete(()=> {

            int num = Space_Map.Instance.m_nNowFace;
            int nStage = ((m_nPlanetID - 1) * 6 + num) + 1;// 1부터 시작 
            m_nNowStage = nStage;
            ChangeLOD2Map(CWGlobal.GetPlanetLocalName(nStage),false);
            m_kAxisRotate.enabled = false;
        });
        
    }
    public AxisRotate GetAxixRotate()
    {

        return m_kAxisRotate;
    }

    public override void ResetRotate()
    {
        m_visible.transform.rotation = new Quaternion();
    }

    public override void SetMap()
    {
        
        

        StartCoroutine("ISetMap");
    }
    IEnumerator ISetMap()
    {

        LoadingFileDlg.Instance.Open();
        int nPlanetID = m_nPlanetID;
        if (m_nPlanetID != 0)
        {
            int[] RoomList = CWArrayManager.Instance.GetCubeRoomList(nPlanetID);
            for (int i = 0; i < 6; i++)
            {
                int nStage = ((nPlanetID - 1) * 6 + i) + 1;// 1부터 시작 
                int nMapID = CWArrayManager.Instance.GetMapID(nStage);
                string szfile = nMapID.ToString();
                m_kMeshs[i].sharedMesh = CWMeshManager.Instance.GetMesh(nMapID);
                yield return new WaitForSeconds(0.1f);
            }

        }
        LoadingFileDlg.Instance.Close();
        yield return null;

    }

    public override void UpdateSelectPlanet()
    {

        ChangeLOD2Map(CWGlobal.GetPlanetLocalName(m_nNowStage),true);// 갱신
    }

}
