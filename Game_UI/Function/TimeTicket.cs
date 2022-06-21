using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
using DG.Tweening;
using CWEnum;
public class TimeTicket : MonoBehaviour
{

   

    Image m_kBar;
    Text m_ktime;
    GameObject m_visible;

    private void OnEnable()
    {
        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        yield return new WaitForSeconds(2f);
        while(true)
        {
            yield return null;
            if (CoininfoDlg.Instance!=null)
            {
                break;
            }
        }
        GameObject gbar = CWLib.FindChild(CoininfoDlg.Instance.gameObject, "time_Fill");
        GameObject gtime = CWLib.FindChild(CoininfoDlg.Instance.gameObject, "timeNumberText");

        m_kBar = gbar.GetComponent<Image>();
        m_ktime = gtime.GetComponent<Text>();

        m_visible = CWLib.FindChild(CoininfoDlg.Instance.gameObject, "time_visible");

        float fstart = Time.time;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (!CWGlobal.G_bGameStart)
            {
                continue;
            }
            float ff  = Time.time - fstart;
            float fr = CWGlobal.GET_TICKETTIME - ff;
            if (fr < 0) fr = 0;
            

            int tcnt = CWCoinManager.Instance.GetCoin(COIN.TICKET);
            if(tcnt< CWGlobal.TICKETTIME_MAX)
            {

                m_visible.SetActive(true);
                m_kBar.fillAmount = ff / CWGlobal.GET_TICKETTIME;
                m_ktime.text = string.Format("{0} ({1}/{2})", CWLib.GetTimeString(fr), tcnt, CWGlobal.TICKETTIME_MAX);
                if (ff >= CWGlobal.GET_TICKETTIME)
                {
                    
                    
                    CWSocketManager.Instance.UseCoinEx(COIN.TICKET, 1, () =>
                    {
                        StartCoroutine("IRun");
                    });
                    break;
                }
            }
            else
            {

                m_visible.SetActive(false);
            }
            
        }
    }
}
