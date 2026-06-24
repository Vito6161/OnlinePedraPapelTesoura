using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
public class Client : MonoBehaviour
{
    [Header("Client Settings")]
    public TcpClient client;
    public NetworkStream stream;
    public Thread receiveThread;
    public int port = 7777;
    public string serverIP = "192.168.0.4";

    [Header("Message Settings")]
    public string receivedMessage;

    void Start()    
    {
        client = new TcpClient();
        client.Connect(serverIP, port);

        Connect();        
    }

    void Connect()
    {
        if(client.Connected)
        {
            stream = client.GetStream();

            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.Start();
        }
        else
        {
            Debug.Log("Não foi possível conectar ao servidor");
        }
    }

    public void ReceiveData()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if(bytesRead == 0)
            {
                Debug.Log("Servidor desconectou");
                break;
            }

            receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
            OnReceivedMessage?.Invoke(receivedMessage);
        }
    }

    public void SendData(string sentMessage)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(sentMessage);
        stream.Write(buffer, 0, buffer.Length); 
    }

    public static Action<string> OnReceivedMessage;

}
