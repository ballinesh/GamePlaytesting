using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement

	[Range(0, 500f)] [SerializeField] private float m_DiveForceDown = 100f;		//Dive force down
	[Range(0, 500f)] [SerializeField] private float m_DiveForceForward = 100f;	//Dive force right
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
	[SerializeField] private Transform m_WallDetection;							// A position marking where to check if the player is touching a wall
	

	const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
	const float k_ForwardRadius = .05f;
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .3f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool justJumped = false;
	private bool m_touchingWall = false;
	
	

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
	private int facingRight = 1;
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		m_touchingWall = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				//Debug.Log(gameObject);
				m_Grounded = true;
			
				if (!wasGrounded)
				{
					OnLandEvent.Invoke();
				}
					
			}
		}

		Collider2D[] frontColliders = Physics2D.OverlapCircleAll(m_WallDetection.position, k_ForwardRadius, m_WhatIsGround);
		for(int i = 0; i < frontColliders.Length; i++)
		{
			if(frontColliders[i].gameObject != gameObject)
			{
				Debug.Log(m_touchingWall);
				m_touchingWall = true;
				//TouchingWall.invoke
			}
		}
	}


	public void Move(float move, bool crouch, bool jump,bool dive)
	{
		// If crouching, check to see if the character can stand up
		if (crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			m_Grounded = false;
			
			
			
		}
		else if( m_touchingWall && jump && !m_Grounded)
		{
				Vector2 vel = m_Rigidbody2D.velocity;
				vel.x = 0f;
				vel.y = 0f;
				m_Rigidbody2D.velocity = vel;
				m_Rigidbody2D.AddForce(new Vector2(m_DiveForceForward * (-facingRight),m_JumpForce));
		}

		
		//If the character is diving
		if(dive)
		{
			Debug.Log("Diving in controller");
			Vector2 vel = m_Rigidbody2D.velocity;
			vel.x = 0f;
			vel.y = 0f;
			m_Rigidbody2D.velocity = vel;
			m_Rigidbody2D.AddForce(new Vector2(m_DiveForceForward * facingRight,-m_DiveForceDown));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		//Change the dive direction
		facingRight = facingRight *(-1);
		transform.Rotate(0f, 180f, 0f);
	}
}
