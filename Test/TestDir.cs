using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDir : MonoBehaviour
{

    public Vector3 m_vForward;
    public Vector3 m_vUp;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_vForward = transform.forward;
        m_vUp = transform.up;
    }

}
