using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHPBar : MonoBehaviour
{
    const int LIFETIMER = 30;
    float m_fTimer;
    void Start()
    {
        m_fTimer = Time.time;
    }
    void Update()
    {
        float ff = Time.time - m_fTimer;
        if(ff> LIFETIMER)
        {
            Destroy(gameObject);
            return;
        }
        CWObject kUser = gameObject.GetComponentInParent<CWObject>();
        if (kUser == null) return;
        Slider kk=gameObject.GetComponentInChildren<Slider>();
        kk.value = kUser.GetHpRate();
    }

    public void SetUpdate()
    {
        m_fTimer = Time.time;
    }
}
