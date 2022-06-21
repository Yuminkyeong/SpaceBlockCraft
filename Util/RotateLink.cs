using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class RotateLink : MonoBehaviour
{
    public GameObject m_LinkPostion;
    public string GlobalValue;
    void Start()
    {
        if (CWLib.IsString(GlobalValue))
        {
            m_LinkPostion = CWGlobal.FindObject(GlobalValue);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (m_LinkPostion == null) return;
         transform.rotation = m_LinkPostion.transform.rotation;
    }
}
