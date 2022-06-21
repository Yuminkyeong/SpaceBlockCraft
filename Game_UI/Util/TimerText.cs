using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class TimerText : MonoBehaviour
{


    public float m_fMaxTime = 30;

    Color m_kColor = Color.white;
    float m_fStart;
    void Start()
    {
        m_fStart = Time.time;
        Text tt = GetComponent<Text>();
        m_kColor=tt.color;
    }
    void Update()
    {
        Text tt = GetComponent<Text>();
        float ff = m_fMaxTime - (Time.time - m_fStart);
        if(ff<=0)
        {
            Destroy(this, 0.1f);
            return;
        }
        
        tt.text = ((int)ff).ToString();//CWLib.GetTimeString(ff);

        if (ff <= 5)
        {

            tt.color = Color.red;
        }

    }
}
