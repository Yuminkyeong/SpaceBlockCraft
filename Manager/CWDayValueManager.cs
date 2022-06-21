using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using System;
using CWUnityLib;

public class CWDayValueManager : CWManager<CWDayValueManager>
{
    CWJSon m_kJson = new CWJSon();
    string m_szName;

    int GetUNIQ()
    {
        if(PlayerPrefs.HasKey("DayUniq"))
        {
            return PlayerPrefs.GetInt("DayUniq");
        }
        int Uniq = CWLib.Random(0, 100000) + 10;
        PlayerPrefs.SetInt("DayUniq",Uniq);
        return Uniq;
    }

    public override void Create()
    {

        m_szName = string.Format("{0}_daycount", GetUNIQ());

        DateTime tt = DateTime.Now;
        if(m_kJson.LoadLocal(m_szName))
        {
            string szval = string.Format("{0}_{1}_{2}", tt.Year, tt.Month, tt.Day);
            string szday= m_kJson.GetString("Now");
            if(szday!=szval)
            {
                // reset;
                m_kJson.Close();
                SetString("Now", szval);
            }
        }
        base.Create();
    }

    public int GetInt(string szval)
    {
        return m_kJson.GetInt(szval);
    }
    public void SetInt(string szval, int nval)
    {
        m_kJson.Add(szval, nval);
        m_kJson.SaveLocal(m_szName);
    }
    public float GetFloat(string szval)
    {
        return m_kJson.GetFloat(szval);
    }
    public void SetFloat(string szval, float nval)
    {
        m_kJson.Add(szval, nval);
        m_kJson.SaveLocal(m_szName);
    }
    public string GetString(string szval)
    {
        return m_kJson.GetString(szval);
    }
    public void SetString(string szval, string nval)
    {
        m_kJson.Add(szval, nval);
        m_kJson.SaveLocal(m_szName);
    }
    public List<T> GetArray<T>(string szval)
    {
        JArray ja = (JArray)m_kJson.GetJson(szval);
        if (ja == null) return null;
        List<T> tt = new List<T>();
        for(int i=0;i<ja.Count;i++)
        {
            tt.Add(ja[i].Value<T>());
        }
        return tt;
    }
    public void SetArray<T>(string szval,List<T> kData )
    {
        JArray ja = new JArray();
        foreach(var v in kData)
        {
            ja.Add(v);
        }
        m_kJson.Add(szval, ja);
        m_kJson.SaveLocal(m_szName);
    }

}
