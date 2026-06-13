using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public void Host()
    {
        SceneManager.LoadScene(1);
    }

    public void Join() // falta implementar. Somente na segunda fase do projeto
    {
        return;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
