using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Throwable : MonoBehaviour
{
    //create a reference object that when picked up will place the prefab version of the object into the Player's Mark
    [Header("Basic Info")]
    [Space]

    public PlayerManager playerManager;
    public GameObject pickupObject;

    public bool isMarked = true;
    public bool itemGrounded = false;

    private bool facingRight = true;
    private Rigidbody2D rb;

    [Header("Modifiers")]
    [Space]

    public float groundRadius = 2f;
    public float normGravity = 3f;
    
    [SerializeField] private LayerMask groundLayers;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public UnityEvent OnWallEvent;

    private void Awake()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

        rb = GetComponent<Rigidbody2D>();
        normGravity = rb.gravityScale;

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnWallEvent == null)
            OnWallEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = itemGrounded;
        itemGrounded = false;

        // The throwable item is grounded if a circlecast to  each cardinal position hits anything designated as ground
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, groundLayers);
        for (int i = 0; i < groundColliders.Length; i++)
        {
            if (groundColliders[i].gameObject != gameObject)
            {
                itemGrounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        if(itemGrounded && playerManager.mark == gameObject)
        {
            playerManager.mark = null;
            playerManager.nullDisplayPickup();
            Instantiate(pickupObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Flip()
    {
        // Switch the way the item is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;

        transform.localScale = theScale;
    }
}
