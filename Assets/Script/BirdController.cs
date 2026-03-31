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
    //private Animator animator;
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
        isDead = true;
        rb.velocity = Vector2.zero;
        birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Death);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Coin"))
            return;

        experience += coinExperience;
        Destroy(collision.gameObject);
    }
}