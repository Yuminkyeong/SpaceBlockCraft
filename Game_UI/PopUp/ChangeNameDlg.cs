using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWEnum;
using CWStruct;
public class ChangeNameDlg : WindowUI<ChangeNameDlg>
{

    public InputField m_kInput;
    public Text m_kCoinText;
    public override void Open()
    {
        m_kInput.text = CWHero.Instance.name;
        m_kCoinText.text = CWGlobal.RENAMEPRICE.ToString();
        base.Open();
    }
    public void OnChange()
    {
        if(m_kInput.text.Length<2)
        {
            NoticeMessage.Instance.Show("2자 이상입니다");
            return;
        }
        if (m_kInput.text.Length > 8)
        {
            NoticeMessage.Instance.Show("8자 이하입니다");
            return;
        }
        CWSocketManager.Instance.SendName(m_kInput.text, (jData) => {

            if (jData["Result"].ToString() == "ok")
            {
                CWSocketManager.Instance.UseCoinEx(COIN.GEM, -CWGlobal.RENAMEPRICE, () => {

                    CWHero.Instance.name = m_kInput.text;
                    NoticeMessage.Instance.Show("이름이 변경되었습니다!");

                    Close();

                });
            }
            else
            {
                NoticeMessage.Instance.Show("동일한 이름이 존재합니다");
            }

        });
        

    }
}
