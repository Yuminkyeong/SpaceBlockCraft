using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Lifetime : MonoBehaviour
{
    
    public delegate void CloseFunction(GameObject gg);

    CloseFunction CBFunc;

    float m_fLifetime;
    public void Begin(float fTime, CloseFunction cc)
    {
        m_fLifetime = fTime;
        CBFunc = cc;
        gameObject.SetActive(true);

        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.Clear();
        }
        ParticleSystem[] ss = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (var v in ss)
        {
            var trails= v.trails;
            trails.enabled = false;
            trails.ratio = 0.5f;
            v.Clear();
            v.Stop();
            v.Play();
        }


        StopAllCoroutines();
        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        yield return null;
        CWPlaySound ps = gameObject.GetComponent<CWPlaySound>();
        if(ps)
        {
            ps.Play();
        }
        yield return new WaitForSeconds(m_fLifetime);
        if (CBFunc != null)
        {
            CBFunc(gameObject);
        }
        gameObject.SetActive(false);

    }


}
