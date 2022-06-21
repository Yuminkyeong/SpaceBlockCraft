using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreDlg : WindowUI<StoreDlg>
{

    public GameObject [] FirstUI;
    public GameObject [] Storelist;
    public GameObject [] ToggleButtons;
    int m_nSelect = 0;

    public void Show(int nSelect=0)
    {
        Open();
        OnSelectItem(nSelect);
  //      SelectActiveUI();
//
    }

    public void SelectActiveUI()
    {
        Storelist[m_nSelect].SetActive(true);
    }
    public override void Open()
    {
        foreach (var v in Storelist) v.SetActive(false);
        foreach (var v in ToggleButtons) v.SetActive(false);
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

    #region 버튼들
    public void OnSelectItem(int num)
    {
        if ( num != 0)
        {
            CWResourceManager.Instance.PlaySound("button1");

        }

        foreach (var v in Storelist) v.SetActive(false);
        foreach (var v in ToggleButtons) v.SetActive(false);
        m_nSelect = num;

        ToggleButtons[m_nSelect].SetActive(true);
        Storelist[m_nSelect].SetActive(true);

    }
    #endregion

}
