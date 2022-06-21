using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWFollowEffect : MonoBehaviour
{


    public float m_fLifetime = 1f;
    GameObject m_gTarget;

    public float m_fSpeed = 8.0f;

    bool m_bStart=false;

    public void Begin(GameObject gTarget,bool bBommb=false)
    {
        m_gTarget = gTarget;
        if (bBommb) // 폭팔 후에 
        {
            m_bStart = false;
            StartCoroutine("BombRun");
        }
        else
        {
            m_bStart = true;
        }
    }
    IEnumerator BombRun()
    {
        // 1. 램던하게 날아간다
        //2. 회전한다. 

        float fx = Random.Range(-0.4f, 0.4f);
        float fz = Random.Range(-0.4f, 0.4f);
        float fy = Random.Range(0.2f, 0.6f);


        Vector3 vDir = new Vector3(fx, fy, fz); //Random.insideUnitSphere;


        float fStart = Time.time;
        Vector3 rr = Random.insideUnitSphere;
        float fbmSpeed = 20f;
        while (true)
        {
            float ff = Time.time - fStart;
            if (ff > m_fLifetime) break;
            transform.position += vDir * fbmSpeed * Time.deltaTime;
            Vector3 v = transform.eulerAngles;
            v += rr * Time.deltaTime * m_fSpeed;
            transform.eulerAngles = v;

            fbmSpeed += 30f * Time.deltaTime;

            yield return null;
        }
        m_bStart = true;
    }
    private void OnEnable()
    {
        m_bStart = false;
        m_gTarget = null;
    }
    // Update is called once per frame
    void Update()
    {
        if (!m_bStart) return;
        if (m_gTarget == null) return;

        Vector3 vPos = m_gTarget.transform.position;

        Vector3 position = Vector3.Lerp(transform.position, vPos, 1.0f - Mathf.Exp(-m_fSpeed * Time.deltaTime));

        transform.position = position;
    }
}
