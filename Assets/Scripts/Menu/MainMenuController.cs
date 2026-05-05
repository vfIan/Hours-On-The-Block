using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject controlsPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenControls()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        if (mainPanel != null)
            mainPanel.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}