using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class GameManager : MonoBehaviour
{
    private Server server;
    public bool isPlayerConnected;

    [Header("Game State")]
    public string escolha1;
    public string escolha2;
    public string vencedor;

    [Header("Canvas")]
    [SerializeField] private GameObject canvas0;
    [SerializeField] private GameObject canvas1;
    [SerializeField] private GameObject canvas2;
    [SerializeField] private GameObject canvas3;

    [Header("Final Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI escolha1Text;
    [SerializeField] private TMPro.TextMeshProUGUI escolha2Text;
    [SerializeField] private TMPro.TextMeshProUGUI vencedorText;
    [SerializeField] private TMPro.TextMeshProUGUI ipText;

    //Fila de mensagens (thread segura)
    private readonly Queue<string> mensagens = new Queue<string>();

    void Awake()
    {
        server = FindObjectOfType<Server>();

        //ChangeCanvas(0);
    }

    void OnEnable()
    {
        Server.OnReceivedMessage += ReceberMensagem;
    }

    void OnDisable()
    {
        Server.OnReceivedMessage -= ReceberMensagem;
    }

    void Start()
    {
        ipText.text = GetLocalIPAddress();
    }

    void Update()
    {
        while (mensagens.Count > 0)
        {
            ProcessarMensagem(mensagens.Dequeue());
        }

        if(isPlayerConnected) 
        {
            ChangeCanvas(0);
            isPlayerConnected = false;
        }
    }

    void ReceberMensagem(string mensagem)
    {
        lock (mensagens)
        {
            mensagens.Enqueue(mensagem);
        }
    }

    void ProcessarMensagem(string mensagem)
    {
        if (mensagem.StartsWith("escolha2:"))
        {
            escolha2 = mensagem.Substring(9);

            CheckWinner();
            ChangeCanvas(2);
        }
    }

    //Player 1 escolheu
    public void setEscolha1(string escolha)
    {
        escolha1 = escolha;

        server.SendData("state:yourturn");
        server.SendData("escolha1:" + escolha1);

        ChangeCanvas(1);
    }

    void CheckWinner()
    {
        if (escolha1 == escolha2)
        {
            vencedor = "Empate!";
            server.SendData("winner:tie");
        }
        else if (
            (escolha1 == "Pedra" && escolha2 == "Tesoura") ||
            (escolha1 == "Papel" && escolha2 == "Pedra") ||
            (escolha1 == "Tesoura" && escolha2 == "Papel"))
        {
            vencedor = "Player 1 Ganhou!";
            server.SendData("winner:player1");
        }
        else
        {
            vencedor = "Player 2 Ganhou!";
            server.SendData("winner:player2");
        }

        FinalScreen();
    }

    public void ChangeCanvas(int index)
    {
        canvas1.SetActive(index == 0);
        canvas2.SetActive(index == 1);
        canvas3.SetActive(index == 2);
        canvas0.SetActive(index == 3);
    }

    void FinalScreen()
    {
        escolha1Text.text = escolha1;
        escolha2Text.text = escolha2;
        vencedorText.text = vencedor;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "IP não encontrado";
    }
}