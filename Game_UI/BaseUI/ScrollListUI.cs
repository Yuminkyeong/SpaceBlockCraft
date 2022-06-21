using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using System.IO;
using SimpleJSON;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWStruct;
using CWEnum;
using CWUnityLib;
using System;

// 테이블에서 불러 오는 개념
// JSon에서 오는 개념 


public class ScrollListUI : MonoBehaviour
{

    

    public bool m_bFixMove;// 움직이 않는다

    public delegate int SortFunction(JToken a, JToken b);
    public delegate bool CheckData(JToken kToken);

    public SortFunction CBSortFunction = null;
    public CheckData CBCheckData=null;
    

    public GameObject m_gEmptyBox;
    static public ScrollListUI g_kSelectScrol = null;

    public bool m_bRefrshLoof=false;// 주기적으로 보여준다
    public float m_fRefreshtime=3f;
    public int m_nSelect;//0부터

    public int NCursor
    {
        get
        {
            return m_nSelect;
        }
        set
        {
            SetCusor(value);
        }
    }

    public SlotItemUI m_kPrefab;

    int m_nListCount;
    

    

    public SlotItemUI m_kSelect;
    BaseUI m_kParentUI;

    public bool m_bStartClick = true;// 시작 할때 클릭을 연출 시킬 것인가?
    
    // 개념 선택된 상태에서 누르면, 버튼으로 인식한다
    public bool m_bTwoClick = false;// 두번 클릭을 허용할 것인가..


    public static bool m_bDBEndTask;

    


    private void Awake()
    {
    }
   
    
    void ResetSlot()
    {

        if(m_kList == null) return;
        SlotItemUI[] Slots = m_kList.gameObject.GetComponentsInChildren<SlotItemUI>(true);
        foreach(var v in Slots)
        {
            v.gameObject.SetActive(false);
        }
        //m_kList.anchoredPosition = new Vector2();

    }
    IEnumerator  IRestPostion()
    {
        yield return null;
        ScrollRect gScroll; // 스크롤 개념
        gScroll = GetComponent<ScrollRect>();
        if (gScroll != null)
        {
            if (gScroll.verticalScrollbar)
            {
                gScroll.verticalScrollbar.value = 1;
            }
            if (gScroll.horizontalScrollbar)
            {
                gScroll.horizontalScrollbar.value = 0;
            }
        }


    }
    public void UpdateData(DgAddFirstData _AddFirstData=null,bool bselect=true)// UpdateData는 모든 값을 초기화 새로 시작 
    {
        if (gameObject.activeInHierarchy == false) return;
        CBAddFirstData = _AddFirstData;

        
        ResetSlot();
        _sendflag = false;
        m_bDBEndTask = false;

        m_kParentUI = GetComponentInParent<BaseUI>();//
        m_kGrid = GetComponentInChildren<GridLayoutGroup>();
        if(m_kList==null)
        {
            if (m_kGrid)
                m_kList = m_kGrid.GetComponent<RectTransform>();
            else
            {
                SlotItemUI ss=GetComponentInChildren<SlotItemUI>();
                m_kList = ss.transform.parent.GetComponent<RectTransform>();
                
            }
        }

        m_jsData = new JArray();
        MakeData();
        if(bselect)
            SetSelect(m_nSelect);
        

    }
    public void SetSelect(int num)
    {
        OnSelect(GetSlot(num));
    }
    public virtual void EmptySlot()
    {
        m_kParentUI = GetComponentInParent<BaseUI>();//
        if(m_kParentUI)
            m_kParentUI.EmptySlot();
    }
    public void OnDeSelect()
    {
        if (m_kSelect)
            m_kSelect.OnDeSelectActive();
    }
    public void OnSelect(SlotItemUI kSelect)
    {
        if (kSelect == null)
        {
            EmptySlot();
            return;
        }
        if (kSelect.m_nItemID > 0)
        {

            ValueUI.g_kSelectItemData = CWArrayManager.Instance.GetItemData(kSelect.m_nItemID);

        }


        if (m_kSelect)
        {
            if(m_kSelect!=kSelect)
                m_kSelect.OnDeSelectActive();
            else
            {
                // 같은 슬롯 두번클릭
                if(m_bTwoClick)
                    OnButtonClick(kSelect);
                //return;
            }
            
        }
        m_kSelect = kSelect;
        m_nSelect = m_kSelect.m_nNumber;
        m_kParentUI.OnSelect(m_nSelect);
        m_kSelect.OnSelectActive();

    }
    // 클릭 이벤트  슬롯의 특정 버튼을 클릭 한다
    public void OnButtonClick(SlotItemUI kSelect)
    {
        if (m_kSelect)
        {
            if (m_kSelect != kSelect)
                m_kSelect.OnDeSelectActive();
        }
        m_kSelect = kSelect;
        m_nSelect = m_kSelect.m_nNumber;
        m_kParentUI.OnButtonClick(m_nSelect);
    }

    #region 데이타 관련
    public enum TYPE { TABLE, DB, RESOURCE,LIST, NONE };
    public TYPE m_Type;
    public string m_szValues;
    public string m_szCondition;
    public string m_szDBParam;
    public System.Action CBReceivfunc;
    public System.Action OnScrollFinish;// 스크롤 마지막

    public delegate void DgReceiveDataFunc(JObject jData);

    public DgReceiveDataFunc CBReceiveDataFunc;

    public delegate void DgAddFirstData(JArray jArr);

    DgAddFirstData CBAddFirstData;

    public bool m_bDbError = false;


    public bool IsData(int num)
    {
        if (num < 0) return false;
        if (num >= m_jsData.Count) return false;

        return true;
    }
    public bool IsRecord(int num,string szColumn)
    {
        if (num < 0) return false;
        if (num >= m_jsData.Count) return false;
        if (m_Type != TYPE.DB)
        {
            szColumn = szColumn.ToUpper();
        }
        if (m_jsData[num]==null)
        {
            return false;
        }
        if (m_jsData[num][szColumn]==null)// 체크 
        {
            return false;
        }

        return true;

    }
    public string GetSelectValue(string szColumn)
    {
        return GetString(m_nSelect, szColumn);
    }

    public int GetSelectValueInt(string szColumn)
    {
        return GetInt(m_nSelect, szColumn);
    }


    public DateTime GetDateTime(int num,string szColumn)
    {
        if (m_jsData == null) return DateTime.MinValue;
        if (num < 0) return DateTime.MinValue; ;
        if (num >= m_jsData.Count) return DateTime.MinValue; ;

        string szval = szColumn;
        if (m_Type != TYPE.DB)
        {
            szval = szColumn.ToUpper();
        }
        if (m_jsData[num][szval] == null)
        {
            return DateTime.MinValue; ;
        }
        if (m_jsData[num][szval].ToString()=="")
        {
            return DateTime.MinValue; ;
        }


        return m_jsData[num][szval].Value<DateTime>();

    }
    public byte[] GetBytes(int num, string szColumn)
    {
        if (m_jsData == null) return null;
        if (num < 0) return null;
        if (num >= m_jsData.Count) return null;

        string szval = szColumn;
        if (m_Type != TYPE.DB)
        {
            szval = szColumn.ToUpper();
        }
        if (m_jsData[num][szval] == null)
        {
            return null;
        }

        return m_jsData[num][szval].Value<byte[]>();

    }
    public string GetString(int num, string szColumn)
    {
        if (m_jsData == null) return "";
        if (num < 0) return "";
        if (num >= m_jsData.Count) return "";
        string szval = szColumn;
        if (!CWLib.IsJSonData(m_jsData[num]))
        {
            return "";
        }
        
        if (m_Type != TYPE.DB)
        {
            szval = szColumn.ToUpper();
        }
        else
        {
            if (!CWLib.IsJSonData(m_jsData[num][szval]))
            {
                return "";
            }
            if (m_jsData[num][szval].Type== JTokenType.Array)
            {
                int nval = CWPrefsManager.Instance.GetLanguage();// 현재 랭귀지 
                JArray jaa =(JArray) m_jsData[num][szval];
                if(jaa.Count<=nval)
                {
                    nval = 1;
                }

                if (CWLib.IsJSonData(m_jsData[num][szval][nval]))
                    return m_jsData[num][szval][nval].ToString();
            }

        }
        if (!CWLib.IsJSonData(m_jsData[num][szval]))
        {
            return "";
        }

        return m_jsData[num][szval].ToString();

    }

    public JToken GetJObject(int num, string szColumn)
    {
        if (m_jsData == null) return null;
        if (num < 0) return null;
        if (num >= m_jsData.Count) return null;

        string szval = szColumn;
        if (m_Type != TYPE.DB)
        {
            szval = szColumn.ToUpper();
        }
        if (m_jsData[num][szval] == null)
        {
            return null;
        }
        return m_jsData[num][szval];

    }

    public void SetString(int num, string szColumn,string szValue)
    {
        if (m_jsData == null) return ;
        if (num < 0) return ;
        if (num >= m_jsData.Count) return ;

        string szval = szColumn;
        if (m_Type != TYPE.DB)
        {
            szval = szColumn.ToUpper();
        }

        m_jsData[num][szval] = szValue;
        

    }

   

    public int GetInt(int num, string szColumn)
    {
        if (m_jsData == null) return 0;
        if (num < 0) return 0;
        if (num >= m_jsData.Count) return 0;

        string szval = szColumn;
        if (m_Type != TYPE.DB)
        {
            szval = szColumn.ToUpper();
        }
        if (m_jsData[num][szval] == null) return 0;
        string sz = m_jsData[num][szval].ToString();
        return CWLib.ConvertInt(sz.Replace(",", ""));

    }
    public float GetFloat(int num, string szColumn)
    {
        if (m_jsData == null) return 0;
        if (num < 0) return 0;
        if (num >= m_jsData.Count) return 0;

        string szval = szColumn;
        if (m_Type != TYPE.DB)
        {
            szval = szColumn.ToUpper();
        }
        if (m_jsData[num][szval] == null) return 0;
        string sz = m_jsData[num][szval].ToString();
        return CWLib.ConvertFloat(sz.Replace(",", ""));

    }
    public int GetCount()
    {
        return m_jsData.Count;
    }

    #endregion
    #region JSon 데이타 

    JArray m_jsData;
    // 리스트에 값이 들어가고, 슬롯은 그것을 참조 한다 
    string ParseValue(string szstr)
    {
        string szval = szstr.Trim();
        string[] pp = { ">", "<", "==", ">=", "<=", "=", "," };
        string[] szret = szval.Split(pp, pp.Length, System.StringSplitOptions.RemoveEmptyEntries);
        if (szret.Length < 2) return "";
        return szret[0].Trim(); 

    }
    string ParseOper(string szstr)
    {
        string szval = szstr.Trim();
        string[] pp = { ">", "<", "==", ">=", "<=", "=", "," };

        foreach (var v in pp)
        {
            if (szval.Contains(v)) return v;
        }
        return "";
    }
    string ParseResult(string szstr)
    {
        string szval = szstr.Trim();
        string[] pp = { ">", "<", "==", ">=", "<=", "=", "," };
        string[] szret = szval.Split(pp, pp.Length, System.StringSplitOptions.RemoveEmptyEntries);
        if (szret.Length < 2) return "";
        return szret[1].Trim();

    }
    bool IsValue(string szoper, string sz1, string sz2)
    {
        int chknum;
        bool isnumber = int.TryParse(sz1, out chknum);
        if(isnumber)
        {
            if (szoper == "==" || szoper == "=")
            {

                if (CWLib.ConvertFloat(sz1) == CWLib.ConvertFloat(sz2)) return true;
            }
            if (szoper == ">")
            {
                if (CWLib.ConvertFloat(sz1) > CWLib.ConvertFloat(sz2)) return true;
            }
            if (szoper == "<")
            {
                if (CWLib.ConvertFloat(sz1) < CWLib.ConvertFloat(sz2)) return true;
            }
            if (szoper == "<=")
            {
                if (CWLib.ConvertFloat(sz1) <= CWLib.ConvertFloat(sz2)) return true;
            }
            if (szoper == ">=")
            {
                if (CWLib.ConvertFloat(sz1) >= CWLib.ConvertFloat(sz2)) return true;
            }
        }
        else
        {
            if (szoper == "==" || szoper == "=")
            {
                if (CWLib.IsEqual(sz1, sz2)) return true;
            }

        }
        return false;

    }

    // HP > 0
    // 변수,오퍼레이터, 답 
    bool Check(JToken jsData)
    {
        if(CBCheckData!=null)
        {
            if (!CBCheckData(jsData)) return false;
        }
        if (jsData.ToString() == "null")
        {
            return false;
        }
        
        
        if (m_szCondition == null) return true;
        if (m_szCondition.Length < 2) return true;

        string szValue = ParseValue(m_szCondition.ToUpper());
        if (szValue == "") return true;

        if (jsData[szValue] == null) return false;
        if (!CWLib.IsString(jsData[szValue].ToString())) return false; 

        string szResult = ParseResult(m_szCondition.ToUpper());
        if (szResult == "") return true;


        string szOper = ParseOper(m_szCondition.ToUpper());
        if (szOper == "") return true;


        if (IsValue(szOper, jsData[szValue].ToString(), szResult))
        {
            return true;
        }

        return false;
    }

    public int m_nPageCount = 0;
    public bool m_bLastPage = false;


    bool _sendflag = false;
    public void DlgReceive(JObject jData)
    {
        if (gameObject.activeInHierarchy == false) return;

        LoadingDlg.Instance.Close();
        if (m_Type != TYPE.DB)
        {
            return;
        }

        if (jData["Result"].ToString() == "ok")
        {
            
            int nCount = m_jsData.Count;
            if(MakeList((JArray)jData["List"])==false)
            {
                m_bDBEndTask = true;
                
            }
            if(nCount== m_jsData.Count)
            {
                m_bDBEndTask = true;
                
            }
            m_bDbError = false;
            if (m_jsData.Count == 0)
                m_bDbError = true;
            _sendflag = false;

            MakeListEndTask();
            m_kParentUI.ReceiveDB(jData);
            UpdateSlot();

        }
        else
        {
            m_bDbError = true;
        }
        if (CBReceivfunc != null)
            CBReceivfunc();



    }
    public bool IsSendOK()
    {
        return (_sendflag==false);
    }
    public bool AskDBList()
    {
        
        if (m_bDBEndTask) return false;// DB 데이타 끝
        if (_sendflag) return true; // 
        _sendflag = true;
        CWSocketManager.Instance.SendDlg(m_szValues, m_szDBParam, DlgReceive, m_szValues);
        return true;

    }
    // DB에 값을 요청
    protected virtual void AddListDB()
    {
        if (m_szValues == null) return;
        if (m_szValues == "") return;
        if (m_szValues.Length <= 1) return;
        if (_sendflag) return; // 한번 보내고, 리시브 받을 때 까지 안받는다
        _sendflag = true;
        m_bDBEndTask = false;
        m_jsData = new JArray();
        LoadingDlg.Instance.Show(false);
        CWSocketManager.Instance.SendDlg(m_szValues, m_szDBParam, DlgReceive, m_szValues);
    }

    protected virtual void AddListResource()
    {

        JArray JsArray = new JArray();

        string szPath = Application.dataPath;
        szPath += "/Resources/"+m_szValues;
        DirectoryInfo dirs = new DirectoryInfo(szPath);
        FileInfo[] files = dirs.GetFiles();
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] != null)
                {

                    if (files[i].Name.Contains(".meta")) continue;
                    JObject jVal = new JObject();

                    string szname = files[i].Name.Substring(0, files[i].Name.LastIndexOf('.'));

                    jVal.Add("NAME", szname);


                    JsArray.Add(jVal);

                }
            }
        }

        //GameObject[] txts = Resources.LoadAll<GameObject>(m_szValues);
        //foreach (var v in txts)
        //{
        //    //JSONClass JsValue = new JSONClass();

        //    //JSONData k = new JSONData(v.name);
        //    //JsValue.Add("NAME", k);

        //    JObject jVal = new JObject();
        //    jVal.Add("NAME",v.name);
        //    JsArray.Add(jVal);



        //}

        m_jsData = new JArray();
        MakeList(JsArray);
        MakeListEndTask();
    }
    //table 값을 읽어옴 
    protected virtual void AddListTable()
    {
        m_jsData = new JArray();
        MakeList(CWTableManager.Instance.ConvertJson(m_szValues));
        MakeListEndTask();
    }
    #region 사용자 정의 리스트 

    public delegate int dgGetListCountFunc();
    public delegate string dgGetListFunc(int Col);
    public delegate string dgGetListValue(int Row, int Col);

    public dgGetListCountFunc GetListCount;
    public dgGetListCountFunc GetListColumnCount;

    public dgGetListFunc GetListColumn;
    public dgGetListValue GetListValue;
    
    


    protected virtual void AddList()
    {
        

        JArray JsArray = new JArray();

        int tcnt = GetListCount();
        if(tcnt==0)
        {

            ResetSlot();
            return;// 클리어
        }
        for(int i=0;i<tcnt;i++)
        {
            JObject jVal = new JObject();
            int ccnt = GetListColumnCount();
            for(int j=0;j<ccnt;j++)
            {
                jVal.Add(GetListColumn(j).ToUpper(), GetListValue(i, j));
            }
            JsArray.Add(jVal);
        }
        
        m_jsData = new JArray();
        MakeList(JsArray);
        MakeListEndTask();


        ///JObject
        //m_jsData.Sort

    }
    #endregion
    //유저 정의 값
    protected virtual void AddListNone()
    {

      
    }
    // 크기를 새로 정한다
    

    public virtual void MakeListEndTask()
    {
        if (m_bFixMove) return;
        if (m_kList == null) return;
        if (m_kList == null) return;
        Vector2 vv = CalGridSize(m_nListCount);
        ScrollRect gScroll; // 스크롤 개념
        gScroll = GetComponent<ScrollRect>();
        if (gScroll == null) return;

        Vector2 vStartPos = m_kList.anchoredPosition;
        Vector2 vSizeDelta = m_kList.sizeDelta;
       
        if (gScroll.vertical)// 스크롤 방향이 상하 일때
        {
            // 높이값만 변화
            if(vv.y> vSizeDelta.y)
            {
                vSizeDelta.y = vv.y;
                vStartPos.y = -vv.y / 2;
            }
        }
        else// 방향이 좌우 일때
        {
            if(vv.x> vSizeDelta.x)
            {
                vSizeDelta.x = vv.x;
                vStartPos.x = vv.x / 2;

            }
        }
        // 결과


        m_kList.sizeDelta = vSizeDelta;
        m_kList.anchoredPosition = vStartPos;

        
    }
    // json 값으로 
    public virtual bool MakeList(JArray jsData)
    {
        if (jsData == null) return false;
        bool bisData=false;// 데이타 처리가 있는가?
        int tcnt = 0;

        if (jsData.Count==0)
        {
            if(m_jsData.Count==0)// 누적된 데이타가 없다면
            {
                if (CBAddFirstData == null)
                {
                    
                    if (m_gEmptyBox)
                    {
                        m_gEmptyBox.SetActive(true);
                    }
                    SlotItemUI[] Slots2 = m_kList.gameObject.GetComponentsInChildren<SlotItemUI>(true);
                    foreach (var v in Slots2) v.gameObject.SetActive(false);

                    return false;
                }

            }
        }

        if (m_gEmptyBox)
        {
            m_gEmptyBox.SetActive(false);
        }

        if (CBAddFirstData!=null)
        {
            bisData = true;
            CBAddFirstData(m_jsData);
            CBAddFirstData = null;
        }



        for (int i = 0; i < jsData.Count; i++)
        {
            if (!Check(jsData[i]))
            {
                continue;
            }
            tcnt++;
            m_jsData.Add(jsData[i]);
            bisData = true;
        }

        if (CBSortFunction != null)
        {
            List<JToken> kk = new List<JToken>();

            kk.AddRange(m_jsData);
            kk.Sort((a, b) => {
                return CBSortFunction(a, b);
            });
            m_jsData.Clear();
            foreach (var v in kk)
            {
                m_jsData.Add(v);
            }
        }



        m_nListCount = m_jsData.Count;
        SlotItemUI[] Slots = m_kList.gameObject.GetComponentsInChildren<SlotItemUI>(true);
        if(m_kPrefab==null)
        {
            if(Slots.Length>0)
            {
                m_kPrefab = Slots[0];
            
            }
                
        }
        if(m_kPrefab==null)
        {
            Debug.LogError("프리펩이 없다!");
            return false;
        }
        m_kPrefab.gameObject.SetActive(true);

        if (Slots.Length< m_nListCount) // 모자란다면
        {
            
            for (int i=0;i< m_nListCount- Slots.Length; i++)
            {
                SlotItemUI ss = Instantiate(m_kPrefab);
                ss.transform.SetParent(m_kList.transform);
                ss.transform.localPosition = Vector3.zero;
                ss.transform.localScale = Vector3.one;
            }
        }
      

        Slots = m_kList.gameObject.GetComponentsInChildren<SlotItemUI>(true);
        for (int i = 0; i < Slots.Length; i++)
        {
          
            // 시작할 때 클릭이 되어여야 하는 원도우만 해야 한다
            Slots[i].Create(this, i );
            if(m_bStartClick)
            {
                if (i == m_nSelect)
                {
                    Slots[m_nSelect].OnPointerClick(null);
                }

            }
        }



        return bisData;
    }

    public void UpdateSlot()
    {
        SlotItemUI[] Slots = m_kList.gameObject.GetComponentsInChildren<SlotItemUI>(true);
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].UpdateData();
        }



    }
    public void MakeData()
    {
        
        if (m_Type == TYPE.TABLE)
        {
            AddListTable();
        }
        else if (m_Type == TYPE.DB)
        {
            AddListDB();
        }
        else if (m_Type == TYPE.RESOURCE)
        {
            AddListResource();
        }
        else if (m_Type == TYPE.LIST)
        {
            AddList();
        }
        else
        {
            AddListNone();
        }
   
   

    }


    #endregion
    #region 스크롤 컨트롤

    public RectTransform m_kList;
    GridLayoutGroup m_kGrid;

    Vector2 CalGridSize( int nCnt)
    {
        if (m_kGrid == null) return new Vector2();
        // 이 그리가 컬럼이 몇개인지 확인 
        // 그리드의 셀싸이즈 확인 
        Vector2 vSellSize = m_kGrid.cellSize;
        Vector2 vSpacing = m_kGrid.spacing;
        RectOffset RR = m_kGrid.padding;

        vSellSize.x += vSpacing.x;
        vSellSize.y += vSpacing.y;

        int dx = 1;
        int dy = 1;
        if (m_kGrid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            dx = m_kGrid.constraintCount;
            dy = nCnt / dx;
            if (nCnt % dx != 0) dy++; // 나머지 있다면
        }
        else if (m_kGrid.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            dy = m_kGrid.constraintCount;
            dx = nCnt / dy;
        }
        else
        {
            dx = nCnt;
            dy = nCnt;
        }
        



        Vector2 vResult = Vector2.zero;
        vResult.x = (RR.left + vSellSize.x * dx);
        vResult.y = RR.top + vSellSize.y * dy;
        if (m_kGrid.startAxis == GridLayoutGroup.Axis.Horizontal)
        {
            
            vResult.y = 0;
        }
        else
        {

            vResult.x = 0;
        }

        return vResult;

    }

    public void SetCusor(int num)
    {
        m_nSelect = num;
        ScrollRect gScroll; // 스크롤 개념
        gScroll = GetComponent<ScrollRect>();

        m_kGrid = GetComponentInChildren<GridLayoutGroup>();
        if (m_kGrid == null) return;
        Vector2 vSellSize = m_kGrid.cellSize;
        Vector2 vSpacing = m_kGrid.spacing;
        RectOffset RR = m_kGrid.padding;

        vSellSize.x += vSpacing.x;
        vSellSize.y += vSpacing.y;
        if (gScroll.vertical)// 수직 
        {
            gScroll.verticalNormalizedPosition = vSellSize.y*num;
        }
        else// 수평
        {
            gScroll.horizontalNormalizedPosition = vSellSize.x * num;
        }

            

    }
    int CalSelectNumber()
    {
        if (m_kGrid == null) return 0;
        // 이 그리가 컬럼이 몇개인지 확인 
        // 그리드의 셀싸이즈 확인 
        Vector2 vSellSize = m_kGrid.cellSize;
        Vector2 vSpacing = m_kGrid.spacing;
        RectOffset RR = m_kGrid.padding;

        vSellSize.x += vSpacing.x;
        vSellSize.y += vSpacing.y;

        Vector3 vPos = m_kList.anchoredPosition3D;
        if (m_kGrid.startAxis == GridLayoutGroup.Axis.Horizontal)
        {
            vPos.y -= RR.top;
            float result = vPos.y / (vSellSize.y * m_kGrid.constraintCount);
            return (int)result;

        }
        else
        {
            vPos.x -= RR.left;
            float result = vPos.x / (vSellSize.x * m_kGrid.constraintCount);
            return (int)Mathf.Abs(result);
        }


    }
    public void OnNext()
    {

        m_nSelect = CalSelectNumber();
        m_nSelect++;
        if (m_nSelect >= m_nListCount)
        {
            m_nSelect = m_nListCount;
        }
//        Vector3 vv = CalGridSize( m_nSelect);
  //      m_kList.anchoredPosition3D = vv;

    }
    public void OnPrev()
    {
        m_nSelect = CalSelectNumber();
        m_nSelect--;
        if (m_nSelect < 0)
        {
            m_nSelect = 0;
        }
//        Vector3 vv = CalGridSize( m_nSelect);
//        m_kList.anchoredPosition3D = vv;

    }

    public SlotItemUI GetFirstSlot()
    {
        SlotItemUI[] Slots = m_kList.gameObject.GetComponentsInChildren<SlotItemUI>();

        return Slots[0];
    }
    public SlotItemUI GetSlot(int num)
    {
        if (m_kList == null) return null;
        SlotItemUI[] Slots = m_kList.gameObject.GetComponentsInChildren<SlotItemUI>();
        if (num < 0) return null;
        if (num >= Slots.Length) return null;
        return Slots[num];

    }

    public void OnBuy(int num)
    {
        m_kParentUI.OnBuy(num);

    }

    #endregion
}
