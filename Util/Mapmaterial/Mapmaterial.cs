using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class Mapmaterial : MonoBehaviour
{
    public GameObject m_gLight;
    public Vector3 UP;
    void Start()
    {
        
    }
/*
    void Update()
    {
        UP = transform.up;

        
        //if (transform.up.y==1)// 위로 보는 개념
        if(CWMath.IsEqual(transform.up.y, 1, 0.1f))
        {
            m_gLight.SetActive(true);
            Renderer rr = GetComponent<Renderer>();
            if (rr != null)
            {
                rr.material.color = Color.white;
//                rr.material.SetColor("_TintColor", nColor);
            }

        }
        else
        {
            m_gLight.SetActive(false);
        }
        //
        //if (transform.up.z == -1)//
        if(CWMath.IsEqual(transform.up.z, -1, 0.1f))
        {
            Renderer rr = GetComponent<Renderer>();
            if (rr != null)
            {
                rr.material.color = new Color(0.5f, 0.5f, 0.5f);
                //                rr.material.SetColor("_TintColor", nColor);
            }

        }
        //if (transform.up.x == -1)//
        if(CWMath.IsEqual(transform.up.x, 1, 0.1f))
        {
            Renderer rr = GetComponent<Renderer>();
            if (rr != null)
            {
                rr.material.color = new Color(0.8f, 0.8f, 0.8f);
                //                rr.material.SetColor("_TintColor", nColor);
            }

        }

    }
*/
}
