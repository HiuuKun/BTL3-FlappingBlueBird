using UnityEngine;

public class SpriteFrameAnimator : MonoBehaviour
{
    public enum AnimState
    {
        Hover,
        Fly,
        Death
    }

    [Header("Reference")]
    public SpriteRenderer spriteRenderer;

    [Header("Animations")]
    public Sprite[] hoverFrames;
    public float hoverFps = 8f;

    public Sprite[] flyFrames;
    public float flyFps = 14f;

    public Sprite[] deathFrames;
    public float deathFps = 10f;

    [Header("State")]
    public AnimState currentState = AnimState.Hover;
    public bool loop = true;

    private float timer;
    private int currentFrame;
    private AnimState previousState;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        previousState = currentState;
        ApplyCurrentFrame();
    }

    void Update()
    {
        if (currentState != previousState)
        {
            timer = 0f;
            currentFrame = 0;
            previousState = currentState;
            ApplyCurrentFrame();
        }

        Sprite[] frames = GetCurrentFrames();
        float fps = GetCurrentFps();

        if (frames == null || frames.Length == 0 || spriteRenderer == null || fps <= 0f)
            return;

        timer += Time.deltaTime;

        float frameDuration = 1f / fps;

        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (ShouldLoopCurrentState())
                    currentFrame = 0;
                else
                    currentFrame = frames.Length - 1;
            }

            ApplyCurrentFrame();
        }
    }

    public void SetState(AnimState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
    }

    private Sprite[] GetCurrentFrames()
    {
        switch (currentState)
        {
            case AnimState.Fly:
                return flyFrames;
            case AnimState.Death:
                return deathFrames;
            default:
                return hoverFrames;
        }
    }

    private float GetCurrentFps()
    {
        switch (currentState)
        {
            case AnimState.Fly:
                return flyFps;
            case AnimState.Death:
                return deathFps;
            default:
                return hoverFps;
        }
    }

    private bool ShouldLoopCurrentState()
    {
        if (currentState == AnimState.Death)
            return false;

        return loop;
    }

    private void ApplyCurrentFrame()
    {
        Sprite[] frames = GetCurrentFrames();

        if (frames == null || frames.Length == 0 || spriteRenderer == null)
            return;

        currentFrame = Mathf.Clamp(currentFrame, 0, frames.Length - 1);
        spriteRenderer.sprite = frames[currentFrame];
    }
}