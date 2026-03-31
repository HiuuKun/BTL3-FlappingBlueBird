using System.Collections.Generic;
using UnityEngine;

public class GroundParallaxLoop : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public int initialSegments = 4;
    public float extraLifeSeconds = 5f;

    [Header("Reference")]
    public Transform spriteChild;

    private class Segment
    {
        public Transform transform;
        public bool spawnedReplacement;
        public float destroyTime = -1f;
        public bool isManager;
    }

    private readonly List<Segment> segments = new List<Segment>();

    private GameObject templateObject;
    private float width;
    private float startX;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 baseScale;
    private bool nextMirror;

    void Start()
    {
        if (spriteChild == null)
        {
            Debug.LogError("ParallaxLoop needs spriteChild assigned.");
            enabled = false;
            return;
        }

        SpriteRenderer sr = spriteChild.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("spriteChild needs a SpriteRenderer.");
            enabled = false;
            return;
        }

        if (initialSegments < 1)
            initialSegments = 1;

        width = sr.bounds.size.x;
        startX = transform.position.x;
        startPosition = transform.position;
        startRotation = transform.rotation;
        baseScale = transform.localScale;
        templateObject = gameObject;

        AddSegment(transform, false, true);

        for (int i = 1; i < initialSegments; i++)
        {
            bool mirrored = (i % 2 == 1);
            Transform newSeg = CreateSegment(startX + i * width, mirrored);
            AddSegment(newSeg, mirrored, false);
        }

        nextMirror = (initialSegments % 2 == 1);
    }

    void Update()
    {
        float move = speed * Time.deltaTime;

        for (int i = segments.Count - 1; i >= 0; i--)
        {
            Segment seg = segments[i];

            if (seg.transform == null)
            {
                segments.RemoveAt(i);
                continue;
            }

            seg.transform.position += Vector3.left * move;

            if (!seg.spawnedReplacement && seg.transform.position.x <= startX - width)
            {
                float rightMostX = GetRightMostX();
                Transform newSeg = CreateSegment(rightMostX + width, nextMirror);
                AddSegment(newSeg, nextMirror, false);

                nextMirror = !nextMirror;
                seg.spawnedReplacement = true;
                seg.destroyTime = Time.time + extraLifeSeconds;
            }

            if (seg.spawnedReplacement && seg.destroyTime > 0f && Time.time >= seg.destroyTime)
            {
                if (seg.isManager)
                {
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

    private void AddSegment(Transform t, bool mirrored, bool isManager)
    {
        SetMirror(t, mirrored);

        segments.Add(new Segment
        {
            transform = t,
            spawnedReplacement = false,
            destroyTime = -1f,
            isManager = isManager
        });
    }

    private Transform CreateSegment(float x, bool mirrored)
    {
        GameObject clone = Instantiate(templateObject);
        clone.name = gameObject.name + "_Segment";
        clone.transform.SetParent(transform.parent, true);

        ParallaxLoop loop = clone.GetComponent<ParallaxLoop>();
        if (loop != null)
            Destroy(loop);

        Transform t = clone.transform;
        t.position = new Vector3(x, startPosition.y, startPosition.z);
        t.rotation = startRotation;
        t.localScale = templateObject.transform.localScale;

        SetMirror(t, mirrored);

        return t;
    }

    private float GetRightMostX()
    {
        float rightMost = float.MinValue;

        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].transform != null && segments[i].transform.position.x > rightMost)
                rightMost = segments[i].transform.position.x;
        }

        return rightMost;
    }

    private void SetMirror(Transform target, bool mirror)
    {
        Vector3 scale = templateObject.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (mirror ? -1f : 1f);
        target.localScale = scale;
    }
}