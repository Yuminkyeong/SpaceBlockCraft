using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_WaitEdit : MonoBehaviour
{
    void Start()
    {

        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        while (true)
        {

            if (!GameEdit.Instance.IsShow())
            {
                CWProductionPage pt = GetComponentInParent<CWProductionPage>();
                pt.OnClose();
                break;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }
}
