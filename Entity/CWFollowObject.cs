using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWFollowObject : MonoBehaviour
{
    public GameObject m_gTarget;
    public float m_fSpeed = 20f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_gTarget==null)return;
        Vector3 vEuler = gameObject.transform.eulerAngles;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, m_gTarget.transform.position,Time.deltaTime*m_fSpeed);
        vEuler.y = Mathf.Lerp(vEuler.y,m_gTarget.transform.eulerAngles.y, Time.deltaTime * m_fSpeed);
        gameObject.transform.eulerAngles = vEuler;

    }
}
