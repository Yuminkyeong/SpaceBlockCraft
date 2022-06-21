using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideSlot : MonoBehaviour
{


    public string m_szColumn;
    public GameObject m_gvisible;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        StartCoroutine("IRun");
    }

    void UpdateData()
    {
        ScrollListUI gList=GetComponentInParent< ScrollListUI>();
        if (gList == null) return;
        SlotItemUI gSlot = GetComponentInParent<SlotItemUI>();
        if (gSlot == null) return;
        int values= gList.GetInt(gSlot.m_nNumber, m_szColumn);
        if(values==1)
        {
            m_gvisible.SetActive(true);
        }
        else
        {
            m_gvisible.SetActive(false);
        }


    }
    IEnumerator IRun()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3f);
            UpdateData();
        }
    }


}
