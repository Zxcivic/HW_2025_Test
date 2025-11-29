using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public DoofusDiary diary;
    public Pulpit lastPulpit;
    public DoofusController doofusController;

    public string diaryUrl = "https://s3.ap-south-1.amazonaws.com/superstars.assetbundles.testbuild/doofus_game/doofus_diary.json";
    public GameObject doofus;
    public GameObject pulpitPrefab;

    public float minTime;
    public float maxTime;
    public float spawnTriggerTime;
    public float stepSize = 9f;

    public int score = 0;
    public List<Pulpit> activePulpits = new List<Pulpit>();
    HashSet<Pulpit> visitedPulpits = new HashSet<Pulpit>();


    public GameObject gameOverPanel;
    public float gameOverFadeDuration = 1f;

    bool isGameRunning = false;
    bool isGameOver = false;
    public int highScore = 0;
    public GameObject winPanel;
    public float fallYThreshold = -5f;
    public AudioSource uiAudioSource;
    public AudioClip youDiedClip;
    public AudioClip youWinClip;




    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            var cg = gameOverPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
        }

        StartCoroutine(LoadDiaryAndStart());
    }

    IEnumerator LoadDiaryAndStart()
    {
        UnityWebRequest req = UnityWebRequest.Get(diaryUrl);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load diary: " + req.error);
            yield break;
        }

        diary = JsonUtility.FromJson<DoofusDiary>(req.downloadHandler.text);

        minTime = diary.pulpit_data.min_pulpit_destroy_time;
        maxTime = diary.pulpit_data.max_pulpit_destroy_time;
        spawnTriggerTime = diary.pulpit_data.pulpit_spawn_time;

        doofusController = doofus.GetComponent<DoofusController>();
        doofusController.gm = this;
        doofusController.speed = diary.player_data.speed;

        StartGame();
    }

    public void StartGame()
    {
        visitedPulpits.Clear();
        score = 0;
        UIManager.Instance.SetScore(score);

        isGameOver = false;
        isGameRunning = false;

        foreach (var p in activePulpits)
        {
            if (p != null) Destroy(p.gameObject);
        }
        activePulpits.Clear();

        score = 0;
        if (UIManager.Instance != null)
            UIManager.Instance.SetScore(score);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            var cg = gameOverPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
        }

        if (doofusController == null)
            doofusController = doofus.GetComponent<DoofusController>();

        doofusController.enabled = true;

        SpawnInitialPulpit();

        isGameRunning = true;
    }

    void SpawnInitialPulpit()
    {
        Vector3 pos = Vector3.zero;
        GameObject pObj = Instantiate(pulpitPrefab, pos, Quaternion.identity);
        Pulpit p = pObj.GetComponent<Pulpit>();

        float life = Random.Range(minTime, maxTime);
        p.Init(this, pos, life, spawnTriggerTime);

        activePulpits.Add(p);
        lastPulpit = p;

        doofus.transform.position = pos + Vector3.up * 1.5f;
        doofusController.SetCurrentPulpit(p);

        if (UIManager.Instance != null)
            UIManager.Instance.SetScore(score);
    }

    public void OnPulpitThresholdReached(Pulpit source)
    {
        if (!isGameRunning) return;
        if (source != lastPulpit) return;

        Vector3 newPos = GetAdjacentPosition(source.gridPos);
        GameObject pObj = Instantiate(pulpitPrefab, newPos, Quaternion.identity);
        Pulpit p = pObj.GetComponent<Pulpit>();

        float life = Random.Range(minTime, maxTime);
        p.Init(this, newPos, life, spawnTriggerTime);

        activePulpits.Add(p);
        lastPulpit = p;
    }

    Vector3 GetAdjacentPosition(Vector3 from)
    {
        Vector3[] dirs =
        {
            new Vector3(stepSize, 0, 0),
            new Vector3(-stepSize, 0, 0),
            new Vector3(0, 0, stepSize),
            new Vector3(-stepSize, 0, 0),
            new Vector3(0, 0, -stepSize)
        };

        List<Vector3> candidates = new List<Vector3>();
        foreach (var d in dirs)
        {
            Vector3 pos = from + d;
            bool occupied = activePulpits.Exists(p => p.gridPos == pos);
            if (!occupied) candidates.Add(pos);
        }

        if (candidates.Count == 0) return from + new Vector3(stepSize, 0, 0);
        return candidates[Random.Range(0, candidates.Count)];
    }

    public void OnPulpitExpired(Pulpit p)
    {
        activePulpits.Remove(p);

        if (!isGameRunning || isGameOver) return;

        if (doofusController != null && doofusController.currentPulpit == p)
        {
            Debug.Log("Game Over: pulpit under Doofus destroyed");
            GameOver();
        }
    }
    void GameWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        isGameRunning = false;

        if (doofusController != null)
            doofusController.enabled = false;

        SaveHighScore();

        if (BGMManager.Instance != null)
            BGMManager.Instance.PauseMusic();

        if (uiAudioSource != null && youWinClip != null)
            uiAudioSource.PlayOneShot(youWinClip);

        if (winPanel != null)
            StartCoroutine(FadeInWin());

        StartCoroutine(ReturnToMainMenu(4f));
    }

    IEnumerator FadeInWin()
    {
        winPanel.SetActive(true);
        CanvasGroup cg = winPanel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        cg.alpha = 0f;
        float t = 0f;

        while (t < gameOverFadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / gameOverFadeDuration);
            cg.alpha = k;
            yield return null;
        }
    }


    public void OnPulpitStepped(Pulpit p)
    {
        if (!isGameRunning || isGameOver) return;

        Debug.Log("Stepped on pulpit at " + p.gridPos);
        if (visitedPulpits.Contains(p)) return;

        visitedPulpits.Add(p);
        score++;
        if (UIManager.Instance != null)
            UIManager.Instance.SetScore(score);
        if (score >= 50)
        {
            GameWin();
        }
    }

    public void OnDoofusFell()
    {
        if (!isGameRunning || isGameOver) return;

        Debug.Log("Game Over: Doofus fell off");
        GameOver();
    }
    IEnumerator ReturnToMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);  // pause before leaving the game
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }


    void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        isGameRunning = false;

        if (doofusController != null)
            doofusController.enabled = false;

        if (BGMManager.Instance != null)
            BGMManager.Instance.PauseMusic();

        if (uiAudioSource != null && youDiedClip != null)
            uiAudioSource.PlayOneShot(youDiedClip);

        if (gameOverPanel != null)
            StartCoroutine(FadeInGameOver());

        StartCoroutine(ReturnToMainMenu(7f));
    }



    IEnumerator FadeInGameOver()
    {
        gameOverPanel.SetActive(true);
        CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        cg.alpha = 0f;
        float t = 0f;

        while (t < gameOverFadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / gameOverFadeDuration);
            cg.alpha = k;
            yield return null;
        }
    }
    void SaveHighScore()
    {
        int previousHigh = PlayerPrefs.GetInt("HighScore", 0);

        if (score > previousHigh)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            Debug.Log("New High Score Saved: " + highScore);
        }
        else
        {
            Debug.Log("No new high score. Existing: " + previousHigh + ", Current: " + score);
        }
    }
    void Update()
    {
        if (!isGameRunning || isGameOver) return;

        if (doofus != null && doofus.transform.position.y < fallYThreshold)
        {
            OnDoofusFell();
        }
    }

}
