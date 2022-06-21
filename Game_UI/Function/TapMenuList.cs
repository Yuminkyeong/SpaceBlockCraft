using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapMenuList : MonoBehaviour
{

    public delegate void CBClickEvent(int number);
    public CBClickEvent CBClickFunction;

    public List<string> m_kList;
    Transform m_kFirstChild;
    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;
        MakeList();
    }
    void OnClick(int number)
    {
        CBClickFunction?.Invoke(number);
    }
    public void MakeList()
    {
        if (m_kList.Count == 0) return;
        m_kFirstChild = transform.GetChild(0);
        List<Transform> kTemp = new List<Transform>();
        for (int i=1; i<transform.childCount;i++)
        {
            kTemp.Add(transform.GetChild(i));
        }
        foreach(var  v in kTemp)
        {
            Destroy(v.gameObject);
        }
        int number = 0;

        {
            CWButton cs = m_kFirstChild.GetComponent<CWButton>();
            cs.m_nNumber = number++;
            cs.CBClickFunction = OnClick;
            Text tt = m_kFirstChild.GetComponentInChildren<Text>();
            if (tt)
            {
                tt.text = m_kList[0];
            }
        }
        for(int i=1;i<m_kList.Count;i++)
        {
            Transform tChild = Instantiate(m_kFirstChild,transform);
            tChild.name = m_kList[i];
            CWButton cs = tChild.GetComponent<CWButton>();
            Text tt = tChild.GetComponentInChildren<Text>();
            if(tt)
            {
                tt.text = CWLocalization.Instance.GetLanguage(m_kList[i]);
            }
            cs.m_nNumber = number++;
            cs.CBClickFunction = OnClick;
        }


    }
   
    
}
