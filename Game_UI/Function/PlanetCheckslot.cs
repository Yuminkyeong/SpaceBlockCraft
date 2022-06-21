using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;

public class PlanetCheckslot : CWBehaviour
{

    public GameObject m_fEffect;
    public GameObject m_gSelect;
    public GameObject m_gP1; // 정복전
    public GameObject m_gP2; // 정복완료

    public int m_nNumber;

    protected override bool OnceRun()
    {


        if(m_fEffect)
        {
            m_fEffect.SetActive(false);
        }
        StartCoroutine("IRun");

        return base.OnceRun();
    }
    void UpdateData()
    {
        if (Space_Map.Instance == null) return;
        if(m_gSelect)
            m_gSelect.SetActive(false);
        if(m_gP1)
            m_gP1.SetActive(false);
        if (m_gP2)
            m_gP2.SetActive(false);

        int nPlanetID = Space_Map.Instance.GetPlanetID();

        int nStage = (nPlanetID - 1) * 6 + m_nNumber;
        if (nStage == Space_Map.Instance.GetStageID())
        {
            if (m_gSelect)
                m_gSelect.SetActive(true);
        }
        if (CWHeroManager.Instance.IsEndTask(nStage))
        {
            if (m_gP1)
                m_gP1.SetActive(true);
        }
        else
        {
            if (m_gP2)
                m_gP2.SetActive(true);
        }

    }
    IEnumerator IRun()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3f);
            UpdateData();

        }
    }

}
