using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
public class DailyDlg : WindowUI<DailyDlg>
{

    public override void Open()
    {
        
        //for (int i = 0; i < 7; i++)
        //{
        //    if(CWHeroManager.Instance.m_kDailyList[i]==1)
        //    {
                
        //        return;
        //    }
        //}

        base.Open();

    }
    protected override void _Open()
    {

        base._Open();


        for(int i=0;i<7;i++)
        {
            SlotItemUI ss = m_gScrollList.GetSlot(i);

            GameObject gg1 = CWLib.FindChild(ss.gameObject, "click_event");
            if (gg1)
            {
                gg1.SetActive(false);
            }
            GameObject gg2 = CWLib.FindChild(ss.gameObject, "check_complete");
            if (gg2)
            {
                gg2.SetActive(false);
            }

            GameObject gg5 = CWLib.FindChild(ss.gameObject, "bkimage2");
            if (gg5)
            {
                gg5.SetActive(true);
            }

            if (CWHeroManager.Instance.m_kDailyList[i] == 1)
            {
                GameObject gg3 = CWLib.FindChild(ss.gameObject, "click_event");
                if (gg3)
                {
                    gg3.SetActive(true);
                }
                GameObject gg4 = CWLib.FindChild(ss.gameObject, "bkimage2");
                if (gg4)
                {
                    gg4.SetActive(false);
                }

            }
            else if (CWHeroManager.Instance.m_kDailyList[i] == 2)
            {

                GameObject gg = CWLib.FindChild(ss.gameObject, "check_complete");
                if (gg)
                {
                    gg.SetActive(true);
                }

            }

            /*
                        if (i <= CWHeroManager.Instance.m_nDayCount-1)
                        {
                            if (CWHeroManager.Instance.m_kDailyList[i]==1)
                            {
                                GameObject gg3 = CWLib.FindChild(ss.gameObject, "click_event");
                                if (gg3)
                                {
                                    gg3.SetActive(true);
                                }
                                GameObject gg4 = CWLib.FindChild(ss.gameObject, "bkimage2");
                                if (gg4)
                                {
                                    gg4.SetActive(false);
                                }

                            }
                            else if (CWHeroManager.Instance.m_kDailyList[i] == 2)
                            {

                                GameObject gg = CWLib.FindChild(ss.gameObject, "check_complete");
                                if (gg)
                                {
                                    gg.SetActive(true);
                                }

                            }
                        }
            */
        }


    }

    void RewardItem(int num)
    {
        int nCount= CWTableManager.Instance.GetTableInt("DayCheck - 시트1", "reward_count",num);
        string szvalue = CWTableManager.Instance.GetTable("DayCheck - 시트1", "name", num);
        if(szvalue=="골드")
        {
            CWSocketManager.Instance.UseCoinEx(COIN.GOLD, nCount, () => {

            });
        }
        if (szvalue == "보석")
        {
            CWSocketManager.Instance.UseCoinEx(COIN.GEM, nCount, () => {

            });

        }
        if (szvalue == "입장권")
        {
            //
            CWSocketManager.Instance.UseCoinEx(COIN.TICKET, nCount, () => {

            });

        }
       
        if (szvalue == "패키지")
        {
            // 보석
            // 티켓
            // 블록 
            int gem= CWTableManager.Instance.GetTableInt("DayCheck - 시트1", "p_gem", num);
            int ticket= CWTableManager.Instance.GetTableInt("DayCheck - 시트1", "p_drink", num);
            int blockItem= CWTableManager.Instance.GetTableInt("DayCheck - 시트1", "p_block", num);
            int blockItemcnt= CWTableManager.Instance.GetTableInt("DayCheck - 시트1", "p_count", num);

            CWSocketManager.Instance.UseCoinEx(COIN.GEM, gem, () => {

                CWSocketManager.Instance.UseCoinEx(COIN.TICKET, ticket, () => {

                    CWInvenManager.Instance.AddItem(blockItem, blockItemcnt);

                });


            });


        }

    }
    public override void OnButtonClick(int num)
    {

        Close();
        CWSocketManager.Instance.SendUpdateDailCheck(num,(jData)=>{

            // 보상을 얻는다
            RewardItem(num+1);
            DayRewardDlg.Instance.Show(num);
        });
        base.OnButtonClick(num);

    }

}
