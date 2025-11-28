using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText;

    void Awake()
    {
        Instance = this;
    }

    public void SetScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }
}
