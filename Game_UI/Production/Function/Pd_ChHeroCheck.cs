using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
public class Pd_ChHeroCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CWChHero.Instance.m_GroundState != DETECTTYPE.EXIT)
        {
            CWProductionPage pt = GetComponentInParent<CWProductionPage>();
            pt.OnClose();
        }
    }
}
