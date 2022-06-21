using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CWUnityLib;

public class ClickEventDlg : WindowUI<ClickEventDlg>
{
    public GameObject m_gRing;
    public void OnClick()
    {
        GameObject gg= GameObject.Instantiate(m_gRing);
        gg.transform.parent = transform;
        gg.transform.localPosition = CWMath.ConvertByMousePos(Input.mousePosition);
        gg.transform.localScale = Vector3.one;
        gg.transform.DOScale(4, 0.5f);
        Destroy(gg, 0.5f);
        
    }

    public override void Open()
    {
        base.Open();
        StartCoroutine("IRun");
    }

    IEnumerator IRun()
    {
        while(true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
            yield return null;
        }
    }

}
