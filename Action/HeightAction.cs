using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightAction : MonoBehaviour
{
    public float m_fSpeed = 10f;
    CWJoystickCtrl m_kJoy;

    Rigidbody m_Rigidbody = null;
    public Rigidbody GetRigidbody()
    {
        if (m_Rigidbody == null)
        {
            m_Rigidbody = gameObject.GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
            {
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

                m_Rigidbody.freezeRotation = true;
                m_Rigidbody.drag = 0.6f;
                m_Rigidbody.angularDrag = 0.8f;
                m_Rigidbody.mass = 1;
            }
        }
        return m_Rigidbody;
    }

    void Start()
    {
        
        m_kJoy = GamePlay.Instance.GetHeightJoy();
    }
    public float GetDir()
    {
        return m_kJoy.RateY;
    }
    public float GetHeight()
    {
        return GetDir() * (m_fSpeed * Time.deltaTime);
    }

    void Move()
    {
        
        AircraftAction aa = GetComponent<AircraftAction>();
        if (aa==null) return;
       
        if (aa.GetDir() == Vector3.zero)
        {
            if (GetDir() == 0)
            {
                GetRigidbody().velocity = Vector3.zero;
                return;
            }
            Vector3 vPos = transform.position;
            vPos.y += GetDir() * (m_fSpeed * Time.deltaTime);
            GetRigidbody().MovePosition(vPos);
        }


    }
    private void LateUpdate()
    {
        if (!GamePlay.Instance) return;
        if (!GamePlay.Instance.IsGamePlay()) return;

        Move();
    }

}
