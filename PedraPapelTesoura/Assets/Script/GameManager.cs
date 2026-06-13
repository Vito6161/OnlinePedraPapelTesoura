using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isHost = true;
    public string escolha1, escolha2, vencedor;

    [Header("Canvas")]
    [SerializeField] private GameObject canvas1;
    [SerializeField] private GameObject canvas2;
    [SerializeField] private GameObject canvas3;

    [Header("Final Texts")]
    [SerializeField] private TMPro.TextMeshProUGUI escolha1Text;
    [SerializeField] private TMPro.TextMeshProUGUI escolha2Text;
    [SerializeField] private TMPro.TextMeshProUGUI vencedorText;

    void OnAwake()
    {
        ChangeCanvas(0);
        isHost = true;
    }


    public void Escolha(int escolha)
    {
        if(isHost)
        {
            switch (escolha)
            {
                case 0:
                    escolha1 = "Pedra";
                    break;
                case 1:
                    escolha1 = "Papel";
                    break;
                case 2:
                    escolha1 = "Tesoura";
                    break;
            }

            isHost = false;
            ChangeCanvas(1);
        }
        else
        {
            switch (escolha)
            {
                case 0:
                    escolha2 = "Pedra";
                    break;
                case 1:
                    escolha2 = "Papel";
                    break;
                case 2:
                    escolha2 = "Tesoura";
                    break;
            }

            CheckWinner();
            ChangeCanvas(2);
        }
    }

    void CheckWinner()
    {
        if(escolha1 == escolha2)
        {
            vencedor = "Empate!";
        }
        else if((escolha1 == "Pedra" && escolha2 == "Tesoura") || (escolha1 == "Papel" && escolha2 == "Pedra") || (escolha1 == "Tesoura" && escolha2 == "Papel"))
        {
            vencedor = "Player 1 Ganhou!";
        }
        else
        {
            vencedor = "Player 2 Ganhou!";
        }

        FinalScreen();
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
