using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class TcpClinet : MonoBehaviour
{
    private Socket clientSocket = null;
    public string m_address = "127.0.0.1";
    private const int m_port = 44444;
    public InputField mAddressPortField;
    string[] mTotalAddress;
    [SerializeField] public PlayerInfo playerinfo;

    public void ClientConnet()
    {
        Debug.Log("[TCP]Start client communication");

        mTotalAddress = mAddressPortField.text.Split(':');

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.NoDelay = true;
        clientSocket.SendBufferSize = 0;
        clientSocket.Connect(mTotalAddress[0], int.Parse(mTotalAddress[1]));

    }

    public void SendMessageToServerMyData(PlayerInfo plinfo)
    {
        Debug.Log("[TCP]Start client communication");

        mTotalAddress = mAddressPortField.text.Split(':');

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.NoDelay = true;
        clientSocket.SendBufferSize = 0;
        clientSocket.Connect(mTotalAddress[0], int.Parse(mTotalAddress[1]));



        mTotalAddress = mAddressPortField.text.Split(':');

        //clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        string data = JsonUtility.ToJson(plinfo);

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(string.Format("PlayerInfo#{0}", data));
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(mTotalAddress[0]),int.Parse(mTotalAddress[1]));
        clientSocket.SendTo(buffer, buffer.Length, SocketFlags.None, endPoint);
    }

    public void datasend()
    {
        SendMessageToServerMyData(playerinfo);
    }

}
