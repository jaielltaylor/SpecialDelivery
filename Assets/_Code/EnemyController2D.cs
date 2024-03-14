using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController2D : MonoBehaviour
{
    [Range(0, 4)] [SerializeField] private float runSpeed = 2f;          // Amount of maxSpeed applied to running movement. 4 = 400%
    [Range(0, .3f)] [SerializeField] private float moveSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private LayerMask groundLayers;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform rightWallCheck;                            // A position marking where to check for walls from the Right side
    [SerializeField] private Transform leftWallCheck;                            // A position marking where to check for walls from the Left side

    const float groundRadius = .2f; // Radius of the overlap circle to determine if grounded
    public bool grounded;            // Whether or not the player is grounded.
    public bool nearWall;           // Whether or not the player is near a wall
    const float wallRadius = .2f;   // Radius of the overlap circle to determine if the player can hang onto a wall
    private Rigidbody2D rb;
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public UnityEvent nearWallEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool wasCrouching = false;
    public BoolEvent OnRunEvent;
    private bool wasRunning = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (nearWallEvent == null)
            nearWallEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        if (OnRunEvent == null)
            OnRunEvent = new BoolEvent();
    }

    void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, groundLayers);
        for (int i = 0; i < groundColliders.Length; i++)
        {
            if (groundColliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        bool wasNearWall = nearWall;
        nearWall = false;

        Collider2D[] wallRightColliders = Physics2D.OverlapCircleAll(rightWallCheck.position, wallRadius, groundLayers);
        Collider2D[] wallLeftColliders = Physics2D.OverlapCircleAll(leftWallCheck.position, wallRadius, groundLayers);
        List<Collider2D> wallColliders = new List<Collider2D>(wallRightColliders);
        wallColliders.AddRange(wallLeftColliders);

        for (int i = 0; i < wallColliders.Count; i++)
        {
            if (wallColliders[i].gameObject != gameObject)
            {
                nearWall = true;
                if (!wasNearWall)
                    nearWallEvent.Invoke();
            }
        }
    }

    public void React()
    {
        //This function will be used to manage the enemy's reaction to specific situations and stimuli. This basic enemy patrols, chases, and attacks.
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;

        //Retain the stamina slider's x scale - Uncomment if we put the Stamina slider above the player
        /*
		Vector3 slideScale = staminaSlider.transform.localScale;
		slideScale.x *= -1;
		*/
        transform.localScale = theScale;
        //staminaSlider.transform.localScale = slideScale; - Uncomment if we put the Stamina slider above the player
    }
}
