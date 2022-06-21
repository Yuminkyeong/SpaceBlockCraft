using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWStruct;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class CharProfileDlg : WindowUI<CharProfileDlg>
{

    int m_nPreNumber = 0;
    public override void Open()
    {
        base.Open();
        m_nPreNumber = CWHeroManager.Instance.m_nCharNumber;
    }
    public override void OnSelect(int num)
    {
        if (!m_bShow) return;
        m_nPreNumber = num + 1;
        base.OnSelect(num);
    }
    void Save()
    {
        if (m_nPreNumber == CWHeroManager.Instance.m_nCharNumber) return;


        CWSocketManager.Instance.UseCoinEx(COIN.GEM, -CWGlobal.CHAR_PRICE, () => {
            CWSocketManager.Instance.UpdateUser("CharNumber", CWHeroManager.Instance.m_nCharNumber.ToString());
            CWChHero.Instance.SettingChar(CWHeroManager.Instance.m_nCharNumber);
            CWHeroManager.Instance.m_nCharNumber= m_nPreNumber;
        });

        Close();
    }

    public void OnBuy()
    {

        MessageBoxDlg.Instance.Show(Save,Close,"","구입하시겠습니까?");

        
    }
}
