using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsMenuController : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}