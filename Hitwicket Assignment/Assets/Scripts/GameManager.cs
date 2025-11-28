using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


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

    void Start()
    {
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

        var controller = doofus.GetComponent<DoofusController>();
        controller.gm = this;
        controller.speed = diary.player_data.speed;

        SpawnInitialPulpit();
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
        var ctrl = doofus.GetComponent<DoofusController>();
        ctrl.SetCurrentPulpit(p);
        UIManager.Instance.SetScore(score);
    }


    public void OnPulpitThresholdReached(Pulpit source)
    {
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
            new Vector3(0, 0, -stepSize)
        };

        List<Vector3> candidates = new List<Vector3>();
        foreach (var d in dirs)
        {
            Vector3 pos = from + d;
            bool occupied = activePulpits.Exists(p => p.gridPos == pos);
            if (!occupied) candidates.Add(pos);
        }

        if (candidates.Count == 0) return from + dirs[0];
        return candidates[Random.Range(0, candidates.Count)];
    }

    public void OnPulpitExpired(Pulpit p)
    {
        activePulpits.Remove(p);

        if (doofusController.currentPulpit == p)
        {
            Debug.Log("Game Over: pulpit under Doofus destroyed");
        }
    }


    public void OnPulpitStepped(Pulpit p)
    {
        Debug.Log("Stepped on pulpit at " + p.gridPos);
        score++;
        UIManager.Instance.SetScore(score);
    }

    public void OnDoofusFell()
    {
        Debug.Log("Game Over: Doofus fell off");
    }
    public void StartGame()
    {
        minTime = diary.pulpit_data.min_pulpit_destroy_time;
        maxTime = diary.pulpit_data.max_pulpit_destroy_time;
        spawnTriggerTime = diary.pulpit_data.pulpit_spawn_time;

        var controller = doofus.GetComponent<DoofusController>();
        controller.speed = diary.player_data.speed;
        controller.gm = this;

        SpawnInitialPulpit();
    }

}
