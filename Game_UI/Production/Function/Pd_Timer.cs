using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pd_Timer : MonoBehaviour
{
    public int Lifttime;
    public Text m_kNumber;

    void Start()
    {
        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {

        float fstart = Time.time;
        while(true)
        {
            float ff = Lifttime-(Time.time - fstart);
            int n2 = (int)ff;
            if (n2 == 0) break;
            m_kNumber.text = n2.ToString();
            yield return null;
        }
        CWProductionPage pt = GetComponentInParent<CWProductionPage>();
        pt.OnClose();

    }
}
