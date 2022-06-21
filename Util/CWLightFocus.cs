using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class CWLightFocus : MonoBehaviour
{
    public GameObject m_gTarget;
    public string m_szTarget;
    
    void Start()
    {
        CWMapManager.Instance.m_bHeroLight = false;
    }
    
    private void OnDisable()
    {
        if(CWMapManager.Instance)
            CWMapManager.Instance.m_bHeroLight = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (CWMapManager.Instance==null) return;
        if (!m_gTarget)
        {
            if (CWLib.IsString(m_szTarget))
            {
                m_gTarget = CWGlobal.FindObject(m_szTarget);
            }
            return;
        }
        CWMapManager.Instance.SetLight(m_gTarget.transform.position.x, m_gTarget.transform.position.z);
        
    }
}
