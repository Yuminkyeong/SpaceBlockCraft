using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LoadingDlg : WindowUI<LoadingDlg>
{
    bool m_bMapLoad = false;
    public Image m_kFadeImage;

    public override void Open()
    {
        
         StartCoroutine("IRun");
        m_kFadeImage.DOFade(1, 0);

        base.Open();
    }

    public void Show(bool bMapLoad)
    {
        m_bMapLoad = bMapLoad;
        Open();
    }

    IEnumerator IRun()
    {
        yield return new WaitForSeconds(0.1f);
        while (m_bMapLoad)
        {
            if (CWMapManager.SelectMap.IsLoadEndTask())
            {

                break;
            }
            yield return null;
            
        }
        m_kFadeImage.DOFade(0, 2f);

        yield return new WaitForSeconds(0.5f);

        Close();
    }

}
