//#define FILESAVE
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using CWUnityLib;
public class CWFile 
{

    MemoryStream m_Stream=new MemoryStream();

    public CWFile()
    {
        m_Stream = new MemoryStream();
    }
    public CWFile(MemoryStream ms)
    {
        m_Stream = ms;
    }
    public CWFile(byte [] bBuffer)
    {
        m_Stream = new MemoryStream(bBuffer);
    }
        
    public byte[] ToArray()
    {
        return m_Stream.ToArray();
    }
    public MemoryStream GetStream()
    {
        return m_Stream;
    }
    public bool Load(string szpath)
    {
        if(File.Exists(szpath))
        {
            using (FileStream fs = File.OpenRead(szpath))
            {
                if (fs.Length == 0)
                {
                    fs.Close();
                    return false;
                }
                               
                byte[] bBuffer = new byte[fs.Length];
                fs.Read(bBuffer, 0, (int)fs.Length);
                fs.Close();

                m_Stream = CWLib.Uncompress(bBuffer);
                if(m_Stream.Length==0)
                {
                    return false;
                }
                
            }
            return true;
        }
        return false;
    }
    public void Save(CWJSon jSon,string szname)
    {
        jSon.Add(szname, ToArray());
    }
    public void Save(string szpath)
    {
        try
        {
            using (FileStream fs = File.OpenWrite(szpath))
            {
                MemoryStream ms = CWLib.compress(m_Stream.ToArray());
                fs.Write(ms.ToArray(), 0, ms.ToArray().Length);
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to deserialize a file " + szpath + " (Reason: " + e.Message + ")");
        }

    }

    //json 값을 가져온다
    public void ImportJSon(CWJSon jSon)
    {
        m_Stream = new MemoryStream(jSon.ToArray());
    }
    //json 값을 내보낸다
    public void ExportJSon(CWJSon jSon)
    {
        jSon.SetData(m_Stream.ToArray());
    }

#if FILESAVE
    public bool LoadResources(string szfile)
    {
        string szname = string.Format("Gamedata/{0}",szfile);
        TextAsset textAsset = Resources.Load<TextAsset>(szname);
        if (textAsset != null)
        {
            m_Stream = CWLib.Uncompress(textAsset.bytes); 
            return true;
        }
        return false;
    }
    public void SaveResources(string szfile)
    {
#if UNITY_EDITOR
        string szpath = string.Format("{0}/Resources/Gamedata/", Application.dataPath);
        string szname = string.Format("{0}{1}.bytes", szpath, szfile);
        try
        {
            using (FileStream fs = File.OpenWrite(szname))
            {
                MemoryStream ms= CWLib.compress(m_Stream.ToArray());
                fs.Write(ms.ToArray(), 0, ms.ToArray().Length);
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to deserialize a file " + szname + " (Reason: " + e.Message + ")");
        }
#endif


    }
#else
    public bool Load(CWJSon jSon,string szname)
    {
        byte[] bdata = jSon.GetBytes(szname);
        if (bdata == null) return false;
        m_Stream = new MemoryStream(bdata);
        return true;
    }

    public bool LoadResources(string szfile)
    {
        m_Stream= CWFileManager.Instance.OpenRead(CWLib.DelExtString(szfile));
        if (m_Stream != null) return true;
        return false;
    }
    public void SaveResources(string szfile)
    {
        CWFileManager.Instance.AddFile(CWLib.DelExtString(szfile), m_Stream.ToArray());

    }
#endif

    public bool LoadGamedata(string szfile)
    {
        string szname = string.Format("Gamedata/{0}", szfile);
        TextAsset textAsset = Resources.Load<TextAsset>(szname);
        if (textAsset != null)
        {
            m_Stream = CWLib.Uncompress(textAsset.bytes);
            return true;
        }
        return false;
    }
    public void SaveGamedata(string szfile)
    {
#if UNITY_EDITOR
        string szpath = string.Format("{0}/Resources/Gamedata/", Application.dataPath);
        string szname = string.Format("{0}{1}.bytes", szpath, szfile);
        try
        {
            using (FileStream fs = File.OpenWrite(szname))
            {
                MemoryStream ms = CWLib.compress(m_Stream.ToArray());
                fs.Write(ms.ToArray(), 0, ms.ToArray().Length);
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to deserialize a file " + szname + " (Reason: " + e.Message + ")");
        }
#endif


    }


    public void Seek(int nSeek)
    {
        m_Stream.Seek(nSeek, SeekOrigin.Begin);
    }
    public int GetPosition()
    {
        return (int)m_Stream.Position;
    }
    public int GetByte()
    {
        return m_Stream.ReadByte();
    }
    public  char GetChar()
    {
        return (char)m_Stream.ReadByte();
    }
    public  double GetDouble( )
    {
        int pos = (int)m_Stream.Position;
        if (pos + 8 > m_Stream.Length) return 0;

        m_Stream.Seek(pos+8, SeekOrigin.Begin);
        return  BitConverter.ToDouble(m_Stream.ToArray(),pos); 
    }
    public  int GetInt16()
    {
        int pos = (int)m_Stream.Position;
        if (pos + 2 > m_Stream.Length) return 0;

        m_Stream.Seek(pos + 2, SeekOrigin.Begin);
        return (int)BitConverter.ToInt16(m_Stream.ToArray(), pos);

    }
    public int GetInt()
    {
        int pos = (int)m_Stream.Position;
        if (pos + 4 > m_Stream.Length) return 0;

        m_Stream.Seek(pos + 4, SeekOrigin.Begin);
        return BitConverter.ToInt32(m_Stream.ToArray(), pos);
    }
    public int GetInt32( )
    {
        int pos = (int)m_Stream.Position;
        if (pos + 4 > m_Stream.Length) return 0;
        m_Stream.Seek(pos + 4, SeekOrigin.Begin);
        return BitConverter.ToInt32(m_Stream.ToArray(), pos);
    }
    public long GetInt64()
    {
        int pos = (int)m_Stream.Position;
        if (pos + 8 > m_Stream.Length) return 0;

        m_Stream.Seek(pos + 8, SeekOrigin.Begin);
        return BitConverter.ToInt64(m_Stream.ToArray(), pos);
    }
    public float GetFloat()
    {
        int pos = (int)m_Stream.Position;
        if (pos + 4 > m_Stream.Length) return 0;

        m_Stream.Seek(pos + 4, SeekOrigin.Begin);
        return BitConverter.ToSingle(m_Stream.ToArray(), pos);
    }
    public  ushort GetUInt16()
    {
        int pos = (int)m_Stream.Position;
        if (pos + 2 > m_Stream.Length) return 0;

        m_Stream.Seek(pos + 2, SeekOrigin.Begin);
        return BitConverter.ToUInt16(m_Stream.ToArray(), pos);
    }
    public uint GetUInt32( )
    {
        int pos = (int)m_Stream.Position;
        if (pos + 4 > m_Stream.Length) return 0;

        m_Stream.Seek(pos + 4, SeekOrigin.Begin);
        return BitConverter.ToUInt32(m_Stream.ToArray(), pos);
    }
    public ulong GetUInt64()
    {
        int pos = (int)m_Stream.Position;
        if (pos + 8 > m_Stream.Length) return 0;

        m_Stream.Seek(pos + 8, SeekOrigin.Begin);
        return BitConverter.ToUInt64(m_Stream.ToArray(), pos);
    }
    public string GetString()
    {
        int nLen = GetInt16();
        byte[] buffer = new byte[nLen];
        m_Stream.Read(buffer, 0, nLen);
        return Encoding.UTF8.GetString(buffer);
    }
    public string GetStringLong()
    {
        int nLen = GetInt();
        byte[] buffer = new byte[nLen];
        m_Stream.Read(buffer, 0, nLen);
        return Encoding.UTF8.GetString(buffer);
    }

    public byte [] GetBuffer()
    {
        int nLen = GetInt32();
        if(nLen<=0)
        {
            CWUnityLib.DebugX.Log("file error!!");
            return null;
        }
        byte[] buffer = new byte[nLen];
        m_Stream.Read(buffer, 0, nLen);
        return buffer;
    }
    public byte[] GetBuffer(int nLen)
    {
        byte[] buffer = new byte[nLen];
        m_Stream.Read(buffer, 0, nLen);
        return buffer;
    }

    public void GetBufferByBuffer(byte[] buffer)
    {
        if(buffer==null)
        {
            return;
        }
        int nLen = GetInt32();
        if (nLen <= 0)
        {
            Debug.LogError("file error!!");
            return ;
        }
        m_Stream.Read(buffer, 0, nLen);

    }
    public void GetBufferSkip()
    {
        int nLen = GetInt32();
        if (nLen <= 0)
        {
            Debug.LogError("file error!!");
            return;
        }
        int pos = (int)m_Stream.Position;
        m_Stream.Seek(pos + nLen, SeekOrigin.Begin);


    }

    public void PutByte(int nval)
    {
        m_Stream.WriteByte((byte)nval);
    }
    public void PutChar(char nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal),0,1);
    }
    public void PutDouble(double nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 8);
    }
    public void PutInt16(int nVal)
    {
        m_Stream.Write(BitConverter.GetBytes((short)nVal), 0, 2);
    }
    public void PutInt32(int nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 4);
    }
    public void PutInt(int nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 4);
    }

    public void PutInt64(long nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 8);
    }
    public void PutUInt16(ushort nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 2);
    }
    public void PutUInt32(uint nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 4);
    }
    public void PutUInt64(ulong nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 8);
    }
    public void PutFloat(float nVal)
    {
        m_Stream.Write(BitConverter.GetBytes(nVal), 0, 4);
    }
    public void PutString(string szdata)
    {
        byte [] szbyte= Encoding.UTF8.GetBytes(szdata);
        short len = (short)szbyte.Length;
        PutInt16(len);
        m_Stream.Write(szbyte, 0, len);
    }
    public void PutStringLong(string szdata)
    {
        byte[] szbyte = Encoding.UTF8.GetBytes(szdata);
        int len = szbyte.Length;
        PutInt(len);
        m_Stream.Write(szbyte, 0, len);
    }

    public void PutBuffer(byte[] buffer)
    {
        int len = buffer.Length;
        PutInt32(len);
        m_Stream.Write(buffer, 0, len);

    }
    public void PutBuffer(byte[] buffer,int offset,int len)
    {
        m_Stream.Write(buffer, offset, len);

    }


}
