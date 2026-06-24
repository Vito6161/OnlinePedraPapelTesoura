using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

public class Server : MonoBehaviour
{
    [Header("Server Settings")]
    public TcpListener server;
    public TcpClient client;
    public int port = 7777;
    public NetworkStream stream;
    Thread receiveThread, waitingClientThread;

    [Header("Message Settings")]
    public string receivedMessage;
    
    void Start()
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Debug.Log("Servidor iniciado na porta: " + port);    

        waitingClientThread = new Thread(new ThreadStart(WaitForClient));
        waitingClientThread.Start();
    }

    void WaitForClient()
    {

        client = server.AcceptTcpClient();
        stream = client.GetStream();

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.Start();
            
    }


    public void ReceiveData()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if(bytesRead == 0)
            {
                Debug.Log("Cliente desconectou");
                break;
            }

            receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
            OnReceivedMessage?.Invoke(receivedMessage);
        }
    }

    public void SendData(string sentMessage)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(sentMessage);
        stream.Write(data, 0, data.Length);
    }

    public static Action<string> OnReceivedMessage;


}
