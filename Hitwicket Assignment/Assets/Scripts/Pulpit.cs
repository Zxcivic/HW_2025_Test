using UnityEngine;

public class Pulpit : MonoBehaviour
{
    public GameManager gm;
    public float lifetime;
    public float spawnTriggerTime;
    public bool spawnTriggered;
    public Vector3 gridPos;

    float elapsed;

    public void Init(GameManager manager, Vector3 pos, float life, float triggerTime)
    {
        gm = manager;
        gridPos = pos;
        lifetime = life;
        spawnTriggerTime = triggerTime;
        elapsed = 0f;
        spawnTriggered = false;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

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
}
