using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class CWUDP {

    
    int m_nSendPort;
    int m_nReceivePort;
    string m_szIP;

    

    UdpClient m_kudpClient;

    IPEndPoint serverEndpoint ;
    Thread m_kreceiveThread=null;
    private bool m_bthreadRunning = false;
    private readonly Queue<byte[]> incomingQueue = new Queue<byte[]>();

    public string StartConnection(string szip,int nsport,int nrport)
    {
        m_szIP = szip;
        m_nSendPort = nsport;
        m_nReceivePort = nrport;

        try { m_kudpClient = new UdpClient(m_nReceivePort); }
        catch (Exception e)
        {
            string sz = string.Format("Failed to listen for UDP at port " + m_nReceivePort + ": " + e.Message);
            CWUnityLib.DebugX.Log(sz);
            return sz;
        }


        try
        {


            IPHostEntry _HE = Dns.GetHostEntry(m_szIP);// Find host name

            foreach (var v in _HE.AddressList)
            {
                if (v.AddressFamily == AddressFamily.InterNetwork)
                {
                    serverEndpoint = new IPEndPoint(v, m_nSendPort);
                    break;
                }
            }



        }
        catch (Exception e)
        {
            IPAddress ip = IPAddress.Parse(m_szIP);
            serverEndpoint = new IPEndPoint(ip, m_nSendPort);
            Debug.Log(e);
        }




        CWDebugManager.Instance.Log(string.Format("udp ip= {0} ,{1} ", szip, m_nSendPort));

        //.Instance.Log("Set sendee at ip " + m_szIP + " and port " + m_nSendPort);

        StartReceiveThread();
        return "ok!";
    }
    private void StartReceiveThread()
    {
        m_kreceiveThread = new Thread(() => ListenForMessages(m_kudpClient));
        m_kreceiveThread.IsBackground = true;
        m_bthreadRunning = true;
        m_kreceiveThread.Start();
    }

    private void ListenForMessages(UdpClient client)
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (m_bthreadRunning)
        {
            try
            {
                Byte[] receiveBytes = client.Receive(ref remoteIpEndPoint); // Blocks until a message returns on this socket from a remote host.

                lock (incomingQueue)
                {
                    incomingQueue.Enqueue(receiveBytes);
                }
            }
            catch (SocketException e)
            {
                // 10004 thrown when socket is closed
                if (e.ErrorCode != 10004) Debug.Log("Socket exception while receiving data from udp client: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error receiving data from udp client: " + e.Message);
            }
            Thread.Sleep(1);
        }
    }
    public List<byte[]> getMessages()
    {
        
        List<byte[]> pendingMessages = new List<byte[]>();

        lock (incomingQueue)
        {
            pendingMessages = new List<byte[]>();
            int i = 0;
            while (incomingQueue.Count != 0)
            {
                pendingMessages.Add(incomingQueue.Dequeue());
                i++;
            }
        }
        return pendingMessages;
    }

    public void Send(byte [] sendBytes)
    {

        if(m_kudpClient==null)
        {
            return;
        }




        ///////////////////////////
//        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(m_szIP), m_nSendPort);
        m_kudpClient.Send(sendBytes, sendBytes.Length, serverEndpoint);
        

    }

    public void Close()
    {
        m_bthreadRunning = false;
        if (m_kreceiveThread == null) return;

        m_kreceiveThread.Abort();
        m_kudpClient.Close();
        m_kreceiveThread = null;
    }
}
