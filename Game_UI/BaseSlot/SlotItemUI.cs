using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;
using CWEnum;

public class SlotItemUI : MonoBehaviour, IPointerClickHandler
{

    public delegate void CBClickEvent(int number);
    public CBClickEvent CBClickFunction;

    public int m_nNumber = 0;
    protected ScrollListUI m_gList;

    public GameObject m_gSelectBox;

    public int m_nItemID;

    bool m_bCreated;

    public int m_nGType = -1;// 구릅을 슬롯별로 나눈다
    public GameObject[] m_gGroupType;// 나누는 개념



    private void Awake()
    {
        if (m_gSelectBox)
            m_gSelectBox.SetActive(false);

    }
    public void Close()
    {
        
        if (m_gSelectBox)
            m_gSelectBox.SetActive(false);
        StopAllCoroutines();
    }
    public bool IsCreate()
    {
        return m_bCreated;
    }
    public void Create(ScrollListUI _gList, int num)
    {
       // Close();
        gameObject.SetActive(true);
        m_nNumber = num;
        m_gList = _gList;
        if(gameObject.activeInHierarchy==false)
        {
            return;
        }

        if(m_gList.IsData(num))
        {
            if(m_gList.m_bRefrshLoof)
            {
                StartCoroutine("IRun");
            }
            else
            {
                UpdateData();
            }
            
            
        }
        else
        {
            gameObject.SetActive(false);
        }
        m_bCreated = true;

    }


    IEnumerator IRun()
    {
        while (true)
        {
            bool bRet = UpdateData();
            if (bRet == false)
            {
                yield return new WaitForSeconds(2f);
            }
            yield return new WaitForSeconds(m_gList.m_fRefreshtime);
        }

    }

    
    public virtual void TypeFunction()
    {
        if (m_gGroupType == null) return;
        if (m_gGroupType.Length == 0) return;
        if(m_nGType==-1)
        {
            BaseUI bs = GetComponentInParent<BaseUI>();
            if (bs)
            {
                foreach (var v in m_gGroupType) v.SetActive(false);
                int num = bs.m_nGroupType - 1;
                if (num < 0) return;
                m_gGroupType[num].SetActive(true);
            }
        }
        else// 슬롯별로 나누는 개념
        {
            foreach (var v in m_gGroupType) v.SetActive(false);
            m_gGroupType[m_nGType].SetActive(true);
        }

    }

    public virtual bool UpdateData()
    {
        bool bRet = true;
        if (m_gList == null) return false;
        TypeFunction();

        Text [] tt =GetComponentsInChildren<Text>(true);
        foreach(var v in tt)
        {
            if(v.name.Contains("/"))
            {
                string[] ssz = v.name.Split('/');
                string sz1 = m_gList.GetString(m_nNumber, ssz[0]);// 대소문자 무시 
                string sz2 = m_gList.GetString(m_nNumber, ssz[1]);// 대소문자 무시 
                v.text = string.Format("{0}/{1}",sz1,sz2);
            }
            if (v.name == "Grade")
            {
                int rp= m_gList.GetInt(m_nNumber, "RankPoint");
                int gr = CWGlobal.GetGrade(rp);
                v.text = gr.ToString();
                continue;
            }

            if (v.name == "Ranking")
            {
                int nn = m_gList.GetInt(m_nNumber, v.name);
                if (nn <=3)
                {
                    v.text = "";
                }
                else
                {
                    v.text = m_gList.GetString(m_nNumber, v.name);// 대소문자 무시 
                }
                continue;

            }
            //// 시세에 따른 가격
            //if (v.name.Equals("Value", StringComparison.CurrentCultureIgnoreCase))// 
            //{
            //    int Shoptype = m_gList.GetInt(m_nNumber, "Type");// 
            //    if(Shoptype==1)// 골드 상점이라면
            //    {
            //        int nPrice = m_gList.GetInt(m_nNumber, "Price");// 대소문자 무시 
            //        v.text = CWGlobal.GetGoldbyPrice(nPrice).ToString();
            //    }
            //    else
            //    {
            //        v.text = m_gList.GetString(m_nNumber, v.name);// 대소문자 무시 
            //    }

            //    continue;
            //}
            if (v.name.Equals("Price", StringComparison.CurrentCultureIgnoreCase))// 
            {
                int nCoinType = m_gList.GetInt(m_nNumber, "CoinType");// 대소문자 무시
                if(nCoinType==3)
                {
                    // 광고라면
                    v.text = "";
                    continue;
                }
                int nPrice = m_gList.GetInt(m_nNumber, "Price");// 대소문자 무시 

                string szPID = m_gList.GetString(m_nNumber, "PID");// 대소문자 무시 

                if(!CWLib.IsString(szPID))
                {
                    szPID = m_gList.GetString(m_nNumber, "ProductId");// 대소문자 무시 
                }

                if (!CWLib.IsString(szPID))
                {
                    if (nPrice == 0)
                    {
                        v.text = CWLocalization.Instance.GetLanguage("무료");
                        continue;
                    }

                }

                v.text = CWGlobal.GetPrice(nPrice, nCoinType, szPID).ToString();
                continue;
            }
            if (v.name.Equals("Regdate", StringComparison.CurrentCultureIgnoreCase))// 
            {
                DateTime dt = m_gList.GetDateTime(m_nNumber, v.name);
                v.text = CWLib.GetStringDate(dt);
                continue;
            }
            // 남아 있는 시간
            if (v.name.Equals("Enddate", StringComparison.CurrentCultureIgnoreCase))// 
            {
                DateTime dt = m_gList.GetDateTime(m_nNumber, v.name);
                TimeSpan ts = dt - DateTime.Now;
                if(ts.TotalDays>300)
                {
                    v.text = "-";
                }
                else
                {
                    v.text = CWLib.GetStringDateBefore(dt);
                }
                
                continue;
            }
            if(v.name.Equals("UserTitle", StringComparison.CurrentCultureIgnoreCase))// 
            {
                string sz = m_gList.GetString(m_nNumber, v.name);// 대소문자 무시 
                v.text = CWLib.ChangeString(sz, "@User",CWHero.Instance.name);
                continue;
            }

            if (m_gList.IsRecord(m_nNumber, v.name))
            {
                v.text = m_gList.GetString(m_nNumber, v.name);// 대소문자 무시 
             
                continue;
            }

            //if (v.name.Equals("Level", StringComparison.CurrentCultureIgnoreCase))// 아이템 아이디로 검색 
            //{
            //    int nExp=m_gList.GetInt(m_nNumber, "Exp");
            //    if(nExp>0)
            //    {
            //        int lv = CWArrayManager.Instance.Getlevel(nExp);
            //        v.text = lv.ToString();

            //    }
            //}
        }
        //RawImage는 Face와 아이템,계급등으로 나눈다 
        //RawImage Face 아이템아이디, 일반텍스쳐로 나눔 
        //RawImage는 아이템과 유저 Face 혹은 계급이 존재 
        //데이타 베이스와 같은 이름일 경우 데이타베이스로 통일 
        //Icon,ItemID 으로 통일?

        Image [] ii = GetComponentsInChildren<Image>(true);
        foreach(var v in ii)
        {

            // 슬라이더개념 
            if(v.type== Image.Type.Filled)
            {
                if (m_gList.IsRecord(m_nNumber, v.name))
                {
                    v.fillAmount = m_gList.GetFloat(m_nNumber, v.name);//
                    continue;
                }
            }

            if (v.name == "Ranking")
            {
                int nn = m_gList.GetInt(m_nNumber, v.name);
                if(nn==1)
                {
                    v.sprite = CWResourceManager.Instance.GetSprite("icon_rank_gold");
                }
                else if (nn == 2)
                {
                    v.sprite = CWResourceManager.Instance.GetSprite("icon_rank_silver");
                }
                else if (nn == 3)
                {
                    v.sprite = CWResourceManager.Instance.GetSprite("icon_rank_bronze");
                }
                else
                {
                    v.sprite = CWResourceManager.Instance.GetSprite("icon_rank_default");
                }

                continue;
            }

            if (v.name == "CoinType")
            {
                int nn = m_gList.GetInt(m_nNumber, "CoinType");//
                string szstr= CWGlobal.GetCoinSpriteStr(nn);
                v.sprite = CWResourceManager.Instance.GetSprite(szstr);
                continue;
            }
            //개념: 'icon_coin'은 유니티 필드에있고, '코인유형'은 테이블에 있다 
            // 정해져 있음
            if (v.name== "icon_coin")
            {
                int nn = m_gList.GetInt(m_nNumber, "코인유형");//
                string str= CWGlobal.GetCoinSpriteStr(nn);
                v.sprite = CWResourceManager.Instance.GetSprite(str);
                continue;
            }
            if (!m_gList.IsRecord(m_nNumber, v.name)) continue;
            if(v.name=="Check")
            {
                string szval2 = m_gList.GetString(m_nNumber, v.name);//
                if(szval2=="true")
                {
                    v.sprite = CWResourceManager.Instance.GetSprite("Check");
                    
                }
                else
                {
                    v.sprite = CWResourceManager.Instance.GetSprite("UnCheck");
                }
                continue;
            }

            if (m_gList.IsRecord(m_nNumber, v.name))
            {
                string szval = m_gList.GetString(m_nNumber, v.name);//
                v.sprite = CWResourceManager.Instance.GetSprite(szval);

            }


        }

        RawImage[] rr = GetComponentsInChildren<RawImage>(true);
        foreach(var v in rr)
        {
            

            string szval = m_gList.GetString(m_nNumber, v.name);//
            //if (v.name.Equals("IconId", StringComparison.CurrentCultureIgnoreCase))// 유저 얼굴 데이타 베이스 컬럼과 동일하게 한다 
            //{
            //    int nId = CWLib.ConvertInt(szval);
            //    v.texture= CWResourceManager.Instance.GetFaceTexture(nId);
            //    if(v.texture==null)
            //    {
            //        bRet = false;
            //        v.texture = CWResourceManager.Instance.GetTexture("chobo_1");
            //    }
            //    continue;
            //}


            if (v.name.Equals("ItemID", StringComparison.CurrentCultureIgnoreCase))// 아이템 아이디로 검색 
            {
                int n = m_gList.GetInt(m_nNumber, v.name);// 대소문자 무시 
                if (n > 0)
                {
                    if(n==1001)
                    {
                        v.texture = CWResourceManager.Instance.GetTexture("Gold_1");
                        continue;
                    }
                    if (n == 1002)// 다이아
                    {
                        v.texture = CWResourceManager.Instance.GetTexture("Dia_1");
                        continue;
                    }
                    if (n == 1003)// 티켓
                    {
                        v.texture = CWResourceManager.Instance.GetTexture("Ticket_1");
                        continue;
                    }


                    v.texture = CWResourceManager.Instance.GetItemIcon(n);
                    m_nItemID = n; // 아이템 아이디 검출!!
                }
                    
                else v.texture = CWResourceManager.Instance.GetTexture("Translucent");


                continue;
            }
           
            if (v.name.Equals("Grade", StringComparison.CurrentCultureIgnoreCase))// 아이템 아이디로 검색 
            {

                int rp = m_gList.GetInt(m_nNumber, "RankPoint");
                int nGrade = CWGlobal.GetGrade(rp);

                v.texture = CWResourceManager.Instance.GetTexture(CWGlobal.GetGradeFileName(nGrade));
                continue;
            }

            if (v.name.Equals("TierCircle", StringComparison.CurrentCultureIgnoreCase))// 아이템 아이디로 검색 
            {

                szval = m_gList.GetString(m_nNumber, "Grade");//
                int nGrade = CWLib.ConvertInt(szval);
                v.texture = CWResourceManager.Instance.GetTexture(CWGlobal.GetGradeCircleFileName(nGrade));
                continue;
            }

            if (v.name.Equals("New", StringComparison.CurrentCultureIgnoreCase))// 새롭게 얻은 
            {
                int n = m_gList.GetInt(m_nNumber, v.name);// 대소문자 무시 
                if(n==1)
                {
                    v.texture = CWResourceManager.Instance.GetTexture("New");
                }
                else v.texture = CWResourceManager.Instance.GetTexture("Translucent");

                continue;
            }

            if (m_gList.IsRecord(m_nNumber, v.name))
            {
                
                v.texture = CWResourceManager.Instance.GetTexture(szval);
                if(v.texture==null)
                {
                    int nID= CWArrayManager.Instance.GetItemNumber(szval);
                    if(nID>0)
                    {
                        v.texture = CWResourceManager.Instance.GetItemIcon(nID);
                    }
                    
                }

                if (!CWLib.IsString(szval))
                {
                      v.texture = CWResourceManager.Instance.GetTexture("Empty");
                    continue;
                }


            }


        }
        // 타입별로 자동으로 나눈다 
        //TypeFunction();

        


        return bRet;
    }
    public virtual void OnSelectActive()
    {
        if (m_gSelectBox)
            m_gSelectBox.SetActive(true);
    }

    public void OnDeSelectActive()
    {
        if (m_gSelectBox)
            m_gSelectBox.SetActive(false);
    }
    public virtual void OnClick()
    {
        if (!m_gList) return;
        OnSelectActive();
        m_gList.OnButtonClick(this);
        if (m_nItemID > 0)
        {

            ValueUI.g_kSelectItemData = CWArrayManager.Instance.GetItemData(m_nItemID);
        }

        CBClickFunction?.Invoke(m_nNumber);
        CBClickFunction = null;

    }

    public virtual void OnSelect()
    {
        if (!m_gList) return;
        m_gList.OnSelect(this);
        if (m_nItemID > 0)
        {

            ValueUI.g_kSelectItemData = CWArrayManager.Instance.GetItemData(m_nItemID);
        }


    }
    public void OnPointerClick(PointerEventData data)
    {
        if (!m_gList) return;
        OnSelect();
       // m_gList.OnButtonClick(this);
    }
   
    private void OnDisable()
    {
        if (m_gSelectBox)
            m_gSelectBox.SetActive(false);

    }

    // 구입후 함수
    public virtual void ResultEvent()
    {

    }
    // 상점에서만 사용 
    //주의 할것  정확하게 정해진 내용만 처리가 된다
//    1: 골드
//2: 보석
//3: 아이템
//4: 티켓
//5:광고제거
//6:아이템 패키지



    public virtual void OnBuy()
    {

        m_gList.OnSelect(this);

        int ctype = m_gList.GetInt(m_nNumber, "CoinType");//
        string price = m_gList.GetString(m_nNumber, "Price");//
        string PID = m_gList.GetString(m_nNumber, "PID");//
        if(!CWLib.IsString(PID))
        {
            PID = m_gList.GetString(m_nNumber, "ProductId");//
        }
        if (CWLib.IsString(PID))
        {
            price= CWArrayManager.Instance.GetPrice(PID);
        }

        int Value = m_gList.GetInt(m_nNumber, "Value");//
        int Result = m_gList.GetInt(m_nNumber, "Result");//

        int Item = m_gList.GetInt(m_nNumber, "Item");//
        
        if(Item==0)
        {
            Item = m_gList.GetInt(m_nNumber, "ItemID");//
        }


        int Count = m_gList.GetInt(m_nNumber, "Count");//

        int Shoptype = m_gList.GetInt(m_nNumber, "Type");// 

        string szGoods = m_gList.GetString(m_nNumber, "title");//
        string szGoodsHelp2 = CWLocalization.Instance.GetLanguage(m_gList.GetString(m_nNumber, "Help"));//
        string szImage = m_gList.GetString(m_nNumber, "Icon");//

        if(Count>0)
        {
            Value = Count;
        }
        if(Count==0)
        {
            Count = 1;
        }
        string szGoodsHelp = string.Format(szGoodsHelp2,Value);


        if(Item>0&&Count==1)
        {
            // 무조건 아이템 Result
            Result = 3;
        }

        ValueUI.g_szPrice = price;
        ValueUI.g_nCoinType = ctype;
        ValueUI.g_szGoodsHelp = szGoodsHelp;
        ValueUI.g_szGoodsName = szGoods;

        if (Result==6)
        {

            string szStr1 = m_gList.GetString(m_nNumber, "ItemArray");//
            string[] szArray = szStr1.Split(',');

            string szStrCnt = m_gList.GetString(m_nNumber, "CountArray");//
            string[] szArrayCnt = szStrCnt.Split(',');

            PakageInfoDlg.Instance.Show(szGoods,szStr1, szStrCnt, szImage,  () => {

                CWGlobal.Buy(ctype, CWLib.ConvertInt(price), PID, () => {

                    if (Result == 6)// 아이템 패키지
                    {
                        TakeItemDlg.Instance.ShowImage(szImage, szGoods, szGoodsHelp2, ResultEvent);
                        int Ticket = 0;
                        int Gem = 0;
                        int Gold = 0;
                        for (int i = 0; i < szArray.Length; i++)
                        {
                            int nn = CWLib.ConvertInt(szArray[i]);
                            int cc = CWLib.ConvertInt(szArrayCnt[i]);
                            if(nn==(int)GITEM.Ticket)
                            {

                                Ticket=cc;
                                continue;
                            }
                            if (nn == (int)GITEM.Diamond)
                            {
                                Gem = cc;
                                continue;
                            }
                            if (nn == (int)GITEM.Gold)
                            {
                                Gold = cc;
                                continue;
                            }

                            if (nn==(int)GITEM.ADDel)
                            {

                                CWSocketManager.Instance.UpdateUser("AdDel", "1");
                                CWHeroManager.Instance.m_bAdDel = true;
                                continue;
                            }
                            CWInvenManager.Instance.AddItem(nn, cc);
                        }
                        if(Ticket>0||Gem>0 || Gold > 0)
                        {
                            CWSocketManager.Instance.UseCoin3( Ticket, Gem,Gold, CWSocketManager.UseCoin_ResultFuc, "UseCoin_ResultFuc");
                        }

                    }
                    m_gList.OnBuy(m_nNumber);

                });
            });

        }
        else
        {
            BuyMessageDlg.Instance.Show(szGoods, szGoodsHelp, szImage, Item, ctype, price, () => {

                CWGlobal.Buy(ctype, CWLib.ConvertInt(price), PID, () => {
                    m_gList.OnBuy(m_nNumber);
                    if (Result == 0)
                    {
                        // 사용자 정의 

                        m_gList.OnBuy(m_nNumber);// 물건 구입함수
                        ResultEvent();
                    }
                    if (Result == 1)
                    {
                        // 골드 
                        CWSocketManager.Instance.UseCoinEx(COIN.GOLD, Value, () => {

                            //NoticeMessage.Instance.Show("골드를 구입하였습니다");
                            TakeItemDlg.Instance.ShowCoin(COIN.GOLD, ResultEvent);
                        });
                    }
                    if (Result == 2)
                    {
                        CWSocketManager.Instance.UseCoinEx(COIN.GEM, Value, () => {
                            //NoticeMessage.Instance.Show("보석을 구입하였습니다");
                            TakeItemDlg.Instance.ShowCoin(COIN.GEM, ResultEvent);
                        });

                    }
                    if (Result == 4)
                    {
                        CWSocketManager.Instance.UseCoinEx(COIN.TICKET, Value, () => {
                            //NoticeMessage.Instance.Show("입장권을 구입하였습니다");
                            TakeItemDlg.Instance.ShowCoin(COIN.TICKET, ResultEvent);
                        });

                    }
                    if (Result == 3)
                    {
                        CWInvenManager.Instance.AddItem(Item, Count);
                        TakeItemDlg.Instance.Show(Item, ResultEvent);
                    }
                    //if (Result == 5)// 광고 제거
                    //{

                    //    CWSocketManager.Instance.UpdateUser("AdDel", "1");
                    //    CWHeroManager.Instance.m_bAdDel = true;
                    //    NoticeMessage.Instance.Show("광고 제거를 하였습니다!");

                    //}
                    

                });
            });

        }


    }

}
