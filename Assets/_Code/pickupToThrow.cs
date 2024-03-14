using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupToThrow : MonoBehaviour
{
    public PlayerManager playerManager;
    public GameObject realObject;

    public float groundRadius = 2f;
    public bool pickupGrounded = false;
    [SerializeField] private LayerMask groundLayers;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.\

    private Rigidbody2D rb;

    void Awake()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        bool wasGrounded = pickupGrounded;
        pickupGrounded = false;

        // The throwable item is grounded if a circlecast to  each cardinal position hits anything designated as ground
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, groundLayers);
        for (int i = 0; i < groundColliders.Length; i++)
        {
            if (groundColliders[i].gameObject != gameObject)
            {
                pickupGrounded = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player")) 
        { 
            playerManager.touchingPickup = true;
            playerManager.temp = gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player")) 
        { 
            playerManager.touchingPickup = false;
            playerManager.temp = null;
        }
    }

    public void pickupDestroy() 
    {
        playerManager.mark = realObject;
        Destroy(gameObject); 
    }
}
