using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADDialDlg : WindowUI<ADDialDlg>
{
    public bool IsAdBtn()
    {
        
        string szID = string.Format("{0}_0", m_gScrollList.m_szValues);
        DateTime tDate = CWHeroManager.Instance.GetStoreTimer(szID);
        TimeSpan ts = DateTime.Now - tDate;
        int nTimer =24;
        if (ts.TotalSeconds > 0.5 && ts.TotalSeconds < nTimer * 3600)
        {
            return false;
        }

        return true;
    }
    public override void Open()
    {
        if (!IsAdBtn()) return;
        base.Open();
    }
}
