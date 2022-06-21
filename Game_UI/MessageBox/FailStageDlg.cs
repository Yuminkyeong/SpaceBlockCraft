using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;

using CWEnum;

public class FailStageDlg : WindowUI<FailStageDlg>
{

    #region 초보 유저 도움

    

    #endregion



    public GameObject m_gImage;
    public GameObject m_gTicket;

    





    public void Show(CBClose kClose)
    {
        CWHero.Instance.ResetHP();//
        CloseFuction = kClose;
        CWGlobal.g_GameStop = true;
        GamePlay.Instance.SetShow(false);
        Open();
        
        
    }
    void BeginerFunc()
    {
        CloseFuction = null;
        CWProductionRoot pt = CWResourceManager.Instance.GetProduction("GameBegner");
        pt.Begin(_CBClose);
        Close();

    }
    public override void UpdateData(bool bselect=true)
    {
        m_gImage.transform.DOShakePosition(2,60,20);

        base.UpdateData(bselect);
    }
    void _CBClose()
    {
       
        CloseFuction = null;
    }
    public void OnClickClose()
    {
        // #광고

        //GamePlay.Instance.SetShow(true);
        //GamePlay.Instance.Close();
        //Space_Map.Instance.Show();

        base.Close();
    }

    void _RunMap()
    {

        CWHero.Instance.ResetHP();//
        GamePlay.Instance.ReShow();

    }
    void ShowAD()
    {
        
        CWADManager.Instance.RewardShow(() => {
            _RunMap();
        });

    }
    void MainHome()
    {
        //Space_Map.Instance.Show(100);
        Close();
    }
    // 재 도전
    public void OnEnter()
    {
        CloseFuction = null;
        Close();
        // 입장권이 있는지 검사 
        int tcnt = CWCoinManager.Instance.GetCoin(COIN.TICKET);
        if (tcnt == 0)
        {
            MessageBoxDlg.Instance.Show(ShowAD, MainHome, "입장권이 없습니다!", " 광고보고 입장하시겠습니까?");
            return;
        }
        NoticeMessage.Instance.Show("입장권 1개를 사용합니다.");
        CWSocketManager.Instance.UseCoinEx(COIN.TICKET, -1);
        _RunMap();

    }
    public void OnEnchant()
    {
        if (CWGlobal.g_bGameBegin)
        {
            CWGlobal.g_bCheckValue1 = true;
        }

        CloseFuction = null;
        Close();
      //  AircraftDlg.Instance.Open();

    }

    public override void Close()
    {
        base.Close();
        GamePlay.Instance.SetShow(false);
        GamePlay.Instance.Close();
        CWGlobal.g_GameStop = false;
    }
}
