using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("[MainMenuUI] Loaded HighScore = " + highScore);

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
        else
        {
            Debug.LogWarning("[MainMenuUI] highScoreText is not assigned in Inspector.");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
