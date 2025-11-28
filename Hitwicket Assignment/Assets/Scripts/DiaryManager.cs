using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DiaryManager : MonoBehaviour
{
    public string diaryUrl = "https://s3.ap-south-1.amazonaws.com/superstars.assetbundles.testbuild/doofus_game/doofus_diary.json";
    public GameManager gameManager;

    void Start()
    {
        StartCoroutine(LoadDiary());
    }

    IEnumerator LoadDiary()
    {
        UnityWebRequest request = UnityWebRequest.Get(diaryUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load diary: " + request.error);
            yield break;
        }

        DoofusDiary diary = JsonUtility.FromJson<DoofusDiary>(request.downloadHandler.text);
        gameManager.diary = diary;     // directly assign
        gameManager.StartGame();
        Debug.Log("Speed = " + diary.player_data.speed);
        Debug.Log("Min Time = " + diary.pulpit_data.min_pulpit_destroy_time);
        Debug.Log("Max Time = " + diary.pulpit_data.max_pulpit_destroy_time);
        Debug.Log("Spawn Time = " + diary.pulpit_data.pulpit_spawn_time);
        // start immediately
    }
}
