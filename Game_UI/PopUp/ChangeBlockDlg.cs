using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWEnum;
using CWStruct;
public class ChangeBlockDlg : WindowUI<ChangeBlockDlg>
{

    protected override int GetUINumber()
    {
        return 14;
    }

    public Text m_kIncText1; // 증가 수치 
    public Text m_kIncText2; // 증가 수치 


    public ScrollListUI m_gScrollList2;
    int m_nIncHP;// 증가한 HP량
    protected override void _Open()
    {
        
        FindBlock();
        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;

        m_gScrollList2.GetListCount = GetListCount;
        m_gScrollList2.GetListColumnCount = GetListColumnCount;
        m_gScrollList2.GetListValue = GetListValue2;
        m_gScrollList2.GetListColumn = GetListColumn;
        m_gScrollList2.m_Type = ScrollListUI.TYPE.LIST;

        string str = CWLocalization.Instance.GetLanguage("총 {0}개 변환");
        m_kIncText1.text = string.Format(str, m_kChangeList.Count);
        m_kIncText2.text = string.Format("+{0}", m_nIncHP);//
        base._Open();
    }

    
    public override void UpdateData(bool bselect = true)
    {
        base.UpdateData();
        m_gScrollList2.UpdateData(null,bselect);
    }
    public void OnChange()
    {
        if(m_kChangeList.Count==0)
        {
            NoticeMessage.Instance.Show("교체할 블록이 없습니다!");
            Close();
            return;
        }
        foreach(var v in m_kChangeList)
        {
            
            GameEdit.Instance.m_kAirObject.UpdateBlock(v.nKey, v.nItem2);
            CWInvenManager.Instance.DelItem(v.nItem2);
            CWInvenManager.Instance.AddItem(v.nItem,1);
        }

        GameEdit.Instance.m_kAirObject.CalPower();
        EditInvenDlg.Instance.TakeInven();
        EditInvenDlg.Instance.UpdateData();
        GameEdit.Instance.m_bUpdated = true;
        CWQuestManager.Instance.CheckUpdateData(3, 1);//블록을 교체해서 HP가 올라갔다
        

        string str = CWLocalization.Instance.GetLanguage("블록을 교체하였습니다! HP가 +{0} 증가 하였습니다");
        //NoticeMessage.Instance.Show(string.Format(str, m_nIncHP));
        MessageOneBoxDlg.Instance.Show(() => {
            Close();
            CWShowUI.g_bValueList[7] = true;
        }, "블록교환", string.Format(str, m_nIncHP));


        

    }


    #region 바꾸는 작업
    //찾을 블록을 구한다
    // 비행기 블록에서 자신보다 높은 블록이 인벤에 있는지 검사
    // 비행기 블록을 제일 낮은 단계로 정렬
    // 
    /*  비행기 블록을 제일 낮은 단계로 정렬
     *  가장 낮을 블록이 인벤에서 가장 높은 블록과 교체를 한다 
     * 
     * */
    class TEMPDATA
    {
        public int nKey;
        public int nItem;
        public int nLevel;
        public int nHP;
    }
    class INVENDATA
    {
        public int nItem;
        public int nLevel;
        public int nHP;
    }
    class CHANGBLOC
    {
        public int nKey;
        public int nItem;
        public int nItem2;
        public int nLevel;
        public int nLevel2;

    }
    List<CHANGBLOC> m_kChangeList = new List<CHANGBLOC>();
    List<INVENDATA> m_kTempInven = new List<INVENDATA>();
    int ComparetoBig(TEMPDATA ktemp1, TEMPDATA ktemp2)// 높은 숫자로 정렬
    {
        return ktemp2.nHP - ktemp1.nHP;
    }
    int ComparetoSmall(TEMPDATA ktemp1, TEMPDATA ktemp2)// 낮은 숫자로 정렬
    {
        return ktemp1.nHP - ktemp2.nHP;
    }

    public int FindUpLevelItem(int nHP)
    {
        int nMax = nHP;
        int nItem = 0;
        int nIndex = 0;
        for (int i = 0; i < m_kTempInven.Count; i++)
        {

            if (m_kTempInven[i].nHP > nMax)
            {
                nMax = m_kTempInven[i].nHP;
                nItem = m_kTempInven[i].nItem;
                nIndex = i;
            }
        }
        if(nItem>0)
            m_kTempInven.RemoveAt(nIndex);

        return nItem;
    }

    void FindBlock()
    {
        EditInvenDlg.Instance.MoveToInven();
        m_nIncHP = 0;
        m_kTempInven = new List<INVENDATA>();
        m_kChangeList = new List<CHANGBLOC>();
        Dictionary<int, BlockData> kData = GameEdit.Instance.m_kAirObject.GetData();
        List<TEMPDATA> kTemp = new List<TEMPDATA>();
        foreach (var v in kData)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
            if (gData.type != "shipblock") continue;
            if(  gData.nID==(int)GITEM.tempglass
                || gData.nID == (int)GITEM.glass
                )
            {
                // 유리는 바꾸지 않는다
                continue;
            }

            TEMPDATA tt = new TEMPDATA();
            tt.nKey = v.Key;
            tt.nItem = v.Value.nBlock;
            tt.nHP = gData.hp;
            tt.nLevel = gData.level;
            kTemp.Add(tt);
        }
        kTemp.Sort(ComparetoSmall);

        // 인벤에서 값 가져오기 
        

        foreach (var v in CWInvenManager.Instance.m_nInvenDB)
        {
            GITEMDATA g1 = CWArrayManager.Instance.GetItemData(v.NItem);
            if (g1.type != "shipblock") continue;
            if (g1.nID == (int)GITEM.tempglass
            || g1.nID == (int)GITEM.glass
            )
            {
                // 유리는 바꾸지 않는다
                continue;
            }



            for (int i=0;i<v.NCount;i++)
            {
                INVENDATA kk = new INVENDATA();
                kk.nItem = v.NItem;
                kk.nLevel = g1.level;
                kk.nHP = g1.hp;
                m_kTempInven.Add(kk);
            }

        }


        foreach (var v in kTemp)
        {
            int nItem = FindUpLevelItem(v.nHP);
            if (nItem > 0)
            {

                CHANGBLOC kk = new CHANGBLOC();
                kk.nKey = v.nKey;
                kk.nItem = v.nItem;// 기존 블록
                kk.nItem2 = nItem;// 바뀔 블록
                

                GITEMDATA g1 = CWArrayManager.Instance.GetItemData(kk.nItem);
                GITEMDATA g2 = CWArrayManager.Instance.GetItemData(kk.nItem2);

                kk.nLevel = g1.level;
                kk.nLevel2 = g2.level;

                m_nIncHP +=(g2.hp-g1.hp);

                m_kChangeList.Add(kk);

            }

        }



    }
    #endregion
    #region 오버로드 

    int GetListCount()
    {
        return m_kChangeList.Count;
    }
    int GetListColumnCount()
    {
        return 3;// m_nSlot,NItem,NCount
    }
    // 
    string GetListColumn(int Col)
    {
        if (Col == 0) return "Slot";
        if (Col == 1) return "ItemID";
        if (Col == 2) return "Grade";
        
        return "";
    }
    string GetListValue(int Raw, int Col)
    {
        if (Col == 0)
        {
            return m_kChangeList[Raw].nKey.ToString(); //CWInvenManager.Instance.m_nInvenDB[Raw].m_nSlot.ToString();
        }
        if (Col == 1)
        {
            return m_kChangeList[Raw].nItem.ToString(); // CWInvenManager.Instance.m_nInvenDB[Raw].NItem.ToString();
        }
        if (Col == 2)
        {
            return CWGlobal.GetGradeItemName(m_kChangeList[Raw].nLevel);
        }

        return "";
    }
    string GetListValue2(int Raw, int Col)
    {
        if (Col == 0)
        {
            return m_kChangeList[Raw].nKey.ToString(); //CWInvenManager.Instance.m_nInvenDB[Raw].m_nSlot.ToString();
        }
        if (Col == 1)
        {
            return m_kChangeList[Raw].nItem2.ToString(); // CWInvenManager.Instance.m_nInvenDB[Raw].NItem.ToString();
        }
        if (Col == 2)
        {
            return CWGlobal.GetGradeItemName(m_kChangeList[Raw].nLevel2);
        }

        return "";
    }

    #endregion
}
