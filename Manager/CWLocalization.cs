using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class CWLocalization : CWManager<CWLocalization>
{


    public List<SystemLanguage> m_LanguageList = new List<SystemLanguage>();

    public int m_nSelect=0;

    Dictionary<string, int> m_kData = new Dictionary<string, int>();
    string[] m_kList = new string[2000];

    
#if UNITY_EDITOR
    Dictionary<string, string> m_kWordTemp = new Dictionary<string, string>();
    Dictionary<string, string> m_kNewWordTemp = new Dictionary<string, string>();

#endif
#if UNITY_EDITOR
    public void AddData(string szKey)
    {

        //if (!m_bConvertTrans) return;
      
        //if (CWLib.IsDigit(szKey))
        //{
        //    return;
        //}
        //if (m_kWordTemp.ContainsKey(szKey)) return;

        //m_kNewWordTemp.Add(szKey, szKey);
        //m_kWordTemp.Add(szKey, szKey);




    }
    public void Load()
    {
        

        string szPath = string.Format("{0}/Localtext.txt", Application.dataPath);
        if (!File.Exists(szPath)) return;
        string str= File.ReadAllText(szPath);
        string[] szarray = str.Split('\n');
        
        foreach(var v in szarray)
        {
            AddData(v);
        }

    }
    public void Save()
    {
        //string str = "";
        //foreach(var v in m_kNewWordTemp)
        //{
        //    str += v.Key;
        //    str += "\n";
        //}
        //Debug.Log(str);
        //string szPath = string.Format("{0}/NewLocaltext.txt", Application.dataPath);
        //File.WriteAllText(szPath, str);


        

    }

#endif
    public int GetDataCount()
    {
        return m_kData.Count;
    }

    public int GetCount()
    {
        return m_LanguageList.Count;
    }

    void MakeTable()
    {
        int cnt = 0;
        m_kData.Clear();

        int nSelect = CWPrefsManager.Instance.GetLanguage();
        m_nSelect = nSelect;

        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("Language - new");
        if (cs != null)
        {

            foreach (var v in cs.m_mkData)
            {
                int i = v.Key;
                cnt++;
                string szKorea = cs.GetString(i, "name").Trim();

                if(m_LanguageList[nSelect]== SystemLanguage.English)
                {
                    if(!m_kData.ContainsKey(szKorea))
                    {
                        
                        m_kList[cnt] = cs.GetString(i, "English");
                        m_kData[szKorea] = cnt;
                    }
                }
                else if(m_LanguageList[nSelect] == SystemLanguage.Japanese)
                {
                    if (!m_kData.ContainsKey(szKorea))
                    {
                        m_kList[cnt] = cs.GetString(i, "Japanese");
                        m_kData[szKorea] = cnt;

                    }
                }
                else if (m_LanguageList[nSelect] == SystemLanguage.Korean)
                {
                    if (!m_kData.ContainsKey(szKorea))
                    {
                        m_kList[cnt] = cs.GetString(i, "Korea");
                        m_kData[szKorea] = cnt;

                    }
                }
                else if (m_LanguageList[nSelect] == SystemLanguage.Portuguese)
                {
                    if (!m_kData.ContainsKey(szKorea))
                    {
                        m_kList[cnt] = cs.GetString(i, "포르투칼어");
                        m_kData[szKorea] = cnt;

                    }
                }
                else if (m_LanguageList[nSelect] == SystemLanguage.Russian)
                {
                    if (!m_kData.ContainsKey(szKorea))
                    {
                        m_kList[cnt] = cs.GetString(i, "러시아어");
                        m_kData[szKorea] = cnt;
                    }
                }
                else
                {
                    m_kList[cnt] = szKorea;
                    m_kData[szKorea] = cnt;

                }

            }

        }




    }
    public override void Create()
    {
#if UNITY_EDITOR
        //Load();
#endif
        
        base.Create();
    }

    public void SetLanguage(int num)
    {
        if (Game_App.Instance == null) return;
        if (num < 0) num = 0;
        if (num >= m_LanguageList.Count) num = m_LanguageList.Count-1;

        CWPrefsManager.Instance.SetLanguage(num);
        MakeTable();

        {
            CWText[] tt = Game_App.Instance.m_gUIDir.GetComponentsInChildren<CWText>();
            foreach (var v in tt)
            {
                v.OnChange();
            }

        }
        {
            CWText[] tt = Game_App.Instance.m_gEUIDir.GetComponentsInChildren<CWText>();
            foreach (var v in tt)
            {
                v.OnChange();
            }

        }


    }
    bool m_bConvertTrans = true;
    public void ConvertTrans(bool bflag)
    {

        m_bConvertTrans = bflag;

    }

    public string GetLanguage(string szKey) { 

        if (!CWLib.IsString(szKey)) return "";
        szKey = szKey.Trim();
        if (!m_kData.ContainsKey(szKey))
        {

            string[] separatingStrings = { "Lv." };
            string[] words = szKey.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            if(words.Length==2)
            {
                string szNewKey = words[0].Trim();
                if (m_kData.ContainsKey(szNewKey))
                {
                    int nKey2 = m_kData[szNewKey];
                    return string.Format("{0} Lv.{1}", m_kList[nKey2], words[1]);

                }
            }
            return szKey;
        }
            

        int nKey= m_kData[szKey];
        return m_kList[nKey];
    }
    public string GetLanguage(int nKey)
    {
        return m_kList[nKey];
    }
    public int GetLanguageNumber(string szKey)
    {
        if(szKey==null)
        {
            return 0;
        }
#if UNITY_EDITOR
        AddData(szKey);
#endif

        if (!m_kData.ContainsKey(szKey))
        {
            return 0;
        }
        int nKey = m_kData[szKey];
        return nKey;
    }

 
}
