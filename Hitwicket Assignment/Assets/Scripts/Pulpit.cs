using UnityEngine;
using TMPro;

public class Pulpit : MonoBehaviour
{
    public GameManager gm;
    public float lifetime;
    public float spawnTriggerTime;
    public bool spawnTriggered;
    public Vector3 gridPos;

    public TextMeshPro timerText;

    float elapsed;

    public void Init(GameManager manager, Vector3 pos, float life, float triggerTime)
    {
        gm = manager;
        gridPos = pos;
        lifetime = life;
        spawnTriggerTime = triggerTime;
        elapsed = 0f;
        spawnTriggered = false;

        UpdateTimerText();
        if (timerText == null)
        {
            Debug.LogWarning("Pulpit at " + pos + " has no timerText assigned!");
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        UpdateTimerText();

        if (!spawnTriggered && elapsed >= spawnTriggerTime)
        {
            spawnTriggered = true;
            gm.OnPulpitThresholdReached(this);
        }

        if (elapsed >= lifetime)
        {
            gm.OnPulpitExpired(this);
            Destroy(gameObject);
        }
    }

    void UpdateTimerText()
    {

        float remaining = Mathf.Max(0f, lifetime - elapsed);
        timerText.text = remaining.ToString("0.0") + "s";
    }
}
