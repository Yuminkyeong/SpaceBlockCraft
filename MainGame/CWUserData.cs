using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CWUserData  {


    public int m_ID;
    public int m_Type;// 종류 : 0 유저 1 채집유저, 2 터렛 3 드론 
    public float m_X;
    public float m_Y;
    public float m_Z;
    public float m_Yaw;
    public float m_HP;


    public void Copy(CWUserData nData)
    {
        m_ID = nData.m_ID;
        m_X = nData.m_X;
        m_Y = nData.m_Y;
        m_Z = nData.m_Z;
        m_Yaw = nData.m_Yaw;
        m_HP = nData.m_HP;
    }
    public byte[] GetByte()
    {
        List<byte> temp = new List<byte>();
        byte[] bb;

        bb = BitConverter.GetBytes(m_ID);
        foreach (var v in bb) temp.Add(v);

        bb = BitConverter.GetBytes(m_X);
        foreach (var v in bb) temp.Add(v);

        bb = BitConverter.GetBytes(m_Y);
        foreach (var v in bb) temp.Add(v);

        bb = BitConverter.GetBytes(m_Z);
        foreach (var v in bb) temp.Add(v);

        bb = BitConverter.GetBytes(m_Yaw);
        foreach (var v in bb) temp.Add(v);

        bb = BitConverter.GetBytes(m_HP);
        foreach (var v in bb) temp.Add(v);

        return temp.ToArray();

    }
    public int SetByte(byte[] buffer, int offset)
    {
        if (buffer.Length < offset + 21)
        {
            return offset;
        }

        m_ID = BitConverter.ToInt32(buffer, offset); offset += 4;
        m_X = BitConverter.ToSingle(buffer, offset); offset += 4;
        m_Y = BitConverter.ToSingle(buffer, offset); offset += 4;
        m_Z = BitConverter.ToSingle(buffer, offset); offset += 4;
        m_Yaw = BitConverter.ToSingle(buffer, offset); offset += 4;
        m_HP = (int)BitConverter.ToSingle(buffer, offset); offset += 4;


        return offset;
    }


}
