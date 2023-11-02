using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public enum State
{
    None,
    AcceptClient,
    ServerCommunication,
    StopListener,
    Endcommunication
}

public class TcpServer : MonoBehaviour
{
    public Socket m_listener;
    public int m_port;
    public State m_state;
    public Socket serverSocket;

    public string mAddress = "";
    public InputField protField;
    public Text addressProtText;
    [SerializeField]public PlayerInfo playerInfo;

    private void Start()
    {
        

    }

    void Update()
    {
        switch (m_state)
        {
            case State.AcceptClient:
                AcceptClient();
                break;
            case State.ServerCommunication:
                ServerCommunication();
                break;
            case State.StopListener:
                StopListener();
                break;
            default:
                Debug.Log("오류");
                break;
        }
    }

    public void StartServer()
    {
        mAddress = "";

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Debug.Log(ip.ToString());

                if (mAddress == "") mAddress = ip.ToString();
            }
        }

        mAddress += ":" + protField.text;
        addressProtText.text = mAddress;

        m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_listener.Bind(new IPEndPoint(IPAddress.Any, int.Parse(protField.text))); // 연결상태

        Debug.Log("Start Listen..");
        m_listener.Listen(1); // 연결길이

        m_state = State.AcceptClient;
    }

    void AcceptClient()
    {
        if (m_listener != null && m_listener.Poll(0, SelectMode.SelectRead))
        {
            serverSocket = m_listener.Accept();
            Debug.Log("[TCP]Connected from client");
            m_state = State.ServerCommunication;
        }
    }

    void ServerCommunication()
    {
        byte[] buffer = new byte[1400];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint senderRemote = sender;

        int recvSize = serverSocket.Receive(buffer, buffer.Length, SocketFlags.None);
        if (recvSize > 0)
        {
            string message = System.Text.Encoding.UTF8.GetString(buffer);
            string packetType = message.Split('#')[0];
            string packetData = message.Split('#')[1];
            Debug.Log(packetType);

            switch (packetType)
            {
                case "PlayerInfo":
                    PlayerInfo playerInfo = new PlayerInfo();
                    playerInfo = JsonUtility.FromJson<PlayerInfo>(packetData);
                    Debug.Log(string.Format("닉네임 : {0}, ip : {1}, port : {2}",
                        playerInfo.nickname,
                        playerInfo.playerAddress,
                        playerInfo.playerPort));
                    break;
                default:
                    Debug.Log("정의되지 않은 패킷입니다. : " + packetType);
                    break;
            }
        }
    }

    public void StopListener()
    {
        if (m_listener != null)
        {
            m_listener.Close();
            m_listener = null;
        }

        m_state = State.Endcommunication;
        Debug.Log("[TCP]End server communication");
    }
}
