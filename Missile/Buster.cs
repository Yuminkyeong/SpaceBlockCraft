using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buster : MonoBehaviour
{
    public GameObject m_kfowardBurner;
    public GameObject m_kBackBurner;

    bool m_bParticlePlay = false;
    void SetActive(bool bflag)
    {
        m_kfowardBurner.SetActive(bflag);
        m_kBackBurner.SetActive(!bflag);
    }
    public void Begin(float fy)
    {
        bool bFoward = true;
        if (fy < 0) bFoward = false;
        SetActive(bFoward);

        if (m_bParticlePlay) return;
        m_bParticlePlay = true;
        var systems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in systems)
        {
            ParticleSystem.MainModule mainModule = system.main;
            system.Clear();
            system.Play();
        }
        //CWResourceManager.Instance.PlaySound("Turbo_Buster");
    }
    public void Stop()
    {
        m_bParticlePlay = false;
        m_kfowardBurner.SetActive(false);
        m_kBackBurner.SetActive(false);
    }
    void CloseFuc()
    {
        m_bParticlePlay = false;
        m_kfowardBurner.SetActive(false);
        m_kBackBurner.SetActive(false);
    }

    private void OnEnable()
    {
        m_bParticlePlay = false;
        m_kfowardBurner.SetActive(false);
        m_kBackBurner.SetActive(false);
    }
}
