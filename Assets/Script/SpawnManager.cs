using System.Collections.Generic;
using UnityEngine;
public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Point Parents")]
    public Transform topPillarSpawnParent;
    public Transform bottomPillarSpawnParent;   
    public Transform objectSpawnParent;
    [Header("Pillar Prefabs")]
    public GameObject[] topPillarPrefabs;
    public GameObject[] bottomPillarPrefabs;
    [Header("Object Prefabs")]
    public GameObject[] objectPrefabs;
    [Header("Top Pillar Settings")]
    public bool flipTopPillars = true;
    [Header("Spawn Counts")]
    public int minTopPillars = 0;
    public int maxTopPillars = 1;
    public int minBottomPillars = 0;
    public int maxBottomPillars = 1;
    public int minObjects = 0;
    public int maxObjects = 2;
    [Header("Overlap Settings")]
    public float minDistanceBetweenSpawns = 1.5f;
    [Header("Parent For Spawned Objects")]
    public Transform spawnedParent;
    [Header("First Segment")]
    public bool skipSpawnOnFirstSegment = true;
    private static bool firstSegmentSkipped = false;
    private readonly List<Vector3> usedPositions = new List<Vector3>();
    void Start()
    {
        if (skipSpawnOnFirstSegment && !firstSegmentSkipped)
        {
            firstSegmentSkipped = true;
            return;
        }
        SpawnAll();
    }
    public void SpawnAll()
    {
        usedPositions.Clear();
        if (spawnedParent == null)
            spawnedParent = transform;
        List<Transform> topPillarPoints = GetChildPoints(topPillarSpawnParent);
        List<Transform> bottomPillarPoints = GetChildPoints(bottomPillarSpawnParent);
        List<Transform> objectPoints = GetChildPoints(objectSpawnParent);
        int topPillarCount = GetRandomCount(minTopPillars, maxTopPillars, topPillarPoints.Count);
        int bottomPillarCount = GetRandomCount(minBottomPillars, maxBottomPillars, bottomPillarPoints.Count);
        int objectCount = GetRandomCount(minObjects, maxObjects, objectPoints.Count);
        SpawnPillars(topPillarPoints, topPillarPrefabs, topPillarCount, true);
        SpawnPillars(bottomPillarPoints, bottomPillarPrefabs, bottomPillarCount, false);
        SpawnObjects(objectPoints, objectPrefabs, objectCount);
    }
    private List<Transform> GetChildPoints(Transform parent)
    {
        List<Transform> points = new List<Transform>();
        if (parent == null) return points;
        for (int i = 0; i < parent.childCount; i++)
        {
            points.Add(parent.GetChild(i));
        }
        return points;
    }
    private int GetRandomCount(int min, int max, int available)
    {
        if (available <= 0) return 0;
        min = Mathf.Clamp(min, 0, available);
        max = Mathf.Clamp(max, min, available);
        return Random.Range(min, max + 1);
    }
private void SpawnPillars(List<Transform> points, GameObject[] prefabs, int count, bool isTop)
{
    if (points.Count == 0 || prefabs.Length == 0) return;
    Shuffle(points);
    int spawned = 0;
    for (int i = 0; i < points.Count; i++)
    {
        if (spawned >= count) break;
        Transform point = points[i];
        if (IsOverlapping(point.position)) continue;
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject pillarObject = Instantiate(prefab, point.position, point.rotation, spawnedParent);
        pillarObject.transform.localScale = prefab.transform.localScale;
        if (isTop && flipTopPillars)
        {
            Vector3 scale = pillarObject.transform.localScale;
            scale.y *= -1f;
            pillarObject.transform.localScale = scale;
        }
        Pillar pillar = pillarObject.GetComponent<Pillar>();
        if (pillar != null)
        {
            Debug.Log($"Spawning pillar at {point.position} with spawn chance {pillar.spawnChance}");
            pillar.SpawnCoin();
        }
        else
        {
            Debug.LogWarning($"[{pillarObject.name}] has no Pillar component.");
        }
        usedPositions.Add(point.position);
        spawned++;
    }
}
    private void SpawnObjects(List<Transform> points, GameObject[] prefabs, int count)
    {
        if (points.Count == 0 || prefabs.Length == 0) return;
        Shuffle(points);
        int spawned = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (spawned >= count) break;
            Transform point = points[i];
            if (IsOverlapping(point.position)) continue;
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            Instantiate(prefab, point.position, point.rotation, spawnedParent);
            usedPositions.Add(point.position);
            spawned++;
        }
    }
    private bool IsOverlapping(Vector3 pos)
    {
        foreach (var used in usedPositions)
        {
            if (Vector3.Distance(pos, used) < minDistanceBetweenSpawns)
                return true;
        }
        return false;
    }
    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}