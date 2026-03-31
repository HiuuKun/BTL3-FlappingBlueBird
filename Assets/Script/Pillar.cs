using UnityEngine;

public class Pillar : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform coinSpawnPoint;

    [Range(0f, 1f)]
    public float spawnChance = 1f;

    void Start()
    {
        // delay one frame so transform is fully correct
        //Invoke(nameof(SpawnCoin), 0f);
    }

    public void SpawnCoin()
    {
        
        if (coinPrefab == null)
        {
            Debug.LogWarning($"[{name}] coinPrefab is missing.");
            return;
        }

        if (coinSpawnPoint == null)
        {
            Debug.LogWarning($"[{name}] coinSpawnPoint is missing.");
            return;
        }
        if (coinPrefab == null || coinSpawnPoint == null)
            return;

        if (Random.value > spawnChance)
            return;

        Instantiate(
            coinPrefab,
            coinSpawnPoint.position,
            Quaternion.identity,
            transform.parent
        );
    }
}