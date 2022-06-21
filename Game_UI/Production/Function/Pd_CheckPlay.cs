using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_CheckPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!GamePlay.Instance.IsShow())
        {
            CWProductionPage pt = GetComponentInParent<CWProductionPage>();
            pt.OnClose();

            

        }

    }
}
