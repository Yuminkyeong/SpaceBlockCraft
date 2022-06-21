using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWLookAt : MonoBehaviour
{
    public GameObject m_gTarget;

    private void LateUpdate()
    {
        if (m_gTarget == null) return;

        transform.LookAt(m_gTarget.transform);
    }
}
