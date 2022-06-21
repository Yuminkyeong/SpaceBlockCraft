using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharDetect : MonoBehaviour
{
  
    private void OnTriggerEnter(Collider other)
    {
        GetComponentInParent<CharicAction>().m_bDetected = true;

       // Debug.Log("OnTriggerEnter(Collider other)");
    }
    private void OnTriggerExit(Collider other)
    {
        GetComponentInParent<CharicAction>().m_bDetected = false;
        GetComponentInParent<CharicAction>().m_bStay = false;
       // Debug.Log("OnTriggerExit(Collider other)");
    }
    private void OnTriggerStay(Collider other)
    {
     //   Debug.Log("OnTriggerStay(Collider other)");
        GetComponentInParent<CharicAction>().m_bStay = true;
    }
}
