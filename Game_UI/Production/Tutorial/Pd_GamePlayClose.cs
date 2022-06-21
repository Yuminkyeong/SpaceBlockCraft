using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_GamePlayClose : MonoBehaviour
{



    void Start()
    {

        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        while(true)
        {

            if(!GamePlay.Instance.IsShow())
            {
                CWProductionPage pt = GetComponentInParent<CWProductionPage>();
                pt.OnClose();
                break;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }
}
