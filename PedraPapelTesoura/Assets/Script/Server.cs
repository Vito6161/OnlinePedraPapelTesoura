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

    Thread receiveThread;
    Thread waitingClientThread;

    [Header("Message Settings")]
    public string receivedMessage;

    private string messageBuffer = "";

    public static Action<string> OnReceivedMessage;

    [SerializeField] private GameManager manager;

    void Start()
    {
        Debug.Log("Iniciando servidor...");
        
        server = new TcpListener(IPAddress.Any, port);
        server.Start();

        Debug.Log("Servidor iniciado na porta: " + port);

        waitingClientThread = new Thread(WaitForClient);
        waitingClientThread.IsBackground = true;
        waitingClientThread.Start();
    }

    void WaitForClient()
    {
        Debug.Log("Esperando cliente...");

        client = server.AcceptTcpClient();
        manager.isPlayerConnected = true;

        Debug.Log("Cliente conectado!");

        stream = client.GetStream();

        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    public void ReceiveData()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Debug.Log("Cliente desconectou.");
                    break;
                }

                messageBuffer += System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                while (messageBuffer.Contains("\n"))
                {
                    int index = messageBuffer.IndexOf('\n');

                    string message = messageBuffer.Substring(0, index).Trim();

                    messageBuffer = messageBuffer.Substring(index + 1);

                    receivedMessage = message;

                    Debug.Log("Recebido: " + receivedMessage);

                    OnReceivedMessage?.Invoke(receivedMessage);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Erro ao receber: " + e.Message);
                break;
            }
        }
    }

    public void SendData(string sentMessage)
    {
        if (client == null || !client.Connected || stream == null)
        {
            Debug.LogWarning("Nenhum cliente conectado.");
            return;
        }

        try
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(sentMessage + "\n");
            stream.Write(data, 0, data.Length);

            Debug.Log("Enviado: " + sentMessage);
        }
        catch (Exception e)
        {
            Debug.Log("Erro ao enviar: " + e.Message);
        }
    }

    void OnDestroy()
    {
        receiveThread?.Abort();
        waitingClientThread?.Abort();

        stream?.Close();
        client?.Close();
        server?.Stop();
    }
}