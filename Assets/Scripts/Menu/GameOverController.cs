using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    public void RetryToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}