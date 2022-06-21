using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseDlg : WindowUI<PauseDlg>
{

    public override void Open()
    {
        CWGlobal.g_GameStop = true;
        base.Open();
    }
    public void OnContinue()
    {
        CWGlobal.g_GameStop = false;
        Close();
    }
    public void OnExitGame()
    {
        Close();
        GamePlay.Instance.OnExit();

        // 멀티라면
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.MULTI)
        {
            if (CollectInvenList.Instance != null)
                CollectInvenList.Instance.PutInven();
        }

        ///NoticeMessage.Instance.Show("채집한 블록은 인벤으로 이동됩니다.");
    }
    public void OnOption()
    {
        OptionDlg.Instance.Open();
    }
    public override void Close()
    {
        CWGlobal.g_GameStop = false;
        base.Close();
    }

}
