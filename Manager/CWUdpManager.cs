using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using System.Linq;
using SimpleJSON;

using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;


using CWUnityLib;
using CWEnum;

// UDP는 무조건 위치만 관여한다 
// 개념 현재 접속 유저가 없다면 자동 싱글로 바뀌는 개념 추가 한다

public class CWUdpManager : CWManager<CWUdpManager>
{

    

    #region 초기화

   

    #region 유디피 저장
    PacketFile m_kPacketFile = new PacketFile();

    #endregion

    [HideInInspector]
    public string m_szUDPIP;

    int m_nSendPort= 40003;// 코드로 한다!
    int m_nReceivePort= 40004;
    
    


    CWUDP m_kUDP = new CWUDP();

    public bool m_bConnected = false;




    #endregion

    #region 받기


    // 다른 유저가 근처에 접속 되어 있는가?
    
    void ParseUDPData(byte[] buffer)
    {
        int offset = 0;
        //UDPPACKET ct = (UDPPACKET)buffer[offset]; offset += 1; // (byte)BitConverter.ToInt16(buffer, offset); offset += 1;
        byte nRoomnumber= buffer[offset]; offset += 1;
        int nID = BitConverter.ToInt32(buffer, offset); offset += 4;
//        USERTYPE ntype = (USERTYPE)buffer[offset]; offset += 1;
        //float x = BitConverter.ToSingle(buffer, offset); offset += 4;
        //float y = BitConverter.ToSingle(buffer, offset); offset += 4;
        //float z = BitConverter.ToSingle(buffer, offset); offset += 4;
        byte X = buffer[offset]; offset += 1;
        byte X1 = buffer[offset]; offset += 1;

        byte Z = buffer[offset]; offset += 1;
        byte Z1 = buffer[offset]; offset += 1;

        float x = CWMath.ConvertFloat_Byte(X, X1);
        float z = CWMath.ConvertFloat_Byte(Z, Z1);

        float y = CWMath.ConvertFloatHeight(buffer[offset]); offset += 1;

        float yaw = CWMath.ConvertFloatYaw(buffer[offset]); offset += 1;
        //float yaw = BitConverter.ToSingle(buffer, offset); offset += 4;

        if (nID == CWHero.Instance.m_nID)
        {
            return;
        }

        CWObject pgUser = GetUser(nID);
        if (pgUser == null)
        {
            return;
        }
        pgUser.CopyTransPos(new Vector3(x, y, z), yaw);
            //else if (ct == UDPPACKET.SHOOTPOS)
            //{
            //    float tx = BitConverter.ToSingle(buffer, offset); offset += 4;
            //    float ty = BitConverter.ToSingle(buffer, offset); offset += 4;
            //    float tz = BitConverter.ToSingle(buffer, offset); offset += 4;
            //    pgUser.ShootPos(false, new Vector3(tx, ty, tz));
            //}

        }


    #endregion

    #region 보내기
    void SendUdpData(CWObject gObject,byte[] bBuffer)
    {
        
        if(gObject==CWHero.Instance)
            m_kPacketFile.AddData(bBuffer);
        m_kUDP.Send(bBuffer);
    }
    public void SendMoveData(CWObject gObject)
    {
        if (!m_bConnected) return;
        if (gObject == null) return;
        if(gObject.m_nID==0)
        {
            return;
        }
        // 변한 값이 있는가?
        if (!gObject.IsMoved()) return;

        List<byte> temp = new List<byte>();

        MakeBaseData(gObject, temp);
        SendUdpData(gObject,temp.ToArray());
    }
    // ,AI 전용움직임
    public void SendMoveData(int nID, Vector3 vPos,float fYaw)
    {
        if (!m_bConnected) return;
        if (nID == 0)
        {
            return;
        }
        List<byte> temp = new List<byte>();

        MakeBaseData(nID, vPos.x, vPos.y, vPos.z, CWMath.ConvertByteYaw(fYaw), temp);
        SendUdpData(null, temp.ToArray());

    }





    // 오너 리스트를 요구한다

    #endregion


    #region 내부함수/ 코루틴

    CWObject GetUser(int nID)
    {
        CWObject oUser = CWUserManager.Instance.GetUser(nID);
        if (oUser) return oUser;

       


        
        oUser = CWMobManager.Instance.GetObject(nID);
        if (oUser) return oUser;

        return null;
    }


    
    private void Update()
    {
        if (!m_bConnected) return;
        List<byte[]> kList = m_kUDP.getMessages();
        if (kList.Count > 0)
        {
            foreach (var v in kList)
            {
                ParseUDPData(v);
            }
        }

    }

    IEnumerator MoveUDP()
    {
        while (true)
        {
            SendMoveData(CWHero.Instance);
            //yield return null;
            yield return new WaitForSeconds(0.1f);

        }
    }

    #endregion

    #region 유니티 이벤트함수

    private void OnDestroy()
    {
        m_kUDP.Close();
    }

    #endregion

    #region 외부접근함수

    public override void Create()
    {
        
        base.Create();
    }

    string GetIP()
    {
        return m_szUDPIP;

    }




    public void Connect()
    {
        if(CWMainGame.Instance.m_bTestmode)
        {
            m_nReceivePort = (CWHero.Instance.m_nID) % 10000 + 10000;//10000 ~ 20000
            CWDebugManager.Instance.Log("UDP 테스트모드 !");
        }

        m_kUDP.StartConnection(GetIP(), m_nSendPort, m_nReceivePort);

        


    }

    public void StartUDP()
    {
     
        
        m_bConnected = true;
        StartCoroutine("MoveUDP");
        m_kPacketFile.Begin();

    }

    public void CloseUDP()
    {
        
        if (!m_bConnected) return;
        
        m_bConnected = false;
        CWAIOwnerManager.Instance.Clear();
        StopAllCoroutines();
        m_kPacketFile.Save();



    }


    
    public void MakeBaseData(CWObject gUser, List<byte> temp)
    {
        
        if(gUser==null)
        {
            return;
        }
        
        MakeBaseData(gUser.m_nID, gUser.GetPosX(), gUser.GetPosY(), gUser.GetPosZ(), CWMath.ConvertByteYaw(gUser.GetYaw()),temp);
   


    }

    public void MakeBaseData(int nID, float x,float y,float z,byte yaw, List<byte> temp)
    {

        byte[] bb;

        temp.Add((byte)CWHeroManager.Instance.GetRoomNumber());//1

        bb = BitConverter.GetBytes(nID);
        foreach (var v in bb) temp.Add(v);

        bb= CWMath.ConvertByte_Float(x);
        foreach (var v in bb) temp.Add(v);

        bb = CWMath.ConvertByte_Float(z);
        foreach (var v in bb) temp.Add(v);

        byte by = CWMath.ConvertByteHeight(y);
        temp.Add(by);

        temp.Add(yaw);//1

        temp.ToArray();


    }


    #endregion








}
