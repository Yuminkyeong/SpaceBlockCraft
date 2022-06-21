using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;

using CWEnum;

public class UserAppearDlg : WindowUI<UserAppearDlg>
{

    public Text m_kText;

    public void Show(string szText)
    {
        m_kText.text = szText;
        Open();
    }
    public override void Close()
    {
        m_visible[m_nSelectMode].transform.DOLocalMoveX(0, 0f);
        base.Close();
    }
    protected override void _Open()
    {
        base._Open();



        m_visible[m_nSelectMode].transform.DOLocalMoveX(-785, 0.5f).OnComplete(()=> {

            m_visible[m_nSelectMode].transform.DOLocalMoveX(-785, 3f).OnComplete(() => {
                m_visible[m_nSelectMode].transform.DOLocalMoveX(0, 0.5f).OnComplete(()=> {

                    Close();
                });
            });
        });
    }



}
