using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;
using Newtonsoft.Json.Linq;

public class SeasonDlg : WindowUI<SeasonDlg>
{

    public Text m_kPage;
    public Text m_kSeason;
    int _page=0;
    int Page
    {
        get
        {
            return _page;
        }
        set
        {
            _page = value;
            if (_page <= 0) _page = 0;

            m_kPage.text = _page.ToString();
            string m_szQueryRanking = @"{{Start: {0} }}";
            m_gScrollList.m_szDBParam = string.Format(m_szQueryRanking, Page);
            UpdateData();
        }

    }

    public override void ReceiveDB(JObject jData)
    {
        m_kSeason.text = jData["Season"].ToString();
    }

    public override void Open()
    {
        Page = 0;
       
        base.Open();
    }

    public void OnPrev()
    {
        Page--;
    }
    public void OnNext()
    {
        Page++;
    }

}
