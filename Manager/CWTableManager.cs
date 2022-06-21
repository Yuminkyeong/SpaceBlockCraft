using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CWUnityLib;
using SimpleJSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class CWTableManager : CWManager<CWTableManager>
{
    // 버전 정리 
    public struct CSVVERSION
    {
        public int Ver;
        public string szFile;
        public CSVVERSION(int v,string  sz)
        {
            Ver = v;
            szFile = sz;
        }
    }
    CSVVERSION[] VerionArray =
    {
        new CSVVERSION(3,"무기업그레이드 - 시트1"),
    };


    class CSVFile
    {
        public Dictionary<string, int> m_kData = new Dictionary<string, int>();
        public CWFile m_kFile = new CWFile();

        #region LOAD
        public void Create()
        {
            CWFile cf = new CWFile();
            if (cf.LoadResources("csvfileheader"))
            {
                int tcnt = cf.GetInt16();
                for (int i = 0; i < tcnt; i++)
                {
                    string szName = cf.GetString();
                    int nSeek = cf.GetInt32();
                    m_kData.Add(szName, nSeek);
                }
            }
            m_kFile.LoadResources("csvfile");

            



        }

        #endregion LOAD
        #region SAVE
        void SaveHeader()
        {
            if (m_kFile == null) return;
            CWFile cf = new CWFile();
            cf.PutInt16((short)m_kData.Count);
            foreach (var v in m_kData)
            {
                cf.PutString(v.Key);
                cf.PutInt32(v.Value);
            }
            cf.SaveResources("csvfileheader");


        }
        // 전부 패킹을 한다 
        public void Packing()
        {
#if UNITY_EDITOR

            m_kFile = new CWFile();
            // 디렉토리 전부 읽는다 
            CWFile cf = new CWFile();

            m_kData.Clear();


            string szPath = CWLib.pathForDocumentsPath();
            string szpath = string.Format("{0}/CSV", szPath);


            DirectoryInfo dirs = new DirectoryInfo(szpath);

            FileInfo[] files = dirs.GetFiles();
            foreach (var v in files)
            {
                string szname;
                szname = v.Name.Substring(0, v.Name.LastIndexOf('.'));
                m_kData.Add(szname, cf.GetPosition());
                string szstr = File.ReadAllText(v.FullName);
                cf.PutStringLong(szstr);
            }
            cf.SaveResources("csvfile");
            SaveHeader();

#endif
        }
        #endregion SAVE


    }

    public class CWLoadCSV
    {

        public Dictionary<int, string[]> m_mkData = new Dictionary<int, string[]>();
        public Dictionary<string, int> m_mkColumn = new Dictionary<string, int>();
        public Dictionary<string, int> m_mkName = new Dictionary<string, int>();


        public int GetCount()
        {
            return m_mkData.Count;
        }
        public bool UpdateFile(string szfilestring)
        {
            string[] szArray = szfilestring.Split('\n');

            foreach (var v in szArray)
            {
                string[] szData = v.Split(new char[] { '\r', '\t' });
                int nKey = CWLib.ConvertInt(szData[0]);
                if (nKey == 0) continue;
                if (m_mkData.ContainsKey(nKey))
                {
                    m_mkData[nKey] = szData;
                }
                else
                {
                    m_mkData.Add(nKey, szData);
                    if (!m_mkName.ContainsKey(szData[1].ToUpper()))
                    {
                        m_mkName.Add(szData[1].ToUpper(), nKey);
                    }
                }


            }

            return true;
        }
        public bool Load(string szfilestring)
        {
            string[] szArray = szfilestring.Split('\n');
            bool bflag = false;
            foreach (var v in szArray)
            {
                string[] szData = v.Split(new char[] { '\r', '\t' });
                if (bflag == false)//컬럼값
                {
                    bflag = true;
                    for (int i = 0; i < szData.Length; i++)
                    {
                        if (szData[i].Length == 0) continue;

                        if (m_mkColumn.ContainsKey(szData[i].ToUpper()))
                        {
                            continue;
                        }

                        string szval= szData[i].Trim();

                        m_mkColumn.Add(szval.ToUpper(), i);
                    }

                }
                else
                {
                    int nKey = CWLib.ConvertInt(szData[0]);
                    if (!m_mkData.ContainsKey(nKey))
                    {
                        m_mkData.Add(nKey, szData);

                        if (!m_mkName.ContainsKey(szData[1].ToUpper()))
                        {
                            m_mkName.Add(szData[1].ToUpper(), nKey);
                        }

                    }

                }

            }

            return true;
        }
        public bool Load(TextAsset asset)
        {
            return Load(asset.text);
        }

        public string FindName(int nKey)
        {
            return GetString(nKey, "name");
        }
        public int FindKey(string szname)
        {
            if(m_mkName.ContainsKey(szname.ToUpper()))
                return m_mkName[szname.ToUpper()];
            return 0;
        }
        public int Find(string szColumn,string szvalue)
        {
            if (szvalue == null) return 0;
            if (szvalue.Length <= 1) return 0;
            if (!m_mkColumn.ContainsKey(szColumn.ToUpper())) return 0;
            int nColumn = m_mkColumn[szColumn.ToUpper()];
            //for()
            foreach(var v in m_mkData)
            {
                //if(v.Value[nColumn]== szvalue)
                if (v.Value[nColumn].Equals(szvalue, StringComparison.CurrentCultureIgnoreCase))
                {
                    return v.Key;
                }
            }
            return 0;

        }
        public bool FindColume(string szColumn)
        {
            if (m_mkColumn.ContainsKey(szColumn.ToUpper())) return true;
            return false;
        }
        public string GetString(int nKey, string szColumn)
        {
            
            if (!m_mkData.ContainsKey(nKey)) return "";
            int nColumn=0;
            if(m_mkColumn.ContainsKey(szColumn.ToUpper()))
            {
                nColumn = m_mkColumn[szColumn.ToUpper()];
            }
            else
            {
                return "";
            }
           
            string[] szArray = m_mkData[nKey];

            if (szArray.Length <= nColumn)
            {
                //Debug.LogError(" Error!  " + nKey.ToString() );
                return "";
            }

            return szArray[nColumn];
        }
        public int GetInt(int nKey,string szColmn)
        {
            string sz= GetString(nKey, szColmn);
            return CWLib.ConvertInt(sz.Replace(",",""));
        }
        public float GetFloat(int nKey, string szColmn)
        {
            string sz = GetString(nKey, szColmn);
            return CWLib.ConvertFloat(sz.Replace(",", ""));
        }

        // 컬럼 중에서 랜덤하게 하나만 뽑는다 
        // 컬럼 중에서 랜덤하게 하나를 뽑아서, name 컬럼의 값을 리턴한다 
        public string RandomTable(string szColumn, string szparam,string szreturn = "NAME")
        {
            int nColumn = 0;
            if (m_mkColumn.ContainsKey(szColumn.ToUpper()))
            {
                nColumn = m_mkColumn[szColumn.ToUpper()];
            }
            int nColumn2 = 0;
            if (m_mkColumn.ContainsKey(szreturn.ToUpper()))
            {
                nColumn2 = m_mkColumn[szreturn.ToUpper()];
            }

            List<string> kTemp = new List<string>();
            foreach (var v in m_mkData)
            {
                string[] szArray = v.Value;
                if(szArray[nColumn] ==szparam)
                {
                    kTemp.Add(szArray[nColumn2]);
                }
            }
            if (kTemp.Count == 0) return null;

            return kTemp[UnityEngine.Random.Range(0,kTemp.Count)];

            
        }

        public JSONNode ConvertJsonOld()
        {
            JSONArray JsArray = new JSONArray();
            foreach (var v in m_mkData)
            {
                int i = v.Key;
                if (i == 0) continue;
                JSONClass JsValue = new JSONClass();
                foreach (var vv in m_mkColumn)
                {
                    JSONData k = new JSONData(GetString(i, vv.Key));
                    JsValue.Add(string.Format(vv.Key, i), k);
                }
                JsArray.Add(JsValue);
            }
            JSONClass JsResult = new JSONClass();
            JsResult.Add("List", JsArray);
            return JsResult;
        }

        public JArray ConvertJson()
        {
            JArray JsArray = new JArray();
            foreach (var v in m_mkData)
            {
                int i = v.Key;
                if (i == 0) continue;

                JObject JsValue = new JObject();
                foreach (var vv in m_mkColumn)
                {
                    //JSONData k = new JSONData(GetString(i, vv.Key));
                    //JsValue.Add(string.Format(vv.Key, i), k);
                    JsValue.Add(string.Format(vv.Key, i), GetString(i, vv.Key));
                }
                JsArray.Add(JsValue);
            }
            //JSONClass JsResult = new JSONClass();
            //JsResult.Add("List", JsArray);
            return JsArray;
        }

    }
    public TextAsset[] m_TextAsset;


    public Dictionary<string, CWLoadCSV> m_mkTable = new Dictionary<string, CWLoadCSV>();

    public CWLoadCSV GetTable(string szTable)
    {
        if (m_mkTable.ContainsKey(szTable.ToUpper())) return m_mkTable[szTable.ToUpper()];
        return null;

    }
    public JArray ConvertJson(string szTable)
    {
        CWLoadCSV cs = GetTable(szTable);
        if (cs != null)
        {
            return cs.ConvertJson();
        }
        return null;
    }
    public JSONNode ConvertJsonOld(string szTable)
    {
        CWLoadCSV cs = GetTable(szTable);
        if (cs != null)
        {
            return cs.ConvertJsonOld();
        }
        return null;
    }


    public bool IsTable(string szTable)
    {
        if (m_mkTable.ContainsKey(szTable.ToUpper())) return true;
        return false;
    }
    public int   GetTableCount(string szTable)
    {
        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].GetCount();
        }
        return 0;
    }
    public string RandomTable(string szTable, string szColumn, string szparam, string szreturn = "NAME")
    {

        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].RandomTable(szColumn,szparam, szreturn);
        }
        return null;
    }
    public bool FindColume(string szTable, string szColumn)
    {
        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].FindColume( szColumn);
        }
        return false;

    }
    public string GetTable(string szTable,string szColumn,int nKey)
    {

        if(m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].GetString(nKey, szColumn);
        }
        return "";
    }
    public int GetTableInt(string szTable, string szColumn, int nKey)
    {

        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return (int)m_mkTable[szTable.ToUpper()].GetFloat(nKey, szColumn);
        }
        return 0;
    }
    public float GetTableFloat(string szTable, string szColumn, int nKey)
    {

        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].GetFloat(nKey, szColumn);
        }
        return 0;
    }

    public override void Create()
    {
        
#if UNITY_EDITOR
        CWTableManager.Instance.Packing();
        CWFileManager.Instance.Save();
#endif
    }

    void LoadFile(string szPath,string szFilename)
    {
        
        string szstr= File.ReadAllText(szPath);
        CWLoadCSV kdata = new CWLoadCSV();
        kdata.Load(szstr);
        string szname = CWLib.DelExtString(szFilename);
        string szKey = szname.ToUpper();
        m_mkTable.Add(szKey, kdata);

    }
    static public string GetExtString(string szPath)
    {
        int tcnt = szPath.LastIndexOf('.');
        if (tcnt <= 0) return szPath;
        szPath = szPath.Substring(tcnt+1);
        return szPath;
    }

    bool CheckVersion(string szname)
    {
        string szFile = CWLib.DelExtString(szname);
        string szext = GetExtString(szname);
        int Version =CWLib.ConvertInt(szext);
        foreach(var v in VerionArray)
        {
            if(v.szFile== szFile)
            {
                if(v.Ver >= Version)
                {
                    return false;
                }
            }
        }
        return true;

    }
    void UpdateFile(string szname,string szFile)
    {
        if(!CheckVersion(szname))
        {
            return;
        }

        string szKey = CWLib.DelExtString(szname);
        szKey = szKey.ToUpper();
        CWLoadCSV kdata = new CWLoadCSV();
        kdata.Load(szFile);
        m_mkTable.Add(szKey, kdata);

    }
    void ReceiveCSV(JObject jData)
    {
        // 버전 관리 해야 한다 

        JArray ja = (JArray)jData["CSVFile"];
        if(ja!=null)
        {
            for (int i = 0; i < ja.Count; i++)
            {
                CWJSon jj = new CWJSon((JObject)ja[i]);
                string szname = jj.GetString("Name");

                string szFile = jj.GetString("Filedata");
                UpdateFile(szname, szFile);
            }
        }
        Load();
        CWArrayManager.Instance.InitData();

    }
    // 로드하기전 최우선으로 받는다
    // CSV 파일 버전업 관리 개념을 적용하지 않으면 이 코드를 사용하면 안된다!!

    bool m_bUpdateCSV = false;// 한번만 해야 한다

    public void UpdateCSV(Action func)
    {
        if(m_bUpdateCSV)
        {
            return;
        }
        m_bUpdateCSV = true;
        CWSocketManager.Instance.UpdateCSV((jData)=> {

            JArray ja = (JArray)jData["CSVFile"];
            if (ja != null)
            {
                for (int i = 0; i < ja.Count; i++)
                {
                    CWJSon jj = new CWJSon((JObject)ja[i]);
                    string szname = jj.GetString("Name");

                    string szFile = jj.GetString("Filedata");
                    UpdateFile(szname, szFile);
                }
            }
            Load();
            CWArrayManager.Instance.InitData();
            func();


        }, "ReceiveCSV");
    }

    // 툴용으로 로드 
    public void ToolLoad()
    {
        m_mkTable.Clear();
        string szPath = CWLib.pathForDocumentsPath() + "/CSV";
        CWLib.DirectoryFuction(szPath, LoadFile);


    }
    void Load()
    {
#if UNITY_EDITOR
        if (CWMainGame.Instance.m_bLocalFile)
        {
            m_mkTable.Clear();
            string szPath = CWLib.pathForDocumentsPath() + "/CSV";
            CWLib.DirectoryFuction(szPath, LoadFile);
            return;
        }
#endif


        CSVFile csv = new CSVFile();
        csv.Create();
        foreach (var v in csv.m_kData)
        {
            int nSeek = v.Value;
            csv.m_kFile.Seek(nSeek);

            string strdata = csv.m_kFile.GetStringLong();
            CWLoadCSV kdata = new CWLoadCSV();
            kdata.Load(strdata);
            if (m_mkTable.ContainsKey(v.Key.ToUpper()))
            {
                continue;// 이미 받았음
            }
            m_mkTable.Add(v.Key.ToUpper(), kdata);

        }

        


    }

    public void Packing()
    {
#if UNITY_EDITOR

        CSVFile csv = new CSVFile();
        csv.Packing();

#endif
    }

    public int Find(string szTable,string szColmun,string szvalue)
    {

        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].Find(szColmun, szvalue);
        }

        return 0;
    }
    public string FindName(string szTable, int nKey)
    {

        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].FindName(nKey);
        }

        return null;
    }

    public int FindKey(string szTable, string szname)
    {
        if (szname == null) return 0;
        if (szname.Length == 0) return 0;

        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            return m_mkTable[szTable.ToUpper()].FindKey(szname);
        }

        return 0;
    }
    public string GetString(string szTable, string szname,string szColumn)
    {
        if (m_mkTable.ContainsKey(szTable.ToUpper()))
        {
            int nKey = m_mkTable[szTable.ToUpper()].FindKey(szname);

            return m_mkTable[szTable.ToUpper()].GetString(nKey, szColumn);

        }

        return null;
    }

}
