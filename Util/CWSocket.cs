
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


using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

using CWUnityLib;

public class CWSocket : MonoBehaviour
{

    public enum SOCKETTYPE { ONLINE, DATA };// 항상 연결, 데이타 서버 

    public SOCKETTYPE m_SocketType;
    public float Pingtime = 10;
    //public delegate void RECEIVEFUCION(JObject _Data);

    void Message(string msg)
    {
        //Debug.LogError(msg);
        Debug.Log(msg);
    }

    #region 소켓 
    
    enum N_STATE { NOT_CONNECT, CONNECTED, CONNECTING, ERROR, QUIT };

    bool m_bReConnect = false;
    N_STATE _netstate;
    N_STATE NetState
    {
        get
        {
            return _netstate;
        }
        set
        {
            Debug.Log("상태변경 " + value.ToString());
            _netstate = value;
        }
    }
    /*
     *  개념
     *  
     *  모든 데이타는 서버에 보내야 한다 
     *  보내는 중에 소켓이 끊어지는 경우를 생각한다
     * */
    IEnumerator N_ISend()
    {

        while (true)
        {
            if (NetState == N_STATE.CONNECTED)// 소켓이 연결이 되어있다
            {
                if (m_Socket != null)
                {
                    if (m_DataList.Count > 0)
                    {
                        try
                        {
                            if (m_Socket.Connected)
                            {
                                PacketData _Data = m_DataList[0];
                                m_Socket.BeginSend(_Data.m_SendArray.ToArray(), 0, _Data.m_SendArray.ToArray().Length, SocketFlags.None, new AsyncCallback(SendCallback), m_Socket);
                                RemoveDataList(_Data);
                                N_StartTimeOut(_Data);
                            }
                            else
                            {
                                NetState = N_STATE.ERROR;
                                // 재접속을 해야 한다
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                            //print("SendData Error");
                            ErrorException(e);
                            // 재접속을 해야 한다
                        }
                    }


                }
                else
                {
                    // 소켓이 연결이 되어있는데 소켓이 널이다?
                    // 재접속을 해야 한다

                }
            }

            yield return null;
        }



    }

    public void Send(JObject JData, RECEIVEFUCION rec)
    {
        

        PacketData kPacketData = new PacketData(this,JData, rec);
        if (kPacketData == null) return;
        AddDatatList(kPacketData);

        if (NetState == N_STATE.NOT_CONNECT)
        {
            Debug.Log("접속");
            Connect();
            return;
        }

    }


    public void ExitGame()
    {
        if (NetState == N_STATE.QUIT) return;
        NetState = N_STATE.QUIT;
        MessageBoxDlg.Instance.Show(() => {
            //재접속
            m_bReConnect =false;
            NetState = N_STATE.ERROR;
        },()=> {
            // 종료
            CWMainGame.Instance.Quit();
        }, "통신장애", "다시 접속하시겠습니까?");
        //MessageOneBoxDlg.Instance.Show(CWMainGame.Instance.Quit, "통시 장애", "통신 장애로 게임을 종료합니다.");


    }
    // 접속한다
    // 접속이 되지 않는다면?
    // 변수 설정 
    // 현재 접속 시도 중
    // 접속 실패 
    public void Connect()
    {
        if (m_szIP.Count == 0)
        {
            Debug.LogError("서버가 정해지지 않았음");
            ExitGame();
            return;// 무조건 에라를 내야 한다
        }



        NetState = N_STATE.CONNECTING;// 전송 중이다 

        if (!m_FuctionList.ContainsKey("Connectok"))
        {
            m_FuctionList.Add("Connectok", Connectok);
        }
        IPHostEntry _HE;
        try
        {
            _HE = Dns.GetHostEntry(GetIP());// Find host name
        }
        catch (SocketException e)
        {
            Debug.Log(e);
            //ErrorSocket();
            ExitGame();
            return;
        }


        m_bConnectflag = true;// callback 함수 작동 
        try
        {

            foreach (var v in _HE.AddressList)
            {
                if (v.AddressFamily == AddressFamily.InterNetwork)
                {

                    m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    m_Socket.BeginConnect(v, m_nPort, new AsyncCallback(ConnectCallback), m_Socket);

                    m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);






                    break;
                }
            }

        }
        catch (SocketException e)
        {
            Message("Error" + e.ToString());
            IPAddress ip = IPAddress.Parse(GetIP());
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.BeginConnect(ip, m_nPort, new AsyncCallback(ConnectCallback), m_Socket);
            BeginReceive();


        }


    }

    public void OnReConnect()
    {
        m_bReConnect = true;
        StopCoroutine("N_TimeOut");
        NetState = N_STATE.CONNECTING;// 전송 중이다 
        //NetworkWaitDlg.Instance.Open();
        StartCoroutine(N_IReConnect(true));



    }
    IEnumerator N_IReConnect(bool bflag)
    {

        yield return new WaitForSeconds(1f);

        while(true)
        {
            if(m_SocketType== SOCKETTYPE.DATA)
            {
                m_nSelectSocket++;// 서버 개수 
                if (m_nSelectSocket >= m_szIP.Count) m_nSelectSocket = 0;

            }
            Connect();
            //2초간 기다림
            float ff = Time.time;
            while (NetState != N_STATE.CONNECTED)
            {
                if (Time.time - ff > 3)
                {
                    break;//2초간 기다림
                }
                yield return null;

            }
            if(NetState== N_STATE.CONNECTED)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);

            if (m_SocketType == SOCKETTYPE.DATA)
            {
                if (NetState != N_STATE.CONNECTED)
                {
                    ExitGame();
                    break;
                }
            }
            yield return new WaitForSeconds(3f);/// 접속이 될때 까지

        }




    }

    // 실시간으로 소켓을 체크한다
    bool N_CheckSocket()
    {
        if (m_bReConnect)
        {
            if (NetState == N_STATE.CONNECTED) return true;
            return false;
        }

        // 재전송이라면 들어오면 안됨
        m_bReConnect = false;
        // 에라가 생기면 재전송을 진행한다
        if (NetState == N_STATE.ERROR)
        {

            OnReConnect();
            return false;
        }
        if (NetState == N_STATE.CONNECTED) return true;

        return false;
    }


    List<PacketData> m_CheckData = new List<PacketData>();
    void N_StartTimeOut(PacketData _Data)
    {
        StopCoroutine("N_TimeOut");
        m_CheckData.Add(_Data);
        if (!_Data.m_bReceive) return;
        if (NetState == N_STATE.CONNECTED)
        {
            StartCoroutine("N_TimeOut");
        }

    }
    void N_StopTimeOut()
    {
        if (m_CheckData.Count > 0)
            m_CheckData.RemoveAt(0);

        StopCoroutine("N_TimeOut");
        //        NetworkMessageDlg.Instance.Close();
        //      NetworkWaitDlg.Instance.Close();
    }
    IEnumerator N_TimeOut()
    {
        
        //NetworkWaitDlg.Instance.Open();
        yield return new WaitForSecondsRealtime(3);

        //NetworkWaitDlg.Instance.Close();
        Message("Time out 서버연결 실패!");
        NetState = N_STATE.ERROR;


    }

    bool m_bPing = false;
    public void SendPing()
    {
        if (m_bPing) return;
         m_bPing = true;
        //Debug.Log("Send Ping" );
        JObject JData = new JObject();
        JData.Add("File", "./file/ping");
        JData.Add("IDX", CWHero.Instance.m_nID);
        Send(JData,(jData)=> {

            m_bPing = false;
        });
    }

    IEnumerator IPing()
    {
        while(true)
        {
            yield return new WaitForSeconds(Pingtime);
            SendPing();
        }
    }


    #endregion

    #region 데이타 정의 



    
    public List<string> m_szIP = new List<string>();  // 서버 IP
    public int m_nPort;   // 서버 Port

    class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
    }
    
    

    int m_Uniqe = 1;
    String GetUniqFuncname()
    {
        m_Uniqe++;
        return m_Uniqe.ToString();
    }

    Dictionary<string, RECEIVEFUCION> m_FuctionList = new Dictionary<string, RECEIVEFUCION>();

    private Socket m_Socket = null;
    public class PacketData
    {
        
        public bool m_bReceive = true;
        public List<byte> m_SendArray = new List<byte>();
        public PacketData(CWSocket kSock, JObject JData, RECEIVEFUCION fuc = null)
        {


            

            if (fuc != null)
            {
                string szfuc = kSock.GetUniqFuncname();// 고유한 함수이름이 필요하다!
                JData.Add("Func", szfuc);
                m_bReceive = true;
                if (fuc != null)
                {
                    if (!kSock.m_FuctionList.ContainsKey(szfuc))
                    {
                        kSock.m_FuctionList.Add(szfuc, fuc);
                    }
                }

            }
            else m_bReceive = false;

            MemoryStream ms = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, JData);

                MemoryStream cdata = CWLib.compress(ms.ToArray());

                byte[] _Size = BitConverter.GetBytes((int)(cdata.Length + 4));    // 사이즈 바이트 배열로 변환
                m_SendArray.Clear();
                m_SendArray.AddRange(_Size);
                m_SendArray.AddRange(cdata.ToArray());
            }


        }
    }

    public delegate PacketData SENDFUNC();

    private List<byte> m_kReceiveArray = new List<byte>();    // 리시브 데이터 리스트

    #endregion

    #region 에라관련

    

    // 뒷북치는 에라 방지!
    bool m_bConnectflag = false;// 이값이 true가 될때만 callback 함수 작동


    #endregion

    #region 접속(접속,종료,재접속)


    int m_nSelectSocket = 0;

    // 서버를 고정 시킨다
    

    IEnumerator IConnect(Action func)
    {
        DisConnect();
        yield return new WaitForSeconds(0.1f);
        func();
    }


    string GetIP()
    {

        return m_szIP[m_nSelectSocket];
    }


    void ConnectCallback(IAsyncResult ar)
    {

        if (m_bConnectflag == false)
        {
            print("뒷북 콜백!!");
            return;// 콜백함수가 없어야 한다
        }

        if (ar == null)
        {
            return;
        }


        try
        {
            m_Socket.EndConnect(ar);

            print("Connect OK!");
            m_bConnectflag = false;
            m_bReConnect = false;
            NetState = N_STATE.CONNECTED;//접속 성공
            BeginReceive();
        }
        catch (Exception e)
        {
            print("ConnectCallback error!!");
            Debug.Log(e);
            ErrorException(e);

        }
    }
    void BeginReceive()
    {
        try
        {

            StateObject state = new StateObject();
            state.workSocket = m_Socket;
            // Begin receiving the data from the remote device.  
            m_Socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);


        }
        catch (Exception e)
        {
            ErrorException(e);
        }
    }







    #endregion

    #region Send 관련

    

    private List<PacketData> m_DataList = new List<PacketData>();
    void AddDatatList(PacketData _Data)
    {
        // 접속을 한다
        m_DataList.Add(_Data);
    }

    void RemoveDataList(PacketData _Data)
    {
        m_DataList.Remove(_Data);
    }


    void SendCallback(IAsyncResult ar)
    {

        if (NetState != N_STATE.CONNECTED) return;

        if (m_Socket == null || m_Socket.Connected == false)
        {

            return;
        }
        try
        {

            m_Socket.EndSend(ar);
        }
        catch (Exception e)
        {
            Debug.Log(e);

            ErrorException(e);

        }

    }




    #endregion
    #region Receive 관련

    void Connectok(JObject _Data)
    {
        //m_CheckData
        foreach (var v in m_CheckData)
        {
            AddDatatList(v);
        }
        if(m_SocketType== SOCKETTYPE.ONLINE)
        {
            StartCoroutine("IPing");
        }
    }

    public void DisConnect()
    {
        if (m_Socket == null) return;

        if (m_Socket.Connected) m_Socket.Shutdown(SocketShutdown.Both); // 소켓을 비활성화 시킵니다.
        m_Socket.Close(); //소켓을 닫습니다.
        m_Socket = null;
        NetState = N_STATE.NOT_CONNECT;
        Debug.Log("================Close networking===================");
    }
    void ErrorException(Exception ex)
    {
        Debug.Log("============Error_in Socket Processing : =================" + ex.ToString());
        DisConnect();
        NetState = N_STATE.ERROR;

    }

    const int BeginReceiveDataLength = 4096;
    private byte[] BeginReceiveData = new byte[BeginReceiveDataLength]; //서버로 부터 받을 데이터 크기

    void ReceiveCallback(IAsyncResult _AR)
    {

        if (m_Socket == null || !m_Socket.Connected) return;
        try
        {

            StateObject _State = (StateObject)_AR.AsyncState;
            SocketError socketErr;

            int _ReadLength = m_Socket.EndReceive(_AR, out socketErr);
            if (_ReadLength <= 0)
            {
                BeginReceive();
                return;
            }
            if (socketErr == SocketError.Success)
            {
                if (_ReadLength > 0)
                {

                    lock (m_kReceiveArray)
                    {
                        m_kReceiveArray.AddRange(_State.buffer.Take(_ReadLength)); // 리시브 리스트에 받은 데이터 추가
                    }
                    BeginReceive();

                }

            }
            else
            {
                Debug.LogError("Recieve Error : " + socketErr.ToString());
                return;
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            ErrorException(e);


        }

    }
    void RunFuction(string szFuc, JObject jData)
    {
        if (m_FuctionList.ContainsKey(szFuc))
        {
            RECEIVEFUCION ff = m_FuctionList[szFuc];
            ff(jData);
            m_FuctionList.Remove(szFuc);// 유니크한 함수를 사용하기 위해서 지운다 
        }
    }
    void Update()
    {


        if (N_CheckSocket())
        {
            Receive();
        }



    }
    void Receive()
    {
        if (m_Socket == null) return;
        lock (m_kReceiveArray)
        {
            if (m_kReceiveArray.Count >= 4)   // 리시브 데이터 길이가 4보다 클때
            {
                int _Size = BitConverter.ToInt32(m_kReceiveArray.GetRange(0, 4).ToArray(), 0);    // 사이즈를 받아옴

                if (m_kReceiveArray.Count >= _Size)   // 데이터 사이즈를 모두 받았을때
                {
                    N_StopTimeOut();
                    m_kReceiveArray.RemoveRange(0, 4);    // 사이즈 정보 제거

                    MemoryStream ms = CWLib.Uncompress(m_kReceiveArray.GetRange(0, _Size - 4).ToArray());
                    JObject JData;
                    using (BsonReader reader = new BsonReader(ms))
                    {
                        JData = (JObject)JToken.ReadFrom(reader);
                    }
                    m_kReceiveArray.RemoveRange(0, _Size - 4);    // 데이터 제거
                    if (JData["Func"] != null)
                    {
                        RunFuction(JData["Func"].ToString(), JData);
                    }
                    if(m_SocketType== SOCKETTYPE.ONLINE)
                    {
                        if (JData["Msg"] != null)
                        {
                            CWSocketManager.Instance.gameObject.SendMessage(JData["Msg"].ToString(), JData);
                        }

                    }



#if UNITY_EDITOR
                    if (_Size > 1000)
                    {
                        Debug.Log(string.Format("Bigsize= {0} -JSon{1}", _Size, JData.ToString()));
                    }

#endif


                }
            }
        }
    }
    #endregion

    #region 외부 접근함수

    public bool IsConnected()
    {
        if (NetState == N_STATE.CONNECTED)
        {
            return true;
        }
        return false;

    }
    public bool IsDisconnect()
    {
        if (NetState != N_STATE.CONNECTED)
        {
            return true;
        }
        return false;
    }
    public void Close(Action Func = null)
    {


    }
    private void Start()
    {

        StartCoroutine("N_ISend");
    }
    public void Create(List<string> kList,int Port,SOCKETTYPE kType)
    {

        m_szIP.Clear();
        m_SocketType = kType;
        m_nPort = Port;
        if(m_SocketType== SOCKETTYPE.ONLINE)
        {
            m_szIP.Add(kList[0]);
        }
        else
        {
            m_szIP.AddRange(kList);
        }
    }



    #endregion



}
