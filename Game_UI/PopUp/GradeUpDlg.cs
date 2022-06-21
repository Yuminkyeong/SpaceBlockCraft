
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CWEnum;

public class GradeUpDlg : WindowUI<GradeUpDlg>
{
    public void Show(int num)
    {
        if (num == 1)
            m_nSelectMode = 0;
        else
            m_nSelectMode = 1;
        Open();
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
       

        base.Close();
    }
}
