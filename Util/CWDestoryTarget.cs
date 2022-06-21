using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWDestoryTarget : MonoBehaviour
{
    public float time = 2.0f;
    public GameObject m_gTarget;

    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;
        Destroy(m_gTarget.gameObject, time);
    }
}
