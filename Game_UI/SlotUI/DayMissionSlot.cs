using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
public class DayMissionSlot : MonoBehaviour
{
    public GameObject[] m_gType;
    public Text m_kTitle;
    public Text m_kSubTitle;

    public int m_nNumber;
    public void UpdateData()
    {
        m_kTitle.text = DayMissionDlg.Instance.GetMissionTitle(m_nNumber);
        m_kSubTitle.text = DayMissionDlg.Instance.GetMissionSubTitle(m_nNumber);

        foreach(var v in m_gType)
        {
            v.SetActive(false);
        }
        int nState = DayMissionDlg.Instance.GetState(m_nNumber);
        m_gType[nState].SetActive(true);

    }
    public void OnReset()
    {
        CWSocketManager.Instance.UseCoinEx(COIN.TICKET, -CWGlobal.MISSIONTICKET, () => {

                DayMissionDlg.Instance.ChangeMission(m_nNumber);
                UpdateData();

        });
        //int nState = DayMissionDlg.Instance.GetState(m_nNumber);
        //if(nState==1)
        //{
        //    CWSocketManager.Instance.UseCoinEx(COIN.GEM, CWGlobal.DAYMISSION, () => {

        //        DayMissionDlg.Instance.OnReward(m_nNumber);
        //        TakeItemDlg.Instance.ShowCoin(COIN.GEM, ()=> {
        //            CWADManager.Instance.RewardShow(() => {
        //                DayMissionDlg.Instance.ChangeMission(m_nNumber);
        //                UpdateData();

        //            });

        //        });

        //    });


        //}
        //else
        //{
        //    CWADManager.Instance.RewardShow(() => {

        //        DayMissionDlg.Instance.ChangeMission(m_nNumber);
        //        UpdateData();

        //    });

        //}
    }
    public void OnClick()
    {

        int nState = DayMissionDlg.Instance.GetState(m_nNumber);
        if(nState==1)
        {
            if (m_nNumber == 0)
            {
                CWSocketManager.Instance.UseCoinEx(COIN.TICKET, CWGlobal.DAYMISSIONTICKET, () => {
                    DayMissionDlg.Instance.OnReward(m_nNumber);
                    TakeItemDlg.Instance.ShowCoin(COIN.TICKET, null);
                });

            }
            else
            {
                CWSocketManager.Instance.UseCoinEx(COIN.GEM, CWGlobal.DAYMISSION, () => {

                    DayMissionDlg.Instance.OnReward(m_nNumber);
                    TakeItemDlg.Instance.ShowCoin(COIN.GEM, null);
                });

            }
        }
        UpdateData();
    }
}
