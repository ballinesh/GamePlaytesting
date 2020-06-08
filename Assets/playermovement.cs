using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    public CharacterController2D controller;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;
    bool jumpFlag = false;
    bool dive = false;
    public Animator animator;
    
    public PhysicsMaterial2D slideMaterial;
    public PhysicsMaterial2D standingMaterial;
    private CircleCollider2D playerPhysics;
    private Rigidbody2D playerPhysics2;

    // Start is called before the first frame update
    void Start()
    {
        playerPhysics = GetComponent<CircleCollider2D>();
        playerPhysics2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    
        Debug.Log(playerPhysics.sharedMaterial.friction);
        animator.SetFloat("speed", Mathf.Abs(horizontalMove));
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        //Basic input animation handling
        //Flags for input conditions are also set so that the controller knows what action is being done
        
        //We need this second jump flag as there is a problem with the controller thinking the player landed right after jumping
        if(jumpFlag)
        {
            
            animator.SetBool("jumping",true);
            jumpFlag = false; 
        }
        if(Input.GetButtonDown("Jump"))
        {
            animator.SetBool("jumping",true);
            jump = true;
            
        }
        if(Input.GetButtonDown("Crouch") && !animator.GetBool("jumping"))
        {
            Debug.Log("Crouching");
            animator.SetBool("crouching",true);
            crouch = true;
        }
        else if(Input.GetButtonUp("Crouch"))
        {
            Debug.Log("Not crouching");
            animator.SetBool("crouching", false);
            crouch = false;
            
        }
        if(Input.GetButtonDown("Dive"))
        {
            Debug.Log("Diving");
            animator.SetBool("crouching",true);
            playerPhysics.sharedMaterial = slideMaterial;
            playerPhysics2.sharedMaterial = slideMaterial;
            
            dive = true;
            

        }
        else if (Input.GetButtonUp("Dive"))
        {
            Debug.Log("Not Diving");
            animator.SetBool("crouching",false);

            playerPhysics.sharedMaterial = standingMaterial;
            playerPhysics2.sharedMaterial = standingMaterial;
            
        }
        if(Input.GetButtonDown("TestDive"))
        {
            
            //This does not work, friction does not change :(
            Debug.Log("Diving");
            animator.SetBool("crouching",true);
    
            
            dive = true;
            

        }
        else if (Input.GetButtonUp("TestDive"))
        {
            Debug.Log("Not Diving");
            animator.SetBool("crouching",false);
            playerPhysics.sharedMaterial.friction = 0.4f;
            playerPhysics2.sharedMaterial.friction = 0.4f;
        }
        

    }
    void FixedUpdate() {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, dive);
        dive = false;

        //If a jump is happening we can set the jump flag to true
        if(jump == true)
        {
            jumpFlag = true;
        }
        jump = false;

    }

    public void onLanding()
    {
        //If they land then we can change the animation
        animator.SetBool("jumping",false);
    }
    
}
