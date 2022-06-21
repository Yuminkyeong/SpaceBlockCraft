using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWRotate : MonoBehaviour {

    public float m_fSpeed = 10f;
    public Vector3 m_vAnlge;
    public void FixedUpdate()
    {
        Vector3 v = transform.eulerAngles;
        v += m_vAnlge * Time.deltaTime * m_fSpeed;
        transform.eulerAngles = v;

    }

}
