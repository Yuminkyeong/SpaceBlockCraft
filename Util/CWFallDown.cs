using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 바닥으로 떨어진다 
public class CWFallDown : MonoBehaviour
{

    public float m_fLifetime = 2f;
    void Start()
    {
        Rigidbody rr=GetComponent<Rigidbody>();
        if(rr==null)
        {
             rr = gameObject.AddComponent<Rigidbody>();
        }
        rr.useGravity = true;
        Destroy(this, m_fLifetime);
    }

}
