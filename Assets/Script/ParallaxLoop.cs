using System.Collections.Generic;
using UnityEngine;

public class ParallaxLoop : MonoBehaviour
{
    [Header("Base Speed")]
    public float speed = 2f;
    public int initialSegments = 4;
    public float extraLifeSeconds = 5f;
    public GameObject sourceObject;

    [Header("Increase Speed")]
    public bool increaseSpeed = false;
    public float speedIncreasePerSecond = 0.2f;
    public float maxSpeed = 10f;

    private class Segment
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public bool spawnedReplacement;
        public float destroyTime = -1f;
        public bool isManager;
    }

    private readonly List<Segment> segments = new List<Segment>();

    private GameObject templateObject;
    private float width;
    private float startX;
    private Vector3 baseScale;
    private Vector3 startPosition;
    private bool nextMirror;

    void Start()
    {
        if (sourceObject == null)
        {
            Debug.LogError("ParallaxLoop needs a sourceObject.");
            enabled = false;
            return;
        }

        SpriteRenderer sourceRenderer = sourceObject.GetComponent<SpriteRenderer>();
        if (sourceRenderer == null)
        {
            Debug.LogError("sourceObject must have a SpriteRenderer.");
            enabled = false;
            return;
        }

        if (initialSegments < 1)
            initialSegments = 1;

        width = sourceRenderer.bounds.size.x;
        startX = transform.position.x;
        startPosition = transform.position;
        baseScale = transform.localScale;

        templateObject = gameObject;

        AddSegment(transform, sourceRenderer, false, true);

        for (int i = 1; i < initialSegments; i++)
        {
            Segment prev = segments[segments.Count - 1];
            Transform newSeg = CreateSegmentAtRightOf(prev, i % 2 == 1);
            SpriteRenderer sr = GetRendererForSegment(newSeg.gameObject);
            AddSegment(newSeg, sr, i % 2 == 1, false);
        }

        nextMirror = (initialSegments % 2 == 1);
    }

    void Update()
    {
        if (increaseSpeed)
        {
            speed += speedIncreasePerSecond * Time.deltaTime;
            speed = Mathf.Min(speed, maxSpeed);
        }

        float move = speed * Time.deltaTime;

        for (int i = segments.Count - 1; i >= 0; i--)
        {
            Segment seg = segments[i];

            if (seg.transform == null || seg.renderer == null)
            {
                segments.RemoveAt(i);
                continue;
            }

            seg.transform.position += Vector3.left * move;

            if (!seg.spawnedReplacement && seg.transform.position.x <= startX - width)
            {
                Segment rightMost = GetRightMostSegment();
                Transform newSeg = CreateSegmentAtRightOf(rightMost, nextMirror);
                SpriteRenderer sr = GetRendererForSegment(newSeg.gameObject);
                AddSegment(newSeg, sr, nextMirror, false);

                nextMirror = !nextMirror;
                seg.spawnedReplacement = true;
                seg.destroyTime = Time.time + extraLifeSeconds;
            }

            if (seg.spawnedReplacement && seg.destroyTime > 0f && Time.time >= seg.destroyTime)
            {
                if (seg.isManager)
                {
                    if (seg.renderer != null)
                        seg.renderer.enabled = false;

                    segments.RemoveAt(i);
                }
                else
                {
                    Destroy(seg.transform.gameObject);
                    segments.RemoveAt(i);
                }
            }
        }
    }

    private void AddSegment(Transform t, SpriteRenderer sr, bool mirrored, bool isManager)
    {
        SetMirror(t, mirrored);

        if (sr != null)
            sr.enabled = true;

        segments.Add(new Segment
        {
            transform = t,
            renderer = sr,
            spawnedReplacement = false,
            destroyTime = -1f,
            isManager = isManager
        });
    }

    private Transform CreateSegmentAtRightOf(Segment prev, bool mirrored)
    {
        GameObject clone = Instantiate(templateObject, transform.parent);
        clone.name = gameObject.name + "_Segment";

        ParallaxLoop loop = clone.GetComponent<ParallaxLoop>();
        if (loop != null)
        {
            loop.enabled = false;
            Destroy(loop);
        }

        Transform t = clone.transform;
        t.rotation = transform.rotation;
        t.localScale = baseScale;
        t.position = prev.transform.position;

        SetMirror(t, mirrored);

        SpriteRenderer newRenderer = GetRendererForSegment(clone);
        if (newRenderer == null)
        {
            Debug.LogError("Clone does not contain the source SpriteRenderer.");
            return t;
        }

        float prevRight = prev.renderer.bounds.max.x;
        float newLeft = newRenderer.bounds.min.x;
        float offset = prevRight - newLeft;

        t.position += new Vector3(offset, 0f, 0f);

        return t;
    }

    private Segment GetRightMostSegment()
    {
        Segment rightMost = null;
        float maxRight = float.MinValue;

        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].renderer != null)
            {
                float right = segments[i].renderer.bounds.max.x;
                if (right > maxRight)
                {
                    maxRight = right;
                    rightMost = segments[i];
                }
            }
        }

        return rightMost;
    }

    private SpriteRenderer GetRendererForSegment(GameObject obj)
    {
        if (sourceObject == null)
            return null;

        if (sourceObject == gameObject)
            return obj.GetComponent<SpriteRenderer>();

        Transform sourcePath = sourceObject.transform;
        List<string> path = new List<string>();

        while (sourcePath != null && sourcePath != transform)
        {
            path.Insert(0, sourcePath.name);
            sourcePath = sourcePath.parent;
        }

        Transform current = obj.transform;
        for (int i = 0; i < path.Count; i++)
        {
            current = current.Find(path[i]);
            if (current == null)
                return null;
        }

        return current.GetComponent<SpriteRenderer>();
    }

    private void SetMirror(Transform target, bool mirror)
    {
        target.localScale = baseScale;

        SpriteRenderer sr = GetRendererForSegment(target.gameObject);
        if (sr != null)
        {
            sr.flipX = mirror;
        }
    }
}