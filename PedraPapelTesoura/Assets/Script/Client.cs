using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System;

public class Client : MonoBehaviour
{
    [Header("Client Settings")]
    public TcpClient client;
    public NetworkStream stream;
    public Thread receiveThread;

    public int port = 9000;
    public string serverIP;

    [Header("Message Settings")]
    public string receivedMessage;

    private string messageBuffer = "";

    public static Action<string> OnReceivedMessage;

    void Start()
    {

        try
        {
            client = new TcpClient();
            client.Connect(serverIP, port);

            Connect();
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao conectar: " + e.Message);
        }
    }

    void Connect()
    {
        if (client != null && client.Connected)
        {
            Debug.Log("Conectado ao servidor!");

            stream = client.GetStream();

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        else
        {
            Debug.LogError("Não foi possível conectar ao servidor.");
        }
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
                    Debug.Log("Servidor desconectou.");
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
                Debug.LogError("Erro ao receber: " + e.Message);
                break;
            }
        }
    }

    public void SendData(string sentMessage)
    {
        if (client == null || !client.Connected || stream == null)
        {
            Debug.LogWarning("Não conectado ao servidor.");
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
            Debug.LogError("Erro ao enviar: " + e.Message);
        }
    }

    void OnDestroy()
    {
        receiveThread?.Abort();

        stream?.Close();
        client?.Close();
    }


}
