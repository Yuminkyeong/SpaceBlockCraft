using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class TransNumber : MonoBehaviour {

    string m_szOrg="";
    public string szFormat = "{0:0,0}";
    private void Awake()
    {
    }
    string ConvertString(string szText)
    {
        if(szText.Length<=3)
        {
            return szText;
        }
        return string.Format(szFormat, CWLib.ConvertInt(szText));
    }
    private void OnEnable()
    {
        StartCoroutine("IRun");
    }
    public void UpdateTrans()
    {
        Text tt = GetComponent<Text>();
        tt.text = ConvertString(tt.text);
        m_szOrg = tt.text;
    }
    IEnumerator IRun()
    {
        Text tt = GetComponent<Text>();
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (tt.text!=m_szOrg)
            {
                UpdateTrans();
                break;
            }
        }
    }

}
