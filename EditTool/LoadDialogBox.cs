using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDialogBox : WindowUI<LoadDialogBox>
{

    public delegate void ONSELECT(string szFile);

    public ONSELECT CBSelect;


    public override void OnButtonClick(int num)
    {
        base.OnButtonClick(num);
        string szfile = m_gScrollList.GetString(num, "NAME");
        CBSelect(szfile);
        Close();

    }
    public void Show(string szDir, ONSELECT select)
    {
        m_gScrollList.m_szValues = szDir;
        CBSelect = select;
        Open();

    }
}
