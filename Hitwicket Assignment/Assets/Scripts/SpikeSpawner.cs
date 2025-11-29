using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    public GameObject spikePrefab;
    public float gap = 2f;
    public float yOffset = -10f;

    void Start()
    {
        SpawnGrid();
    }

    void SpawnGrid()
    {
        for (int x = -100; x < 100; x++)
        {
            for (int z = -100; z < 100; z++)
            {
                Vector3 pos = new Vector3(x * gap, yOffset, z * gap);
                Instantiate(spikePrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
