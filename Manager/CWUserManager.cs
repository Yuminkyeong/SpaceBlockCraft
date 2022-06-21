using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using System.Linq;
using SimpleJSON;

using CWUnityLib;

public class CWUserManager : CWManager<CWUserManager>
{

    public GameObject m_gUserUIDir;
    

    public CWUser m_gPrefab;
    public CWFakeUser m_gFakeUserPrefab;
    public CWFightUser m_gFightUserPrefab;


    public Dictionary<int, CWObject> m_kList = new Dictionary<int, CWObject>();

    

    // 유저가 존재하는가?
    public bool IsUser()
    {
        foreach(var v in m_kList)
        {
            if (v.Value.IsUser()) return true;// 유저가 존재 
        }

        return false;
    }
    public void Clear()
    {
        foreach (var v in m_kList)
        {
            if (v.Value == null) continue;
            if (v.Value.gameObject == null) continue;
            GameObject.Destroy(v.Value.gameObject);
        }

        m_kList.Clear();
    }


    public override void Create()
    {

        
        base.Create();
    }

    
    public CWUser RegUser(int nID,int bFakeUser, Vector3 vPos,float fYaw, int HP,int NHP)
    {
        if(bFakeUser==1)
        {
            return (CWUser)MakeFakeUser(nID, vPos, fYaw, HP,NHP);
        }
        else
        {
            return MakeUser( nID, vPos, fYaw, HP, NHP);
        }
    }

    public CWUser MakeUser(int nID,Vector3 vPos,float fYaw, int HP, int NHP)
    {
        if (nID == 0) return null;
        if (CWHero.Instance.m_nID == nID) return null;
        CWObject kTemp = GetUser(nID);
        if (kTemp)
        {
            m_kList.Remove(nID);
        }


        GameObject gg= Instantiate(m_gPrefab.gameObject);
        CWUser kUser = gg.GetComponent<CWUser>();
        gg.transform.parent = transform;
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localRotation = new Quaternion();
        // gg.transform.localScale = Vector3.one;

        kUser.m_nRest = 0;
        kUser.Create(nID);
        AddUser(kUser);

        kUser.m_nHP = HP;
        kUser.m_nNHP = NHP;

        

        kUser.SetPos(vPos);
        kUser.SetYaw(fYaw);
        
        return kUser;
    }
    public CWFightUser MakePVPUser(int nID)
    {
        if (nID == 0) return null;
        CWFightUser kUser = Instantiate(m_gFightUserPrefab);
        kUser.transform.parent = transform;
        kUser.transform.localPosition = Vector3.zero;
        kUser.transform.localRotation = new Quaternion();
        kUser.Create(nID);
        AddUser(kUser);
        return kUser;
    }

    public CWObject MakeFakeUser(int nID, Vector3 vPos, float fYaw, int HP,int NHP)
    {
        

        if (nID == 0) return null;
        if (CWHero.Instance.m_nID == nID) return null;
        CWObject kTemp = GetUser(nID);
        if (kTemp)
        {
            CloseUser(nID);

        }
        CWFakeUser kUser = Instantiate(m_gFakeUserPrefab);
        kUser.transform.parent = transform;
        kUser.transform.localPosition = Vector3.zero;
        kUser.transform.localRotation = new Quaternion();
        kUser.Create(nID);
        AddUser(kUser);

        kUser.SetPos(vPos);
        kUser.SetYaw(fYaw);

        kUser.m_nHP = HP;
        kUser.m_nNHP = NHP;

        kUser.SetFakePower(HP, HP/2);

        return kUser;
    }
    public int GetRestUserCount()
    {
        int tcnt = 0;

        foreach(var v in m_kList)
        {
            if(v.Value.m_nRest==1)
            {
                tcnt++;
            }
        }
        return tcnt;
    }
    public CWObject GetNearRestUser(Vector3 vPos)
    {
        CWObject kResult = null;
        float fMindist = 100000f;
        foreach (var v in m_kList)
        {
            CWObject o = v.Value;
            if (o == null) continue;
            if (o.m_nRest == 0) continue;
            float fDist = Vector3.Distance(o.transform.position, vPos);
            if(kResult == null)
            {
                kResult = o;
                fMindist = fDist;
            }
            else
            {
                if (fDist < fMindist)
                {
                    fMindist = fDist;
                    kResult = o;
                }

            }


        }

        return kResult;
    }
    public CWObject Search(int nTeam, Vector3 vPos,int nSightRange)
    {

        CWObject kResult=null;
        float fMindist = 100000f;
        // hero 포함
       if(CWHero.Instance.m_nRest==0)
        {
            float ff = Vector3.Distance(CWHero.Instance.transform.position, vPos);
            if (ff <= (nSightRange - 1))
            {
                fMindist = ff;
                if (CWHero.Instance.NTeam != nTeam)
                {
                    kResult = CWHero.Instance;
                }

            }

        }
        foreach (var v in m_kList)
        {
            CWObject o = v.Value;
            if (o == null) continue;
            if(o.m_nRest==1) continue;
            if (o.NTeam==nTeam)
            {
                continue;
            }
            float fDist = Vector3.Distance(o.transform.position, vPos);
            if (fDist <= nSightRange - 1)
            {
                //if (!IsFrontEnemy(v.transform.position)) continue;// 내 시야에서 안보이면 적으로 간주 안함 
                //if (IsBlockWall(v.transform.position)) continue;// 블록 장애물이 존재한다 
                if (fDist < fMindist)
                {
                    fMindist = fDist;
                    kResult = o;
                }
            }
        }

        return kResult;
    }
    public CWObject GetUser(int nID)
    {
        if (nID == CWHero.Instance.m_nID) return CWHero.Instance;
        if (m_kList.ContainsKey(nID))
        {
            return m_kList[nID];
        }
        return null;
    }
    public void CloseUser(int nID)
    {
        if (m_kList.ContainsKey(nID))
        {
            m_kList[nID].Close();
            Destroy(m_kList[nID].gameObject);// 실제 유저만 아웃
            m_kList.Remove(nID);
            
        }
            
    }
    public void AddUser(CWObject bObject)
    {
        if (bObject == null) return;
        if (m_kList.ContainsKey(bObject.m_nID)) return;
        CWUnityLib.DebugX.Log("등록! " + bObject.m_nID.ToString());
        m_kList.Add(bObject.m_nID, bObject);
    }
  

    // 테스트 
    #region 테스트

    public UILabel[] m_kTestList;

    #endregion



    


}
