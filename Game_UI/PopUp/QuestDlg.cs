using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDlg: WindowUI<QuestDlg>
{

    public GameObject[] FirstUI;
    public GameObject[] Questlist;
  
    int m_nSelect = 0;

    public void Show(int nSelect = 0)
    {
        Open();
        OnSelectItem(nSelect);
        //      SelectActiveUI();
        //
    }

    public void SelectActiveUI()
    {
        Questlist[m_nSelect].SetActive(true);
    }
    public override void Open()
    {
        foreach (var v in Questlist) v.SetActive(false);
      
        base.Open();
        // StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        yield return null;
        foreach (var v in FirstUI)
        {
            v.SetActive(true);
        }

    }

    #region ¹öÆ°µé
    public void OnSelectItem(int num)
    {
        if (num != 0)
        {
            CWResourceManager.Instance.PlaySound("button1");

        }

        foreach (var v in Questlist) v.SetActive(false);
      
        m_nSelect = num;

      
        Questlist[m_nSelect].SetActive(true);

    }
    #endregion

}
