using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 주인공 맞을 때 리액션

public class CWHitEffect : CWSingleton<CWHitEffect>
{

    public GameObject m_bvisible;
    public RawImage m_kHitImage;
    public float m_fLifetime = 5f;
    public float m_fPower = 0.1f;
    Color m_kColor = Color.red;

    


    private void Start()
    {
        m_bvisible.SetActive(false);
        m_kColor = new Color(1, 1, 1, 0);
    }
    public void OnEnable()
    {
        m_bvisible.SetActive(false);
    }
    public void Begin()
    {
        m_bvisible.SetActive(true);

        m_kColor.a =1- CWHero.Instance.GetHpRate();
        m_kHitImage.color = m_kColor;
        CWTweenColor kTcolor = m_bvisible.GetComponentInChildren<CWTweenColor>();
        kTcolor.Begin();
        
//        m_kHitImage2.DOColor(Color.red, 0.5f);

        StopAllCoroutines();
        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        //2초뒤에 없어짐
        float falpa = m_kColor.a;
        float fstarttime = Time.time;
        Vector3 vPos = Camera.main.transform.localPosition;
        while (true)
        {
            float fRate = m_fLifetime - (Time.time - fstarttime);
            if(fRate> m_fLifetime-0.4f)
            {
                // 카메라 쉐이크
                vPos = Camera.main.transform.localPosition;
                Camera.main.transform.localPosition = vPos + Random.insideUnitSphere * m_fPower;
            }
            if (fRate <= 0) break;//2~0 
            fRate = fRate / m_fLifetime;
            m_kColor.a = falpa * fRate;
            m_kHitImage.color = m_kColor;
            yield return null;
        }
        m_bvisible.SetActive(false);
    }


}
