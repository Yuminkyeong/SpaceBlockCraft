using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;

public class DayRewardDlg : WindowUI<DayRewardDlg>
{

    public GameObject[] m_Image;
    public void Show(int num)
    {

        foreach (var v in m_Image) v.SetActive(false);
        m_Image[num].SetActive(true);
        Open();
    }
    public override void Close()
    {
        DailyDlg.Instance.Open();
        base.Close();
    }
}
