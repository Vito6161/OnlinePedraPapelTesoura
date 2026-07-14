using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManagerClient : MonoBehaviour
{
    private Client client;

    [Header("Game State")]
    public string escolha1;
    public string escolha2;
    public string vencedor;

    [Header("Canvas")]
    [SerializeField] private GameObject canvas1;
    [SerializeField] private GameObject canvas2;
    [SerializeField] private GameObject canvas3;

    [Header("Final Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI escolha1Text;
    [SerializeField] private TMPro.TextMeshProUGUI escolha2Text;
    [SerializeField] private TMPro.TextMeshProUGUI vencedorText;

    private readonly Queue<string> mensagens = new Queue<string>();

    void Awake()
    {
        client = FindObjectOfType<Client>();

        ChangeCanvas(0);
    }

    void OnEnable()
    {
        Client.OnReceivedMessage += ReceberMensagem;
    }

    void OnDisable()
    {
        Client.OnReceivedMessage -= ReceberMensagem;
    }

    void Update()
    {
        lock (mensagens)
        {
            while (mensagens.Count > 0)
            {
                ProcessarMensagem(mensagens.Dequeue());
            }
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
        if (mensagem == "state:yourturn")
        {
            ChangeCanvas(1);
        }
        else if (mensagem.StartsWith("escolha1:"))
        {
            escolha1 = mensagem.Substring(9);
        }
        else if (mensagem == "winner:player1")
        {
            vencedor = "Player 1 Ganhou!";
            ChangeCanvas(2);
            FinalScreen();
        }
        else if (mensagem == "winner:player2")
        {
            vencedor = "Player 2 Ganhou!";
            ChangeCanvas(2);
            FinalScreen();
        }
        else if (mensagem == "winner:tie")
        {
            vencedor = "Empate!";
            ChangeCanvas(2);
            FinalScreen();
        }
    }

    public void setEscolha2(string escolha)
    {
        escolha2 = escolha;

        client.SendData("escolha2:" + escolha2);
    }

    public void ChangeCanvas(int index)
    {
        canvas1.SetActive(index == 0);
        canvas2.SetActive(index == 1);
        canvas3.SetActive(index == 2);
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
}