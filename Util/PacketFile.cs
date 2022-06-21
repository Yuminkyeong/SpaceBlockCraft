using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using CWUnityLib;

public class PacketFile 
{
    struct PCKETDATA
    {
        public int m_DeleyTime;
        public byte[] m_kBytes;
    };

    List<PCKETDATA> m_kList = new List<PCKETDATA>();

    float m_fStarttime = 0;

    public void Begin()
    {
#if UNITY_EDITOR
        if (!CWMainGame.Instance.m_bSavePacket) return;

        m_kList = new List<PCKETDATA>();
        m_fStarttime = Time.time;
#endif
    }

    public void AddData(byte[] bdata)
    {
#if UNITY_EDITOR
        if (!CWMainGame.Instance.m_bSavePacket) return;

        PCKETDATA kdata = new PCKETDATA();
        kdata.m_DeleyTime =(int)((Time.time - m_fStarttime)*1000);
        kdata.m_kBytes = bdata;
        m_kList.Add(kdata);

      

#endif
    }
    public void Save()
    {
#if UNITY_EDITOR
        if (!CWMainGame.Instance.m_bSavePacket) return;


        string szPath = CWLib.pathForDocumentsPath();
        CWFile cf = new CWFile();
        string szpath = string.Format("{0}/Packetfile/Pakcet_{1}.pcf", szPath, CWLib.GetTodayString());
//        string szpath = string.Format("Pakcet_{0}.pcf", tt.ToString());
        try
        {
            
            using (FileStream fs = File.OpenWrite(szpath))
            {
                int tcnt = m_kList.Count;
                byte[] bb;
                bb = BitConverter.GetBytes(m_kList.Count);
                fs.Write(bb,0,bb.Length); 
                foreach(var v in m_kList)
                {
                    int nlen = v.m_kBytes.Length;
                    bb = BitConverter.GetBytes(nlen);
                    fs.Write(bb, 0, bb.Length); 
                    bb = BitConverter.GetBytes(v.m_DeleyTime);
                    fs.Write(bb, 0, bb.Length); 
                    fs.Write(v.m_kBytes,0, nlen); 
                }
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to deserialize a file " + szpath + " (Reason: " + e.Message + ")");
        }

#endif
    }


}
