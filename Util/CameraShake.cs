using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {


    public float m_fLifetime=0.5f;
    public float m_fPower=0.1f;

    Vector3 m_vPos;

    bool m_bFlag = false;
    void Start () {

	}
    private void OnEnable()
    {
        m_vPos = Camera.main.transform.localPosition;
        //   GameObject.Destroy(this, m_fLifetime);	
        m_bFlag = true;
        Invoke("Stopfuc", m_fLifetime);

    }
    void Stopfuc()
    {
        m_bFlag = false;
    }
    private void OnDestroy()
    {
        
    }
    void Update () {
        if (!m_bFlag) return;
        Camera.main.transform.localPosition = m_vPos + Random.insideUnitSphere * m_fPower;
    }
}
