using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerClient : MonoBehaviour
{

    private Client client;
    
    [Header("Game State")]
    public string escolha1, escolha2, vencedor;

    [Header("Canvas")]
    [SerializeField] private GameObject canvas1;
    [SerializeField] private GameObject canvas2;
    [SerializeField] private GameObject canvas3;

    [Header("Final Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI escolha1Text;
    [SerializeField] private TMPro.TextMeshProUGUI escolha2Text;
    [SerializeField] private TMPro.TextMeshProUGUI vencedorText;

    void Awake()
    {
        ChangeCanvas(0);
    }

    void OnEnable()
    {
        Client.OnReceivedMessage += OnReceivedMessage;
    }

    void OnDisable()
    {
        Client.OnReceivedMessage -= OnReceivedMessage;
    }  

    public void setEscolha2(string escolha)
    {
        escolha2 = escolha;
        client.SendData(escolha2);
        ChangeCanvas(1);
    }

    void OnReceivedMessage(string message)
    {
        switch(message)
        {
            case "state:yourturn":
                ChangeCanvas(1);
                break;
            case "escolha1:Pedra":
                escolha1 = "Pedra";
                break;
            case "escolha1:Papel":
                escolha1 = "Papel";
                break;
            case "escolha1:Tesoura":
                escolha1 = "Tesoura";
                break;
            case "winner:player1":
                vencedor = "Player 1 Ganhou!";
                ChangeCanvas(2);
                FinalScreen();
                break;
            case "winner:player2":
                vencedor = "Player 2 Ganhou!";
                ChangeCanvas(2);
                FinalScreen();
                break;
            case "winner:tie":
                vencedor = "Empate!";
                ChangeCanvas(2);
                FinalScreen();
                break;
        }


    }
    
    void ChangeCanvas(int index)
    {
        switch (index)
        {
            case 0:
                canvas1.SetActive(true);
                canvas2.SetActive(false); //desativa tudo e deixa so o 1 (player 1)
                canvas3.SetActive(false);
                break;
            case 1:
                canvas1.SetActive(false);
                canvas2.SetActive(true); // desativa tudo e deixa so o 2 (player 2)
                canvas3.SetActive(false);
                break;
            case 2:
                canvas1.SetActive(false);
                canvas2.SetActive(false); // desativa tudo e deixa so o 3 (tela final)
                canvas3.SetActive(true);
                break;
        
        }
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
