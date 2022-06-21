using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTest : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //        m_Rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
        //m_Rigidbody.isKinematic = true;

    }
    private void OnTriggerExit(Collider other)
    {
        //m_Rigidbody.isKinematic = false;
        
    }


}
