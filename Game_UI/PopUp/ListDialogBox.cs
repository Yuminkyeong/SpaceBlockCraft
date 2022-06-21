using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CWEnum;
public class ListDialogBox : WindowUI<ListDialogBox>
{
    #region Override 함수
    public override void OnButtonClick(int num)
    {
    //    MessageBoxDlg.Instance.Show(null, null, "메세지",string.Format("{0} 슬롯이 눌렸습니다",num));
        base.OnButtonClick(num);
    }
    public override void OnEscKey()
    {

        base.OnEscKey();
    }
    public override void OnBuy(int num)
    {
     ///   MessageBoxDlg.Instance.Show(null, null, "메세지", string.Format("{0} 을 구입하였습니다", num));
        base.OnBuy(num);
    }
    public override void OnSelect(int num)
    {
      //  MessageBoxDlg.Instance.Show(null, null, "메세지", string.Format("{0} 슬롯이 선택되었습니다", num));
        base.OnSelect(num);
    }
    protected override void OnOpen()
    {
        base.OnOpen();
    }
    #endregion
    
    #region 외부 호출함수


    public void OnPopUP()
    {
        TestPopup.Instance.Open();

    }

    #endregion


}
