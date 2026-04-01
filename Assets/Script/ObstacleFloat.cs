using UnityEngine;

public class ObstacleFloat : MonoBehaviour
{
    [Header("Movement")]
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public bool randomPhase = true;

    private Vector3 startLocalPosition;
    private float phaseOffset;

    void Start()
    {
        startLocalPosition = transform.localPosition;

        if (randomPhase)
            phaseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f + phaseOffset) * amplitude;
        transform.localPosition = startLocalPosition + new Vector3(0f, offsetY, 0f);
    }
}