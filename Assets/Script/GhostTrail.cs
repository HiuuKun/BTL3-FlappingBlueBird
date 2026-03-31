using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer birdSpriteRenderer;

    [Header("Trail Settings")]
    public bool isThrusting;
    public float spawnInterval = 0.05f;
    public float ghostLifetime = 0.35f;
    public Color ghostColor = new Color(1f, 1f, 1f, 0.3f);
    public int sortingOrderOffset = -1;

    private float spawnTimer;

    private class GhostInstance
    {
        public GameObject gameObject;
        public SpriteRenderer spriteRenderer;
        public float timer;
        public float lifetime;
        public Color startColor;
    }

    private readonly List<GhostInstance> ghosts = new List<GhostInstance>();

    void Reset()
    {
        birdSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleSpawning();
        HandleFading();
    }

    private void HandleSpawning()
    {
        if (!isThrusting)
        {
            spawnTimer = 0f;
            return;
        }

        if (birdSpriteRenderer == null || birdSpriteRenderer.sprite == null)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnGhost();
            spawnTimer = 0f;
        }
    }

    private void SpawnGhost()
    {
        GameObject ghost = new GameObject("GhostTrail");

        ghost.transform.position = birdSpriteRenderer.transform.position;
        ghost.transform.rotation = birdSpriteRenderer.transform.rotation;
        ghost.transform.localScale = birdSpriteRenderer.transform.lossyScale;

        SpriteRenderer ghostSR = ghost.AddComponent<SpriteRenderer>();
        ghostSR.sprite = birdSpriteRenderer.sprite;
        ghostSR.flipX = birdSpriteRenderer.flipX;
        ghostSR.flipY = birdSpriteRenderer.flipY;
        ghostSR.color = ghostColor;
        ghostSR.sortingLayerID = birdSpriteRenderer.sortingLayerID;
        ghostSR.sortingOrder = birdSpriteRenderer.sortingOrder + sortingOrderOffset;

        ghosts.Add(new GhostInstance
        {
            gameObject = ghost,
            spriteRenderer = ghostSR,
            timer = 0f,
            lifetime = ghostLifetime,
            startColor = ghostColor
        });
    }

    private void HandleFading()
    {
        for (int i = ghosts.Count - 1; i >= 0; i--)
        {
            GhostInstance ghost = ghosts[i];

            if (ghost.gameObject == null || ghost.spriteRenderer == null)
            {
                ghosts.RemoveAt(i);
                continue;
            }

            ghost.timer += Time.deltaTime;

            float t = ghost.timer / ghost.lifetime;

            Color c = ghost.startColor;
            c.a = Mathf.Lerp(ghost.startColor.a, 0f, t);
            ghost.spriteRenderer.color = c;

            if (ghost.timer >= ghost.lifetime)
            {
                Destroy(ghost.gameObject);
                ghosts.RemoveAt(i);
            }
        }
    }
}