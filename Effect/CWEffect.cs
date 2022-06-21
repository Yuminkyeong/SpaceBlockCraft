using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWEffect : MonoBehaviour {


    public float m_fLifetime = 2f;
    public bool m_bPhysics = false;
    public bool m_Direction;
    public float multiplier = 1;
    public float explosionForce = 4;

    void CloseFuc()
    {
        transform.parent= CWEffectManager.Instance.transform;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void Begin(Vector3 vPos, Transform tParent,float fTime=0)
    {
        if(tParent)
        {
            transform.parent = tParent;
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.position = vPos;
        }

        gameObject.SetActive(true);

        if(fTime>0)
        {
            // 둘중에 최저로한다
            if(m_fLifetime > fTime)
            {
                m_fLifetime = fTime;
            }
        }

        if(m_fLifetime>0)
        Invoke("CloseFuc", m_fLifetime);

        
        if (m_Direction && tParent)
        {
            transform.forward = tParent.forward;
        }
        var systems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in systems)
        {
            ParticleSystem.MainModule mainModule = system.main;
            mainModule.startSizeMultiplier *= multiplier;
            mainModule.startSpeedMultiplier *= multiplier;
            mainModule.startLifetimeMultiplier *= Mathf.Lerp(multiplier, 1, 0.5f);

            
            system.Clear();

            

            system.Play();
        }

        AudioSource aa = transform.GetComponentInChildren<AudioSource>();
        if (aa != null)
        {
            aa.volume = CWPrefsManager.Instance.GetFX();
        }
        if(m_bPhysics)
        {
            StartCoroutine("Run");
        }
        
    }

    private IEnumerator Run()
    {
        // wait one frame because some explosions instantiate debris which should then
        // be pushed by physics force
        yield return null;


        float r = 10 * multiplier;
        var cols = Physics.OverlapSphere(transform.position, r);
        var rigidbodies = new List<Rigidbody>();
        foreach (var col in cols)
        {
            if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
            {
                rigidbodies.Add(col.attachedRigidbody);
            }
        }
        foreach (var rb in rigidbodies)
        {
            rb.AddExplosionForce(explosionForce * multiplier, transform.position, r, 1 * multiplier, ForceMode.Impulse);
        }
    }

}
