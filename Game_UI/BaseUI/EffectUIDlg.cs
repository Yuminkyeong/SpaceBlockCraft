using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectUIDlg<T> : WindowUI<T>
{
    

    public override void SetShow(bool bflag)
    {

        for (int i = 0; i < m_visible.Length; i++)
        {
            if (m_visible[i] == null) continue;
            m_visible[i].SetActive(bflag);
        }
    }

}
