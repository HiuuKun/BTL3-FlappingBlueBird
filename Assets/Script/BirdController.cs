using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class BirdController : MonoBehaviour
{
    public int experience = 0;
    public int coinExperience = 10;
    [Header("Movement")]
    public float thrustForce = 10f;
    public float maxRiseSpeed = 4f;
    public float maxFallSpeed = -8f;

    private Rigidbody2D rb;
    private bool isThrusting;
    private bool isDead;
    public GhostTrail ghostTrail;
    public SpriteFrameAnimator birdSpriteAnimator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        birdSpriteAnimator = GetComponentInChildren<SpriteFrameAnimator>();
        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
        {
            //animator.SetBool("IsThrusting", false);
            return;
        }

        isThrusting =
            Input.GetKey(KeyCode.Space) ||
            Input.GetMouseButton(0) ||
            Input.touchCount > 0;
        if (ghostTrail != null) ghostTrail.isThrusting = isThrusting;
        if (isThrusting)
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Fly);
        else
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Hover);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        Vector2 velocity = rb.velocity;

        if (isThrusting)
        {
            velocity.y += thrustForce * Time.fixedDeltaTime;
        }

        velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, maxRiseSpeed);
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Prevent multiple triggers

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            isDead = true;
            rb.velocity = Vector2.zero;
            rb.simulated = false; // Fix #1: Stop physics immediately so you don't drop
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Death);

            StartCoroutine(DelayedGameOver());
        }
    }

    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSecondsRealtime(1f);

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Coin")) return;
        experience += coinExperience;
        Destroy(collision.gameObject);
    }
    public void ResetPlayer()
    {
        isDead = false;
        transform.position = new Vector3(1.831f, 9.2742f, 0f);
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.simulated = true;
        if (birdSpriteAnimator != null)
        {
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Hover);
        }
    }
}