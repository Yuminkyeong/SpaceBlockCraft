using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using CWUnityLib;
using CWStruct;
using Newtonsoft.Json.Linq;

public class CWBridgeList : DynamicScroll
{
    #region 스크롤 관련
    int m_nListCount;
    public int NListCount { get => m_nListCount; set => m_nListCount = value; }


    #endregion



    public enum TYPE { TABLE, DB, RESOURCE,NONE };
    public TYPE m_Type;
    public string m_szValues;
    public string m_szCondition;
    public string m_szDBParam;
    //// 데이타베이스 값이 도착하면 실행되는 함수
    public System.Action CBReceivfunc;
    public System.Action OnScrollFinish;// 스크롤 마지막

    public delegate void DgReceiveDataFunc(JObject jData);

    public DgReceiveDataFunc CBReceiveDataFunc;

    public bool m_bDbError = false;
    JSONNode m_jsData;
    // 리스트에 값이 들어가고, 슬롯은 그것을 참조 한다 
    string ParseValue(string szstr)
    {
        string szval = szstr.Trim();
        string[] pp = { ">", "<", "==", ">=", "<=", " ", "," };
        string[] szret = szval.Split(pp, pp.Length, System.StringSplitOptions.RemoveEmptyEntries);
        if (szret.Length < 2) return "";
        return szret[0];

    }
    string ParseOper(string szstr)
    {
        string szval = szstr.Trim();
        string[] pp = { ">", "<", "==", ">=", "<=", " ", "," };

        foreach (var v in pp)
        {
            if (szval.Contains(v)) return v;
        }
        return "";
    }
    string ParseResult(string szstr)
    {
        string szval = szstr.Trim();
        string[] pp = { ">", "<", "==", ">=", "<=", " ", "," };
        string[] szret = szval.Split(pp, pp.Length, System.StringSplitOptions.RemoveEmptyEntries);
        if (szret.Length < 2) return "";
        return szret[1];

    }
    bool IsValue(string szoper, string sz1, string sz2)
    {
        if (szoper == "==")
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

        return false;

    }

    // HP > 0
    // 변수,오퍼레이터, 답 
    bool Check(JSONNode jsData)
    {
        if (m_szCondition == null) return true;
        if (m_szCondition.Length < 2) return true;
        string szValue = ParseValue(m_szCondition.ToUpper());
        if (szValue == "") return true;

        string szResult = ParseResult(m_szCondition.ToUpper());
        if (szResult == "") return true;


        string szOper = ParseOper(m_szCondition.ToUpper());
        if (szOper == "") return true;

        if (IsValue(szOper, jsData[szValue].Value, szResult))
        {
            return true;
        }

        return false;
    }

    public int m_nPageCount = 0;
    public bool m_bLastPage=false;

    

    public void DlgAddReceive(JObject jData)
    {
        if (jData["Result"].ToString() == "ok")
        {
            
            JSONNode jj = JSONNode.Parse(jData.ToString());
            if(m_jsData==null) m_jsData = new JSONArray();
            if(jj.Count < m_nPageCount)
            {
                m_bLastPage = true;
            }
            SetCursorCapture();
            MakeList(jj);
            BackupCursorCapture();
            m_bDbError = false;
            if (m_jsData.Count == 0)
                m_bDbError = true;
        }
        else
        {
            m_bDbError = true;
        }
        if (CBReceivfunc != null)
            CBReceivfunc();

    }

    public void DlgReceive(JObject jData)
    {
        CBReceiveDataFunc?.Invoke(jData);

        if (jData["Result"].ToString() == "ok")
        {
            JSONNode jj = JSONNode.Parse(jData.ToString());
            m_jsData = new JSONArray();
            MakeList(jj);
            m_bDbError = false;
            if(m_jsData.Count == 0)
                m_bDbError = true;
        }
        else
        {
            m_bDbError = true;
        }
        if (CBReceivfunc != null)
            CBReceivfunc();

    }
    // DB에 값을 요청
    protected virtual void AddListDB()
    {
        if (m_szValues == null) return;
        if (m_szValues == "") return;
        if (m_szValues.Length<=1) return;
        CWSocketManager.Instance.SendDlg(m_szValues, m_szDBParam, DlgReceive, m_szValues);
    }
    
    protected virtual void AddListResource()
    {

        JSONArray JsArray = new JSONArray();
        TextAsset [] txts= Resources.LoadAll<TextAsset>(m_szValues);
        foreach(var v in txts)
        {
            JSONClass JsValue = new JSONClass();

            JSONData k = new JSONData(v.name);
            JsValue.Add("NAME", k);



            JsArray.Add(JsValue);
        }
        JSONClass JsResult = new JSONClass();
        m_jsData = new JSONArray();
        JsResult.Add("List", JsArray);
        MakeList(JsResult);
    }
    //table 값을 읽어옴 
    protected virtual void AddListTable()
    {
        m_jsData = new JSONArray();
        MakeList(CWTableManager.Instance.ConvertJsonOld(m_szValues));

    }
    //유저 정의 값
    protected virtual void AddListNone()
    {
        SetScroll(m_nListCount);
    }
    public override void SetScroll(int _Cnt)
    {
        m_nListCount = _Cnt;

        base.SetScroll(_Cnt);
        
    }
    // json 값으로 
    public void MakeList(JSONNode jsData)
    {
        if (jsData == null) return;
        SetScroll(0);
        
        int tcnt = 0;
        for (int i = 0; i < jsData["List"].Count; i++)
        {
            if (!Check(jsData["List"][i]))
            {
                continue;
            }
            tcnt++;
            m_jsData.Add(jsData["List"][i]);
        }

        m_nListCount = m_jsData.Count;
        SetScroll(m_jsData.Count);
    }
    public void Clear()
    {
        m_jsData = null;
    }
    public override  void Close()
    {
        base.Close();
    }
    void OnDragFinish()
    {
        //OnScrollFinish
        UIScrollView us = GetComponentInChildren<UIScrollView>();
        if (us != null)
        {
            
            float fdel = _UIGrid.cellHeight * _ItemCnt - 500;
            if (_UIGrid.arrangement== UIGrid.Arrangement.Vertical)
            {
                fdel = _UIGrid.cellWidth* _ItemCnt -400;
                
            }
            if (Mathf.Abs(us.transform.localPosition.y) > fdel)
            {
                if(OnScrollFinish!=null)
                {
                    OnScrollFinish();
                }
            }
        }

    }
    public void Create(CWWindow gParent, CWWindow.Returnfuction fuc = null)
    {
        m_gParent=gParent;
        Resultfuc=fuc;
        MakeData();
        PositionReset();
        UIScrollView us = GetComponentInChildren<UIScrollView>();
        if (us != null)
        {
            us.onDragFinished = OnDragFinish;
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
        else
        {
            AddListNone();
        }
       

    }

    public bool IsRecord(int num, string szColumn)
    {
        if (num < 0) return false;
        if (num >= m_jsData.Count) return false;
        if (m_Type == TYPE.TABLE)
        {
            szColumn = szColumn.ToUpper();
        }


        JSONClass js = (JSONClass)m_jsData[num];
        return js.ContainsKey(szColumn);
    }

    public void AddRecord(string szColumn,string szData)
    {
        if (m_jsData == null) m_jsData = new JSONArray();
        JSONNode jj = new JSONNode();
        jj.Add(szColumn,szData);
        m_jsData.Add(jj);
    }
    public string GetRecord(int num, string szColumn)
    {
        if (m_jsData == null) return "";
        if (num < 0) return "";
        if (num >= m_jsData.Count) return "";

        string szval = szColumn;
        if(m_Type==TYPE.TABLE)
        {
            szval = szColumn.ToUpper();
        }


        return m_jsData[num][szval].Value;
    }
    public float GetRecordFloat(int num, string szColumn)
    {
        if (m_jsData == null) return 0;
        if (num < 0) return 0;
        if (num >= m_jsData.Count) return 0;

        string szval = szColumn;
        if (m_Type == TYPE.TABLE)
        {
            szval = szColumn.ToUpper();
        }


        return CWLib.ConvertFloat(m_jsData[num][szval].Value);
    }
    public List<int> GetRecordArray(int num, string szColumn)
    {
        if (m_jsData == null) return null;
        if (num < 0) return null;
        if (num >= m_jsData.Count) return null;

        string szval = szColumn;
        if (m_Type == TYPE.TABLE)
        {
            szval = szColumn.ToUpper();
        }
        if(m_jsData[num][szval]==null)
        {
            return null;
        }
        JSONArray ja =(JSONArray) m_jsData[num][szval];
        if(ja!=null)
        {
            List<int> kTemp = new List<int>();
            for (int i = 0; i < ja.Count; i++)
            {

                kTemp.Add(CWLib.ConvertInt(ja[i].Value));
            }
            return kTemp;
        }

        //kTemp.AddRange(m_jsData[num][szval].ToObject<int[]>());

        return null;

      //  return m_jsData[num][szval].Value<T>();
      //return CWLib.ConvertFloat(m_jsData[num][szval].Value);
    }

    public int GetRecordInt(int num, string szColumn)
    {
        return (int)GetRecordFloat(num, szColumn);

    }

    public void UpdateData()
    {
        if(m_jsData==null)
        {
            SetList(_ItemCnt);
            return;
        }
        SetList(m_jsData.Count);
    }
}
