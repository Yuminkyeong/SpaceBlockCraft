using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeGraph : MonoBehaviour
{

    public AnimationCurve m_kCurve;

    public void Making(int nRange)
    {
        string szValues="";
        for(int i=1;i<= nRange; i++)
        {
            float fRate = (float)i / nRange;
            float fval= m_kCurve.Evaluate(fRate);

            szValues += fval.ToString();
            szValues += "\n";
        }
        print(szValues);

    }
}
