using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_AllKillCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CWMobManager.Instance.IsAllKill())
        {
            CWProductionPage pt = GetComponentInParent<CWProductionPage>();
            pt.OnClose();

        }

    }
}
