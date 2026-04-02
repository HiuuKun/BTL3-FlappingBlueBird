using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class BirdController : MonoBehaviour
{
    public int experience = 0;
    public int coinExperience = 10;
    [Header("Movement")]
    public float thrustForce = 50f;
    public float maxRiseSpeed = 3.5f;
    public float maxFallSpeed = -3.5f;

    [Header("Gravity Item Settings")]
    public bool isGravityMode = false;
    public float gravityModeDuration = 10f;
    private float defaultGravityScale;

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

        //save the default gravity scale when game start
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (isDead)
        {
            //animator.SetBool("IsThrusting", false);
            return;
        }

        bool tapDown = Input.GetKeyDown(KeyCode.Space) ||
                       Input.GetMouseButtonDown(0) ||
                       (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        isThrusting =
            Input.GetKey(KeyCode.Space) ||
            Input.GetMouseButton(0) ||
            Input.touchCount > 0;

        if (ghostTrail != null) ghostTrail.isThrusting = isThrusting && !isGravityMode;

        if (isThrusting)
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Fly);
        else
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Hover);


        if (isGravityMode)
        {
            if (tapDown)
            {
                rb.gravityScale *= -1f; //reverse gravity

                //reverse sprite
                Vector3 scale = transform.localScale;
                scale.y *= -1f;
                transform.localScale = scale;
            }
        }
        else
        {
            //norval mode
            if (isThrusting)
                birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Fly);
            else
                birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Hover);
        }

    }

    void FixedUpdate()
    {
        if (isDead) return;

        Vector2 velocity = rb.velocity;

        if (isThrusting && !isGravityMode)
        {
            velocity.y += thrustForce * Time.fixedDeltaTime;
        }

        if (rb.gravityScale > 0)
        {
            velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, maxRiseSpeed);
        }
        else
        {
            velocity.y = Mathf.Clamp(velocity.y, -maxRiseSpeed, -maxFallSpeed);
        }

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
        if (collision.CompareTag("Coin"))
        {
            experience += coinExperience;
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("GravityItem"))
        {
            Destroy(collision.gameObject); //delete item
            
            //activate gravity mode
            if (!isGravityMode)
            {
                StartCoroutine(GravityModeRoutine());
            }
        }
    }

    private IEnumerator GravityModeRoutine()
    {
        isGravityMode = true;
        
        if (birdSpriteAnimator != null)
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Fly);

        yield return new WaitForSeconds(gravityModeDuration);

        isGravityMode = false;
        
        rb.gravityScale = Mathf.Abs(defaultGravityScale); 
        
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;
    }

    public void ResetPlayer()
    {
        isDead = false;
        transform.position = new Vector3(1.831f, 9.2742f, 0f);
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.simulated = true;

        isGravityMode = false;
        if (defaultGravityScale != 0) rb.gravityScale = Mathf.Abs(defaultGravityScale);

        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;

        if (birdSpriteAnimator != null)
        {
            birdSpriteAnimator.SetState(SpriteFrameAnimator.AnimState.Hover);
        }
    }
}