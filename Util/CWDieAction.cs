using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CWDieAction : MonoBehaviour {


    public float m_fSpeed = 10f;
    public float m_fRotSpeed = 10f;
    public float m_fLiftime = 2f;
    public Action cbclose;
    void Start () {

        CWEffectManager.Instance.GetEffect( transform, "ShipSmoke", m_fLiftime);
        Destroy(this,m_fLiftime);
        //CWEffectManager.Instance.GetEffect(transform, "Ef_ExplosionParticle_01", m_fLiftime);
//        Destroy(this);

    }
    private void OnDestroy()
    {
      
        if (cbclose!=null)
            cbclose();
    }
    void Update () {

        transform.position += Vector3.down * Time.deltaTime * m_fSpeed;
        Vector3 vRot = transform.eulerAngles;
        vRot.y += m_fRotSpeed* Time.deltaTime;
        transform.eulerAngles =vRot;

	}
}
