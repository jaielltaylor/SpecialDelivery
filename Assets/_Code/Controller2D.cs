using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Controller2D : MonoBehaviour
{
	[SerializeField] private float jumpForce = 400f;                          // Amount of force added when the player jumps.
	[SerializeField] private float rollForce = 400f;                          // Amount of force added when the player rolls.
	[Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, 4)] [SerializeField] private float runSpeed = 2f;          // Amount of maxSpeed applied to running movement. 4 = 400%
	[Range(0, .3f)] [SerializeField] private float moveSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool airCtrl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask groundLayers;                          // A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform ceilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Transform rightWallCheck;                            // A position marking where to check for walls from the Right side
	[SerializeField] private Transform leftWallCheck;                            // A position marking where to check for walls from the Left side
	[SerializeField] private Collider2D crouchDisableCollider;                // A collider that will be disabled when crouching

	public List<Transform> throwPoints;                     //A list of positions that marks where Throwable objects will be spawned in and launched from
	public Transform deliveryIndicator;						//Used to flip the delivery arrow if the player Sprite is flipped.

	const float groundRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool grounded;            // Whether or not the player is grounded.
	public bool onWall;           // Whether or not the player is on a wall
	const float ceilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	const float wallRadius = .2f;	// Radius of the overlap circle to determine if the player can hang onto a wall
	private Rigidbody2D rb;
	private float normGravity = 3f;
	private bool facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;

	private PlayerManager manager;
	private PlayerActions inputs;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnWallEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool wasCrouching = false;
	public BoolEvent OnRunEvent;
	private bool wasRunning = false;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		normGravity = rb.gravityScale;

		manager = GetComponent<PlayerManager>();
		inputs = GetComponent<PlayerActions>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnWallEvent == null)
			OnWallEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
		
		if (OnRunEvent == null)
			OnRunEvent = new BoolEvent();
	}

	private void FixedUpdate()
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

		bool wasonWall = onWall;
		onWall = false;

		Collider2D[] wallRightColliders = Physics2D.OverlapCircleAll(rightWallCheck.position, wallRadius, groundLayers);
		Collider2D[] wallLeftColliders = Physics2D.OverlapCircleAll(leftWallCheck.position, wallRadius, groundLayers);
		List<Collider2D> wallColliders = new List<Collider2D>(wallRightColliders);
		wallColliders.AddRange(wallLeftColliders);

		for (int i = 0; i < wallColliders.Count; i++)
		{
			if (wallColliders[i].gameObject != gameObject)
			{
				onWall = true;
				if (!wasonWall)
					OnWallEvent.Invoke();
			}
		}

		if (!grounded && onWall && manager.canWallGrab) //manager action needs to be controlled by one script
		{
			rb.gravityScale = 0;
		}
		else if (!onWall)
        {
			rb.gravityScale = normGravity; 
        }
		else if (!manager.canWallGrab)
        {
			rb.gravityScale = normGravity;
			StartCoroutine(inputs.tiredState());
        }

	}



	public void Move(float move, float offMove, bool crouch, bool jump, bool run, bool dodgeRoll, bool teleport, float rollDir)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, groundLayers))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (grounded || airCtrl)
		{

			// If crouching
			if (crouch)
			{
				if (!wasCrouching)
				{
					wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= crouchSpeed;

				// Disable one of the colliders when crouching
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = true;

				if (wasCrouching)
				{
					wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			if (run && !crouch)
            {
				if (!wasRunning)
				{
					wasRunning = true;
					OnRunEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= runSpeed;
			}

			
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
			// And then smoothing it out and applying it to the character
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, moveSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && facingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		
		if (onWall) //swap to wall controls if the player is on the wall
        {
			if (run && !crouch)
			{
				if (!wasRunning)
				{
					wasRunning = true;
					OnRunEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				offMove *= runSpeed;
			}                                                         

			// Move the character by finding the target velocity
			rb.velocity = Vector2.zero;
			Vector3 targetVelocity = new Vector2(rb.velocity.x, offMove * 20f);
			// And then smoothing it out and applying it to the character
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, moveSmoothing);
		}

		// If the player should jump...
		if (grounded && jump)
		{
			//Debug.Log("Jumping!");
			// Add a vertical force to the player.
			grounded = false;
			rb.AddForce(new Vector2(0f, jumpForce));
		}
		else if (onWall && jump) //if the player should jump while on wall
        {
			onWall = false;
			switch(facingRight)
            {
				case false:
					rb.AddForce(new Vector2(jumpForce, jumpForce));
					break;
				case true:
					rb.AddForce(new Vector2(-jumpForce, jumpForce));
					break;
            }
			
		}

		// If the player should dodgeRoll...
		if (grounded  && dodgeRoll)
        {
			Debug.Log("Rolling!");
			//Add a horizontal force to the player
			rb.AddForce(new Vector2(rollForce * rollDir, 0f));
        }

		if (teleport)
        {
			//create a temporary vector to hold the position of the marked object
			//swap the player's position and velocity with the marked object's
			Vector2 tpHold = manager.mark.transform.position;

			manager.mark.GetComponent<Rigidbody2D>().velocity = rb.velocity;
			manager.mark.transform.position = transform.position;

			transform.position = tpHold;
			rb.AddForce(inputs.throwDir);

			Debug.Log("Successfully teleported!");
        }
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;

		Vector3 arrowScale = deliveryIndicator.localScale;
		arrowScale.x *= -1;

		transform.localScale = theScale;
		deliveryIndicator.localScale = arrowScale;
	}
}
