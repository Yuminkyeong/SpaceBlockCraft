using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class MailPopupDlg : WindowUI<MailPopupDlg>
{

    //메일 수령

    public Text m_kTitle;
    public Text m_kMessage;
    public Text m_kDate;

    public RawImage [] m_kImage;
    public Text [] m_kTextg;


    List<int> m_kItem = new List<int>();
    List<int> m_kItemCount = new List<int>();

    int m_nMailID = 0;
    public override void UpdateData(bool bselect = true)
    {


        base.UpdateData(bselect);
    }
    public void OnMail()
    {

        
        CWSocketManager.Instance.UseMail(m_nMailID, (jData)=>{

            if (jData["Result"].ToString() == "ok")
            {
                CWCoinManager.Instance.SetData(jData["Coins"]);
                int ticket = 0;
                int gold = 0;
                for(int i=0;i<m_kItem.Count;i++)
                {
                    if (m_kItem[i] == 0) continue;
                    if (m_kItemCount[i] == 0) continue;
               
                    if (m_kItem[i] == (int)GITEM.Ticket)
                    {
                        ticket = m_kItemCount[i];
                        continue;
                    }
                    if (m_kItem[i] == (int)GITEM.GoldBlock)
                    {
                        gold = m_kItemCount[i];
                        continue;
                    }

                    CWInvenManager.Instance.AddItem(m_kItem[i], m_kItemCount[i]);

                }
                if(ticket>0)
                {
                    CWSocketManager.Instance.UseCoin(COIN.TICKET, ticket, (jj) => {
                        CWCoinManager.Instance.SetData(jj["Coins"]);
                        MailDlg.Instance.UpdateData();
                        NoticeMessage.Instance.Show("선물을 받았습니다!", 2);
                        Close();
                    }, "UseCoin2");

                }
                else
                {
                    MailDlg.Instance.UpdateData();
                    NoticeMessage.Instance.Show("선물을 받았습니다!", 2);
                    Close();
                }
                if (gold > 0)
                {
                    CWSocketManager.Instance.UseCoin(COIN.GOLD, gold, (jj) => {
                        CWCoinManager.Instance.SetData(jj["Coins"]);
                        MailDlg.Instance.UpdateData();
                        NoticeMessage.Instance.Show("선물을 받았습니다!", 2);
                        Close();
                    }, "UseCoin2");

                }
                else
                {
                    MailDlg.Instance.UpdateData();
                    NoticeMessage.Instance.Show("선물을 받았습니다!", 2);
                    Close();
                }


            }
            else
            {
                print("shop fail!!");
            }

        }, "UseData");
        
        
    }
    public void Show(int mailID,string szTitle, string szMessage,string szDate,int nCash,List<int> kItem,List<int> kCount)
    {
        m_kItem = kItem;
        m_kItemCount = kCount;
        m_kTitle.text = szTitle;
        m_kMessage.text = CWLib.ChangeString(szMessage,"@User",CWHero.Instance.name);
        m_kDate.text = szDate;
        m_nMailID = mailID;

        foreach(var v in m_kImage)
        {
            v.transform.parent.gameObject.SetActive(false);
        }

        int nCount = 0;
        if(nCash>0)
        {

            m_kImage[nCount].transform.parent.gameObject.SetActive(true);
            m_kImage[nCount].texture = CWResourceManager.Instance.GetItemIcon(40);
            m_kTextg[nCount].text = nCash.ToString();
            nCount++;
        }


        for (int i=0;i< kItem.Count;i++)
        {
            int nItem= kItem[i];
            int nItemCount = kCount[i];
            m_kImage[nCount].transform.parent.gameObject.SetActive(true);
            m_kImage[nCount].texture = CWResourceManager.Instance.GetItemIcon(nItem);
            m_kTextg[nCount].text = nItemCount.ToString();
            nCount++;
        }
        
        Open();
    }
  
}
