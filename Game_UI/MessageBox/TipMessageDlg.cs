using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TipMessageDlg : MessageWindow<TipMessageDlg>
{
    public Text m_Message;
    public float m_fStart=590;
    public float m_fEnd=-1604;
    public float m_fTime = 8f;
    public void Show(string szMessage, CBClose Function = null)
    {
        
        if (m_bShow) return;
        CloseFuction = Function;
        m_Message.text = szMessage;
        m_Message.transform.localScale = new Vector3(1, 0, 1);
        m_Message.transform.DOScaleY(1, 0.4f);
        base.Open();

        transform.DOLocalMoveX(590,0).OnComplete(()=> {

            transform.DOLocalMoveX(-1604, m_fTime).OnComplete(() => {
                Close();
            });
        });
    }

}
